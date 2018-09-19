using System;
using System.Collections.Generic;
using System.IO;
using Debug = UnityEngine.Debug;

namespace Assets.I3DShapesTool
{
    public abstract class I3DShapeTool
    {
        private static readonly Dictionary<string, I3DShape[]> Cache = new Dictionary<string, I3DShape[]>();

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

        public static I3DShape[] LoadShapesFile(string path)
        {
            if (Cache.ContainsKey(path))
            {
                return Cache[path];
            }

            List<I3DShape> shapes;

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
                    shapes = new List<I3DShape>();

                    Debug.Log("Found " + itemCount + " shapes");
                    Debug.Log("");
                    for (int i = 0; i < itemCount; i++)
                    {
                        int type = dfs.ReadInt32L();
                        int size = dfs.ReadInt32L();
                        byte[] data = dfs.ReadBytes(size);

                        Debug.Log($"{i+1}: (Type {type}) {size} bytes");

                        string binFileName = $"{i+1}-{type}.bin";
                        //File.WriteAllBytes(Path.Combine(@"F:\SteamLibrary\steamapps\common\Farming Simulator 15\data\maps\decompile", binFileName), data);
                        //File.WriteAllBytes(Path.Combine(@"D:\SteamLibrary\steamapps\common\Farming Simulator 2013\data\maps\decompile\chickenmesh", binFileName), data);
                        
                        using (MemoryStream ms = new MemoryStream(data))
                        {
                            using (BigEndianBinaryReader br = new BigEndianBinaryReader(ms))
                            {
                                try
                                {
                                    switch (type)
                                    {
                                        case 1:
                                            shapes.Add(new I3DShape(br));
                                            //Debug.Log($" - {shapes[i].Name}");
                                            break;
                                        case 2:
                                            new I3DSpline(br);
                                            break;
                                        case 3:
                                            new I3DNavMesh(br);
                                            break;
                                    }
                                }
                                catch (Exception e)
                                {
                                    Debug.Log(e.Message);
                                }
                            }
                        }
                    }
                }
            }

            I3DShape[] shapesArr = shapes.ToArray();

            Cache[path] = shapesArr;
         
            return shapesArr;
        }
    }
}
