using Assets.Components;
using UnityEngine;
using Assets.FarmSim.I3D;

namespace Assets
{
    public class Main : MonoBehaviour
    {
        public Entity GenericShape;

        private I3DImporter _importer;
        public Entity SpawnEntity(string file, Vector3 position)
        {
            Entity part = Instantiate(GenericShape, Vector3.zero, Quaternion.identity);
            if (part == null)
                return null;
            
            _importer.ParseFile(part, file);

            part.transform.localPosition = position;

            return part;
        }

        public void Start()
        {
            Debug.Log("Start");
            
            _importer = new I3DImporter();
            I3DImporter.GenericShape = GenericShape;

            //SpawnVehicle("F:/SteamLibrary/steamapps/common/Farming Simulator 15/data/vehicles/steerable/cars/piQup.i3d", Vector3.zero);
            /*
            for (int i = 0; i < 100; i++)
            {
                SpawnVehicle(@"F:\SteamLibrary\steamapps\common\Farming Simulator 15\data\vehicles\steerable\grimme\grimmeMaxtron620.i3d", new Vector3(10 * i, 0, 0));
            }
            */
            Entity world = SpawnEntity(@"F:\SteamLibrary\steamapps\common\Farming Simulator 15\data\maps\map01.i3d", Vector3.zero);
            //SpawnVehicle(@"D:\SteamLibrary\steamapps\common\Farming Simulator 2013\data\maps\map01.i3d", Vector3.zero);
            //SpawnVehicle(@"D:\SteamLibrary\steamapps\common\Farming Simulator 2013\data\maps\chickennavmesh.i3d", Vector3.zero);

            I3DTerrain i3Dterrain = world.gameObject.GetComponentInChildren<I3DTerrain>();
            i3Dterrain.BuildTerrainLayers();

        }
    }
}