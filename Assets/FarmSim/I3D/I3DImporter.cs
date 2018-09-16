using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Assets.Components;
using UnityEngine;
using I3DShapesTool;

namespace Assets.FarmSim.I3D
{
    public class I3DImporter
    {

        #region ParseTypes

        private static readonly Regex colorRegex = new Regex(@"([0-9\.e\-]+)");

        private static Color ParseColor(string col)
        {
            if (col == null)
                return Color.black;

            Color output = Color.black;

            int i = 0;
            foreach (Match match in colorRegex.Matches(col))
            {
                if (match.Success)
                    output[i] = float.Parse(match.Value, CultureInfo.InvariantCulture);

                i++;
            }

            return output;
        }

        private static Vector2 ParseVector2(string vec2)
        {
            if (vec2 == null)
                return new Vector2();

            Vector2 output = new Vector2();

            int i = 0;
            foreach (Match match in colorRegex.Matches(vec2))
            {
                if (match.Success)
                    output[i] = float.Parse(match.Value, NumberStyles.Float);

                i++;
            }

            return output;
        }

        private static Vector3 ParseVector3(string vec3)
        {
            if (vec3 == null)
                return new Vector3();

            Vector3 output = new Vector3();

            int i = 0;
            foreach (Match match in colorRegex.Matches(vec3))
            {
                if (match.Success)
                    output[i] = ParseFloat(match.Value);

                i++;
            }

            return output;
        }

        private static int ParseInt(string sI, int i = 0)
        {
            int iOut;
            return int.TryParse(sI, out iOut) ? iOut : i;
        }

        private static float ParseFloat(string sI, float f = 0f)
        {
            float iOut;
            return float.TryParse(sI, NumberStyles.Float, CultureInfo.InvariantCulture, out iOut) ? iOut : f;
        }

        private static bool ParseBool(string sI, bool def = false)
        {
            bool iOut;
            return bool.TryParse(sI, out iOut) ? iOut : def;
        }

        private static string ParseString(string sI, string s = "")
        {
            return sI ?? s;
        }

        #endregion

        private string ReplaceExtension(string path, string newext)
        {
            string[] newstr = path.Split('.');
            newstr[newstr.Length - 1] = newext;
            return string.Join(".", newstr);
        }

        private void ParseFile_Files(ref I3DModel model, XmlReader xml)
        {
            Stack<I3DFile> files = new Stack<I3DFile>();

            xml.Read();

            while (xml.NodeType != XmlNodeType.EndElement)
            {
                if (xml.NodeType == XmlNodeType.Whitespace)
                {
                    xml.Read();
                    continue;
                }

                string sFileId = xml.GetAttribute("fileId");
                string sRelativePath = xml.GetAttribute("relativePath");
                if (sFileId != null && sRelativePath != null)
                {
                    I3DFile file = new I3DFile
                    {
                        FileName = xml.GetAttribute("filename"),
                        Id = int.Parse(sFileId),
                        RelativePath = bool.Parse(sRelativePath)
                    };

                    string abspath = Path.Combine(model.Path, file.FileName);
                    if (!File.Exists(file.AbsolutePath))
                    {
                        string newname = ReplaceExtension(file.FileName, "dds");
                        abspath = Path.Combine(model.Path, newname);
                        /*if(!File.Exists(abspath))
                            throw new FileNotFoundException(abspath);*/
                        file.FileName = newname;
                    }
                    file.AbsolutePath = abspath;

                    files.Push(file);
                }
                xml.Read();
            }

            model.Files = files.ToArray();
        }

