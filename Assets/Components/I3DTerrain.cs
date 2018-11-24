using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Components
{
    public class I3DTerrain : MonoBehaviour
    {
        public List<I3DTerrainLayer> Layers = new List<I3DTerrainLayer>();
        public int AlphaWidth = 1024;
        public int AlphaHeight = 1024;

        public Texture2D HeightMap;

        public Terrain Terrain;
        public TerrainData TerrainData;
        public TerrainCollider TerrainCollider;
        public float HeightMapScale;
        public float HeightMapUnitsPerPixel;

        public void BuildHeightmap()
        {
            Assert.AreEqual(HeightMap.width, HeightMap.height, "Height map width must be equal to its height");

            int mapRes = HeightMap.width;

            TerrainData.heightmapResolution = mapRes;
            TerrainData.SetHeights(0, 0, FarmSim.I3D.I3DTerrainUtil.Parse16BitMap(HeightMap));
            TerrainData.size = new Vector3(
                HeightMapUnitsPerPixel * (mapRes - 1),
                HeightMapScale,
                HeightMapUnitsPerPixel * (mapRes - 1)
            );

            gameObject.transform.localPosition += new Vector3(-TerrainData.size.x / 2, 0, -TerrainData.size.z / 2);
        }

        public void BuildTerrainLayers()
        {
            Assert.AreEqual(AlphaWidth, AlphaHeight, "Alpha map width must be equal to its height");

            int numLayers = Layers.Count;

            SplatPrototype[] splats = new SplatPrototype[numLayers];
            float[,,] weights = new float[AlphaWidth, AlphaHeight, numLayers];

            int i = 0;
            foreach(I3DTerrainLayer layer in Layers.OrderBy(x => x.Priority))
            {
                splats[i] = layer.SplatMap;
                
                for (int y = 0; y < AlphaHeight; y++)
                {
                    for (int x = 0; x < AlphaWidth; x++)
                    {
                        weights[x, y, i] = layer.Weights[x, y];
                    }
                }

                i++;
            }

            TerrainData.alphamapResolution = AlphaWidth;
            TerrainData.splatPrototypes = splats;
            TerrainData.SetAlphamaps(0, 0, weights);
            
            Terrain.terrainData = TerrainData;
            TerrainCollider.terrainData = TerrainData;
        }
    }
}
