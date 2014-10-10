using System;
using System.Linq;
using Assets.Components;
using UnityEngine;
using Transform = Assets.Components.Transform;

namespace Assets.FarmSim.I3D
{
    public class I3DSceneShape : ComponentBase
    {
        public string Name
        {
            get { return GetComponent<Transform>().Name; }
        }

        public Vector3 Translation
        {
            get { return GetComponent<Transform>().Translation; }
        }

        public bool Visibility
        {
            get { return GetComponent<Transform>().Visibility; }
        }

        public bool NonRenderable
        {
            get { return GetComponent<Shape>().NonRenderable; }
        }

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
