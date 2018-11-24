using System;
using System.Collections.Generic;
using Pfim;
using UnityEngine;

namespace Assets.FarmSim
{
    public abstract class TextureLoader
    {
        private static readonly Dictionary<string, Texture2D> Cache = new Dictionary<string, Texture2D>();

        private static Texture2D LoadTextureFromBytes(byte[] bytes, int width, int height, TextureFormat format, bool mipmap)
        {
            Texture2D texture;
            try
            {
                texture = new Texture2D(width, height, format, mipmap, false);
                texture.LoadRawTextureData(bytes);
                texture.Apply();
            }
            catch (UnityException)
            {
                // Failed to load, usually is due to the texture data specifying that it has mipmaps but then doesn't contain enough data for all mipmaps??
                // Load the texture without mipmap support
                // In future: add some method for unity to compute mipmaps here instead
                texture = new Texture2D(width, height, format, false, false);
                texture.LoadRawTextureData(bytes);
                texture.Apply();
            }

            return texture;
        }

        /// <summary>
        /// Parses any DXT texture from raw bytes including header using Pfim.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="mipmap">Does the texture contain mipmap data</param>
        /// <returns></returns>
        private static Texture2D LoadTexturePfim(byte[] bytes, bool mipmap)
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

            return LoadTextureFromBytes(imageData.Data, imageData.Width, imageData.Height, texFrmt, mipmap);
        }

        /// <summary>
        /// Parses a DDS texture from raw bytes including header. If DXT3 is detected, Pfim is used instead since Unity doesn't natively support DXT3.
        /// </summary>
        /// <param name="ddsBytes"></param>
        /// <returns></returns>
        private static Texture2D LoadTextureDDS(byte[] ddsBytes)
        {
            byte ddsSizeCheck = ddsBytes[0x4];
            if (ddsSizeCheck != 124)
                throw new Exception("Invalid DDS DXTn texture. Unable to read");  //this header byte should be 124 for DDS image files

            int height = ddsBytes[0xF] << 24 | ddsBytes[0xE] << 16 | ddsBytes[0xD] << 8 | ddsBytes[0xC];
            int width = ddsBytes[0x13] << 24 | ddsBytes[0x12] << 16 | ddsBytes[0x11] << 8 | ddsBytes[0x10];

            int mipmapCount = ddsBytes[0x1F] << 24 | ddsBytes[0x1E] << 16 | ddsBytes[0x1D] << 8 | ddsBytes[0x1C];
            bool hasMipMaps = mipmapCount > 0;

            TextureFormat textureFormat;
            string texFormatStr = System.Text.Encoding.ASCII.GetString(ddsBytes, 0x54, 4);
            switch (texFormatStr)
            {
                case "DXT1":
                    textureFormat = TextureFormat.DXT1;
                    break;
                case "DXT3":
                    return LoadTexturePfim(ddsBytes, hasMipMaps);
                case "DXT5":
                    textureFormat = TextureFormat.DXT5;
                    break;
                default:
                    throw new Exception($"Unknown texture format {texFormatStr}");
            }

            const int ddsHeaderSize = 128;
            byte[] dxtBytes = new byte[ddsBytes.Length - ddsHeaderSize];
            Buffer.BlockCopy(ddsBytes, ddsHeaderSize, dxtBytes, 0, ddsBytes.Length - ddsHeaderSize);
            
            return LoadTextureFromBytes(dxtBytes, width, height, textureFormat, hasMipMaps);
        }

        private static Texture2D LoadTexturePNGOrJPG(byte[] bytes)
        {
            Texture2D ret = new Texture2D(1, 1);
            ret.LoadImage(bytes);
            return ret;
        }

        public static Texture2D GetTexture(string url)
        {
            if (Cache.ContainsKey(url))
            {
                return Cache[url];
            }

            byte[] bytes = System.IO.File.ReadAllBytes(url);

            Texture2D ret;

            if (bytes[0] == 'D' && bytes[1] == 'D' && bytes[2] == 'S') // DDS
                ret = LoadTextureDDS(bytes);
            else if(bytes[1] == 'P' && bytes[2] == 'N' && bytes[3] == 'G') // PNG
                ret = LoadTexturePNGOrJPG(bytes);
            else if (bytes[0] == 0xFF && bytes[1] == 0xD8) // JPG
                ret = LoadTexturePNGOrJPG(bytes);
            else
                throw new Exception($"Unknown image format for {url}");

            Cache[url] = ret;
            return ret;
        }
    }
}
