using System;
using System.Collections;
using Assets.FarmSim.I3D;
using UnityEngine;

namespace Assets
{
    public class Entity : MonoBehaviour
    {
        public I3DSceneShape Scene;
        [HideInInspector]
        public Texture2D Tex;

        public string Name => Scene.Name;

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

        private bool DeepIsVisible()
        {
            if (Scene.NonRenderable)
                return false;

            return VisibleInnerCheck(Scene);
        }

        public static Texture2D LoadTextureDxt(byte[] ddsBytes, TextureFormat textureFormat = TextureFormat.DXT5)
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
            if (Scene.Material.TextureFile != null)
            {
                Material mat = GetComponent<Renderer>().material;
                mat.SetTexture("_MainTex", LoadTexture(Scene.Material.TextureFile.AbsolutePath));

                if (Scene.Material.NormalMapFile != null)
                    mat.SetTexture("_BumpMap", LoadTexture(Scene.Material.NormalMapFile.AbsolutePath));

                if (Scene.Material.ReflectionMap != null)
                    mat.SetTexture("_Cube", LoadTexture(Scene.Material.ReflectionMap.AbsolutePath));
            }

            //Assign name
            name = Scene.Name;

            //Check visibility
            Visible = DeepIsVisible();
            //Visible = !Scene.NonRenderable;
            //Visible = true;
        }
    }
}
