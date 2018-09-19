using System;
using UnityEngine;

namespace Assets.I3DShapesTool
{
    class I3DSpline
    {
        public uint Unknown1 { get; }

        public string Name { get; }

        public ushort ShapeId { get; }

        public uint Unknown2 { get; }

        public uint VertexCount { get; }

        public Vector3[] Vertices { get; }

        public I3DSpline(BigEndianBinaryReader br)
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

                Unknown2 = br.ReadUInt32();
                VertexCount = br.ReadUInt32();

                Vertices = new Vector3[VertexCount];
                for (int i = 0; i < VertexCount; i++)
                {
                    Vertices[i] = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                }
            }
            catch (Exception e)
            {
                if (string.IsNullOrEmpty(Name)) throw;

                if (ShapeId > 0)
                    throw new Exception($"Failed to parse I3DSpline {Name}, ShapeID: {ShapeId}", e);

                throw new Exception($"Failed to parse I3DSpline {Name}", e);
            }
        }
    }
}
