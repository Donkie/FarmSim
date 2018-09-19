using System;
using UnityEngine;

namespace Assets.I3DShapesTool
{
    public class I3DShape
    {
        public uint Unknown1 { get; }

        public string Name { get; }

        public ushort ShapeId { get; }

        public float BVCenterX { get; }

        public float BVCenterY { get; }

        public float BVCenterZ { get; }

        public float BVRadius { get; }

        public uint VertexCount { get; }

        public uint Unknown6 { get; }

        public uint Vertices { get; }

        public uint Unknown7 { get; }

        public uint Unknown8 { get; }

        public uint UvCount { get; }

        public uint Unknown9 { get; }

        public uint VertexCount2 { get; }

        public Mesh Mesh;
        
        public I3DShape(BigEndianBinaryReader br)
        {
            try
            {
                Unknown1 = br.ReadUInt32();
                Name = br.BaseStream.ReadNullTerminatedString();

                br.BaseStream.Align(2); // Align the stream to short

                //This is pretty ugly, but they pretty much zero-pad after the alignment
                //So we read the padding until we found the shapeid
                do
                {
                    ShapeId = br.ReadUInt16();
                } while (ShapeId == 0);

                BVCenterX = br.ReadSingle();
                BVCenterY = br.ReadSingle();
                BVCenterZ = br.ReadSingle();
                BVRadius = br.ReadSingle();
                VertexCount = br.ReadUInt32();
                Unknown6 = br.ReadUInt32();
                Vertices = br.ReadUInt32();
                Unknown7 = br.ReadUInt32();
                Unknown8 = br.ReadUInt32();
                UvCount = br.ReadUInt32();
                Unknown9 = br.ReadUInt32();
                VertexCount2 = br.ReadUInt32();

                Mesh = new Mesh();

                int[] tris = new int[VertexCount];
                for (int i = 0; i < VertexCount; i++)
                {
                    tris[i] = br.ReadUInt16();
                }

                br.BaseStream.Align(4);

                Vector3[] vertices = new Vector3[Vertices];
                for (int i = 0; i < Vertices; i++)
                {
                    vertices[i] = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                }

                Vector3[] normals = new Vector3[Vertices];
                for (int i = 0; i < Vertices; i++)
                {
                    normals[i] = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                }

                Vector2[] uvs = new Vector2[UvCount];
                for (int i = 0; i < UvCount; i++)
                {
                    uvs[i] = new Vector2(br.ReadSingle(), br.ReadSingle());
                }

                Mesh.vertices = vertices;
                Mesh.normals = normals;
                Mesh.triangles = tris;
                Mesh.uv = uvs;

            }
            catch (Exception e)
            {
                if (string.IsNullOrEmpty(Name)) throw;

                if(ShapeId > 0)
                    throw new Exception($"Failed to parse I3DShape {Name}, ShapeID: {ShapeId}", e);

                throw new Exception($"Failed to parse I3DShape {Name}", e);
            }
        }
    }
}