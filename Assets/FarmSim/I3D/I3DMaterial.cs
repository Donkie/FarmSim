using System.Collections.Generic;
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

        public I3DFile TextureFile;
        public I3DFile NormalMapFile;
        public I3DFile GlossMapFile;
        public I3DFile EmissiveMapFile;
        public I3DFile ReflectionMap;
        
        public Dictionary<string, I3DFile> CustomMaps;

        public Dictionary<string, string> CustomParameters;
    }
}