        private static void ParseFile_Materials(ref I3DModel model, XmlReader xml)
        {
            Stack<I3DMaterial> mats = new Stack<I3DMaterial>();

            xml.Read();

            while (xml.NodeType != XmlNodeType.EndElement)
            {
                if (xml.NodeType == XmlNodeType.Whitespace)
                {
                    xml.Read();
                    continue;
                }

                string sName = xml.GetAttribute("name");
                string sMatId = xml.GetAttribute("materialId");
                if (sName != null && sMatId != null)
                {
                    I3DMaterial mat = new I3DMaterial()
                    {
                        Name = sName,
                        Id = ParseInt(sMatId),
                        CosPower = ParseInt(xml.GetAttribute("cosPower")),
                        AlphaBlending = ParseBool(xml.GetAttribute("alphaBlending")),
                        CustomShaderId = ParseInt(xml.GetAttribute("customShaderId")),
                        AmbientColor = ParseColor(xml.GetAttribute("ambientColor")),
                        DiffuseColor = ParseColor(xml.GetAttribute("diffuseColor")),
                        SpecularColor = ParseColor(xml.GetAttribute("specularColor"))
                    };

                    xml.Read();
                    while (xml.NodeType != XmlNodeType.EndElement)
                    {
                        int fileId = ParseInt(xml.GetAttribute("fileId"));

                        I3DFile file = model.Files.FirstOrDefault(f => f.Id == fileId);

                        switch (xml.LocalName)
                        {
                            case "Emissivemap":
                                mat.EmissiveMapFile = file;
                                mat.EmissiveMapFileId = fileId;
                                break;
                            case "Texture":
                                mat.TextureFile = file;
                                mat.TextureFileId = fileId;
                                break;
                            case "Normalmap":
                                mat.NormalMapFile = file;
                                mat.NormalMapFileId = fileId;
                                break;
                            case "Glossmap":
                                mat.GlossMapFile = file;
                                mat.GlossMapFileId = fileId;
                                break;
                            case "Reflectionmap":
                                mat.ReflectionMap = file;
                                mat.ReflectionMapId = fileId;
                                break;
                        }

                        xml.Read();
                    }

                    mats.Push(mat);
                }
                xml.Read();
            }

            model.Materials = mats.ToArray();
        }

        private static void ParseFile_Shapes_VertSet(ref Stack<I3DShapeData> output, XmlReader xml)
        {
            string sId = xml.GetAttribute("shapeId");
            if (sId == null)
                return;

            I3DShapeData shape = new I3DShapeData
            {
                Name = xml.GetAttribute("name"),
                ID = int.Parse(sId)
            };
            XmlReaderExt.Read(xml);

            Mesh mesh = new Mesh();
            while (xml.NodeType != XmlNodeType.EndElement)
            {
                if (xml.NodeType == XmlNodeType.Whitespace)
                {
                    xml.Read();
                    continue;
                }

                string sCnt;
                int cnt;
                switch (xml.LocalName)
                {
                    case "Vertices":
                        sCnt = xml.GetAttribute("count");
                        if (sCnt == null)
                            break;

                        cnt = int.Parse(sCnt);
                        Vector3[] verts = new Vector3[cnt];
                        Vector3[] norms = new Vector3[cnt];
                        Vector2[] uvs = new Vector2[cnt];
                        for (int i = 0; i < cnt; i++)
                        {
                            XmlReaderExt.Read(xml);

                            verts[i] = ParseVector3(xml.GetAttribute("p"));
                            norms[i] = ParseVector3(xml.GetAttribute("n"));
                            Vector2 uv = ParseVector2(xml.GetAttribute("t0"));
                            uv.y = 1 - uv.y;
                            uvs[i] = uv;
                        }

                        XmlReaderExt.Read(xml);

                        mesh.vertices = verts;
                        mesh.normals = norms;
                        mesh.uv = uvs;

                        break;
                    case "Triangles":
                        sCnt = xml.GetAttribute("count");
                        if (sCnt == null)
                            break;

                        XmlReaderExt.Read(xml);

                        cnt = int.Parse(sCnt);
                        int[] tris = new int[cnt*3];
                        for (int i = 0; i < cnt*3; i += 3)
                        {
                            XmlReaderExt.Read(xml);

                            string vi = xml.GetAttribute("vi");
                            if (vi == null)
                                continue;

                            MatchCollection matches = colorRegex.Matches(vi);
                            for (int j = 0; j < 3; j++)
                            {
                                tris[i + j] = int.Parse(matches[j].Value);
                            }
                        }

                        mesh.triangles = tris;
                        break;
                    default:
                        while (xml.Read())
                        {
                            if (xml.NodeType == XmlNodeType.EndElement)
                                break;
                        }
                        break;
                }
                XmlReaderExt.Read(xml);
            }

            mesh.RecalculateBounds();
            shape.Mesh = mesh;
            output.Push(shape);

            XmlReaderExt.Read(xml);
        }
        
