using System;
using System.Linq;
using UnityEngine;

namespace Assets.FarmSim.I3D
{
    public class I3DSceneShape
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
        public bool Visibility = true;
        public bool Kinematic;
        public bool Trigger;
        public bool Dynamic;
        public Vector3 Translation;
        public I3DSceneShape[] Scenes;
        public I3DShape Shape;
        public I3DSceneShape Parent;
        public I3DMaterial Material;

        public I3DSceneShape GetSceneById(string id)
        {
            foreach (I3DSceneShape s in Scenes.Where(s => s.Name == id))
                return s;

            throw new Exception("Invalid Scene with id " + id);
        }
    }
}
