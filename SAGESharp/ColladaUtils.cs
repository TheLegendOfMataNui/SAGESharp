//#define DUMB_FIXUP

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using SAGESharp.Animations;
using SharpDX;

namespace SAGESharp
{
    public static class ColladaUtils
    {
        private const string SCHEMA_URL = "http://www.collada.org/2005/11/COLLADASchema";

        private static XElement FindElementByReference(XElement library, XName type, string reference)
        {
            if (reference.StartsWith("#"))
            {
                reference = reference.Substring(1); // Remove the #
                IEnumerable<XElement> results = from ele in library.Elements(type) where ele.Attribute("id").Value == reference select ele;

                // Return the first result
                foreach (XElement ele in results)
                    return ele;

                return null;
            }
            else
            {
                throw new FormatException("Only ID references (starting with #) are supported.");
            }
        }

        private static float ParseFloat(string input)
        {
            return Single.Parse(input, System.Globalization.CultureInfo.InvariantCulture.NumberFormat); // Don't go off about commas being the decimal point.
        }

        private static Vector4 ParseVector4(XElement element)
        {
            string[] parts = element.Value.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return new Vector4(ParseFloat(parts[0]), ParseFloat(parts[1]), ParseFloat(parts[2]), ParseFloat(parts[3]));
        }

        private static Matrix ReadMatrix(XElement matrix)
        {
            if (matrix.Name != ((XNamespace)SCHEMA_URL) + "matrix")
                throw new ArgumentException("`matrix` must be an element of type `matrix`.");

            string[] parts = matrix.Value.Split(' ');

            if (parts.Length != 16)
                throw new ArgumentException("There must be exactly 16 elements in this matrix. Instead, there are " + parts.Length + ".");

            float[] values = new float[parts.Length];
            for (int i = 0; i < parts.Length; i++)
                values[i] = ParseFloat(parts[i]);

            return new Matrix(values);
        }

        private static XElement WriteMatrix(Matrix m)
        {
            return new XElement((XNamespace)SCHEMA_URL + "matrix", String.Join(" ", m.ToArray()));
        }