        private static void ParseFile_Shapes(ref I3DModel output, XmlReader xml)
        {
            //XmlReaderExt.Read(xml);

            Stack<I3DShapeData> shapes = new Stack<I3DShapeData>();

            xml.MoveToFirstAttribute();
            
            string attr = xml.GetAttribute("externalShapesFile");
            if (attr != null)
            {
                string fullPath = Path.Combine(output.Path, attr);
                Debug.Log($"Shapes full path: {fullPath}");
                if (!File.Exists(fullPath))
                    throw new FileNotFoundException($"{fullPath} not found.");


                I3DShape[] toolShapes = I3DShapeTool.ParseShapesFile(fullPath);
                foreach (I3DShape toolShape in toolShapes)
                {
                    shapes.Push(new I3DShapeData
                    {
                        ID = toolShape.ShapeId,
                        Name = toolShape.Name,
                        Mesh = toolShape.Mesh
                    });
                }

                output.ShapeDatas = shapes.ToArray();

                return;
            }

            while (xml.NodeType != XmlNodeType.EndElement)
            {
                if (xml.NodeType == XmlNodeType.Whitespace)
                {
                    XmlReaderExt.Read(xml);
                    continue;
                }

                switch (xml.LocalName)
                {
                    case "IndexedTriangleSet":
                        ParseFile_Shapes_VertSet(ref shapes, xml);
                        break;
                }
            }

            output.ShapeDatas = shapes.ToArray();
        }

        private static void ParseFile_SceneShapesAttributes(XmlReader xml, ref I3DModel model, Entity part)
        {
            if (xml.GetAttribute("name") != null)
            {
                //I3DTransform
                I3DTransform t = part.gameObject.AddComponent<I3DTransform>();
                t.Name = ParseString(xml.GetAttribute("name"));
                t.Id = ParseInt(xml.GetAttribute("nodeId"));
                t.IndexPath = ""; //TODO: Make this
                t.Visibility = ParseBool(xml.GetAttribute("visibility"), true);
                t.ClipDistance = ParseInt(xml.GetAttribute("clipDistance"), 1000000);
                t.MinClipDistance = ParseInt(xml.GetAttribute("minClipDistance"));
                t.ObjectMask = ParseInt(xml.GetAttribute("objectMask"), 0xffff);
                t.LOD = ParseBool(xml.GetAttribute("lodDistance"));
                
                //Transform
                part.gameObject.transform.localPosition = ParseVector3(xml.GetAttribute("translation"));
                part.gameObject.transform.localEulerAngles = ParseVector3(xml.GetAttribute("rotation"));
                Vector3 scale = xml.GetAttribute("scale") != null ? ParseVector3(xml.GetAttribute("scale")) : Vector3.one;
                //scale.x *= -1;
                part.gameObject.transform.localScale = scale;

                //Rigidbody
                bool bStatic = ParseBool(xml.GetAttribute("static"));
                bool bDynamic = ParseBool(xml.GetAttribute("dynamic"));
                bool bKinematic = ParseBool(xml.GetAttribute("kinematic"));

                if (bStatic || bDynamic || bKinematic)
                {
                    t.RigidBody = true;

                    I3DRigidBody r = part.gameObject.AddComponent<I3DRigidBody>();
                    if(bStatic)
                        r.Type = RigidBodyType.Static;
                    else if (bDynamic)
                        r.Type = RigidBodyType.Dynamic;
                    else
                        r.Type = RigidBodyType.Kinematic;

                    r.Compound = ParseBool(xml.GetAttribute("compound"));
                    r.CompoundChild = ParseBool(xml.GetAttribute("compoundChild"));
                    r.Collision = ParseBool(xml.GetAttribute("collision"), true);
                    r.Trigger = ParseBool(xml.GetAttribute("trigger"));
                    r.CollisionMask = ParseInt(xml.GetAttribute("collisionMask"));
                    r.Restitution = ParseFloat(xml.GetAttribute("restitution"));
                    r.StaticFriction = ParseFloat(xml.GetAttribute("staticFriction"), 0.5f);
                    r.DynamicFriction = ParseFloat(xml.GetAttribute("dynamicFriction"), 0.5f);
                    r.LinearDamping = ParseFloat(xml.GetAttribute("linearDamping"), 0.5f);
                    r.AngularDamping = ParseFloat(xml.GetAttribute("angularDamping"), 0.5f);
                    r.Density = ParseFloat(xml.GetAttribute("density"), 1);
                    r.SolverIterations = ParseInt(xml.GetAttribute("solverIterationCount"), 4);
                    //r.Mass = ParseFloat(xml.GetAttribute("mass"), 0.5f);
                }

                //Shape
                if (xml.LocalName == "Shape")
                {
                    Shape s = part.gameObject.AddComponent<Shape>();
                    s.ID = ParseInt(xml.GetAttribute("shapeId"));
                    s.CastShadows = ParseBool(xml.GetAttribute("castsShadows"));
                    s.ReceiveShadows = ParseBool(xml.GetAttribute("receiveShadows"));
                    s.NonRenderable = ParseBool(xml.GetAttribute("nonRenderable"));
                    s.BuildNavMeshMask = ParseInt(xml.GetAttribute("buildNavMeshMask"), 0x0);
                    s._Materials = Shape.ParseMaterialString(ParseString(xml.GetAttribute("materialIds")));

                    part.Shape = s;
                }
            }

            if (part.Shape != null)
            {
                //Assign the shape object according to ID
                foreach (I3DShapeData sh in model.ShapeDatas)
                {
                    if (part.Shape.ID != sh.ID)
                        continue;

                    part.Shape.ID = sh.ID;
                    part.Shape.Name = sh.Name;
                    part.Shape.Mesh = sh.Mesh;

                    part.GetComponent<MeshFilter>().mesh = sh.Mesh;
                    break;
                }

                //Assign material according to ID
                part.Shape.Materials = new I3DMaterial[part.Shape._Materials.Length];

                for (int i = 0; i < part.Shape._Materials.Length; i++)
                {
                    int s = part.Shape._Materials[i];
                    foreach (I3DMaterial mat in model.Materials)
                    {
                        if (mat.Id != s)
                            continue;

                        part.Shape.Materials[i] = mat;
                    }
                }
            }
        }

