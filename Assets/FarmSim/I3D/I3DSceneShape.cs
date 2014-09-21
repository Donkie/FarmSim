using System;
using UnityEngine;

namespace Assets.FarmSim.I3D
{
    public struct I3DSceneShape
    {
        public string Name;
        public float Density;
        public int ShapeId;
        public int NodeId;
        public int MaterialIds;
        public int CollisionMask;
        public int ObjectMask;
        public int ClipDistance;
        public bool CastShadows;
        public bool ReceiveShadows;
        public bool NonRenderable;
        public bool Visibility;
        public bool Kinematic;
        public bool Trigger;
        public bool Dynamic;
        public Vector3 Translation;
        public I3DSceneShape[] SceneShapes;
    }
}
