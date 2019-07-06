using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SharpDX;

namespace SAGESharp
{
    public class OCLFile
    {
        public class OctreeNode
        {
            public enum NodeType : byte
            {
                Branch = 0,
                Leaf = 1
            }

            public class Triangle
            {
                public Vector3 Normal;
                public float OddThing; // Normal.X * Position1.X - Normal.Y * Position1.Y + Normal.Z * Position1.Z
                public float Angle; // Some obscure formula I can't discern
                public Vector3 Position1;
                public Vector3 Position2;
                public Vector3 Position3;
                public Vector3 One; // Always 1.0f, 1.0f, 1.0f
                public uint MaterialIndex; // 0x48
                public byte Unk10; // 0x4C, LSB is 0x01, others are garbage.
                public byte Padding0 = 0; // 0x4D, always 0x00
                public byte Padding1 = 0; // 0x4E, always 0x00
                public byte Padding2 = 0; // 0x4F, always 0x00

                public Triangle(BinaryReader reader)
                {
                    Normal = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    OddThing = reader.ReadSingle();
                    Angle = reader.ReadSingle();
                    Position1 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    Position2 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    Position3 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    One = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    MaterialIndex = reader.ReadUInt32();
                    Unk10 = reader.ReadByte();
                    Padding0 = reader.ReadByte();
                    Padding1 = reader.ReadByte();
                    Padding2 = reader.ReadByte();
                }

                public void Write(BinaryWriter writer)
                {
                    writer.Write(Normal.X);
                    writer.Write(Normal.Y);
                    writer.Write(Normal.Z);
                    writer.Write(OddThing);
                    writer.Write(Angle);
                    writer.Write(Position1.X);
                    writer.Write(Position1.Y);
                    writer.Write(Position1.Z);
                    writer.Write(Position2.X);
                    writer.Write(Position2.Y);
                    writer.Write(Position2.Z);
                    writer.Write(Position3.X);
                    writer.Write(Position3.Y);
                    writer.Write(Position3.Z);
                    writer.Write(One.X);
                    writer.Write(One.Y);
                    writer.Write(One.Z);
                    writer.Write(MaterialIndex);
                    writer.Write(Unk10);
                    writer.Write(Padding0);
                    writer.Write(Padding1);
                    writer.Write(Padding2);
                }

                private Vector3 CalculateNormal()
                {
                    Vector3 result = Vector3.Cross(Position2 - Position1, Position3 - Position1);
                    result.Normalize();
                    return result;
                }

                private float CalculateOddThing()
                {
                    return -Vector3.Dot(Normal, Position1);
                }

                private float CalculateAngle()
                {
                    return Math.Abs(90.0f - (float)Math.Acos(Vector3.Dot(Normal, Vector3.Up)) * 180.0f / MathUtil.Pi);
                }

                public override string ToString()
                {
                    return "OddThing: " + OddThing + ", Angle: " + Angle + ", MaterialIndex: " + MaterialIndex.ToString("X8") + ", Unk10: " + Unk10.ToString("X8");
                }
            }

            public class AdjacencyData
            {
                public uint[] pAdjacentNodes = new uint[6];
                public OctreeNode[] AdjacentNodes = new OctreeNode[6];

                public AdjacencyData(BinaryReader reader)
                {
                    for (int i = 0; i < AdjacentNodes.Length; i++)
                    {
                        uint ptr = reader.ReadUInt32();
                        pAdjacentNodes[i] = ptr;
                    }
                }

                public void Link(Dictionary<uint, OctreeNode> existingNodes)
                {
                    for (int i = 0; i < pAdjacentNodes.Length; i++)
                    {
                        uint ptr = pAdjacentNodes[i];
                        if (ptr == 0)
                            continue;

                        if (existingNodes.ContainsKey(ptr))
                            AdjacentNodes[i] = existingNodes[ptr];
                        else
                            System.Diagnostics.Debugger.Break();
                    }
                }

                public override string ToString()
                {
                    String s = "";
                    for (int i = 0; i < AdjacentNodes.Length; i++)
                    {
                        s += "," + AdjacentNodes[i]?.Index ?? "<null>";
                    }
                    return s.Substring(1);
                }
            }

            public ushort Index;
            public List<Triangle> Triangles = new List<Triangle>();
            public uint TriangleDataOffset;
            public uint pPolyList = 0; // Always 0x00000000
            public OctreeNode[] Children = new OctreeNode[8];
            public AdjacencyData Adjacency = null;
            public NodeType Type;
            public Vector3 Min;
            public Vector3 Max;

            public OctreeNode(BinaryReader reader, Dictionary<uint, OctreeNode> existingNodes)
            {
                existingNodes.Add((uint)reader.BaseStream.Position, this);
                Index = reader.ReadUInt16();
                int triangleCount = reader.ReadUInt16(); // 2bytes padding
                TriangleDataOffset = reader.ReadUInt32();
                pPolyList = reader.ReadUInt32();

                // Read triangles
                long pos = reader.BaseStream.Position;
                reader.BaseStream.Position = TriangleDataOffset;
                for (uint i = 0; i < triangleCount; i++)
                {
                    Triangles.Add(new Triangle(reader));
                }
                reader.BaseStream.Position = pos;

                // Read children
                for (int i = 0; i < 8; i++)
                {
                    uint offset = reader.ReadUInt32();
                    if (offset != 0)
                    { 
                        long p = reader.BaseStream.Position;
                        reader.BaseStream.Position = offset;
                        Children[i] = new OctreeNode(reader, existingNodes);
                        reader.BaseStream.Position = p;
                    }
                }

                // Read adjacency data
                uint pAdjacencyData = reader.ReadUInt32();
                if (pAdjacencyData != 0)
                {
                    long oldpos = reader.BaseStream.Position;
                    reader.BaseStream.Position = pAdjacencyData;
                    Adjacency = new AdjacencyData(reader);
                    reader.BaseStream.Position = oldpos;
                }
                //pUnk03 = reader.ReadUInt32();

                Type = (NodeType)reader.ReadByte();
                reader.ReadBytes(3); // 3bytes padding
                Min = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Max = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            }
        }

