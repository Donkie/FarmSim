namespace Assets.Components
{
    public enum RigidBodyType
    {
        Static = 0,
        Dynamic = 1,
        Kinematic = 2
    }
    public class RigidBody : ComponentBase
    {
        public RigidBodyType Type;
        /// <summary>
        /// Some sort of grouping of physobjects?
        /// </summary>
        public bool Compound;
        /// <summary>
        /// Tells us this is a child of a compound, should be physically active
        /// </summary>
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