        private static void SetParent(Transform parent, Transform child)
        {
            Vector3 localPosition = child.localPosition;
            Vector3 localAngle = child.localEulerAngles;
            Vector3 localScale = child.localScale;

            child.parent = parent;

            child.localPosition = localPosition;
            child.localEulerAngles = localAngle;
            child.localScale = localScale;
        }

        public static Entity GenericShape;
        private static Entity ParseFile_SceneShapesInner(ref I3DModel model, XmlReader xml)
        {
            Entity part = UnityEngine.Object.Instantiate(GenericShape, Vector3.zero, Quaternion.identity);
            if (part == null)
                return null;

            ParseFile_SceneShapesAttributes(xml, ref model, part);

            part.Setup();
            
            bool empty = xml.IsEmptyElement;

            if (!empty)
            {
                XmlReaderExt.Read(xml);
                while (XmlReaderExt.SafeCheckEndElement(xml))
                {
                    Entity child = ParseFile_SceneShapesInner(ref model, xml);
                    SetParent(part.transform, child.transform);
                    XmlReaderExt.Read(xml);
                }
            }
            
            return part;
        }
        private static void ParseFile_SceneShapes(ref I3DModel model, Entity part, XmlReader xml)
        {
            xml.Read();

            while (XmlReaderExt.SafeCheckEndElement(xml))
            {
                if (xml.NodeType == XmlNodeType.Whitespace)
                {
                    xml.Read();
                    continue;
                }

                ParseFile_SceneShapesAttributes(xml, ref model, part);

                part.Setup();

                XmlReaderExt.Read(xml);

                //Parse any inner shape
                bool empty = xml.IsEmptyElement;
                while (!empty && XmlReaderExt.SafeCheckEndElement(xml))
                {
                    Entity child = ParseFile_SceneShapesInner(ref model, xml);
                    SetParent(part.transform, child.transform);
                    XmlReaderExt.Read(xml);
                }
                
                xml.Read();
            }
        }
        
        public string PrintTime(ref int start)
        {
            string s = "Time Elapsed: " + (Environment.TickCount - start) + "ms";
            start = Environment.TickCount;
            return s;
        }

        public I3DModel ParseFile(Entity part, string file)
        {
            if (!File.Exists(file))
                throw new FileNotFoundException("File not found.", file);

            if (Path.GetExtension(file) != ".i3d")
                throw new Exception("File not I3D format.");

            I3DModel output = new I3DModel {Path = Path.GetDirectoryName(file)};

            string contents = File.ReadAllText(file);

            int start = Environment.TickCount;
            using (XmlReader xml = XmlReader.Create(new StringReader(contents)))
            {
                xml.ReadToFollowing("i3D"); //Skip all the initial shit

                output.Name = xml.GetAttribute("name");

                while (XmlReaderExt.ReadToNextElement(xml))
                {
                    switch (xml.LocalName)
                    {
                        case "Files":
                            ParseFile_Files(ref output, xml);
                            Debug.Log("Parsing Files: " + PrintTime(ref start));
                            break;
                        case "Materials":
                            ParseFile_Materials(ref output, xml);
                            Debug.Log("Parsing Materials: " + PrintTime(ref start));
                            break;
                        case "Shapes":
                            ParseFile_Shapes(ref output, xml);
                            Debug.Log("Parsing Shapes: " + PrintTime(ref start));
                            break;
                        case "Scene":
                            ParseFile_SceneShapes(ref output, part, xml);
                            Debug.Log("Parsing Scenes: " + PrintTime(ref start));
                            break;
                    }
                }
            }

            return output;
        }
    }
}