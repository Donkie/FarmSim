using System;
using System.Collections;
using Assets.FarmSim.I3D;
using UnityEngine;

namespace Assets
{
    public class Shape : MonoBehaviour
    {
        public I3DSceneShape Scene;
        public Texture2D Tex;

        private bool VisibleInnerCheck(I3DSceneShape shape)
        {
            while (true)
            {
                if (shape.Visibility == false)
                    return false;

                if (shape.Parent == null)
                    return true;

                shape = shape.Parent;
            }
        }

        public bool IsVisible()
        {
            if (Scene.NonRenderable)
                return false;

            return VisibleInnerCheck(Scene);
        }

        public static Texture2D LoadTextureDxt(byte[] ddsBytes, TextureFormat textureFormat)
        {
            if (textureFormat != TextureFormat.DXT1 && textureFormat != TextureFormat.DXT5)
                throw new Exception("Invalid TextureFormat. Only DXT1 and DXT5 formats are supported by this method.");

            byte ddsSizeCheck = ddsBytes[4];
            if (ddsSizeCheck != 124)
                throw new Exception("Invalid DDS DXTn texture. Unable to read");  //this header byte should be 124 for DDS image files

            int height = ddsBytes[13] * 256 + ddsBytes[12];
            int width = ddsBytes[17] * 256 + ddsBytes[16];

            const int ddsHeaderSize = 128;
            byte[] dxtBytes = new byte[ddsBytes.Length - ddsHeaderSize];
            Buffer.BlockCopy(ddsBytes, ddsHeaderSize, dxtBytes, 0, ddsBytes.Length - ddsHeaderSize);

            Texture2D texture = new Texture2D(width, height, textureFormat, false);
            texture.LoadRawTextureData(dxtBytes);
            texture.Apply();

            return (texture);
        }

        public Texture2D LoadTexture(string url)
        {
            byte[] bytes = System.IO.File.ReadAllBytes(url);
            return LoadTextureDxt(bytes, TextureFormat.DXT5);
        }

        public void Setup()
        {
            if (this.Scene.Material.TextureFile == null)
                return;

            Renderer re = this.renderer;
            Material mat = re.material;
            mat.mainTexture = LoadTexture(this.Scene.Material.TextureFile.AbsolutePath);
        }
        
        // Use this for initialization
        void Start () {
	        
        }
	
        // Update is called once per frame
        void Update () {
	
        }
    }
}
