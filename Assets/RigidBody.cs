using UnityEngine;

namespace Assets
{
    public enum RigidBodyType
    {
        Static = 0,
        Dynamic = 1,
        Kinematic = 2
    }
    public class RigidBody : MonoBehaviour
    {
        public RigidBodyType Type;
        public bool Compound;
        public bool CompoundChild;
        public bool Collision;
        public bool Trigger;
        public int CollisionMask;
        public float Restitution;
        public float StaticFriction;
        public float DynamicFriction;
        public float LinearDamping;
        public float AngularDamping;
        public float Density;
        public int SolverIterations;
        public float Mass;
    }
}
