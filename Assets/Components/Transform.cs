using UnityEngine;

namespace Assets.Components
{
    public class Transform : ComponentBase
    {
        public string Name;
        public int Id;
        public string IndexPath;
        public Vector3 Translation;
        public Vector3 Rotation;
        public Vector3 Scale;
        public bool Visibility;
        public int ClipDistance;
        public int MinClipDistance;
        public int ObjectMask;
        public bool LOD;
        public bool RigidBody;
        public bool Joint;
    }
}
