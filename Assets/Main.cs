using System;
using UnityEditor;
using UnityEngine;
using Assets.FarmSim.I3D;

namespace Assets
{
    public class Main : MonoBehaviour
    {
        public Entity GenericShape;

        private Entity SpawnVehicle_Rec(I3DModel model, I3DSceneShape shape, Entity parent)
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
                SpawnVehicle_Rec(model, s, part);
            }

            return part;
        }

        public Entity SpawnVehicle(I3DModel model)
        {
            I3DSceneShape shape = model.Scenes[0];

            return SpawnVehicle_Rec(model, shape, null);
        }

        public void Start()
        {
            Debug.Log("Start");

            I3DImporter importer = new I3DImporter();
            //importer.ObjFile = @"D:\SteamLibrary\SteamApps\common\Farming Simulator 2013\data\maps\map01.obj";
            //importer.ObjFile = @"D:\SteamLibrary\SteamApps\common\Farming Simulator 2013\data\vehicles\balers\kroneBigPack1290.obj";

            //I3DModel model = importer.ParseFile(@"D:\SteamLibrary\SteamApps\common\Farming Simulator 2013\data\maps\map01.i3d");
            I3DModel model =
                importer.ParseFile(
                    "F:/SteamLibrary/steamapps/common/Farming Simulator 15/data/vehicles/steerable/cars/piQup.i3d");

            SpawnVehicle(model);

            /*MeshFilter modelmesh = (MeshFilter) Instantiate(Vehicle);
            modelmesh.mesh = model.GetSceneById("car_01").GetSceneById("car_01_vis").Shape.Mesh;*/
        }
    }
}