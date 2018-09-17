using System;
using System.Collections;
using Assets.Components;
using Assets.FarmSim.I3D;
using Pfim;
using UnityEngine;

namespace Assets
{
    public class Entity : MonoBehaviour
    {
        public Shape Shape;
        [HideInInspector]
        public Texture2D Tex;
        
        private bool _visible;
        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                GetComponent<MeshRenderer>().enabled = value;
            }
        }

        private bool VisibleInnerCheck(GameObject part)
        {
            while (true)
            {
                if (part.GetComponent<I3DTransform>().Visibility == false)
                    return false;

                if (part.transform.parent == null)
                    return true;

                part = part.transform.parent.gameObject;
            }
        }

        private bool DeepIsVisible()
        {
            if (!Shape || Shape.NonRenderable)
                return false;

            if (gameObject.transform.parent == null)
                return true;

            return VisibleInnerCheck(gameObject.transform.parent.gameObject);
        }

        public static Texture2D LoadTextureDxt(byte[] ddsBytes)
        {
            byte ddsSizeCheck = ddsBytes[0x4];
            if (ddsSizeCheck != 124)
                throw new Exception("Invalid DDS DXTn texture. Unable to read");  //this header byte should be 124 for DDS image files
            
            int height = ddsBytes[0xF] << 24 | ddsBytes[0xE] << 16 | ddsBytes[0xD] << 8 | ddsBytes[0xC];
            int width = ddsBytes[0x13] << 24 | ddsBytes[0x12] << 16 | ddsBytes[0x11] << 8 | ddsBytes[0x10];

            int mipmapCount = ddsBytes[0x1F] << 24 | ddsBytes[0x1E] << 16 | ddsBytes[0x1D] << 8 | ddsBytes[0x1C];

            TextureFormat textureFormat;
            string texFormatStr = System.Text.Encoding.ASCII.GetString(ddsBytes, 0x54, 4);
            switch (texFormatStr)
            {
                case "DXT1":
                    textureFormat = TextureFormat.DXT1;
                    break;
                case "DXT3":
                    return LoadTexturePfim(ddsBytes);
                case "DXT5":
                    textureFormat = TextureFormat.DXT5;
                    break;
                default:
                    throw new Exception($"Unknown texture format {texFormatStr}");
            }

            const int ddsHeaderSize = 128;
            byte[] dxtBytes = new byte[ddsBytes.Length - ddsHeaderSize];
            Buffer.BlockCopy(ddsBytes, ddsHeaderSize, dxtBytes, 0, ddsBytes.Length - ddsHeaderSize);

            Texture2D texture = new Texture2D(width, height, textureFormat, false, false);
            
            texture.LoadRawTextureData(dxtBytes);
            texture.Apply();



            return texture;
        }

        public static Texture2D LoadTexturePfim(byte[] bytes)
        {
            IImage imageData = Dds.Create(bytes, new PfimConfig());
            imageData.Decompress();

            ImageFormat frmt = imageData.Format;

            TextureFormat texFrmt;
            switch (imageData.Format)
            {
                case ImageFormat.Rgb24:
                    texFrmt = TextureFormat.RGB24;
                    break;
                case ImageFormat.Rgba32:
                    texFrmt = TextureFormat.RGBA32;
                    break;
                default:
                    throw new Exception($"Unknown raw image format {frmt}");
            }

            Texture2D texture = new Texture2D(imageData.Width, imageData.Height, texFrmt, false, false);

            texture.LoadRawTextureData(imageData.Data);
            texture.Apply();

            return texture;
        }

        public Texture2D LoadTexture(string url)
        {
            byte[] bytes = System.IO.File.ReadAllBytes(url);
            return LoadTextureDxt(bytes);
        }

        public void Setup()
        {
            //Assign material
            if (Shape != null)
            {
                foreach (I3DMaterial shapeMaterial in Shape.Materials)
                {
                    if (shapeMaterial.TextureFile == null)
                        continue;

                    Material mat = GetComponent<Renderer>().material;
                    mat.mainTextureScale = new Vector2(1, -1);
                    try
                    {
                        mat.mainTexture = LoadTexture(shapeMaterial.TextureFile.AbsolutePath);
                    }
                    catch (UnityException e)
                    {
                        Debug.LogError($"Failed to parse texture {shapeMaterial.TextureFile.AbsolutePath}\n{e.Message}");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to parse texture {shapeMaterial.TextureFile.AbsolutePath}\n{e.Message}");
                    }

                    if (shapeMaterial.NormalMapFile != null)
                        mat.SetTexture("_BumpMap", LoadTexture(shapeMaterial.NormalMapFile.AbsolutePath));

                    //if (shapeMaterial.ReflectionMap != null)
                    //    mat.SetTexture("_Cube", LoadTexture(shapeMaterial.ReflectionMap.AbsolutePath));
                }
            }

            //Assign name
            name = GetComponent<I3DTransform>().Name;

            //Check visibility
            Visible = DeepIsVisible();
            //Visible = !Scene.NonRenderable;
            //Visible = true;
        }
    }
}
