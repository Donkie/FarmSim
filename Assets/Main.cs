using UnityEngine;

public class Main : MonoBehaviour
{
    public MeshFilter Vehicle;
	void Awake () {
        Debug.Log("Start");

        I3DImporter importer = new I3DImporter();
	    //importer.ObjFile = @"D:\SteamLibrary\SteamApps\common\Farming Simulator 2013\data\maps\map01.obj";
        //importer.ObjFile = @"D:\SteamLibrary\SteamApps\common\Farming Simulator 2013\data\vehicles\balers\kroneBigPack1290.obj";

        //I3DModel model = importer.ParseFile(@"D:\SteamLibrary\SteamApps\common\Farming Simulator 2013\data\maps\map01.i3d");
        I3DModel model = importer.ParseFile(@"D:\SteamLibrary\SteamApps\common\Farming Simulator 2013\data\vehicles\cars\car1.i3d");
        

	    MeshFilter modelmesh = (MeshFilter)Instantiate(Vehicle);
	    //modelmesh.mesh = model.Shapes[7].Mesh;
	}
}
