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
        public I3DFile TextureFile;
        public int NormalMapFileId;
        public I3DFile NormalMapFile;
        public int GlossMapFileId;
        public I3DFile GlossMapFile;
        public int EmissiveMapFileId;
        public I3DFile EmissiveMapFile;
        public I3DMap ReflectionMap;
    }
}