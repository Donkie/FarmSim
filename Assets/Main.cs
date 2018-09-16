using System;
using UnityEditor;
using UnityEngine;
using Assets.FarmSim.I3D;

namespace Assets
{
    public class Main : MonoBehaviour
    {
        public Entity GenericShape;
        /*
        private Entity SpawnVehicleRecurse(I3DModel model, I3DSceneShape shape, Entity parent)
        {
            Entity part = Instantiate(GenericShape, shape.Translation, Quaternion.identity);
            if (part == null)
                return null;

            part.Scene = shape;
            part.Setup();

            if (parent != null)
                part.transform.parent = parent.transform;

            if (shape.Shape.Mesh != null)
            {
                MeshFilter mesh = part.GetComponent<MeshFilter>();
                mesh.mesh = shape.Shape.Mesh;
            }



            foreach (I3DSceneShape s in shape.Scenes)
            {
                SpawnVehicleRecurse(model, s, part);
            }

            return part;
        }
        */
        private I3DImporter _importer;
        public Entity SpawnVehicle(string file, Vector3 position)
        {
            Entity part = Instantiate(GenericShape, position, Quaternion.identity);
            if (part == null)
                return null;
            
            I3DModel model = _importer.ParseFile(part, file);
            return part;
            //I3DSceneShape shape = model.Scenes[0];

            //return SpawnVehicleRecurse(model, shape, null);
        }

        public void Start()
        {
            Debug.Log("Start");
            
            _importer = new I3DImporter();
            I3DImporter.GenericShape = GenericShape;

            SpawnVehicle("F:/SteamLibrary/steamapps/common/Farming Simulator 15/data/vehicles/steerable/cars/piQup.i3d", Vector3.zero);
            
        }
    }
}