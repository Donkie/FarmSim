using Assets.FarmSim.I3D;
using UnityEngine;

namespace Assets
{
    public class Shape : MonoBehaviour
    {
        public bool CastShadows;
        public bool ReceiveShadows;
        public bool NonRenderable;
        public int BuildNavMeshMask = 0;
        public I3DMaterial[] Materials = new I3DMaterial[8];
    }
}