        private static XObject ImportCOLLADAMesh(string name, XElement materialBinding, Matrix totalTransform, XElement geometryElement, Dictionary<string, XObject> materials, bool flipV, bool mergeVertices, List<List<int>> positionUsage = null)
        {
            XNamespace ns = SCHEMA_URL;

            XObject mesh = XReader.NativeTemplates["Mesh"].Instantiate();
            XObject meshNormals = XReader.NativeTemplates["MeshNormals"].Instantiate();
            mesh.Children.Add(new XChildObject(meshNormals, false));
            XObject meshTextureCoords = XReader.NativeTemplates["MeshTextureCoords"].Instantiate();
            mesh.Children.Add(new XChildObject(meshTextureCoords, false));
            XObject meshVertexColors = XReader.NativeTemplates["MeshVertexColors"].Instantiate();
            XObject meshMaterialList = XReader.NativeTemplates["MeshMaterialList"].Instantiate();
            mesh.Children.Add(new XChildObject(meshMaterialList, false));

            //if (Program.Config.GetValueOrDefault("DAE", "ForceMerge", "").ToLowerInvariant() == "never")
            //{
            //    mergeVertices = false;
            //}
            //else if (Program.Config.GetValueOrDefault("DAE", "ForceMerge", "").ToLowerInvariant() == "always")
            //{
            //    mergeVertices = true;
            //}

            if (mergeVertices)
                Console.WriteLine("  Rigid Mesh - Merging Enabled");
            else
                Console.WriteLine("  Skinned Mesh - Merging Disabled");

            // Material Binding - relates material names (the key) to material URIs (the value)
            Dictionary<string, string> materialBinds = null;
            if (materialBinding != null)
            {
                materialBinds = new Dictionary<string, string>();
                foreach (XElement bind in materialBinding.Element(ns + "technique_common").Elements(ns + "instance_material"))
                {
                    materialBinds.Add(bind.Attribute("symbol").Value, bind.Attribute("target").Value);
                }
            }
            Dictionary<string, int> includedMaterials = new Dictionary<string, int>();

            XElement meshElement = geometryElement.Element(ns + "mesh");

            // .X meshes treat position, uv, and color as one welded item, meaning we need to store unique combinations (permutations, i suppose).
            /*List<Vector3> positions = new List<Vector3>();*/
            List<Vector3> normals = new List<Vector3>();
            /*List<Vector2> uvs = new List<Vector2>();
            List<Vector4> colors = new List<Vector4>();*/
            List<Tuple<Vector3, Vector2, Vector4>> vertices = new List<Tuple<Vector3, Vector2, Vector4>>();

            bool hasColors = false;
            if (positionUsage != null)
            {
                positionUsage.Clear();
            }
            int priorPositions = 0; // The total number of positions in all previous <triangles> elements

            foreach (XElement trianglesElement in meshElement.Elements(ns + "triangles"))
            {
                int triangleCount = Int32.Parse(trianglesElement.Attribute("count").Value);
                string materialName = trianglesElement.Attribute("material").Value;

                // Map the material name
                if (materialBinds.ContainsKey(materialName))
                    materialName = materialBinds[materialName];
                else
                    throw new Exception("[ERROR]: Material '" + materialName + "' wasn't bound!");

                int materialIndex = -1;
                if (includedMaterials.ContainsKey(materialName))
                {
                    materialIndex = includedMaterials[materialName];
                }
                else
                {
                    materialIndex = includedMaterials.Count;
                    includedMaterials.Add(materialName, materialIndex);
                    meshMaterialList.Children.Add(new XChildObject(materials[materialName], false));
                }

                string[] indexData = trianglesElement.Element(ns + "p").Value.Split(new char[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                int[] indices = new int[indexData.Length];
                for (int i = 0; i < indexData.Length; i++)
                    indices[i] = Int32.Parse(indexData[i].Trim());

                int posOffset = -1;
                string[] posData = null;
                int normalOffset = -1;
                string[] normalData = null;
                int uvOffset = -1;
                string[] uvData = null;
                int colorOffset = -1;
                string[] colorData = null;

                int stride = indices.Length / (triangleCount * 3); // Because counting the number of inputs is too much work

                foreach (XElement inputElement in trianglesElement.Elements(ns + "input"))
                {
                    string semantic = inputElement.Attribute("semantic").Value.ToUpper();
                    int offset = Int32.Parse(inputElement.Attribute("offset").Value);
                    string sourceReference = inputElement.Attribute("source").Value;

                    if (semantic == "VERTEX")
                    {
                        // Each input in the <vertices> referenced by `sourceReference` has its own semantic and source, but shares a single offset.
                        XElement verticesElement = FindElementByReference(meshElement, ns + "vertices", sourceReference);
                        foreach (XElement verticesInputElement in verticesElement.Elements(ns + "input"))
                        {
                            semantic = verticesInputElement.Attribute("semantic").Value.ToUpper();
                            sourceReference = verticesInputElement.Attribute("source").Value;

                            XElement dataElement = FindElementByReference(meshElement, ns + "source", sourceReference).Element(ns + "float_array");
                            string[] data = dataElement.Value.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                            if (semantic == "POSITION")
                            {
                                posData = data;
                                posOffset = offset;
                            }
                            else if (semantic == "NORMAL")
                            {
                                normalData = data;
                                normalOffset = offset;
                            }
                            else if (semantic == "TEXCOORD")
                            {
                                if (inputElement.Attribute("set") == null || Int32.Parse(inputElement.Attribute("set").Value) == 0)
                                {
                                    uvData = data;
                                    uvOffset = offset;
                                }
                            }
                            else if (semantic == "COLOR")
                            {
                                colorData = data;
                                colorOffset = offset;
                            }
                            else
                            {
                                Console.WriteLine("[WARNING]: Unknown COLLADA vertices input semantic '" + semantic + "'!");
                            }
                        }
                    }
                    else
                    {
                        XElement dataElement = FindElementByReference(meshElement, ns + "source", sourceReference).Element(ns + "float_array");
                        string[] data = dataElement.Value.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                        if (semantic == "POSITION")
                        {
                            posData = data;
                            posOffset = offset;
                        }
                        else if (semantic == "NORMAL")
                        {
                            normalData = data;
                            normalOffset = offset;
                        }
                        else if (semantic == "TEXCOORD")
                        {
                            if (inputElement.Attribute("set") == null || Int32.Parse(inputElement.Attribute("set").Value) == 0)
                            {
                                uvData = data;
                                uvOffset = offset;
                            }
                        }
                        else if (semantic == "COLOR")
                        {
                            colorData = data;
                            colorOffset = offset;
                        }
                        else
                        {
                            Console.WriteLine("[WARNING]: Unknown COLLADA triangles input semantic '" + semantic + "'!");
                        }
                    }
                }

                if (positionUsage != null && positionUsage.Count == 0) // If we haven't initialized the usage yet
                {
                    for (int i = 0; i < posData.Length / 3; i++)
                        positionUsage.Add(new List<int>()); // Each position starts out not being used in any vertices
                }

                for (int i = 0; i < triangleCount; i++)
                {
                    List<int> vertexIndices = new List<int>();
                    /*if (vertexIndices == null)
                        vertexIndices = new List<int>();
                    else
                        vertexIndices.Clear();*/ // Just to be sure we aren't adding any residue from the list that was passed in into the mesh.
                    List<int> normalIndices = new List<int>();

                    for (int v = 0; v < 3; v++)
                    {
                        int pIndex = i * 3 * stride + v * stride;

                        int posIndex = indices[pIndex + posOffset];
                        float posX = ParseFloat(posData[posIndex * 3]);
                        float posY = ParseFloat(posData[posIndex * 3 + 1]);
                        float posZ = ParseFloat(posData[posIndex * 3 + 2]);

                        if (normalData == null)
                            throw new FormatException("[ERROR]: No normal data!");
                        int normalIndex = indices[pIndex + normalOffset];
                        float normX = ParseFloat(normalData[normalIndex * 3]);
                        float normY = ParseFloat(normalData[normalIndex * 3 + 1]);
                        float normZ = ParseFloat(normalData[normalIndex * 3 + 2]);

                        if (uvData == null)
                            throw new FormatException("[ERROR]: No texture coordinates!");
                        int uvIndex = indices[pIndex + uvOffset];
                        float uvX = ParseFloat(uvData[uvIndex * 2]);
                        float uvY = ParseFloat(uvData[uvIndex * 2 + 1]);
                        if (flipV)
                            uvY = 1.0f - uvY;

                        float colorR = 0.0f;
                        float colorG = 0.0f;
                        float colorB = 0.0f;
                        float colorA = 0.0f;

                        if (colorData != null)
                        {
                            hasColors = true;
                            int colorIndex = indices[pIndex + colorOffset];
                            colorR = ParseFloat(colorData[colorIndex * 4]);
                            colorG = ParseFloat(colorData[colorIndex * 4 + 1]);
                            colorB = ParseFloat(colorData[colorIndex * 4 + 2]);
                            colorA = ParseFloat(colorData[colorIndex * 4 + 3]);
                        }
                        else if (hasColors)
                        {
                            throw new FormatException("Part of the mesh has vertex colors, but this part doesn't!");
                        }

                        // Now lookup the indices
                        Vector4 normal = new Vector4(normX, normY, normZ, 0);
                        normal = Vector4.Transform(normal, totalTransform);
                        Vector4 transPos = Vector4.Transform(new Vector4(posX, posY, posZ, 1.0f), totalTransform);
                        Tuple<Vector3, Vector2, Vector4> vertex = new Tuple<Vector3, Vector2, Vector4>(new Vector3(transPos.X, transPos.Y, transPos.Z), new Vector2(uvX, uvY), new Vector4(colorR, colorG, colorB, colorA));

                        int normIndex = normals.IndexOf(new Vector3(normal.X, normal.Y, normal.Z));
                        if (normIndex > -1)
                        {
                            normalIndices.Add(normIndex);
                        }
                        else
                        {
                            normals.Add(new Vector3(normal.X, normal.Y, normal.Z));
                            normalIndices.Add(normals.Count - 1);
                        }

                        int vertexIndex = vertices.IndexOf(vertex);


                        if (mergeVertices)
                        {
                            if (vertexIndex > -1)
                            {
                                vertexIndices.Add(vertexIndex);
                            }
                            else
                            {
                                vertices.Add(vertex);
                                vertexIndex = vertices.Count - 1;
                                vertexIndices.Add(vertexIndex);
                            }
                        }
                        else
                        {
                            vertices.Add(vertex);
                            vertexIndex = vertices.Count - 1;
                            vertexIndices.Add(vertexIndex);
                        }


                        if (positionUsage != null)
                        {
                            if (!positionUsage[priorPositions + posIndex].Contains(vertexIndex))
                                positionUsage[priorPositions + posIndex].Add(vertexIndex);
                        }
                        //System.Diagnostics.Debug.WriteLine("Position " + (priorPositions + posIndex) + " used for vertex " + vertexIndex);
                    }

                    mesh["faces"].Values.Add(XUtils.Face(vertexIndices));
                    meshNormals["faceNormals"].Values.Add(XUtils.Face(normalIndices));
                    meshMaterialList["faceIndexes"].Values.Add(materialIndex);
                }
                //priorPositions += posData.Length / 3;
            }

            int vIndex = 0;
            foreach (Tuple<Vector3, Vector2, Vector4> vertex in vertices)
            {
                mesh["vertices"].Values.Add(XUtils.Vector(vertex.Item1));
                meshTextureCoords["textureCoords"].Values.Add(XUtils.TexCoord(vertex.Item2));
                meshVertexColors["vertexColors"].Values.Add(XUtils.IndexedColor(vIndex, vertex.Item3));
                vIndex++;
            }

            foreach (Vector3 normal in normals)
            {
                meshNormals["normals"].Values.Add(XUtils.Vector(normal));
            }

            if (hasColors)
            {
                mesh.Children.Add(new XChildObject(meshVertexColors, false));
            }

            // Fix all the counts.
            mesh["nVertices"].Values.Add(mesh["vertices"].Values.Count);
            mesh["nFaces"].Values.Add(mesh["faces"].Values.Count);
            meshNormals["nNormals"].Values.Add(meshNormals["normals"].Values.Count);
            meshNormals["nFaceNormals"].Values.Add(meshNormals["faceNormals"].Values.Count);
            meshTextureCoords["nTextureCoords"].Values.Add(meshTextureCoords["textureCoords"].Values.Count);
            meshVertexColors["nVertexColors"].Values.Add(meshVertexColors["vertexColors"].Values.Count);
            meshMaterialList["nMaterials"].Values.Add(meshMaterialList.Children.Count); // Because MeshMaterialList is a restricted template, all children are guaranteed to be Material objects.
            meshMaterialList["nFaceIndexes"].Values.Add(meshMaterialList["faceIndexes"].Values.Count);

            return mesh;
        }

        private class BoneInfo
        {
            public string Name { get; }
            public Matrix InverseBindPose { get; }
            public List<Tuple<int, float>> Influences { get; } = new List<Tuple<int, float>>();

            public BoneInfo(string name, Matrix inverseBindPose)
            {
                this.Name = name;
                this.InverseBindPose = inverseBindPose;
            }
        }
        private static XObject ImportCOLLADASkinnedMesh(string name, XElement materialBinding, Matrix totalTransform, XElement geometryLibrary, XElement controllerElement, Dictionary<string, XObject> materials, bool flipV)
        {
            XNamespace ns = SCHEMA_URL;

            XElement skinElement = controllerElement.Element(ns + "skin");
            XElement geometryElement = FindElementByReference(geometryLibrary, ns + "geometry", skinElement.Attribute("source").Value);

            // TODO: take bind_shape_matrix into consideration

            // Take advantage of the existing static mesh import
            List<List<int>> positionUsage = new List<List<int>>(); // We need to know which .X vertices correspond to which .DAE positions. XIndices = positionUsage[DAEPositionIndex]
            XObject mesh = ImportCOLLADAMesh(name, materialBinding, totalTransform, geometryElement, materials, flipV, false, positionUsage);

            string[] names = null;

            // Get all the bones for this controller
            XElement jointsNameSource = null;
            XElement jointsMatrixSource = null;
            Dictionary<string, BoneInfo> bones = new Dictionary<string, BoneInfo>();
            foreach (XElement input in skinElement.Element(ns + "joints").Elements(ns + "input"))
            {
                string semantic = input.Attribute("semantic").Value;
                if (semantic == "JOINT")
                    jointsNameSource = FindElementByReference(skinElement, ns + "source", input.Attribute("source").Value);
                else if (semantic == "INV_BIND_MATRIX")
                    jointsMatrixSource = FindElementByReference(skinElement, ns + "source", input.Attribute("source").Value);
            }
            if (jointsNameSource == null)
                throw new FormatException("Controller '" + name + "' needs a JOINT input in the <joints> element.");
            if (jointsMatrixSource == null)
                throw new FormatException("Controller '" + name + "' needs an INV_BIND_MATRIX input in the <joints> element.");
            string[] jointsMatrixElements = jointsMatrixSource.Element(ns + "float_array").Value.Split(new char[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            int jointsNameIndex = 0;
            foreach (string boneName in jointsNameSource.Element(ns + "Name_array").Value.Split(new char[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (names == null)
                {
                    if (BHDFile.BipedBoneNames.Contains(boneName))
                        names = BHDFile.BipedBoneNames;
                    else if (BHDFile.NonBipedBoneNames.Contains(boneName))
                        names = BHDFile.NonBipedBoneNames;
                    else
                        throw new Exception("Bone named '" + boneName + "' is not in either list of bones in the game EXE.");
                }
                float[] matrixValues = new float[16];
                for (int i = 0; i < matrixValues.Length; i++)
                {
                    matrixValues[i] = Single.Parse(jointsMatrixElements[jointsNameIndex * 16 + i]); // Assumes 4x4 matrices, matrix order matches bone name order
                }
                Matrix boneMat = new Matrix(matrixValues);
                boneMat.Transpose();
                boneMat = Matrix.Multiply(Matrix.Invert(totalTransform), boneMat);
                bones.Add(boneName, new BoneInfo(boneName, boneMat));
                jointsNameIndex++;
            }

            // Add all the vertex weights to the appropriate BoneInfo
            XElement weightsNameSource = null;
            XElement weightsWeightSource = null;
            foreach (XElement input in skinElement.Element(ns + "vertex_weights").Elements(ns + "input"))
            {
                string semantic = input.Attribute("semantic").Value;
                if (semantic == "JOINT")
                    weightsNameSource = FindElementByReference(skinElement, ns + "source", input.Attribute("source").Value);
                else if (semantic == "WEIGHT")
                    weightsWeightSource = FindElementByReference(skinElement, ns + "source", input.Attribute("source").Value);
            }
            if (weightsNameSource == null)
                throw new FormatException("Controller '" + name + "' needs a JOINT input in the <vertex_weights> element.");
            if (jointsMatrixSource == null)
                throw new FormatException("Controller '" + name + "' needs a WEIGHT input in the <vertex_weights> element.");
            string[] weightsNames = weightsNameSource.Element(ns + "Name_array").Value.Split(new char[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            string[] weightsWeights = weightsWeightSource.Element(ns + "float_array").Value.Split(new char[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            string[] weightsCounts = skinElement.Element(ns + "vertex_weights").Element(ns + "vcount").Value.Split(new char[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            string[] weightsValues = skinElement.Element(ns + "vertex_weights").Element(ns + "v").Value.Split(new char[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            int positionIndex = 0;
            int influenceIndex = 0;
            //int maxInfluenceCount = 0;
            foreach (string count in weightsCounts)
            {
                int countValue = Int32.Parse(count);
                for (int i = 0; i < countValue; i++)
                {
                    string boneName = weightsNames[Int32.Parse(weightsValues[influenceIndex * 2])];
                    float weight = Single.Parse(weightsWeights[Int32.Parse(weightsValues[influenceIndex * 2 + 1])]);
                    //int vertIndex = vertex

                    // The influenced position could be used in any number of vertices 
                    List<int> thisPositionUsages = positionUsage[positionIndex];
                    foreach (int vertexIndex in thisPositionUsages)
                    {
                        bool no = false; // is the given bone already influencing this vertex
                        foreach (Tuple<int, float> influence in bones[boneName].Influences)
                        {
                            if (influence.Item1 == vertexIndex)
                            {
                                no = true;
                                break;
                            }
                        }
                        if (!no)
                            bones[boneName].Influences.Add(new Tuple<int, float>(vertexIndex, weight));
                    }
                    //if (bones[boneName].Influences.Count > maxInfluenceCount)
                    //    maxInfluenceCount = bones[boneName].Influences.Count;

                    influenceIndex++;
                }
                positionIndex++;
            }

            // Add all this info to the XObject
            XObject skinMeshHeader = XReader.NativeTemplates["XSkinMeshHeader"].Instantiate();
            skinMeshHeader["nMaxSkinWeightsPerVertex"].Values.Add(1);
            skinMeshHeader["nMaxSkinWeightsPerFace"].Values.Add(1 * 3);
            
            mesh.Children.Add(new XChildObject(skinMeshHeader, false));

            int inf = 0;
            int usedBones = 0;

            string[] nameArray = null;
            if (BHDFile.BipedBoneNames.Contains(bones.ElementAt(0).Key))
            {
                nameArray = BHDFile.BipedBoneNames;
            }
            else if (BHDFile.NonBipedBoneNames.Contains(bones.ElementAt(0).Key))
            {
                nameArray = BHDFile.NonBipedBoneNames;
            }
            else
            {
                // Unknown bone name!
                System.Diagnostics.Debugger.Break();
            }

            nameArray = new List<string>(nameArray).ToArray(); // The slacker's array copy
            Array.Sort(nameArray);

            //foreach (BoneInfo bone in bones.Values)
            foreach (string boneName in nameArray)
            {
                if (!bones.ContainsKey(boneName))
                    continue;
                BoneInfo bone = bones[boneName];



                if (bone.Influences.Count > 0)
                    usedBones++;
                XObject skinWeights = XReader.NativeTemplates["SkinWeights"].Instantiate();
                skinWeights["transformNodeName"].Values.Add(bone.Name);
                skinWeights["nWeights"].Values.Add(bone.Influences.Count);
                foreach (Tuple<int, float> influence in bone.Influences)
                {
                    skinWeights["vertexIndices"].Values.Add(influence.Item1);
                    skinWeights["weights"].Values.Add(influence.Item2);
                    inf++;
                }
                skinWeights["matrixOffset"].Values.Add(new XObjectStructure(XReader.NativeTemplates["Matrix4x4"], new XObjectMember("matrix", new XToken(XToken.TokenID.FLOAT))));
                float[] matrixValues = bone.InverseBindPose.ToArray();
                for (int i = 0; i < 16; i++)
                    (skinWeights["matrixOffset"].Values[0] as XObjectStructure)["matrix"].Values.Add(matrixValues[i]);
                mesh.Children.Add(new XChildObject(skinWeights, false));
            }
            skinMeshHeader["nBones"].Values.Add(bones.Count);
            System.Diagnostics.Debug.WriteLine(inf + " influences.");

#if DUMB_FIXUP
            // Dumb fixup: for each face, ensure that for all bones, if the bone has a weight for the first vertex then it must also have weights for the second and third vertices.
            int addedCount = 0;
            foreach (XObjectStructure meshFace in mesh["faces"].Values)
            {
                // For each index,
                foreach (int vertexIndex in meshFace["faceVertexIndices"].Values)
                {
                    // For each bone,
                    foreach (XChildObject meshChild in mesh.Children)
                    {
                        if (meshChild.Object.DataType.NameData == "SkinWeights")
                        {
                            List<object> influenceIndexes = meshChild.Object["vertexIndices"].Values;
                            List<object> influenceWeights = meshChild.Object["weights"].Values;
                            // If the index in influenced by this bone, make sure all the other indices are also influenced by adding them with a weight of zero if necessary.
                            if (influenceIndexes.Contains(vertexIndex))
                            {
                                foreach (int otherIndex in meshFace["faceVertexIndices"].Values)
                                {
                                    if (!influenceIndexes.Contains(otherIndex))
                                    {
                                        int insertIndex = influenceIndexes.IndexOf(vertexIndex) + 1;
                                        influenceIndexes.Insert(insertIndex, otherIndex);
                                        influenceWeights.Insert(insertIndex, 0.0f);
                                        addedCount++;
                                        meshChild.Object["nWeights"].Values[0] = (int)meshChild.Object["nWeights"].Values[0] + 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine("Dumb Fixup: Added " + addedCount + " influences.");
#endif

            return mesh;
        }

        public static XFile ImportCOLLADA(string filename, Matrix transform, bool flipV = true, bool stripExtensions = true)
        {
            XFile result = new XFile(new XHeader());

            // These templates are present in all LOMN .X files, so add them just in case.
            result.Templates.Add(XReader.NativeTemplates["XSkinMeshHeader"]);
            result.Templates.Add(XReader.NativeTemplates["VertexDuplicationIndices"]);
            result.Templates.Add(XReader.NativeTemplates["SkinWeights"]);

            XObject frameObject = new XObject(new XToken(XToken.TokenID.NAME) { NameData = "Frame" }, "Root");
            XObject frameTransformObject = new XObject(new XToken(XToken.TokenID.NAME) { NameData = "FrameTransformMatrix" });
            frameTransformObject.Members.Add(new XObjectMember("frameMatrix", new XToken(XToken.TokenID.NAME) { NameData = "Matrix4x4" }, new XObjectStructure(XReader.NativeTemplates["Matrix4x4"], new XObjectMember("matrix", new XToken(XToken.TokenID.FLOAT), 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0))));
            frameObject.Children.Add(new XChildObject(frameTransformObject, false));

            XDocument doc = XDocument.Load(filename);
            XNamespace ns = SCHEMA_URL;

            XElement COLLADA = doc.Root;
            XElement imagesLibrary = COLLADA.Element(ns + "library_images");
            XElement materialsLibrary = COLLADA.Element(ns + "library_materials");
            XElement effectsLibrary = COLLADA.Element(ns + "library_effects");
            XElement geometryLibrary = COLLADA.Element(ns + "library_geometries");
            XElement visualSceneLibrary = COLLADA.Element(ns + "library_visual_scenes");
            XElement controllerLibrary = COLLADA.Element(ns + "library_controllers");

            XElement sceneElement = COLLADA.Element(ns + "scene");
            XElement visualSceneInstance = sceneElement.Element(ns + "instance_visual_scene");
            XElement visualScene = FindElementByReference(visualSceneLibrary, ns + "visual_scene", visualSceneInstance.Attribute("url").Value);

            // Read all the textures
            Dictionary<string, string> textureFilenames = new Dictionary<string, string>();
            foreach (XElement imageElement in imagesLibrary.Elements(ns + "image"))
            {
                string path = imageElement.Element(ns + "init_from").Value.Trim();
                if (stripExtensions)
                    path = System.IO.Path.GetFileNameWithoutExtension(path);
                else
                    path = System.IO.Path.GetFileName(path);
                textureFilenames.Add(imageElement.Attribute("id").Value, path);
            }

            // Read all the materials
            Dictionary<string, XObject> materials = new Dictionary<string, XObject>();
            foreach (XElement materialElement in materialsLibrary.Elements(ns + "material"))
            {
                string id = materialElement.Attribute("id").Value;
                string name = materialElement.Attribute("name").Value;

                XElement effectInstanceElement = materialElement.Element(ns + "instance_effect");
                string effectReference = effectInstanceElement.Attribute("url").Value;
                XElement effectElement = FindElementByReference(effectsLibrary, ns + "effect", effectReference);

                XElement shaderElement = effectElement.Element(ns + "profile_COMMON").Element(ns + "technique").Element(ns + "phong");
                if (shaderElement == null)
                {
                    Console.WriteLine("[WARNING]: Material '" + id + "' isn't a phong! Skipping.");
                    continue;
                }

                XObject material = XReader.NativeTemplates["Material"].Instantiate();

                XElement diffuseElement = shaderElement.Element(ns + "diffuse");
                XElement diffuseTextureElement = diffuseElement.Element(ns + "texture");
                if (diffuseTextureElement != null)
                {
                    material["faceColor"].Values.Add(XUtils.ColorRGBA(1.0f, 1.0f, 1.0f, 1.0f));
                    XObject textureFilename = XReader.NativeTemplates["TextureFilename"].Instantiate();
                    textureFilename["filename"].Values.Add(textureFilenames[diffuseTextureElement.Attribute("texture").Value]);
                    material.Children.Add(new XChildObject(textureFilename, false));
                }
                else
                {
                    XElement diffuseColorElement = diffuseElement.Element(ns + "color");
                    Vector4 color = ParseVector4(diffuseColorElement);
                    material["faceColor"].Values.Add(XUtils.ColorRGBA(color.X, color.Y, color.Z, color.W));
                }
                material["power"].Values.Add(ParseFloat(shaderElement.Element(ns + "shininess").Element(ns + "float").Value));
                Vector4 specColor = ParseVector4(shaderElement.Element(ns + "specular").Element(ns + "color"));
                material["specularColor"].Values.Add(XUtils.ColorRGB(specColor.X * specColor.W, specColor.Y * specColor.W, specColor.Z * specColor.W));
                Vector4 emColor = ParseVector4(shaderElement.Element(ns + "emission").Element(ns + "color"));
                material["emissiveColor"].Values.Add(XUtils.ColorRGB(emColor.X * emColor.W, emColor.Y * emColor.W, emColor.Z * emColor.W));
                materials.Add("#" + id, material);
            }

            // Read all the meshes
            foreach (XElement node in visualScene.Elements(ns + "node"))
            {
                Matrix matrix = Matrix.Identity;
                XElement matrixElement = node.Element(ns + "matrix");
                if (matrixElement != null)
                {
                    matrix = ReadMatrix(matrixElement);
                }
                Matrix totalTransform = matrix * transform;

                XElement geoInstanceElement = node.Element(ns + "instance_geometry");
                XElement controllerInstanceElement = node.Element(ns + "instance_controller");
                if (geoInstanceElement != null)
                {
                    string name = node.Attribute("name").Value;
                    XElement materialBinding = geoInstanceElement.Element(ns + "bind_material");
                    XElement geometryElement = FindElementByReference(geometryLibrary, ns + "geometry", geoInstanceElement.Attribute("url").Value);
                    XObject mesh = ImportCOLLADAMesh(name, materialBinding, totalTransform, geometryElement, materials, flipV, true);
                    frameObject.Children.Add(new XChildObject(mesh, false));
                }
                else if (controllerInstanceElement != null)
                {
                    string name = node.Attribute("name").Value;
                    XElement controllerElement = FindElementByReference(controllerLibrary, ns + "controller", controllerInstanceElement.Attribute("url").Value);
                    XElement materialBinding = controllerInstanceElement.Element(ns + "bind_material");
                    XObject mesh = ImportCOLLADASkinnedMesh(name, materialBinding, totalTransform, geometryLibrary, controllerElement, materials, flipV);
                    frameObject.Children.Add(new XChildObject(mesh, false));
                }
            }

            result.Objects.Add(frameObject);

            return result;
        }

        private static XElement ExportCOLLADABone(BHDFile.Bone bone, Matrix transform, string[] boneNames)
        {
            XElement result = new XElement((XNamespace)SCHEMA_URL + "node", new XAttribute("type", "JOINT"), new XAttribute("name", boneNames[(int)bone.Index]), new XAttribute("sid", boneNames[(int)bone.Index]), new XAttribute("id", boneNames[(int)bone.Index]), WriteMatrix(transform * bone.Transform));

            for (int i = 0; i < bone.Children.Count; i++)
            {
                result.Add(ExportCOLLADABone(bone.Children[i], Matrix.Identity, boneNames)); // Don't pass on the transform, children inherit it anyway because of how hierarchies work
            }

            return result;
        }

        public static void ExportCOLLADA(XFile file, BHDFile skeleton, BKD animation, string filename, Matrix transform, bool flipV = true, string textureExtension = null, bool stripUnusedMaterials = false)
        {
            XNamespace ns = SCHEMA_URL;
            XDocument doc = new XDocument();
            XElement COLLADA = new XElement(ns + "COLLADA", new XAttribute("xmlns", SCHEMA_URL), new XAttribute("version", "1.4.1"));

            COLLADA.Add(new XElement(ns + "asset",
                new XElement(ns + "contributor",
                    new XElement(ns + "author"),
                    new XElement(ns + "authoring_tool", "LOMNTool " + System.Reflection.Assembly.GetAssembly(typeof(ColladaUtils)).GetName().Version.ToString()),
                    new XElement(ns + "comments")),
                new XElement(ns + "created", DateTime.Now.ToString("o")),
                new XElement(ns + "keywords"),
                new XElement(ns + "modified", DateTime.Now.ToString("o")),
                new XElement(ns + "unit", new XAttribute("meter", "0.01"), new XAttribute("name", "centimeter")),
                new XElement(ns + "up_axis", "Y_UP")));

            XElement library_images = new XElement(ns + "library_images");
            XElement library_materials = new XElement(ns + "library_materials");
            XElement library_effects = new XElement(ns + "library_effects");
            XElement library_geometries = new XElement(ns + "library_geometries");
            XElement library_controllers = new XElement(ns + "library_controllers");
            XElement libarary_animations = new XElement(ns + "library_animations");
            XElement visual_scene = new XElement(ns + "visual_scene", new XAttribute("id", "Scene"), new XAttribute("name", "Scene"));

            List<int> usedMaterials = new List<int>();

            int meshID = 1;
            foreach (XObject frame in file.Objects)
            {
                foreach (XChildObject frameChild in frame.Children)
                {
                    XObject obj = frameChild.Object;
                    if (obj.DataType.ID == XToken.TokenID.NAME && obj.DataType.NameData == "Mesh")
                    {
                        int vertexCount = (int)obj["nVertices"].Values[0];
                        int faceCount = (int)obj["nFaces"].Values[0];

                        bool hasNormals = false;
                        bool hasTexCoords = false;
                        bool hasColors = false;

                        XObject normalObject = null;
                        Dictionary<string, XObject> skinWeightObjects = new Dictionary<string, XObject>();

                        XElement geometry = new XElement(ns + "geometry", new XAttribute("id", "Mesh" + meshID + "-GEOMETRY"), new XAttribute("name", "Mesh" + meshID));
                        XElement mesh = new XElement(ns + "mesh");
                        XElement bindMaterialCommon = new XElement(ns + "technique_common");
                        geometry.Add(mesh);

                        XElement controller = null;

                        int materialCount = -1;
                        // Maps material indices to face indices
                        Dictionary<int, List<int>> materialGroups = null;

                        List<string> posList = new List<string>();
                        for (int i = 0; i < vertexCount; i++)
                        {
                            //XObjectStructure vec = ;
                            Vector3 vec = XUtils.Vector((XObjectStructure)obj["vertices"].Values[i]);
                            Vector4 newVec = Vector4.Transform(new Vector4(vec.X, vec.Y, vec.Z, 1.0f), transform);
                            posList.Add(newVec.X.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                            posList.Add(newVec.Y.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                            posList.Add(newVec.Z.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                        }
                        /*FloatDataSource positions = new FloatDataSource("mesh_positions", "mesh_positions_array", posList,
                            new DataAccessorParam("X", "float"),
                            new DataAccessorParam("Y", "float"),
                            new DataAccessorParam("Z", "float"));*/

                        XElement posSource = new XElement(ns + "source", new XAttribute("id", "Mesh" + meshID + "-POSITION"));
                        XElement posArray = new XElement(ns + "float_array", new XAttribute("id", "Mesh" + meshID + "-POSITION-array"), new XAttribute("count", vertexCount * 3 + ""));
                        posArray.Add(String.Join(" ", posList));
                        posSource.Add(posArray);
                        posSource.Add(new XElement(ns + "technique_common",
                            new XElement(ns + "accessor", new XAttribute("source", "#Mesh" + meshID + "-POSITION-array"), new XAttribute("count", vertexCount.ToString()), new XAttribute("stride", "3"),
                                new XElement(ns + "param", new XAttribute("name", "X"), new XAttribute("type", "float")),
                                new XElement(ns + "param", new XAttribute("name", "Y"), new XAttribute("type", "float")),
                                new XElement(ns + "param", new XAttribute("name", "Z"), new XAttribute("type", "float")))));
                        mesh.Add(posSource);

                        foreach (XChildObject child in obj.Children)
                        {
                            if (child.Object.DataType.NameData == "MeshNormals")
                            {
                                hasNormals = true;
                                normalObject = child.Object;

                                List<string> normList = new List<string>();
                                foreach (XObjectStructure value in child.Object["normals"].Values)
                                {
                                    Vector3 vec = XUtils.Vector(value);
                                    Vector4 newVec = Vector4.Transform(new Vector4(vec.X, vec.Y, vec.Z, 0.0f), transform);
                                    normList.Add(newVec.X.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                                    normList.Add(newVec.Y.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                                    normList.Add(newVec.Z.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                                }

                                XElement source = new XElement(ns + "source", new XAttribute("id", "Mesh" + meshID + "-NORMAL"));
                                XElement array = new XElement(ns + "float_array", new XAttribute("id", "Mesh" + meshID + "-NORMAL-array"), new XAttribute("count", (int)child.Object["nNormals"].Values[0] * 3 + ""));
                                array.Add(String.Join(" ", normList));
                                source.Add(array);
                                source.Add(new XElement(ns + "technique_common",
                                    new XElement(ns + "accessor", new XAttribute("source", "#Mesh" + meshID + "-NORMAL-array"), new XAttribute("count", (int)child.Object["nNormals"].Values[0] + ""), new XAttribute("stride", "3"),
                                        new XElement(ns + "param", new XAttribute("name", "X"), new XAttribute("type", "float")),
                                        new XElement(ns + "param", new XAttribute("name", "Y"), new XAttribute("type", "float")),
                                        new XElement(ns + "param", new XAttribute("name", "Z"), new XAttribute("type", "float")))));
                                mesh.Add(source);
                            }
                            else if (child.Object.DataType.NameData == "MeshTextureCoords")
                            {
                                hasTexCoords = true;

                                List<string> uvList = new List<string>();
                                foreach (XObjectStructure value in child.Object["textureCoords"].Values)
                                {
                                    double u = Convert.ToDouble(value["u"].Values[0]);
                                    double v = Convert.ToDouble(value["v"].Values[0]);
                                    if (flipV)
                                        v = 1.0 - v;
                                    uvList.Add(u.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                                    uvList.Add(v.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                                }

                                XElement source = new XElement(ns + "source", new XAttribute("id", "Mesh" + meshID + "-UV"));
                                XElement array = new XElement(ns + "float_array", new XAttribute("id", "Mesh" + meshID + "-UV-array"), new XAttribute("count", (int)child.Object["nTextureCoords"].Values[0] * 2 + ""));
                                array.Add(String.Join(" ", uvList));
                                source.Add(array);
                                source.Add(new XElement(ns + "technique_common",
                                    new XElement(ns + "accessor", new XAttribute("source", "#Mesh" + meshID + "-UV-array"), new XAttribute("count", (int)child.Object["nTextureCoords"].Values[0] + ""), new XAttribute("stride", "2"),
                                        new XElement(ns + "param", new XAttribute("name", "S"), new XAttribute("type", "float")),
                                        new XElement(ns + "param", new XAttribute("name", "T"), new XAttribute("type", "float")))));
                                mesh.Add(source);
                            }
                            else if (child.Object.DataType.NameData == "MeshVertexColors")
                            {
                                hasColors = true;

                                List<string> colorList = new List<string>();
                                Vector4[] colors = new Vector4[(int)child.Object["nVertexColors"].Values[0]];
                                foreach (XObjectStructure value in child.Object["vertexColors"].Values)
                                {
                                    int index = (int)value["index"].Values[0];
                                    Vector4 color = XUtils.ColorRGBA((XObjectStructure)value.Members[1].Values[0]); // ["indexColor"]
                                    if (colors[index] == Vector4.Zero)
                                    {
                                        colors[index] = color;
                                    }
                                    else
                                    {
                                        Console.WriteLine("ExportCOLLADA: Multiple colors defined for vertex " + index + "!");
                                    }
                                }

                                foreach (Vector4 v in colors)
                                {
                                    colorList.Add(v.X.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                                    colorList.Add(v.Y.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                                    colorList.Add(v.Z.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                                    colorList.Add(v.W.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                                }

                                XElement source = new XElement(ns + "source", new XAttribute("id", "Mesh" + meshID + "-COLOR"));
                                XElement array = new XElement(ns + "float_array", new XAttribute("id", "Mesh" + meshID + "-COLOR-array"), new XAttribute("count", (int)child.Object["nVertexColors"].Values[0] * 4 + ""));
                                array.Add(String.Join(" ", colorList));
                                source.Add(array);
                                source.Add(new XElement(ns + "technique_common",
                                    new XElement(ns + "accessor", new XAttribute("source", "#Mesh" + meshID + "-COLOR-array"), new XAttribute("count", (int)child.Object["nVertexColors"].Values[0] + ""), new XAttribute("stride", "4"),
                                        new XElement(ns + "param", new XAttribute("name", "R"), new XAttribute("type", "float")),
                                        new XElement(ns + "param", new XAttribute("name", "G"), new XAttribute("type", "float")),
                                        new XElement(ns + "param", new XAttribute("name", "B"), new XAttribute("type", "float")),
                                        new XElement(ns + "param", new XAttribute("name", "A"), new XAttribute("type", "float")))));
                                mesh.Add(source);
                            }
                            else if (child.Object.DataType.NameData == "MeshMaterialList")
                            {
                                materialCount = (int)child.Object["nMaterials"].Values[0];
                                int nFaceIndexes = (int)child.Object["nFaceIndexes"].Values[0];
                                materialGroups = new Dictionary<int, List<int>>();

                                for (int i = 0; i < nFaceIndexes; i++)
                                {
                                    int matIndex = (int)child.Object["faceIndexes"].Values[i];

                                    if (!usedMaterials.Contains(matIndex))
                                        usedMaterials.Add(matIndex);

                                    if (materialGroups.ContainsKey(matIndex))
                                        materialGroups[matIndex].Add(i);
                                    else
                                        materialGroups.Add(matIndex, new List<int>(new int[] { i }));
                                }

                                int materialIndex = 0;
                                foreach (XChildObject material in child.Object.Children)
                                {
                                    if (usedMaterials.Contains(materialIndex) || !stripUnusedMaterials)
                                    {
                                        Vector4 faceColor = XUtils.ColorRGBA((XObjectStructure)material.Object["faceColor"].Values[0]);
                                        double power = Convert.ToDouble(material.Object["power"].Values[0]);
                                        Vector3 specularColor = XUtils.ColorRGB((XObjectStructure)material.Object["specularColor"].Values[0]);
                                        Vector3 emissiveColor = XUtils.ColorRGB((XObjectStructure)material.Object["emissiveColor"].Values[0]);

                                        // Material
                                        library_materials.Add(new XElement(ns + "material", new XAttribute("id", "Material" + materialIndex), new XAttribute("name", "Material" + materialIndex),
                                            new XElement(ns + "instance_effect", new XAttribute("url", "#Material" + materialIndex + "-EFFECT"))));

                                        // Bindings in Geometry Instance
                                        bindMaterialCommon.Add(new XElement(ns + "instance_material", new XAttribute("symbol", "Material" + materialIndex), new XAttribute("target", "#Material" + materialIndex)));

                                        // Texture
                                        bool hasTexture = false;
                                        string textureID = "";
                                        XElement textureReference = null;
                                        foreach (XChildObject texChild in material.Object.Children)
                                        {
                                            if (texChild.Object.DataType.NameData == "TextureFilename")
                                            {
                                                hasTexture = true;
                                                textureID = "Texture" + library_images.Elements().Count();
                                                string texFilename = (string)texChild.Object["filename"].Values[0];
                                                if (textureExtension != null)
                                                    texFilename = System.IO.Path.ChangeExtension(texFilename, textureExtension);

                                                library_images.Add(new XElement(ns + "image", new XAttribute("id", textureID), new XAttribute("name", textureID),
                                                    new XElement(ns + "init_from", "file://" + texFilename)));

                                                textureReference = new XElement(ns + "texture", new XAttribute("texture", textureID), new XAttribute("texcoord", "CHANNEL0")); // TODO: Add Maya-specific wrap info

                                                break; // There should only be one TextureFilename per Material.
                                            }
                                        }

                                        // Effect
                                        library_effects.Add(new XElement(ns + "effect", new XAttribute("id", "Material" + materialIndex + "-EFFECT"), new XAttribute("name", "Material" + materialIndex),
                                            new XElement(ns + "profile_COMMON",
                                                new XElement(ns + "technique", new XAttribute("sid", "standard"),
                                                    new XElement(ns + "phong",
                                                        new XElement(ns + "emission",
                                                            new XElement(ns + "color", new XAttribute("sid", "emission"), "0.0 0.0 0.0 1.0")),
                                                        new XElement(ns + "ambient",
                                                            new XElement(ns + "color", new XAttribute("sid", "ambient"), emissiveColor.X + " " + emissiveColor.Y + " " + emissiveColor.Z + " 1.000000")),
                                                        new XElement(ns + "diffuse",
                                                            hasTexture ? textureReference : new XElement(ns + "color", new XAttribute("sid", "diffuse"), faceColor.X + " " + faceColor.Y + " " + faceColor.Z + " 1.0")),
                                                        new XElement(ns + "specular",
                                                            new XElement(ns + "color", new XAttribute("sid", "specular"), power > 0.0 ? specularColor.X + " " + specularColor.Y + " " + specularColor.Z + " 1.0" : "0.0 0.0 0.0 1.0")),
                                                        new XElement(ns + "shininess",
                                                            new XElement(ns + "float", new XAttribute("sid", "shininess"), power.ToString())),
                                                        new XElement(ns + "reflective",
                                                            new XElement(ns + "color", new XAttribute("sid", "reflective"), "0.0 0.0 0.0 1.0")),
                                                        new XElement(ns + "reflectivity",
                                                            new XElement(ns + "float", new XAttribute("sid", "reflectivity"), "0.5")),
                                                        new XElement(ns + "transparent", new XAttribute("opaque", "RGB_ZERO"),
                                                            new XElement(ns + "color", new XAttribute("sid", "transparent"), "0.0 0.0 0.0 1.0")),
                                                        new XElement(ns + "transparency",
                                                            new XElement(ns + "float", new XAttribute("sid", "transparency"), faceColor.W.ToString())))))));
                                    }
                                    materialIndex++;
                                }
                            }
                            else if (child.Object.DataType.NameData == "XSkinMeshHeader")
                            {

                            }
                            else if (child.Object.DataType.NameData == "SkinWeights")
                            {
                                string name = (string)child.Object["transformNodeName"].Values[0];
                                Console.WriteLine("    " + name);
                                skinWeightObjects.Add(name, child.Object);

                                // Detect biped vs non-biped
                                if (skeleton.NameSlots == null)
                                {
                                    if (name == BHDFile.BipedBoneNames[0])
                                    {
                                        skeleton.NameSlots = BHDFile.BipedBoneNames;
                                    }
                                    else if (name == BHDFile.NonBipedBoneNames[0])
                                    {
                                        skeleton.NameSlots = BHDFile.NonBipedBoneNames;
                                    }
                                    else
                                    {
                                        Console.WriteLine("[ERROR]: Root bone '" + name + "' isn't biped or non-biped!");
                                    }
                                }
                            }
                        }

                        mesh.Add(new XElement(ns + "vertices", new XAttribute("id", "Mesh" + meshID + "-VERTEX"),
                            new XElement(ns + "input", new XAttribute("semantic", "POSITION"), new XAttribute("source", "#Mesh" + meshID + "-POSITION"))));

                        if (materialGroups == null)
                        {
                            throw new FormatException("ExportCOLLADA: Mesh didn't have a MeshMaterialList!");
                        }
                        else
                        {
                            foreach (KeyValuePair<int, List<int>> pair in materialGroups)
                            {
                                int offset = 0;
                                XElement triangles = new XElement(ns + "triangles", new XAttribute("count", pair.Value.Count.ToString()), new XAttribute("material", "Material" + pair.Key),
                                    new XElement(ns + "input", new XAttribute("semantic", "VERTEX"), new XAttribute("offset", offset.ToString()), new XAttribute("source", "#Mesh" + meshID + "-VERTEX")));
                                offset++;

                                List<List<string>> pInputs = new List<List<string>>(); // Will be interleaved.

                                // VERTEX input
                                List<string> vertexIndices = new List<string>();
                                foreach (int faceIndex in pair.Value)
                                {
                                    XObjectStructure face = (XObjectStructure)obj["faces"].Values[faceIndex];

                                    foreach (int vertexIndex in face["faceVertexIndices"].Values)
                                    {
                                        vertexIndices.Add(vertexIndex.ToString());
                                    }
                                }
                                pInputs.Add(vertexIndices);

                                // NORMAL input
                                if (hasNormals)
                                {
                                    triangles.Add(new XElement(ns + "input", new XAttribute("semantic", "NORMAL"), new XAttribute("offset", offset.ToString()), new XAttribute("source", "#Mesh" + meshID + "-NORMAL")));
                                    offset++;

                                    List<string> normalIndices = new List<string>();
                                    foreach (int faceIndex in pair.Value)
                                    {
                                        XObjectStructure face = (XObjectStructure)normalObject["faceNormals"].Values[faceIndex];

                                        foreach (int normalIndex in face["faceVertexIndices"].Values)
                                        {
                                            normalIndices.Add(normalIndex.ToString());
                                        }
                                    }
                                    pInputs.Add(normalIndices);
                                }

                                // TEXCOORD input
                                if (hasTexCoords)
                                {
                                    triangles.Add(new XElement(ns + "input", new XAttribute("semantic", "TEXCOORD"), new XAttribute("offset", offset.ToString()), new XAttribute("set", "0"), new XAttribute("source", "#Mesh" + meshID + "-UV")));
                                    offset++;

                                    // .X texture coordinate indices match position indices.
                                    pInputs.Add(vertexIndices);
                                }

                                // COLOR input
                                if (hasColors)
                                {
                                    triangles.Add(new XElement(ns + "input", new XAttribute("semantic", "COLOR"), new XAttribute("offset", offset.ToString()), new XAttribute("source", "#Mesh" + meshID + "-COLOR")));
                                    offset++;

                                    // We have already arranged the colors to match vertex order, so we can use position indices.
                                    pInputs.Add(vertexIndices);
                                }

                                // Interleave pInputs into the <p> element!
                                XElement p = new XElement(ns + "p");

                                int index = 0;
                                foreach (int faceIndex in pair.Value)
                                {
                                    XObjectStructure face = (XObjectStructure)obj["faces"].Values[faceIndex];

                                    for (int vertexIndex = 0; vertexIndex < face["faceVertexIndices"].Values.Count; vertexIndex++)
                                    {
                                        foreach (List<string> input in pInputs)
                                            p.Add(input[index] + " ");
                                        index++;
                                    }
                                }

                                triangles.Add(p);

                                mesh.Add(triangles);
                            }
                        }

                        library_geometries.Add(geometry);

                        if (skeleton == null)
                        {
                            visual_scene.Add(new XElement(ns + "node", new XAttribute("name", "Mesh" + meshID + "Instance"), new XAttribute("id", "Mesh" + meshID + "Instance"), new XAttribute("sid", "Mesh" + meshID + "Instance"),
                                new XElement(ns + "matrix", new XAttribute("sid", "matrix"), "1.0 0.0 0.0 0.0 0.0 1.0 0.0 0.0 0.0 0.0 1.0 0.0 0.0 0.0 0.0 1.0"),
                                new XElement(ns + "instance_geometry", new XAttribute("url", "#Mesh" + meshID + "-GEOMETRY"),
                                    new XElement(ns + "bind_material",
                                        bindMaterialCommon))));
                        }
                        else
                        {
                            List<string> usedNames = new List<string>();
                            List<float> bindPoseData = new List<float>();
                            List<float> weightsData = new List<float>();

                            // Aggregate all the bone influences from per-bone (.X style) data to per-vertex (.DAE style) data.
                            List<int> countData = new List<int>(); // The counts of influences per vertex
                            List<int> indexData = new List<int>(); // for each vertex, for each influence, name index followed by weight index.
                            List<List<Tuple<int, int>>> vertexData = new List<List<Tuple<int, int>>>(); // List of List of indexes into usedNames and weightsData
                            for (int i = 0; i < vertexCount; i++)
                                vertexData.Add(new List<Tuple<int, int>>()); // Pre-populate with empty lists
                            for (int bone = 0; bone < skeleton.Bones.Count; bone++)
                            {
                                BHDFile.Bone b = skeleton.Bones[bone];
                                if (b.ParentIndex == bone) // Stand-alone (root) bones are their own parents.
                                {
                                    // Create the bone nodes in the scene
                                    visual_scene.Add(ExportCOLLADABone(b, Matrix.Transpose(transform), skeleton.NameSlots));
                                }
                                if (b.ParentIndex != 0xFFFFFFFF)
                                {
                                    string name = skeleton.NameSlots[b.Index];
                                    usedNames.Add(name);
                                    XObject skinWeights = skinWeightObjects[name];
                                    List<float> matrixParts = new List<float>();
                                    foreach (object ob in ((XObjectStructure)skinWeights["matrixOffset"].Values[0])["matrix"].Values)
                                        matrixParts.Add((float)(double)ob);
                                    Matrix trans = new Matrix(matrixParts.ToArray());
                                    trans = Matrix.Multiply(Matrix.Invert(transform), trans);
                                    trans.Transpose();
                                    foreach (float f in trans.ToArray())
                                        bindPoseData.Add(f);

                                    List<object> indices = skinWeights["vertexIndices"].Values;
                                    List<object> weights = skinWeights["weights"].Values;
                                    int nWeights = (int)skinWeights["nWeights"].Values[0];
                                    int usedCount = 0;
                                    for (int i = 0; i < nWeights; i++)
                                    {
                                        float weight = (float)(double)weights[i];
                                        if (weight > 0.0001)
                                        {
                                            int weightIndex = weightsData.IndexOf(weight);
                                            if (weightIndex == -1)
                                            {
                                                weightIndex = weightsData.Count;
                                                weightsData.Add(weight);
                                            }
                                            usedCount++;
                                            vertexData[(int)indices[i]].Add(new Tuple<int, int>(usedNames.IndexOf(name), weightIndex));
                                        }
                                    }
                                    if (usedCount == 0)
                                        Console.WriteLine("        [WARNING]: Bone '" + skeleton.NameSlots[b.Index] + "' influences zero vertices!");
                                }
                            }
                            for (int vertex = 0; vertex < vertexData.Count; vertex++)
                            {
                                countData.Add(vertexData[vertex].Count);
                                foreach (Tuple<int, int> vIndices in vertexData[vertex])
                                {
                                    indexData.Add(vIndices.Item1);
                                    indexData.Add(vIndices.Item2);
                                }
                            }

                            XElement nameSource = new XElement(ns + "source", new XAttribute("id", "libctl-Mesh" + meshID + "-SKIN-NAMES"),
                                                    new XElement(ns + "Name_array", new XAttribute("id", "libctl-Mesh" + meshID + "-SKIN-NAMES-DATA"), new XAttribute("count", usedNames.Count), String.Join(" ", usedNames)),
                                                    new XElement(ns + "technique_common", 
                                                        new XElement(ns + "accessor", new XAttribute("source", "#libctl-Mesh" + meshID + "-SKIN-NAMES-DATA"), new XAttribute("count", usedNames.Count), new XAttribute("stride", "1"),
                                                            new XElement(ns + "param", new XAttribute("name", "JOINT"), new XAttribute("type", "name")))));

                            XElement bindPoseSource = new XElement(ns + "source", new XAttribute("id", "libctl-Mesh" + meshID + "-SKIN-POSE"),
                                                        new XElement(ns + "float_array", new XAttribute("id", "libctl-Mesh" + meshID + "-SKIN-POSE-DATA"), new XAttribute("count", bindPoseData.Count.ToString()), String.Join(" ", bindPoseData)),
                                                        new XElement(ns + "technique_common",
                                                            new XElement(ns + "accessor", new XAttribute("source", "#libctl-Mesh" + meshID + "-SKIN-POSE-DATA"), new XAttribute("count", bindPoseData.Count / 16), new XAttribute("stride", "16"),
                                                                new XElement(ns + "param", new XAttribute("name", "TRANSFORM"), new XAttribute("type", "float4x4")))));

                            XElement weightsSource = new XElement(ns + "source", new XAttribute("id", "libctl-Mesh" + meshID + "-SKIN-WEIGHTS"),
                                                        new XElement(ns + "float_array", new XAttribute("id", "libctl-Mesh" + meshID + "-SKIN-WEIGHTS-DATA"), new XAttribute("count", weightsData.Count.ToString()), String.Join(" ", weightsData)),
                                                        new XElement(ns + "technique_common",
                                                            new XElement(ns + "accessor", new XAttribute("source", "#libctl-Mesh" + meshID + "-SKIN-WEIGHTS-DATA"), new XAttribute("count", weightsData.Count.ToString()), new XAttribute("stride", "1"),
                                                                new XElement(ns + "param", new XAttribute("name", "WEIGHT"), new XAttribute("type", "float")))));

                            XElement joints = new XElement(ns + "joints",
                                                new XElement(ns + "input", new XAttribute("semantic", "JOINT"), new XAttribute("source", "#libctl-Mesh" + meshID + "-SKIN-NAMES")),
                                                new XElement(ns + "input", new XAttribute("semantic", "INV_BIND_MATRIX"), new XAttribute("source", "#libctl-Mesh" + meshID + "-SKIN-POSE")));

                            XElement vertexWeights = new XElement(ns + "vertex_weights", new XAttribute("count", vertexCount),
                                                        new XElement(ns + "input", new XAttribute("semantic", "JOINT"), new XAttribute("source", "#libctl-Mesh" + meshID + "-SKIN-NAMES"), new XAttribute("offset", "0")),
                                                        new XElement(ns + "input", new XAttribute("semantic", "WEIGHT"), new XAttribute("source", "#libctl-Mesh" + meshID + "-SKIN-WEIGHTS"), new XAttribute("offset", "0")),
                                                        new XElement(ns + "vcount", String.Join(" ", countData)),
                                                        new XElement(ns + "v", String.Join(" ", indexData)));

                            controller = new XElement(ns + "controller", new XAttribute("id", "libctl-Mesh" + meshID + "-SKIN"), new XAttribute("name", "Mesh" + meshID + "-SKIN"),
                                                        new XElement(ns + "skin", new XAttribute("source", "#Mesh" + meshID + "-GEOMETRY"),
                                                            new XElement(ns + "bind_shape_matrix", "1 0 0 0 0 1 0 0 0 0 1 0 0 0 0 1"), // Identity matrix
                                                            nameSource,
                                                            bindPoseSource,
                                                            weightsSource,
                                                            joints,
                                                            vertexWeights));

                            library_controllers.Add(controller);

                            visual_scene.Add(new XElement(ns + "node", new XAttribute("name", "Mesh" + meshID + "Instance"), new XAttribute("id", "Mesh" + meshID + "Instance"), new XAttribute("sid", "Mesh" + meshID + "Instance"),
                                new XElement(ns + "instance_controller", new XAttribute("url", "#libctl-Mesh" + meshID + "-SKIN"),
                                    new XElement(ns + "skeleton", "#" + skeleton.NameSlots[0]), // HACK: assume there is always exactly one valid root bone, and it is the first bone.
                                    new XElement(ns + "bind_material",
                                        bindMaterialCommon))));
                        }

                        meshID++;
                    }
                }
            }

            if (animation != null)
            {
                foreach (TransformAnimation track in animation.Entries)
                {
                    // Glob keys in the same frame together.
                    Dictionary<int, Tuple<QuaternionKeyframe, VectorKeyframe, VectorKeyframe>> keyframes = new Dictionary<int, Tuple<QuaternionKeyframe, VectorKeyframe, VectorKeyframe>>();
                    foreach (QuaternionKeyframe quat in track.RotationKeyframes)
                    {
                        keyframes.Add(quat.Frame, new Tuple<QuaternionKeyframe, VectorKeyframe, VectorKeyframe>(quat, null, null));
                    }
                    foreach (VectorKeyframe trans in track.TranslationKeyframes)
                    {
                        if (keyframes.ContainsKey(trans.Frame))
                        {
                            keyframes[trans.Frame] = new Tuple<QuaternionKeyframe, VectorKeyframe, VectorKeyframe>(keyframes[trans.Frame].Item1, trans, null);
                        }
                        else 
                        {
                            keyframes.Add(trans.Frame, new Tuple<QuaternionKeyframe, VectorKeyframe, VectorKeyframe>(null, trans, null));
                        }
                    }
                    foreach (VectorKeyframe scale in track.ScaleKeyframes)
                    {
                        if (keyframes.ContainsKey(scale.Frame))
                        {
                            keyframes[scale.Frame] = new Tuple<QuaternionKeyframe, VectorKeyframe, VectorKeyframe>(keyframes[scale.Frame].Item1, keyframes[scale.Frame].Item2, scale);
                        }
                        else
                        {
                            keyframes.Add(scale.Frame, new Tuple<QuaternionKeyframe, VectorKeyframe, VectorKeyframe>(null, null, scale));
                        }
                    }

                    string boneName = skeleton.NameSlots[track.BoneID];

                    // Keyframe times
                    XElement timeFloatArray = new XElement(ns + "float_array", new XAttribute("id", boneName + "-Matrix-animation-input-array"), new XAttribute("count", keyframes.Count.ToString()));
                    StringBuilder sb = new StringBuilder();
                    foreach (int frame in keyframes.Keys)
                    {
                        sb.Append((float)frame / BKD.FRAMES_PER_SECOND);
                        sb.Append(" ");
                    }
                    timeFloatArray.Value = sb.ToString();
                    XElement timeSource = new XElement(ns + "source", new XAttribute("id", boneName + "-Matrix-animation-input"),
                        timeFloatArray,
                        new XElement(ns + "technique_common",
                            new XElement(ns + "accessor", new XAttribute("source", "#" + boneName + "-Matrix-animation-input-array"), new XAttribute("count", keyframes.Count.ToString()),
                                new XElement(ns + "param", new XAttribute("name", "TIME"), new XAttribute("type", "float")))));

                    // Keyframe values
                    XElement transformFloatArray = new XElement(ns + "float_array", new XAttribute("id", boneName + "-Matrix-animation-output-transform-array"), new XAttribute("count", keyframes.Count * 16));
                    sb.Clear();
                    foreach (var pair in keyframes)
                    {
                        // Calculate the transform at this keyframe
                        Matrix bindPoseTransform = skeleton.Bones[track.BoneID].Transform;
                        bindPoseTransform.Transpose();
                        Vector3 scale = Vector3.One;
                        if (pair.Value.Item3 != null)
                        {
                            //bindPoseTransform.Decompose(out scale, out _, out _);
                            //scale = new Vector3(scale.X * pair.Value.Item3.Float1, scale.Y * pair.Value.Item3.Float2, scale.Z * pair.Value.Item3.Float3);
                            scale = new Vector3(pair.Value.Item3.X, pair.Value.Item3.Y, pair.Value.Item3.Z);
                        }
                        else
                        {
                            bindPoseTransform.Decompose(out scale, out _, out _);
                        }
                        Vector3 translation = Vector3.Zero;
                        if (pair.Value.Item2 != null)
                        {
                            //translation = bindPoseTransform.TranslationVector;
                            //translation += new Vector3(pair.Value.Item2.Float1, pair.Value.Item2.Float2, pair.Value.Item2.Float3);
                            translation = new Vector3(pair.Value.Item2.X, pair.Value.Item2.Y, pair.Value.Item2.Z);
                        }
                        else
                        {
                            translation = bindPoseTransform.TranslationVector;
                        }
                        Quaternion rotation = Quaternion.Identity;
                        if (pair.Value.Item1 != null)
                        {
                            //bindPoseTransform.Decompose(out _, out rotation, out _);
                            //rotation = new Quaternion(DecompressShort(pair.Value.Item1.Short2), DecompressShort(pair.Value.Item1.Short3), DecompressShort(pair.Value.Item1.Short4),
                            //    DecompressShort(pair.Value.Item1.Short5)) * rotation;
                            rotation = new Quaternion(pair.Value.Item1.X, pair.Value.Item1.Y, pair.Value.Item1.Z, pair.Value.Item1.W);
                        }
                        else
                        {
                            bindPoseTransform.Decompose(out _, out rotation, out _);
                        }
                        //rotation = Quaternion.RotationMatrix(transform) * rotation;
                        Matrix trans = Matrix.Scaling(scale) * Matrix.RotationQuaternion(rotation) * Matrix.Translation(translation);

                        //trans = bindPoseTransform;

                        if (track.BoneID == 0)
                            trans = trans * transform;

                        trans.Transpose();
                        float[] elements = trans.ToArray();
                        for (int j = 0; j < 16; j++)
                        {
                            sb.Append(elements[j]);
                            sb.Append(" ");
                        }
                    }
                    transformFloatArray.Value = sb.ToString();
                    XElement transformSource = new XElement(ns + "source", new XAttribute("id", boneName + "-Matrix-animation-output-transform"),
                        transformFloatArray,
                        new XElement(ns + "technique_common",
                            new XElement(ns + "accessor", new XAttribute("source", "#" + boneName + "-Matrix-animation-output-transform-array"), new XAttribute("count", keyframes.Count.ToString()), new XAttribute("stride", "16"),
                                new XElement(ns + "param", new XAttribute("type", "float4x4")))));

                    // Keyframe interpolations
                    XElement interpolationNameArray = new XElement(ns + "Name_array", new XAttribute("id", boneName + "-Interpolations-array"), new XAttribute("count", keyframes.Count.ToString()));
                    sb.Clear();
                    for (int k = 0; k < keyframes.Count; k++)
                        sb.Append("LINEAR ");
                    interpolationNameArray.Value = sb.ToString();
                    XElement interpolationSource = new XElement(ns + "source", new XAttribute("id", boneName + "-Interpolations"),
                        interpolationNameArray,
                        new XElement(ns + "technique_common",
                            new XElement(ns + "accessor", new XAttribute("source", "#" + boneName + "-Interpolations-array"), new XAttribute("count", keyframes.Count.ToString()),
                                new XElement(ns + "param", new XAttribute("type", "name")))));

                    XElement animationTrack = new XElement(ns + "animation", new XAttribute("id", boneName + "-anim"), new XAttribute("name", boneName),
                        new XElement(ns + "animation",
                            timeSource,
                            transformSource,
                            interpolationSource,
                            new XElement(ns + "sampler", new XAttribute("id", boneName + "-Matrix-animation-transform"),
                                new XElement(ns + "input", new XAttribute("semantic", "INPUT"), new XAttribute("source", "#" + boneName + "-Matrix-animation-input")),
                                new XElement(ns + "input", new XAttribute("semantic", "OUTPUT"), new XAttribute("source", "#" + boneName + "-Matrix-animation-output-transform")),
                                new XElement(ns + "input", new XAttribute("semantic", "INTERPOLATION"), new XAttribute("source", "#" + boneName + "-Interpolations"))),
                            new XElement(ns + "channel", new XAttribute("source", "#" + boneName + "-Matrix-animation-transform"), new XAttribute("target", boneName + "/matrix"))));

                    libarary_animations.Add(animationTrack);
                }
            }

            COLLADA.Add(library_images);
            COLLADA.Add(library_materials);
            COLLADA.Add(library_effects);
            COLLADA.Add(library_geometries);
            if (skeleton != null)
                COLLADA.Add(library_controllers);
            if (animation != null)
                COLLADA.Add(libarary_animations);
            COLLADA.Add(new XElement(ns + "library_visual_scenes", visual_scene));
            COLLADA.Add(new XElement(ns + "scene",
                new XElement(ns + "instance_visual_scene", new XAttribute("url", "#Scene"))));

            doc.Add(COLLADA);
            doc.Save(filename);
        }
    }
}
