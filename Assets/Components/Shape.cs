using Assets.FarmSim.I3D;
using UnityEngine;

namespace Assets.Components
{
    public class Shape : MonoBehaviour
    {
        public string Name;
        public int ID;
        public Mesh Mesh;
        public bool CastShadows;
        public bool ReceiveShadows;
        public bool NonRenderable;
        public int BuildNavMeshMask = 0;
        public I3DMaterial[] Materials;
        public int[] _Materials;

        public static int[] ParseMaterialString(string s)
        {
            string[] strings = s.Split(new [] { ", " }, System.StringSplitOptions.RemoveEmptyEntries);
            int[] outints = new int[strings.Length];
            for (int i = 0; i < strings.Length; i++)
            {
                outints[i] = int.Parse(strings[i]);
            }
            return outints;
        }
    }
}
