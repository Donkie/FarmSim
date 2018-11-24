using UnityEngine;

namespace Assets.Components
{
    public class I3DTerrainLayer
    {
        public int Priority { get; set; }
        public Vector4 Attributes { get; set; }
        public SplatPrototype SplatMap { get; set; }
        public float[,] Weights { get; set; }
    }
}
