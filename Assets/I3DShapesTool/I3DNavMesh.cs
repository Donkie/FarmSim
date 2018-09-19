using System;
using UnityEngine;

namespace Assets.I3DShapesTool
{
    public class I3DNavMeshPolygon
    {
        public uint Unknown1 { get; }

        public uint VertexCount { get; }

        public short[] Polygon { get; }

        public short[] Unknown2 { get; }

        public I3DNavMeshPolygon(BigEndianBinaryReader br)
        {
            Unknown1 = br.ReadUInt32();

            VertexCount = br.ReadUInt32();

            Polygon = new short[VertexCount];
            for (int i = 0; i < VertexCount; i++)
            {
                Polygon[i] = br.ReadInt16();
            }

            Unknown2 = new short[VertexCount];
            for (int i = 0; i < VertexCount; i++)
            {
                Unknown2[i] = br.ReadInt16();
            }
        }
    }

    public class I3DNavMesh
    {
        public uint Unknown1 { get; }

        public string Name { get; }

        public ushort ShapeId { get; }

        public float GridCellSize { get; }

        public float GridCellHeight { get; }

        public Vector3 BoundsMin { get; }

        public Vector3 BoundsMax { get; }

        public uint VertexCount { get; }

        /// <summary>
        /// Also known as Node Count
        /// </summary>
        public uint PolygonCount { get; }

        public uint PolygonVertexCount { get; }

        public Vector3[] Vertices { get; }

        public I3DNavMeshPolygon[] Polygons { get;  }

        public I3DNavMesh(BigEndianBinaryReader br)
        {
            try
            {
                Unknown1 = br.ReadUInt32();
                Name = br.BaseStream.ReadNullTerminatedString();

                br.BaseStream.Align(2); // Align the stream to short

                do
                {
                    ShapeId = br.ReadUInt16();
                } while (ShapeId == 0);

                GridCellSize = br.ReadSingle();
                GridCellHeight = br.ReadSingle();

                BoundsMin = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                BoundsMax = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

                VertexCount = br.ReadUInt32();
                PolygonCount = br.ReadUInt32();
                PolygonVertexCount = br.ReadUInt32();

                Vertices = new Vector3[VertexCount];
                for (int i = 0; i < VertexCount; i++)
                {
                    Vertices[i] = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                }

                Polygons = new I3DNavMeshPolygon[PolygonCount];
                for (int i = 0; i < PolygonCount; i++)
                {
                    Polygons[i] = new I3DNavMeshPolygon(br);
                }
            }
            catch (Exception e)
            {
                if (string.IsNullOrEmpty(Name)) throw;

                if (ShapeId > 0)
                    throw new Exception($"Failed to parse I3DNavMesh {Name}, ShapeID: {ShapeId}", e);

                throw new Exception($"Failed to parse I3DNavMesh {Name}", e);
            }
        }
    }
}
