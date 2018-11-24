using System.Linq;

namespace Assets.FarmSim.I3D
{
    public class I3DModel
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public I3DFile[] Files { get; set; }
        public I3DMaterial[] Materials { get; set; }
        public I3DShapeData[] ShapeDatas { get; set; }

        public I3DFile GetFile(int fileId)
        {
            return Files.FirstOrDefault(f => f.Id == fileId);
        }

        /*
        public I3DSceneShape GetSceneById(string id)
        {
            foreach (I3DSceneShape s in Scenes.Where(s => s.Name == id))
                return s;

            throw new Exception("Invalid Scene with id " + id);
        }
        */
    }
}