        public OctreeNode RootNode = null;

        public OCLFile(BinaryReader reader)
        {
            reader.BaseStream.Position = 0x1C;
            Dictionary<uint, OctreeNode> nodes = new Dictionary<uint, OctreeNode>();
            RootNode = new OctreeNode(reader, nodes);
            foreach (KeyValuePair<uint, OctreeNode> pair in nodes)
            {
                if (pair.Value.Adjacency == null)
                    continue;

                pair.Value.Adjacency.Link(nodes);
            }
        }

        /// <summary>
        /// Dumps a representation of this file to the debug stream.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="prefix"></param>
        public void LogDebug(OctreeNode node = null, string prefix = "  ")
        {
            if (node == null)
                node = RootNode;

            System.Diagnostics.Debug.WriteLine(prefix + "[Node] Min: " + node.Min.ToString() + ", Max: " + node.Max.ToString());
            //System.Diagnostics.Debug.WriteLine(prefix + "Index: " + node.Index.ToString("X4") + ", TriangleDataOffset: " + node.TriangleDataOffset.ToString("X8") + ", pPolyList: " + node.pPolyList.ToString("X8") + ", Node Type: " + node.Type.ToString());
            //System.Diagnostics.Debug.WriteLine(prefix + "Adjacency: " + (node.Adjacency != null ? node.Adjacency.ToString() : "(none)"));
            System.Diagnostics.Debug.WriteLine(prefix + node.Triangles.Count.ToString("X8") + " Triangles:");
            foreach (OctreeNode.Triangle t in node.Triangles)
                System.Diagnostics.Debug.WriteLine(prefix + " - " + t.ToString());

            System.Diagnostics.Debug.WriteLine(prefix + "Child Nodes:");
            for (int i = 0; i < node.Children.Length; i++)
            {
                if (node.Children[i] != null)
                    LogDebug(node.Children[i], prefix + "  ");
            }
        }

        public void DumpOBJ(string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename, false))
            {
                List<Vector3> vertices = new List<Vector3>();
                List<Vector3> normals = new List<Vector3>();
                List<Tuple<int, int, int, int>> faces = new List<Tuple<int, int, int, int>>();

                DumpOBJNode(RootNode, writer, vertices, normals, faces);

                foreach (Vector3 vertex in vertices)
                    writer.WriteLine("v " + vertex.X.ToString(System.Globalization.CultureInfo.InvariantCulture) + " "
                        + vertex.Y.ToString(System.Globalization.CultureInfo.InvariantCulture) + " "
                        + vertex.Z.ToString(System.Globalization.CultureInfo.InvariantCulture));

                foreach (Vector3 normal in normals)
                    writer.WriteLine("vn " + normal.X.ToString(System.Globalization.CultureInfo.InvariantCulture) + " "
                        + normal.Y.ToString(System.Globalization.CultureInfo.InvariantCulture) + " "
                        + normal.Z.ToString(System.Globalization.CultureInfo.InvariantCulture));

                foreach (Tuple<int, int, int, int> face in faces)
                    writer.WriteLine("f " + (face.Item1 + 1) + "//" + (face.Item4 + 1) + " " + (face.Item2 + 1) + "//" + (face.Item4 + 1) + " " + (face.Item3 + 1) + "//" + (face.Item4 + 1));
            }
        }

        private void DumpOBJNode(OctreeNode node, StreamWriter writer, List<Vector3> vertices, List<Vector3> normals, List<Tuple<int, int, int, int>> faces)
        {
            foreach (OctreeNode.Triangle triangle in node.Triangles)
            {
                int position1Index = vertices.IndexOf(triangle.Position1);
                if (position1Index == -1)
                {
                    position1Index = vertices.Count;
                    vertices.Add(triangle.Position1);
                }

                int position2Index = vertices.IndexOf(triangle.Position2);
                if (position2Index == -1)
                {
                    position2Index = vertices.Count;
                    vertices.Add(triangle.Position2);
                }

                int position3Index = vertices.IndexOf(triangle.Position3);
                if (position3Index == -1)
                {
                    position3Index = vertices.Count;
                    vertices.Add(triangle.Position3);
                }

                int normalIndex = normals.IndexOf(triangle.Normal);
                if (normalIndex == -1)
                {
                    normalIndex = normals.Count;
                    normals.Add(triangle.Normal);
                }

                faces.Add(new Tuple<int, int, int, int>(position1Index, position2Index, position3Index, normalIndex));
            }

            for (int i = 0; i < node.Children.Length; i++)
            {
                if (node.Children[i] != null)
                    DumpOBJNode(node.Children[i], writer, vertices, normals, faces);
            }
        }
    }
}
