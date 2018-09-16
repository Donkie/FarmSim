using System;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace I3DShapesTool
{
    class I3DShapeTool
    {
        private static I3DShapesHeader ParseFileHeader(Stream fs)
        {
            byte b1 = fs.ReadInt8();
            byte b2 = fs.ReadInt8();
            byte b3 = fs.ReadInt8();
            byte b4 = fs.ReadInt8();

            byte seed;
            short version;

            if (b1 == 5)
            {
                //Some testing
                version = b1;
                seed = b3;
            }
            else if (b4 == 2 || b4 == 3)
            {
                version = b4;
                seed = b2;
            }
            else
            {
                throw new NotSupportedException("Unknown version");
            }

            return new I3DShapesHeader
            {
                Seed = seed,
                Version = version
            };
        }

        public static I3DShape[] ParseShapesFile(string path)
        {
            I3DShape[] shapes;

            using (FileStream fs = File.OpenRead(path))
            {
                string fileName = Path.GetFileName(fs.Name) ?? "N/A";
                Debug.Log("Loading file: " + fileName);

                Debug.Log("File Size: " + new FileInfo(fs.Name).Length + " bytes");

                I3DShapesHeader header = ParseFileHeader(fs);

                Debug.Log("File Seed: " + header.Seed);
                Debug.Log("File Version: " + header.Version);

                if (header.Version != 2 && header.Version != 3)
                    throw new NotSupportedException("Unsupported version");

                Debug.Log("");

                using (I3DDecryptorStream dfs = new I3DDecryptorStream(fs, header.Seed))
                {
                    int itemCount = dfs.ReadInt32L();
                    shapes = new I3DShape[itemCount];

                    Debug.Log("Found " + itemCount + " shapes");
                    Debug.Log("");
                    for (int i = 0; i < itemCount; i++)
                    {
                        Console.Write("{0}: ", i + 1);

                        int type = dfs.ReadInt32L();
                        int size = dfs.ReadInt32L();
                        Console.Write("(Type {0}) ", type);
                        Console.Write(size + " bytes");
                        byte[] data = dfs.ReadBytes(size);
                        
                        using (MemoryStream ms = new MemoryStream(data))
                        {
                            using (BigEndianBinaryReader br = new BigEndianBinaryReader(ms))
                            {
                                shapes[i] = new I3DShape(br);
                            }
                        }
                        
                        Debug.Log($" - {shapes[i].Name}");
                    }
                }
            }
         
            return shapes;
        }
    }
}
