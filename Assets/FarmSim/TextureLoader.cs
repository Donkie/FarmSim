using System;
using System.Collections.Generic;
using Pfim;
using UnityEngine;

namespace Assets.FarmSim
{
    public abstract class TextureLoader
    {
        private static readonly Dictionary<string, Texture2D> Cache = new Dictionary<string, Texture2D>();

        /// <summary>
        /// Parses any DXT texture from raw bytes including header using Pfim.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static Texture2D LoadTexturePfim(byte[] bytes)
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

        /// <summary>
        /// Parses a DXT1/3/5 texture from raw bytes including header. If DXT3 is detected, Pfim is used instead since Unity doesn't natively support DXT3.
        /// </summary>
        /// <param name="ddsBytes"></param>
        /// <returns></returns>
        private static Texture2D LoadTextureDxt(byte[] ddsBytes)
        {
            byte ddsSizeCheck = ddsBytes[0x4];
            if (ddsSizeCheck != 124)
                throw new Exception("Invalid DDS DXTn texture. Unable to read");  //this header byte should be 124 for DDS image files

            int height = ddsBytes[0xF] << 24 | ddsBytes[0xE] << 16 | ddsBytes[0xD] << 8 | ddsBytes[0xC];
            int width = ddsBytes[0x13] << 24 | ddsBytes[0x12] << 16 | ddsBytes[0x11] << 8 | ddsBytes[0x10];

            //int mipmapCount = ddsBytes[0x1F] << 24 | ddsBytes[0x1E] << 16 | ddsBytes[0x1D] << 8 | ddsBytes[0x1C];

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

        public static Texture2D GetTexture(string url)
        {
            if (Cache.ContainsKey(url))
            {
                return Cache[url];
            }

            byte[] bytes = System.IO.File.ReadAllBytes(url);

            Texture2D ret = LoadTextureDxt(bytes);
            Cache[url] = ret;
            return ret;
        }
    }
}
