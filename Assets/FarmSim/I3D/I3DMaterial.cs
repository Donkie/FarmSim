using UnityEngine;

namespace Assets.FarmSim.I3D
{
    public struct I3DMaterial
    {
        public string Name;
        public int Id;
        public int CustomShaderId;
        public int CosPower;
        public Color DiffuseColor;
        public Color SpecularColor;
        public Color AmbientColor;
        public bool AlphaBlending;

        public int TextureFileId;
        public int NormalMapFileId;
        public int GlossMapFileId;
        public int EmissiveMapFileId;
        public I3DMap ReflectionMap;
    }
}