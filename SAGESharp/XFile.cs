using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SAGESharp
{
    /// <summary>
    /// Represents a DirectX SDK mesh. (.x)
    /// </summary>
    public class XFile
    {
        public XHeader Header { get; } = new XHeader();
        public List<XTemplate> Templates { get; } = new List<XTemplate>();
        public List<XObject> Objects { get; } = new List<XObject>();

        public XFile(XHeader header)
        {
            Header = header;
        }

        public XFile(BinaryReader reader)
        {
            Header = new XHeader(reader);
            XReader xReader = new XReader(reader, Header, Templates, Objects);

            /*foreach (XToken token in xReader.Tokens)
            {
                System.Diagnostics.Debug.WriteLine(token.ToString());
            }*/

            while (!xReader.EndOfStream)
            {
                if (xReader.PeekToken().ID == XToken.TokenID.TEMPLATE)
                {
                    Templates.Add(new XTemplate(xReader));
                }
                else
                {
                    Objects.Add(new XObject(xReader));
                }
            }
        }

        public void Write(BinaryWriter writer)
        {
            Header.Write(writer);
            XWriter xWriter = new XWriter();

            foreach (XTemplate template in Templates)
            {
                //Console.WriteLine("    Writing template '" + template.Name + "'...");
                template.Write(xWriter);
            }

            foreach (XObject obj in Objects)
            {
                //Console.WriteLine("    Writing object '" + obj.Name + "'...");
                obj.Write(xWriter);
            }

            foreach (XToken token in xWriter.Tokens)
            {
                //System.Diagnostics.Debug.WriteLine(token.ToString());
                token.Write(writer, Header);
            }
        }
    }

    public class XHeader
    {
        private const uint FORMAT_MAGIC = 0x20666F78; // 'xof '

        public enum HeaderFormat : uint
        {
            Binary = 0x206E6962, // 'bin '
            Text = 0x20747874, // 'txt '
            Compressed = 0x20706D63, // 'cmp '
        }

        public byte VersionMain { get; } = 3;
        public byte VersionSub { get; } = 3;
        public HeaderFormat Format { get; } = HeaderFormat.Binary;
        public int Precision { get; } = 32;

        public XHeader()
        {

        }

        public XHeader(byte versionMain, byte versionSub, HeaderFormat format, int precision)
        {
            VersionMain = versionMain;
            VersionSub = versionSub;
            Format = format;
            Precision = precision;
        }

        public XHeader(BinaryReader reader)
        {
            uint magic = reader.ReadUInt32();
            if (magic != FORMAT_MAGIC)
                throw new FormatException("Bad XFile magic!");

            VersionMain = Byte.Parse(Encoding.ASCII.GetString(reader.ReadBytes(2)));
            VersionSub = Byte.Parse(Encoding.ASCII.GetString(reader.ReadBytes(2)));
            Format = (HeaderFormat)reader.ReadUInt32();
            Precision = Int32.Parse(Encoding.ASCII.GetString(reader.ReadBytes(4)));
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(FORMAT_MAGIC);
            writer.Write(Encoding.ASCII.GetBytes(VersionMain.ToString().PadLeft(2, '0')));
            writer.Write(Encoding.ASCII.GetBytes(VersionSub.ToString().PadLeft(2, '0')));
            writer.Write((uint)Format);
            writer.Write(Encoding.ASCII.GetBytes(Precision.ToString().PadLeft(4, '0')));
        }
    }

    public class XToken
    {
        public enum TokenID : ushort
        {
            INVALID = 0,

            // Data-carrying tokens
            NAME = 1,
            STRING = 2,
            INTEGER = 3,
            GUID = 5,
            INTEGER_LIST = 6,
            FLOAT_LIST = 7,

            // Syntax tokens
            OBRACE = 10,
            CBRACE = 11,
            OPAREN = 12,
            CPAREN = 13,
            OBRACKET = 14,
            CBRACKET = 15,
            OANGLE = 16,
            CANGLE = 17,
            DOT = 18,
            COMMA = 19,
            SEMICOLON = 20,

            TEMPLATE = 31,

            // Primitive type tokens
            WORD = 40,
            DWORD = 41,
            FLOAT = 42,
            DOUBLE = 43,
            CHAR = 44,
            UCHAR = 45,
            SWORD = 46,
            SDWORD = 47,
            VOID = 48,
            LPSTR = 49,
            UNICODE = 50,
            CSTRING = 51,
            ARRAY = 52
        }

        public TokenID ID { get; private set; } = TokenID.INVALID;

        private string _nameData = "";
        public string NameData
        {
            get { if (ID != TokenID.NAME) throw new InvalidOperationException(); return _nameData; }
            set { ID = TokenID.NAME; _nameData = value; }
        }

        private string _stringData = "";
        private TokenID _stringTerminator = TokenID.INVALID;
        public string StringData
        {
            get { if (ID != TokenID.STRING) throw new InvalidOperationException(); return _stringData; }
            set { ID = TokenID.STRING; _stringData = value; }
        }
        public TokenID StringTerminator
        {
            get { if (ID != TokenID.STRING) throw new InvalidOperationException(); return _stringTerminator; }
            set { if (ID != TokenID.STRING) throw new InvalidOperationException(); _stringTerminator = value; }
        }

        private int _integerData = 0;
        public int IntegerData
        {
            get { if (ID != TokenID.INTEGER) throw new InvalidOperationException(); return _integerData; }
            set { ID = TokenID.INTEGER; _integerData = value; }
        }

        private Guid _guidData = Guid.Empty;
        public Guid GUIDData
        {
            get { if (ID != TokenID.GUID) throw new InvalidOperationException(); return _guidData; }
            set { ID = TokenID.GUID; _guidData = value; }
        }

        private List<int> _integerListData = null;
        public List<int> IntegerListData
        {
            get { if (ID != TokenID.INTEGER_LIST) throw new InvalidOperationException(); return _integerListData; }
            set { ID = TokenID.INTEGER_LIST; _integerListData = value; }
        }

        private List<double> _floatListData = null;
        public List<double> FloatListData
        {
            get { if (ID != TokenID.FLOAT_LIST) throw new InvalidOperationException(); return _floatListData; }
            set { ID = TokenID.FLOAT_LIST; _floatListData = value; }
        }

        public XToken(TokenID id)
        {
            ID = id;
            if (id == TokenID.FLOAT_LIST)
                FloatListData = new List<double>();
            else if (id == TokenID.INTEGER_LIST)
                IntegerListData = new List<int>();
            else if (id == TokenID.STRING)
                StringTerminator = TokenID.SEMICOLON;
        }

        public XToken(BinaryReader reader, XHeader header)
        {
            ID = (TokenID)reader.ReadUInt16();
            if (ID == TokenID.NAME)
            {
                int count = reader.ReadInt32();
                NameData = Encoding.ASCII.GetString(reader.ReadBytes(count));
            }
            else if (ID == TokenID.STRING)
            {
                int count = reader.ReadInt32();
                StringData = Encoding.ASCII.GetString(reader.ReadBytes(count));
                StringTerminator = (TokenID)reader.ReadUInt16();
            }
            else if (ID == TokenID.INTEGER)
            {
                IntegerData = reader.ReadInt32();
            }
            else if (ID == TokenID.GUID)
            {
                GUIDData = new Guid(reader.ReadInt32(), reader.ReadInt16(), reader.ReadInt16(), reader.ReadBytes(8));
            }
            else if (ID == TokenID.INTEGER_LIST)
            {
                int count = reader.ReadInt32();
                IntegerListData = new List<int>();
                for (int i = 0; i < count; i++)
                {
                    IntegerListData.Add(reader.ReadInt32());
                }
            }
            else if (ID == TokenID.FLOAT_LIST)
            {
                int count = reader.ReadInt32();
                FloatListData = new List<double>();
                for (int i = 0; i < count; i++)
                {
                    if (header.Precision == 32)
                    {
                        FloatListData.Add(reader.ReadSingle());
                    }
                    else if (header.Precision == 64)
                    {
                        FloatListData.Add(reader.ReadDouble());
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("XToken: Header's precision was not 32 or 64.");
                    }
                }
            }
        }

        public void Write(BinaryWriter writer, XHeader header)
        {
            writer.Write((ushort)ID);
            if (ID == TokenID.NAME)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(NameData);
                writer.Write((int)bytes.Length);
                writer.Write(bytes);
            }
            else if (ID == TokenID.STRING)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(StringData);
                writer.Write((int)bytes.Length);
                writer.Write(bytes);
                writer.Write((ushort)StringTerminator);
            }
            else if (ID == TokenID.INTEGER)
            {
                writer.Write((int)IntegerData);
            }
            else if (ID == TokenID.GUID)
            {
                writer.Write(GUIDData.ToByteArray());
            }
            else if (ID == TokenID.INTEGER_LIST)
            {
                writer.Write((int)IntegerListData.Count);
                foreach (int i in IntegerListData)
                    writer.Write((int)i);
            }
            else if (ID == TokenID.FLOAT_LIST)
            {
                writer.Write((int)FloatListData.Count);
                foreach (double d in FloatListData)
                {
                    if (header.Precision == 64)
                        writer.Write((double)d);
                    else if (header.Precision == 32)
                        writer.Write((float)d);
                    else
                        throw new ArgumentOutOfRangeException("XToken: Header's precision was not 32 or 64!");
                }
            }
        }

        public override string ToString()
        {
            string result = ID.ToString();
            if (ID == TokenID.NAME)
            {
                result += ": '" + NameData + "'";
            }
            else if (ID == TokenID.STRING)
            {
                result += ": '" + StringData + "'" + StringTerminator.ToString();
            }
            else if (ID == TokenID.INTEGER)
            {
                result += ": " + IntegerData;
            }
            else if (ID == TokenID.GUID)
            {
                result += ": " + GUIDData.ToString();
            }
            else if (ID == TokenID.INTEGER_LIST)
            {
                result += ": {" + String.Join(",", IntegerListData) + "}";
            }
            else if (ID == TokenID.FLOAT_LIST)
            {
                result += ": {" + String.Join(",", FloatListData) + "}";
            }
            else if (ID == TokenID.OBRACE)
            {
                result = "{";
            }
            else if (ID == TokenID.CBRACE)
            {
                result = "}";
            }
            else if (ID == TokenID.OBRACKET)
            {
                result = "[";
            }
            else if (ID == TokenID.CBRACKET)
            {
                result = "]";
            }
            return result;
        }
    }

    public class XReader
    {
        private static Dictionary<string, XTemplate> _nativeTemplates = null;
        public static Dictionary<string, XTemplate> NativeTemplates
        {
            get
            {
                if (_nativeTemplates == null)
                {
                    _nativeTemplates = new Dictionary<string, XTemplate>();
                    // Time to init all the builtin templates... ugh

                    XTemplate animation = new XTemplate("Animation", new Guid("3D82AB4F-62DA-11cf-AB39-0020AF71E433"), new List<XTemplateRestriction>());
                    _nativeTemplates.Add(animation.Name, animation);

                    XTemplate animationKey = new XTemplate("AnimationKey", new Guid("10DD46A8-775B-11CF-8F52-0040333594A3"), null);
                    animationKey.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "keyType"));
                    animationKey.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "nKeys"));
                    animationKey.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.NAME) { NameData = "TimedFloatKeys" }, "keys", new List<XToken>(new XToken[] { new XToken(XToken.TokenID.NAME) { NameData = "nKeys" } })));
                    _nativeTemplates.Add(animationKey.Name, animationKey);

                    XTemplate animationOptions = new XTemplate("AnimationOptions", new Guid("E2BF56C0-840F-11cf-8F52-0040333594A3"), null);
                    animationOptions.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "openclosed"));
                    animationOptions.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "positionquality"));
                    _nativeTemplates.Add(animationOptions.Name, animationOptions);

                    XTemplate animationSet = new XTemplate("AnimationSet", new Guid("3D82AB50-62DA-11cf-AB39-0020AF71E433"), new List<XTemplateRestriction>(new XTemplateRestriction[] { new XTemplateRestriction("Animation", animation.GUID) }));
                    _nativeTemplates.Add(animationSet.Name, animationSet);

                    XTemplate animTicksPerSecond = new XTemplate("AnimTicksPerSecond", new Guid("9E415A43-7BA6-4a73-8743-B73D47E88476"), null);
                    animTicksPerSecond.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "AnimTicksPerSecond"));
                    _nativeTemplates.Add(animTicksPerSecond.Name, animTicksPerSecond);

                    XTemplate boolean = new XTemplate("Boolean", new Guid("537da6a0-ca37-11d0-941c-0080c80cfa7b"), null);
                    boolean.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "truefalse"));
                    _nativeTemplates.Add(boolean.Name, boolean);

                    XTemplate boolean2d = new XTemplate("Boolean2d", new Guid("4885AE63-78E8-11cf-8F52-0040333594A3"), null);
                    boolean2d.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.NAME) { NameData = boolean.Name }, "u"));
                    boolean2d.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.NAME) { NameData = boolean.Name }, "v"));
                    _nativeTemplates.Add(boolean2d.Name, boolean2d);

                    XTemplate colorRGB = new XTemplate("ColorRGB", new Guid("D3E16E81-7835-11cf-8F52-0040333594A3"), null);
                    colorRGB.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.FLOAT), "red"));
                    colorRGB.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.FLOAT), "green"));
                    colorRGB.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.FLOAT), "blue"));
                    _nativeTemplates.Add(colorRGB.Name, colorRGB);

                    XTemplate colorRGBA = new XTemplate("ColorRGBA", new Guid("35FF44E0-6C7C-11cf-8F52-0040333594A3"), null);
                    colorRGBA.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.FLOAT), "red"));
                    colorRGBA.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.FLOAT), "green"));
                    colorRGBA.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.FLOAT), "blue"));
                    colorRGBA.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.FLOAT), "alpha"));
                    _nativeTemplates.Add(colorRGBA.Name, colorRGBA);

                    // CompressedAnimationSet

                    XTemplate coords2d = new XTemplate("Coords2d", new Guid("F6F23F44-7686-11cf-8F52-0040333594A3"), null);
                    coords2d.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.FLOAT), "u"));
                    coords2d.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.FLOAT), "v"));
                    _nativeTemplates.Add(coords2d.Name, coords2d);

                    // DeclData

                    // EffectDWord

                    // EffectFloats

                    // EffectInstance

                    // EffectParamDWord

                    // EffectParamFloats

                    // EffectParamString

                    // EffectString

                    // FaceAdjecency

                    // FloatKeys

                    XTemplate frame = new XTemplate("Frame", new Guid("3D82AB46-62DA-11CF-AB39-0020AF71E433"), new List<XTemplateRestriction>());
                    _nativeTemplates.Add(frame.Name, frame);

                    XTemplate frameTransformMatrix = new XTemplate("FrameTransformMatrix", new Guid("F6F23F41-7686-11cf-8F52-0040333594A3"), null);
                    frameTransformMatrix.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.NAME) { NameData = "Matrix4x4" }, "frameMatrix"));
                    _nativeTemplates.Add(frameTransformMatrix.Name, frameTransformMatrix);

                    // FVFData

                    // Guid

                    XTemplate indexedColor = new XTemplate("IndexedColor", new Guid("1630B820-7842-11cf-8F52-0040333594A3"), null);
                    indexedColor.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "index"));
                    indexedColor.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.NAME) { NameData = "ColorRGBA" }));
                    _nativeTemplates.Add(indexedColor.Name, indexedColor);

                    XTemplate material = new XTemplate("Material", new Guid("3D82AB4D-62DA-11CF-AB39-0020AF71E433"), new List<XTemplateRestriction>());
                    material.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.NAME) { NameData = "ColorRGBA" }, "faceColor"));
                    material.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.FLOAT), "power"));
                    material.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.NAME) { NameData = "ColorRGB" }, "specularColor"));
                    material.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.NAME) { NameData = "ColorRGB" }, "emissiveColor"));
                    _nativeTemplates.Add(material.Name, material);

                    // MaterialWrap

                    XTemplate matrix4x4 = new XTemplate("Matrix4x4", new Guid("F6F23F45-7686-11cf-8F52-0040333594A3"), null);
                    matrix4x4.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.FLOAT), "matrix", new List<XToken>() { new XToken(XToken.TokenID.INTEGER) { IntegerData = 16 } }));
                    _nativeTemplates.Add(matrix4x4.Name, matrix4x4);

                    XTemplate mesh = new XTemplate("Mesh", new Guid("3D82AB44-62DA-11CF-AB39-0020AF71E433"), new List<XTemplateRestriction>());
                    mesh.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "nVertices"));
                    mesh.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.NAME) { NameData = "Vector" }, "vertices", new List<XToken>() { new XToken(XToken.TokenID.NAME) { NameData = "nVertices" } }));
                    mesh.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "nFaces"));
                    mesh.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.NAME) { NameData = "MeshFace" }, "faces", new List<XToken>() { new XToken(XToken.TokenID.NAME) { NameData = "nFaces" } }));
                    _nativeTemplates.Add(mesh.Name, mesh);

                    XTemplate meshFace = new XTemplate("MeshFace", new Guid("3D82AB5F-62DA-11cf-AB39-0020AF71E433"), null);
                    meshFace.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "nFaceVertexIndices", null));
                    meshFace.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "faceVertexIndices", new List<XToken>() { new XToken(XToken.TokenID.NAME) { NameData = "nFaceVertexIndices" } }));
                    _nativeTemplates.Add(meshFace.Name, meshFace);

                    // MeshFaceWraps

                    XTemplate meshMaterialList = new XTemplate("MeshMaterialList", new Guid("F6F23F42-7686-11CF-8F52-0040333594A3"), new List<XTemplateRestriction>() { new XTemplateRestriction(material.Name, material.GUID) });
                    meshMaterialList.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "nMaterials"));
                    meshMaterialList.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "nFaceIndexes"));
                    meshMaterialList.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "faceIndexes", new List<XToken>() { new XToken(XToken.TokenID.NAME) { NameData = "nFaceIndexes" } }));
                    _nativeTemplates.Add(meshMaterialList.Name, meshMaterialList);

                    XTemplate meshNormals = new XTemplate("MeshNormals", new Guid("F6F23F43-7686-11cf-8F52-0040333594A3"), null);
                    meshNormals.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "nNormals"));
                    meshNormals.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.NAME) { NameData = "Vector" }, "normals", new List<XToken>() { new XToken(XToken.TokenID.NAME) { NameData = "nNormals" } }));
                    meshNormals.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "nFaceNormals"));
                    meshNormals.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.NAME) { NameData = "MeshFace" }, "faceNormals", new List<XToken>() { new XToken(XToken.TokenID.NAME) { NameData = "nFaceNormals" } }));
                    _nativeTemplates.Add(meshNormals.Name, meshNormals);

                    XTemplate meshTextureCoords = new XTemplate("MeshTextureCoords", new Guid("F6F23F40-7686-11cf-8F52-0040333594A3"), null);
                    meshTextureCoords.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "nTextureCoords"));
                    meshTextureCoords.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.NAME) { NameData = "Coords2d" }, "textureCoords", new List<XToken>() { new XToken(XToken.TokenID.NAME) { NameData = "nTextureCoords" } }));
                    _nativeTemplates.Add(meshTextureCoords.Name, meshTextureCoords);

                    XTemplate meshVertexColors = new XTemplate("MeshVertexColors", new Guid("1630B821-7842-11cf-8F52-0040333594A3"), null);
                    meshVertexColors.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "nVertexColors"));
                    meshVertexColors.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.NAME) { NameData = "IndexedColor" }, "vertexColors", new List<XToken>() { new XToken(XToken.TokenID.NAME) { NameData = "nVertexColors" } }));
                    _nativeTemplates.Add(meshVertexColors.Name, meshVertexColors);

                    // Patch

                    // PatchMesh

                    // PatchMesh9

                    // PMAttributeRange

                    // PMInfo

                    // PMVSplitRecord

                    XTemplate skinWeights = new XTemplate("SkinWeights", new Guid("6F0D123B-BAD2-4167-A0D0-80224F25FABB"), null);
                    skinWeights.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.LPSTR), "transformNodeName"));
                    skinWeights.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "nWeights"));
                    skinWeights.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "vertexIndices", new List<XToken>() { new XToken(XToken.TokenID.NAME) { NameData = "nWeights" } }));
                    skinWeights.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.FLOAT), "weights", new List<XToken>() { new XToken(XToken.TokenID.NAME) { NameData = "nWeights" } }));
                    skinWeights.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.NAME) { NameData = "Matrix4x4" }, "matrixOffset"));
                    _nativeTemplates.Add(skinWeights.Name, skinWeights);

                    XTemplate textureFilename = new XTemplate("TextureFilename", new Guid("A42790E1-7810-11cf-8F52-0040333594A3"), null);
                    textureFilename.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.LPSTR), "filename"));
                    _nativeTemplates.Add(textureFilename.Name, textureFilename);

                    // TimedFloatKeys

                    XTemplate vector = new XTemplate("Vector", new Guid("3D82AB5E-62DA-11cf-AB39-0020AF71E433"), null);
                    vector.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.FLOAT), "x"));
                    vector.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.FLOAT), "y"));
                    vector.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.FLOAT), "z"));
                    _nativeTemplates.Add(vector.Name, vector);

                    XTemplate vertexDuplicationIndices = new XTemplate("VertexDuplicationIndices", new Guid("B8D65549-D7C9-4995-89CF-53A9A8B031E3"), null);
                    vertexDuplicationIndices.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "nIndices"));
                    vertexDuplicationIndices.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "nOriginalVertices"));
                    vertexDuplicationIndices.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "indices", new List<XToken>() { new XToken(XToken.TokenID.NAME) { NameData = "nIndices" } }));
                    _nativeTemplates.Add(vertexDuplicationIndices.Name, vertexDuplicationIndices);

                    XTemplate vertexElement = new XTemplate("VertexElement", new Guid("F752461C-1E23-48f6-B9F8-8350850F336F"), null);
                    vertexElement.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "Type"));
                    vertexElement.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "Method"));
                    vertexElement.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "Usage"));
                    vertexElement.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.DWORD), "UsageIndex"));
                    _nativeTemplates.Add(vertexElement.Name, vertexElement);

                    XTemplate xSkinMeshHeader = new XTemplate("XSkinMeshHeader", new Guid("3CF169CE-FF7C-44ab-93C0-F78F62D172E2"), null);
                    xSkinMeshHeader.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.WORD), "nMaxSkinWeightsPerVertex"));
                    xSkinMeshHeader.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.WORD), "nMaxSkinWeightsPerFace"));
                    xSkinMeshHeader.Members.Add(new XTemplateMember(new XToken(XToken.TokenID.WORD), "nBones"));
                    _nativeTemplates.Add(xSkinMeshHeader.Name, xSkinMeshHeader);
                }
                return _nativeTemplates;
            }
        }

        public List<XToken> Tokens { get; } = new List<XToken>();
        public List<XTemplate> Templates { get; } = new List<XTemplate>();
        public List<XObject> Objects { get; } = new List<XObject>();
        public int Position { get; set; } = 0;
        public bool EndOfStream { get => Position >= Tokens.Count; }

        public XReader(BinaryReader reader, XHeader header, List<XTemplate> templates, List<XObject> objects)
        {
            Templates = templates;
            Objects = objects;
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                Tokens.Add(new XToken(reader, header));
            }
        }

        public XReader(List<XToken> tokens, List<XTemplate> templates, List<XObject> objects)
        {
            Tokens = tokens;
            Templates = templates;
            Objects = objects;
        }

        public XToken PeekToken()
        {
            if (Position < Tokens.Count)
                return Tokens[Position];
            return null;
        }

        public XToken ReadToken()
        {
            if (Position < Tokens.Count)
            {
                return Tokens[Position++];
            }
            return null;
        }

        public XTemplate FindTemplate(string name)
        {
            foreach (XTemplate template in Templates)
                if (template.Name == name)
                    return template;
            if (NativeTemplates.ContainsKey(name))
                return NativeTemplates[name];
            return null;
        }

        public XTemplate FindTemplate(Guid guid)
        {
            foreach (XTemplate template in Templates)
                if (template.GUID == guid)
                    return template;
            foreach (XTemplate template in NativeTemplates.Values)
                if (template.GUID == guid)
                    return template;
            return null;
        }

        public XObject FindObject(string name)
        {
            foreach (XObject obj in Objects)
                if (obj.Name == name)
                    return obj;
            return null;
        }

        public XObject FindObject(string name, Guid classGUID)
        {
            if (classGUID == Guid.Empty)
                return FindObject(name);

            foreach (XObject obj in Objects)
            {
                if (obj.Name == name)
                {
                    if (obj.DataType.ID == XToken.TokenID.NAME)
                    {
                        if (FindTemplate(obj.DataType.NameData)?.GUID == classGUID)
                            return obj;
                    }
                }
            }
            return null;
        }
    }

    public class XWriter
    {
        public List<XToken> Tokens { get; } = new List<XToken>();

        public void Write(XToken token)
        {
            Tokens.Add(token);
        }
    }

    /// <summary>
    /// Allows a specified type as a child of an instance of the owning <see cref="XTemplate"/>.
    /// </summary>
    public class XTemplateRestriction
    {
        public string Name = null;
        public Guid GUID = Guid.Empty;

        public XTemplateRestriction(string name, Guid guid)
        {
            Name = name;
            GUID = guid;
        }

        public XTemplateRestriction(XReader reader)
        {
            if (reader.PeekToken().ID != XToken.TokenID.NAME)
                throw new FormatException("XTemplateOption: template_option_part must start with a NAME token!");
            Name = reader.ReadToken().NameData;
            if (reader.PeekToken().ID == XToken.TokenID.GUID)
                GUID = reader.ReadToken().GUIDData;
        }

        public void Write(XWriter writer)
        {
            writer.Write(new XToken(XToken.TokenID.NAME) { NameData = Name });
            if (GUID != Guid.Empty)
                writer.Write(new XToken(XToken.TokenID.GUID) { GUIDData = GUID });
        }

        public override string ToString()
        {
            return "'" + Name + "' (" + GUID.ToString() + ")";
        }
    }

    public class XTemplateMember
    {
        public XToken DataType { get; } = null;
        public string Name { get; } = null;
        public List<XToken> Dimensions { get; } = null;

        public XTemplateMember(XToken dataType, string name = null, List<XToken> dimensions = null)
        {
            DataType = dataType;
            Name = name;
            Dimensions = dimensions;
            if (dimensions?.Count == 0)
                throw new ArgumentException("If 'dimensions' is not null, it must contain at least one dimension.");
        }

        public XTemplateMember(XReader reader)
        {
            bool isArray = reader.PeekToken().ID == XToken.TokenID.ARRAY;
            if (isArray)
            {
                reader.ReadToken();
                Dimensions = new List<XToken>();
            }
            DataType = reader.ReadToken();

            // Read name if it's there
            if (reader.PeekToken().ID == XToken.TokenID.NAME)
            {
                Name = reader.ReadToken().NameData;
            }
            else if (isArray)
            {
                throw new FormatException("XTemplateMember: array members must have a NAME token!");
            }

            // Read dimensions
            while (isArray && reader.PeekToken().ID == XToken.TokenID.OBRACKET)
            {
                reader.ReadToken(); // Open bracket
                Dimensions.Add(reader.ReadToken()); // Name or Integer
                if (reader.ReadToken().ID != XToken.TokenID.CBRACKET)
                    throw new FormatException("XTemplateMember: dimension instances must have a CBRACE token after the dimension!");
            }

            if (reader.ReadToken().ID != XToken.TokenID.SEMICOLON)
                throw new FormatException("XTemplateMember: array, primitive, or template_reference must end with a SEMICOLON token!");
        }

        public void Write(XWriter writer)
        {
            bool isArray = Dimensions != null;
            if (isArray)
                writer.Write(new XToken(XToken.TokenID.ARRAY));
            writer.Write(DataType);
            if (Name != null)
                writer.Write(new XToken(XToken.TokenID.NAME) { NameData = Name });
            if (isArray)
            {
                foreach (XToken token in Dimensions)
                {
                    writer.Write(new XToken(XToken.TokenID.OBRACKET));
                    writer.Write(token);
                    writer.Write(new XToken(XToken.TokenID.CBRACKET));
                }
            }
            writer.Write(new XToken(XToken.TokenID.SEMICOLON));
        }

        public XObjectMember ReadMember(XTemplate template, List<XObjectMember> previousMembers, XReader reader, XObjectReader objReader)
        {
            XObjectMember result = new XObjectMember(Name, DataType);

            int totalItems = 1;
            if (Dimensions != null)
            {
                foreach (XToken token in Dimensions)
                {
                    if (token.ID == XToken.TokenID.INTEGER)
                    {
                        totalItems *= token.IntegerData;
                    }
                    else if (token.ID == XToken.TokenID.NAME)
                    {
                        int? dimensionSize = null;
                        foreach (XTemplateMember member in template.Members)
                        {
                            if (member.Name == token.NameData)
                            {
                                foreach (XObjectMember m in previousMembers)
                                {
                                    if (m.Name == member.Name)
                                    {
                                        dimensionSize = m.Values[0] as int?;
                                        if (dimensionSize == null)
                                            throw new FormatException("XTemplateMember: Member listed as dimension must hold integers!");
                                        break;
                                    }
                                }
                                break;
                            }
                        }

                        if (dimensionSize == null)
                            throw new FormatException("XTemplateMember: Couldn't find the member listed as a dimension!");
                        else
                            totalItems *= dimensionSize.Value;
                    }
                }
            }

            for (int i = 0; i < totalItems; i++)
            {
                if (DataType.ID == XToken.TokenID.NAME)
                {
                    // Read template instance member
                    XTemplate structType = reader.FindTemplate(DataType.NameData);
                    if (structType == null)
                        throw new FormatException("XTemplateMember: Can't find template '" + DataType.NameData + "'!");
                    XObjectStructure structure = new XObjectStructure(structType, previousMembers, reader, objReader);
                    result.Values.Add(structure);
                }
                else if (DataType.ID == XToken.TokenID.WORD || DataType.ID == XToken.TokenID.DWORD
                    || DataType.ID == XToken.TokenID.CHAR || DataType.ID == XToken.TokenID.UCHAR
                    || DataType.ID == XToken.TokenID.SWORD || DataType.ID == XToken.TokenID.SDWORD)
                {
                    // Read integer member
                    if (!objReader.HasInteger())
                        throw new FormatException("XTemplateMember: Member data should be an integer!");
                    result.Values.Add(objReader.Read());
                }
                else if (DataType.ID == XToken.TokenID.FLOAT || DataType.ID == XToken.TokenID.DOUBLE)
                {
                    // Read float member
                    if (!objReader.HasFloat())
                        throw new FormatException("XTemplateMember: Member data should be a float!");
                    result.Values.Add(objReader.Read());
                }
                else if (DataType.ID == XToken.TokenID.LPSTR)
                {
                    // Read string member
                    if (!objReader.HasString())
                        throw new FormatException("XTemplateMember: Member data should be a string!");
                    result.Values.Add(objReader.Read());
                }
                else
                {
                    throw new FormatException("XTemplateMember: token " + DataType.ID.ToString() + " is not a valid member type!");
                }

            }

            return result;
        }

        public override string ToString()
        {
            string result = DataType.ToString();
            if (Name != null)
                result += " " + Name;
            if (Dimensions != null)
            {
                foreach (XToken dimension in Dimensions)
                {
                    result += "[" + dimension.ToString() + "]";
                }
            }
            return result + ";";
        }
    }

    public class XTemplate
    {
        public enum TemplateRestrictionState
        {
            Closed = 0,
            Restricted = 1,
            Open = 2,
        }

        public string Name { get; } = "";
        public Guid GUID { get; } = Guid.Empty;

        public TemplateRestrictionState RestrictionState { get; } = TemplateRestrictionState.Closed;
        public List<XTemplateRestriction> Restrictions { get; } = new List<XTemplateRestriction>();

        public List<XTemplateMember> Members = new List<XTemplateMember>();

        public XTemplate(string name, Guid guid, List<XTemplateRestriction> restrictions = null)
        {
            Name = name;
            GUID = guid;
            Restrictions = restrictions;
            if (Restrictions == null)
            {
                RestrictionState = TemplateRestrictionState.Closed;
            }
            else if (Restrictions.Count == 0)
            {
                RestrictionState = TemplateRestrictionState.Open;
            }
            else
            {
                RestrictionState = TemplateRestrictionState.Restricted;
            }
        }

        public XTemplate(XReader reader)
        {
            if (reader.ReadToken().ID != XToken.TokenID.TEMPLATE)
                throw new FormatException("Template must start with TEMPLATE token!");

            // Name
            XToken name = reader.ReadToken();
            if (name.ID != XToken.TokenID.NAME)
                throw new FormatException("Template name must be a NAME token!");
            Name = name.NameData;

            if (reader.ReadToken().ID != XToken.TokenID.OBRACE)
                throw new FormatException("Expected OBRACE in template");

            // GUID
            XToken guid = reader.ReadToken();
            if (guid.ID != XToken.TokenID.GUID)
                throw new Exception("Template GUID must be a GUID token!");
            GUID = guid.GUIDData;

            // Read members_list until closing brace of template or open brace of options
            while (reader.PeekToken().ID != XToken.TokenID.CBRACE && reader.PeekToken().ID != XToken.TokenID.OBRACE)
            {
                // Read a member
                Members.Add(new XTemplateMember(reader));
            }
            // Read template_option_info
            if (reader.PeekToken().ID == XToken.TokenID.OBRACE)
            {
                if (reader.PeekToken().ID == XToken.TokenID.DOT)
                {
                    // Confirm ellipsis
                    if (reader.ReadToken().ID != XToken.TokenID.DOT)
                        throw new FormatException("Ellipses have more than one dots...");
                    if (reader.ReadToken().ID != XToken.TokenID.DOT)
                        throw new FormatException("Ellipses have more than two dots...");
                    RestrictionState = TemplateRestrictionState.Open;

                    if (reader.ReadToken().ID != XToken.TokenID.CBRACE)
                        throw new FormatException("XTemplate: Restriction ellipses must end with a CBRACE token!");
                }
                else
                {
                    // Read template_option_list
                    while (reader.PeekToken().ID != XToken.TokenID.CBRACE)
                    {
                        Restrictions.Add(new XTemplateRestriction(reader));
                    }
                    RestrictionState = TemplateRestrictionState.Restricted;
                }
            }
            if (reader.ReadToken().ID != XToken.TokenID.CBRACE)
                throw new FormatException("XTemplate: template must end with CBRACE token!");
        }

        public void Write(XWriter writer)
        {
            writer.Write(new XToken(XToken.TokenID.TEMPLATE));
            writer.Write(new XToken(XToken.TokenID.NAME) { NameData = Name });
            writer.Write(new XToken(XToken.TokenID.OBRACE));
            writer.Write(new XToken(XToken.TokenID.GUID) { GUIDData = GUID });

            foreach (XTemplateMember member in Members)
            {
                member.Write(writer);
            }

            if (RestrictionState != TemplateRestrictionState.Closed)
            {
                writer.Write(new XToken(XToken.TokenID.OBRACE));
                if (RestrictionState == TemplateRestrictionState.Open)
                {
                    writer.Write(new XToken(XToken.TokenID.DOT));
                    writer.Write(new XToken(XToken.TokenID.DOT));
                    writer.Write(new XToken(XToken.TokenID.DOT));
                }
                else if (RestrictionState == TemplateRestrictionState.Restricted)
                {
                    foreach (XTemplateRestriction restriction in Restrictions)
                        restriction.Write(writer);
                }
                writer.Write(new XToken(XToken.TokenID.CBRACE));
            }

            writer.Write(new XToken(XToken.TokenID.CBRACE));
        }

        /// <summary>
        /// Creates an XObject with the members and datatype of this XTemplate.
        /// No values are assigned to the members.
        /// </summary>
        /// <param name="name">The optional name of the new XObject.</param>
        /// <returns></returns>
        public XObject Instantiate(string name = null)
        {
            XObject result = new XObject(new XToken(XToken.TokenID.NAME) { NameData = Name }, name);

            foreach (XTemplateMember member in Members)
            {
                XObjectMember objMember = new XObjectMember(member.Name, member.DataType);
                result.Members.Add(objMember);
            }

            return result;
        }

        public override string ToString()
        {
            return "Template '" + Name + "' (" + RestrictionState.ToString() + ")";
        }
    }

    public class XChildObject
    {
        public XObject Object;
        public bool IsReference = false;

        public XChildObject(XObject _object, bool isReference)
        {
            Object = _object;
            IsReference = isReference;
        }

        public XChildObject(XReader reader)
        {
            if (reader.PeekToken().ID == XToken.TokenID.OBRACE)
            {
                // Read reference
                reader.ReadToken(); // Pass OBRACE
                if (reader.PeekToken().ID != XToken.TokenID.NAME)
                    throw new FormatException("XObjectChild: References must have a NAME token after the OBRACE token!");
                string objectName = reader.ReadToken().NameData;
                Guid classGUID = Guid.Empty;
                if (reader.PeekToken().ID == XToken.TokenID.GUID)
                    classGUID = reader.ReadToken().GUIDData;
                if (reader.PeekToken().ID != XToken.TokenID.CBRACE)
                    throw new FormatException("XObjectChild: References must end with a CBRACE token!");
                reader.ReadToken(); // Pass CBRACE

                // Now to actually find the referenced XObject.
                Object = reader.FindObject(objectName, classGUID);
                IsReference = true;
            }
            else
            {
                // Read object
                Object = new XObject(reader);
                IsReference = false;
            }
        }

        public void Write(XWriter writer)
        {
            if (IsReference)
            {
                writer.Write(new XToken(XToken.TokenID.OBRACE));
                writer.Write(new XToken(XToken.TokenID.NAME) { NameData = Object.Name });
                //writer.Write(new XToken(XToken.TokenID.GUID) { GUIDData = Object.GUID }); // TODO: Do we write the class GUID or the object GUID? I'm not certain.
                writer.Write(new XToken(XToken.TokenID.CBRACE));
            }
            else
            {
                Object.Write(writer);
            }
        }

        public override string ToString()
        {
            return (IsReference ? "External " : "Internal ") + " " + Object.ToString();
        }
    }

    /// <summary>
    /// Enables reading individual pieces of data (data_part s which are not object s or object references) from the contents of an object.
    /// </summary>
    public class XObjectReader
    {
        public List<object> Data = new List<object>();
        public int Position { get; set; } = 0;
        public bool EndOfStream { get => Position >= Data.Count; }

        public XObjectReader(List<object> data)
        {
            Data = data;
        }

        public object Peek()
        {
            if (EndOfStream)
                return null;
            return Data[Position];
        }

        public object Read()
        {
            if (EndOfStream)
                return null;
            return Data[Position++];
        }

        public bool HasInteger()
        {
            return !EndOfStream && Peek().GetType() == typeof(int);
        }

        public bool HasFloat()
        {
            return !EndOfStream && (Peek().GetType() == typeof(float) || Peek().GetType() == typeof(double));
        }

        public bool HasString()
        {
            return !EndOfStream && Peek().GetType() == typeof(string);
        }
    }

    /// <summary>
    /// Takes in member data in the form of doubles, floats, ints, and strings, and writes them to STRING, INTEGER_LIST and FLOAT_LIST tokens.
    /// </summary>
    public class XObjectWriter
    {
        public XWriter Writer { get; }
        //private XToken.TokenID CurrentListType = XToken.TokenID.INVALID;

        private XToken CurrentList = null;

        public XObjectWriter(XWriter writer)
        {
            Writer = writer;
        }

        public void Write(object item)
        {
            if (item.GetType() == typeof(string))
            {
                if (CurrentList != null)
                {
                    Writer.Write(CurrentList);
                    CurrentList = null;
                }
                Writer.Write(new XToken(XToken.TokenID.STRING) { StringData = item as string });
            }
            else if (item.GetType() == typeof(float) || item.GetType() == typeof(double))
            {
                if (CurrentList != null)
                {
                    if (CurrentList.ID != XToken.TokenID.FLOAT_LIST)
                    {
                        Writer.Write(CurrentList);
                        CurrentList = new XToken(XToken.TokenID.FLOAT_LIST);
                    }
                }
                else
                {
                    CurrentList = new XToken(XToken.TokenID.FLOAT_LIST);
                }
                CurrentList.FloatListData.Add(Convert.ToDouble(item)); // can't cast because objects need to be unboxed and then cast
            }
            else if (item.GetType() == typeof(int))
            {
                if (CurrentList != null)
                {
                    if (CurrentList.ID != XToken.TokenID.INTEGER_LIST)
                    {
                        Writer.Write(CurrentList);
                        CurrentList = new XToken(XToken.TokenID.INTEGER_LIST);
                    }
                }
                else
                {
                    CurrentList = new XToken(XToken.TokenID.INTEGER_LIST);
                }
                CurrentList.IntegerListData.Add((int)item);
            }
            else
            {
                throw new FormatException("XObjectWriter: Cannot write primitive of type '" + item.GetType().Name + "'. Primitives must be string, float, double, or int.");
            }
        }

        public void Dispose()
        {
            if (CurrentList != null)
            {
                Writer.Write(CurrentList);
                CurrentList = null;
            }
        }
    }

    public class XObjectMember
    {
        public string Name { get; }
        public XToken DataType { get; }
        public List<object> Values { get; set; } = new List<object>();

        public XObjectMember(string name, XToken dataType, params object[] values)
        {
            Name = name;
            DataType = dataType;
            foreach (object value in values)
                Values.Add(value);
        }

        public void Write(XObjectWriter writer)
        {
            if (DataType.ID == XToken.TokenID.NAME)
            {
                // Write XObjectStructures
                foreach (object value in Values)
                {
                    XObjectStructure structure = value as XObjectStructure;
                    foreach (XObjectMember member in structure.Members)
                    {
                        member.Write(writer);
                    }
                }
            }
            else
            {
                // Write primitives
                foreach (object value in Values)
                {
                    writer.Write(value);
                }
            }
        }
    }

    public class XObjectStructure
    {
        public XTemplate Template;
        public List<XObjectMember> Members = new List<XObjectMember>();

        public XObjectMember this[string name]
        {
            get
            {
                foreach (XObjectMember member in Members)
                    if (member.Name == name)
                        return member;
                return null;
            }
        }

        public XObjectStructure(XTemplate template, List<XObjectMember> previousMembers, XReader reader, XObjectReader objReader)
        {
            Template = template;
            foreach (XTemplateMember member in Template.Members)
            {
                Members.Add(member.ReadMember(Template, Members, reader, objReader));
            }
        }

        public XObjectStructure(XTemplate template, params XObjectMember[] members)
        {
            Template = template;
            foreach (XObjectMember member in members)
                Members.Add(member);
        }
    }

    public class XObject
    {
        public XToken DataType = null;
        public string Name = null;
        public Guid GUID = Guid.Empty;

        public List<XChildObject> Children = new List<XChildObject>();
        public List<XObjectMember> Members = new List<XObjectMember>();

        public XChildObject this[int i]
        {
            get { return Children[i]; }
        }

        public XObjectMember this[string name]
        {
            get
            {
                foreach (XObjectMember member in Members)
                    if (member.Name == name)
                        return member;
                return null;
            }
        }

        public XObject(XToken dataType, string name = null)
        {
            DataType = dataType;
            Name = name;
        }

        public XObject(XReader reader)
        {
            DataType = reader.ReadToken();
            if (reader.PeekToken().ID != XToken.TokenID.OBRACE)
            {
                if (reader.PeekToken().ID != XToken.TokenID.NAME)
                    throw new FormatException("XObject: object may have an optional name before the opening brace and after the datatype, but that's it!");
                Name = reader.ReadToken().NameData;
            }

            if (reader.PeekToken().ID != XToken.TokenID.OBRACE)
                throw new FormatException("XObject: object must have an opening brace here!");
            reader.ReadToken(); // Discard OBRACE

            // Optional GUID
            if (reader.PeekToken().ID == XToken.TokenID.GUID)
                GUID = reader.ReadToken().GUIDData;

            List<object> data = new List<object>();
            while (reader.PeekToken().ID != XToken.TokenID.CBRACE)
            {
                // Read data_part
                XToken.TokenID nextID = reader.PeekToken().ID;
                if (nextID == XToken.TokenID.OBRACE || nextID == XToken.TokenID.NAME
                    || nextID >= XToken.TokenID.WORD && nextID <= XToken.TokenID.CSTRING)
                {
                    // Read child object, whether reference or embedded
                    XChildObject child = new XChildObject(reader);
                    XTemplate childTemplate = reader.FindTemplate(child.Object.DataType.NameData);
                    // Determine whether the child object is allowed
                    if (DataType.ID == XToken.TokenID.NAME)
                    {
                        XTemplate template = reader.FindTemplate(DataType.NameData);
                        if (template.RestrictionState == XTemplate.TemplateRestrictionState.Closed)
                        {
                            throw new FormatException("XObject: Child object s cannot be present for an instance of a closed template!");
                        }
                        else if (template.RestrictionState == XTemplate.TemplateRestrictionState.Restricted)
                        {
                            bool allowed = false;
                            foreach (XTemplateRestriction restriction in template.Restrictions)
                            {
                                if (restriction.Name == childTemplate.Name && (restriction.GUID == Guid.Empty || restriction.GUID == childTemplate.GUID))
                                {
                                    allowed = true;
                                    break;
                                }
                            }
                            if (!allowed)
                                throw new FormatException("XObject: Child object is of template '" + childTemplate.Name + "' which does not fit any of the filters on parent template '" + template.Name + "'!");
                        }
                    }
                    Children.Add(child);
                }
                else if (nextID == XToken.TokenID.INTEGER_LIST)
                {
                    // Read integer list
                    foreach (int i in reader.ReadToken().IntegerListData)
                    {
                        data.Add(i);
                    }
                }
                else if (nextID == XToken.TokenID.FLOAT_LIST)
                {
                    // Read float list
                    foreach (double d in reader.ReadToken().FloatListData)
                    {
                        data.Add(d);
                    }
                }
                else if (nextID == XToken.TokenID.STRING)
                {
                    // Read string
                    data.Add(reader.ReadToken().StringData);
                }
                else
                {
                    throw new FormatException("XObject: Unexpected token starting a data_part!");
                }
            }
            reader.ReadToken(); // Discard CBRACE

            // Read the integers, floats, and strings into actual members
            XObjectReader objReader = new XObjectReader(data);
            if (DataType.ID == XToken.TokenID.NAME)
            {
                // Read member data according to the template
                XTemplate template = reader.FindTemplate(DataType.NameData);
                if (template == null)
                    throw new FormatException("XObject: This object has a datatype of template '" + DataType.NameData + "' which does not exist!");

                foreach (XTemplateMember member in template.Members)
                {
                    Members.Add(member.ReadMember(template, Members, reader, objReader));
                }
            }
            else
            {
                throw new NotSupportedException("XObject: object s with a datatype other than a template are not supported (and I've never seen one in the wild or this message wouldn't be here!");
            }
        }

        public void Write(XWriter writer)
        {
            writer.Write(DataType);
            if (!String.IsNullOrEmpty(Name))
                writer.Write(new XToken(XToken.TokenID.NAME) { NameData = Name });
            writer.Write(new XToken(XToken.TokenID.OBRACE));
            if (GUID != Guid.Empty)
                writer.Write(new XToken(XToken.TokenID.GUID) { GUIDData = GUID });

            XObjectWriter xWriter = new XObjectWriter(writer);
            foreach (XObjectMember member in Members)
            {
                member.Write(xWriter);
            }
            xWriter.Dispose();

            foreach (XChildObject child in Children)
            {
                child.Write(writer);
            }
            writer.Write(new XToken(XToken.TokenID.CBRACE));
        }

        public override string ToString()
        {
            return DataType.ToString() + " '" + Name + "'";
        }
    }
}
