
using UnityEngine;

namespace Assets.FarmSim.I3D
{
    public class I3DTerrain
    {
        public static float[,] ParseHeightMapFromDEM(Texture2D dem)
        {
            float[,] map = new float[dem.height, dem.width];

            Color32[] colorData = dem.GetPixels32(0);
            int demI = 0;
            for(int y = dem.height - 1; y >= 0; y--) // Mirrored in Y
            {
                for (int x = 0; x < dem.width; x++)
                {
                    Color32 pixel = colorData[demI];

                    // Height data is encoded in to both red and green channels
                    // The red channel contains a more significant height than the green channel
                    // One unit increase in the red channel equals to 256 unit increments in the green channel
                    
                    ushort height = (ushort)((pixel.r << 8) | pixel.g);
                    map[y, x] = height / 65535f;
                    
                    demI++;
                }
            }

            return map;
        }
    }
}
