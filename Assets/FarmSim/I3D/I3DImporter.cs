using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;

namespace Assets.FarmSim.I3D
{
    public class I3DImporter
    {
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
                    I3DFile i3DFile = new I3DFile
                    {
                        FileName = xml.GetAttribute("filename"),
                        Id = int.Parse(sFileId),
                        RelativePath = bool.Parse(sRelativePath)
                    };
                    files.Push(i3DFile);
                }
                xml.Read();
            }

            model.Files = files.ToArray();
        }

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
                    output[i] = float.Parse(match.Value);

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
                    output[i] = float.Parse(match.Value, NumberStyles.Float);

                i++;
            }

            return output;
        }

        private static int ParseInt(string sI)
        {
            int iOut;
            return int.TryParse(sI, out iOut) ? iOut : 0;
        }

        private static float ParseFloat(string sI)
        {
            float iOut;
            return float.TryParse(sI, out iOut) ? iOut : 0f;
        }

        private static bool ParseBool(string sI)
        {
            bool iOut;
            return bool.TryParse(sI, out iOut) && iOut;
        }

        private static string ParseString(string sI)
        {
            return sI ?? "";
        }

        #endregion

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
                        string sFileId;
                        switch (xml.LocalName)
                        {
                            case "Emissivemap":
                                sFileId = xml.GetAttribute("fileId");
                                if (sFileId == null)
                                    break;

                                mat.EmissiveMapFileId = int.Parse(sFileId);
                                break;
                            case "Texture":
                                sFileId = xml.GetAttribute("fileId");
                                if (sFileId == null)
                                    break;

                                mat.TextureFileId = int.Parse(sFileId);
                                break;
                            case "Normalmap":
                                sFileId = xml.GetAttribute("fileId");
                                if (sFileId == null)
                                    break;

                                mat.NormalMapFileId = int.Parse(sFileId);
                                break;
                            case "Glossmap":
                                sFileId = xml.GetAttribute("fileId");
                                if (sFileId == null)
                                    break;

                                mat.GlossMapFileId = int.Parse(sFileId);
                                break;
                            case "Reflectionmap":
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

        private static void ParseFile_Shapes_VertSet(ref Stack<I3DShape> output, XmlReader xml)
        {
            Debug.Log("VertSet");
            string sId = xml.GetAttribute("shapeId");
            if (sId == null)
                return;

            I3DShape shape = new I3DShape()
            {
                Name = xml.GetAttribute("name"),
                Id = int.Parse(sId)
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

                Debug.Log(xml.NodeType);
                Debug.Log(xml.LocalName);
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
                            uvs[i] = ParseVector2(xml.GetAttribute("t0"));
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

        private static void ParseFile_Shapes_Obj(ref Stack<I3DShape> output, StreamReader reader)
        {
            Debug.Log("hi");
        }

        private static void ParseFile_Shapes(ref I3DModel output, XmlReader xml, string ObjFile)
        {
            XmlReaderExt.Read(xml);

            Stack<I3DShape> shapes = new Stack<I3DShape>();

            Debug.Log("ParseFile_Shapes");
            Debug.Log("LocalName: " + xml.LocalName);
            Debug.Log("Has attributes: " + xml.HasAttributes);
            Debug.Log("Attribute Count: " + xml.AttributeCount);
            //If we detect that the model wants external file, we look for and parse the Obj file instead
            string attr = xml.GetAttribute("externalShapesFile");
            Debug.Log("externalShapesFile: " + attr);
            Debug.Log("Use .obj File: " + (ObjFile.Length > 0));

            if (attr != null && ObjFile.Length > 0)
            {
                if (!File.Exists(ObjFile))
                    throw new FileNotFoundException("Obj file not found.");

                using (StreamReader reader = new StreamReader(ObjFile))
                {
                    ParseFile_Shapes_Obj(ref shapes, reader);
                }

                output.Shapes = shapes.ToArray();
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

            output.Shapes = shapes.ToArray();
        }

        private static I3DSceneShape ParseFile_SceneShapesAttributes(XmlReader xml)
        {
            return new I3DSceneShape()
            {
                Name = ParseString(xml.GetAttribute("name")),
                ShapeId = ParseInt(xml.GetAttribute("shapeId")),
                NodeId = ParseInt(xml.GetAttribute("nodeId")),
                Dynamic = ParseBool(xml.GetAttribute("dynamic")),
                Density = ParseFloat(xml.GetAttribute("density")),
                CollisionMask = ParseInt(xml.GetAttribute("collisionMask")),
                CastShadows = ParseBool(xml.GetAttribute("castsShadows")),
                ReceiveShadows = ParseBool(xml.GetAttribute("receiveShadows")),
                NonRenderable = ParseBool(xml.GetAttribute("nonRenderable")),
                MaterialIds = ParseInt(xml.GetAttribute("materialIds")),
                ObjectMask = ParseInt(xml.GetAttribute("objectMask")),
                ClipDistance = ParseInt(xml.GetAttribute("clipDistance")),
                Translation = ParseVector3(xml.GetAttribute("translation")),
                Visibility = ParseBool(xml.GetAttribute("visibility")),
                Kinematic = ParseBool(xml.GetAttribute("kinematic")),
                Trigger = ParseBool(xml.GetAttribute("trigger"))
            };
        }

        private static void ParseFile_SceneShapesInner(ref Stack<I3DSceneShape> parent, XmlReader xml, int depth)
        {
            I3DSceneShape shape = ParseFile_SceneShapesAttributes(xml);
            Stack<I3DSceneShape> children = new Stack<I3DSceneShape>();
            bool empty = xml.IsEmptyElement;

            Debug.Log(ParseString(xml.GetAttribute("name")));
            string curname = ParseString(xml.GetAttribute("name"));

            if (!empty)
            {
                XmlReaderExt.Read(xml);
                while (XmlReaderExt.SafeCheckEndElement(xml))
                {
                    ParseFile_SceneShapesInner(ref children, xml, depth + 1);
                    XmlReaderExt.Read(xml);
                    curname = ParseString(xml.GetAttribute("name"));
                }
            }

            shape.SceneShapes = children.ToArray();

            parent.Push(shape);
        }
        private static void ParseFile_SceneShapes(ref I3DModel model, XmlReader xml)
        {
            Stack<I3DSceneShape> shapes = new Stack<I3DSceneShape>();

            xml.Read();

            while (XmlReaderExt.SafeCheckEndElement(xml))
            {
                if (xml.NodeType == XmlNodeType.Whitespace)
                {
                    xml.Read();
                    continue;
                }

                I3DSceneShape shape = ParseFile_SceneShapesAttributes(xml);
                Stack<I3DSceneShape> children = new Stack<I3DSceneShape>();
                bool empty = xml.IsEmptyElement;

                XmlReaderExt.Read(xml);
                string curname = ParseString(xml.GetAttribute("name"));
                //xml.MoveToContent();
                while (!empty && XmlReaderExt.SafeCheckEndElement(xml))
                {
                    ParseFile_SceneShapesInner(ref children, xml, 0);
                    XmlReaderExt.Read(xml);
                    curname = ParseString(xml.GetAttribute("name"));
                }

                shape.SceneShapes = children.ToArray();

                shapes.Push(shape);

                xml.Read();
            }

            model.SceneShapes = shapes.ToArray();
        }

        private string mObjFile = "";

        public string ObjFile
        {
            get
            {
                return mObjFile;
            }
            set
            {
                mObjFile = value;
            }
        }

        public I3DModel ParseFile(string file)
        {
            if (!File.Exists(file))
                throw new FileNotFoundException("File not found.", file);

            if (Path.GetExtension(file) != ".i3d")
                throw new Exception("File not I3D format.");

            I3DModel output = new I3DModel();

            string contents = File.ReadAllText(file);
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
                            break;
                        case "Materials":
                            ParseFile_Materials(ref output, xml);
                            break;
                        case "Shapes":
                            ParseFile_Shapes(ref output, xml, ObjFile);
                            break;
                        case "Scene":
                            ParseFile_SceneShapes(ref output, xml);
                            break;
                    }
                }
            }

            return output;
        }
    }
}