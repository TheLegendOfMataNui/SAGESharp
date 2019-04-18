/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SharpDX;

namespace SAGESharp
{
    /// <summary>
    /// A file that stores a model's skeleton.
    /// </summary>
    public class BHDFile
    {
        /// <summary>
        /// The biped bone names as defined in the Legend of Mata Nui binary.
        /// </summary>
        public static string[] BipedBoneNames =
        {
            "Bip01",
            "Bip01_Pelvis",
            "Bip01_Spine",
            "Bip01_Spine1",
            "Bip01_Spine2",
            "Bip01_Spine3",
            "Bip01_R_Thigh",
            "Bip01_L_Thigh",
            "Bip01_R_Calf",
            "Bip01_L_Calf",
            "Bip01_R_Foot",
            "Bip01_L_Foot",
            "Bip01_R_Clavicle",
            "Bip01_L_Clavicle",
            "Bip01_R_UpperArm",
            "Bip01_L_UpperArm",
            "Bip01_R_Forearm",
            "Bip01_L_Forearm",
            "Bip01_R_Hand",
            "Bip01_L_Hand",
            "Bip01_Neck",
            "Bip01_Neck1",
            "Bip01_Head",
            "Bip01_R_Finger0",
            "Bip01_L_Finger0",
            "Bip01_R_Finger1",
            "Bip01_L_Finger1",
            "Bip01_R_Finger2",
            "Bip01_L_Finger2",
            "Bip01_R_Finger3",
            "Bip01_L_Finger3",
            "Bip01_R_Finger4",
            "Bip01_L_Finger4",
            "Bip01_R_Finger01",
            "Bip01_L_Finger01",
            "Bip01_R_Finger11",
            "Bip01_L_Finger11",
            "Bip01_R_Finger21",
            "Bip01_L_Finger21",
            "Bip01_R_Finger31",
            "Bip01_L_Finger31",
            "Bip01_R_Finger41",
            "Bip01_L_Finger41",
            "Bip01_R_Finger02",
            "Bip01_L_Finger02",
            "Bip01_R_Finger12",
            "Bip01_L_Finger12",
            "Bip01_R_Finger22",
            "Bip01_L_Finger22",
            "Bip01_R_Finger32",
            "Bip01_L_Finger32",
            "Bip01_R_Finger42",
            "Bip01_L_Finger42",
            "Bip01_R_Toe0",
            "Bip01_L_Toe0",
            "Bip01_R_Toe1",
            "Bip01_L_Toe1",
            "Bip01_R_Toe2",
            "Bip01_L_Toe2",
            "Bip01_R_Toe01",
            "Bip01_L_Toe01",
            "Bip01_R_Toe11",
            "Bip01_L_Toe11",
            "Bip01_R_Toe21",
            "Bip01_L_Toe21",
            "Bip01_R_Toe3",
            "Bip01_L_Toe3",
            "Bip01_R_Toe31",
            "Bip01_L_Toe31",
            "Bip01_R_Toe4",
            "Bip01_L_Toe4",
            "Bip01_R_Toe41",
            "Bip01_L_Toe41",
            "MiscBone0",
            "MiscBone1",
            "MiscBone2",
            "MiscBone3",
            "MiscBone4",
            "MiscBone5",
            "MiscBone6",
            "MiscBone7",
            "MiscBone8",
            "MiscBone9",
            "MiscBone10",
            "MiscBone11",
            "MiscBone12",
            "MiscBone13",
            "MiscBone14",
            "MiscBone15",
            "MiscBone16",
            "MiscBone17",
            "MiscBone18",
            "Bip01_Tail4",
            "Bip01_Tail3",
            "Bip01_Tail2",
            "Bip01_Neck4",
            "Bip01_Neck3",
            "Bip01_Neck2",
            "Bip01_Tail",
            "Bip01_Tail1"
        };

        /// <summary>
        /// The non-biped bone names as defined in the Legend of Mata Nui binary.
        /// </summary>
        public static string[] NonBipedBoneNames =
        {
            "root",
            "Dummy01",
            "Dummy02",
            "Dummy03",
            "Dummy04",
            "Dummy05",
            "Dummy06",
            "Dummy07",
            "Dummy08",
            "Dummy09",
            "Dummy10",
            "Dummy11",
            "Dummy12",
            "Dummy13",
            "Dummy14",
            "Dummy15",
            "Dummy16",
            "Dummy17",
            "Dummy18",
            "Dummy19",
            "Dummy20",
            "Dummy21",
            "Dummy22",
            "Dummy23",
            "Dummy24",
            "Dummy25",
            "Dummy26",
            "Dummy27",
            "Dummy28",
            "Dummy29",
            "Dummy30",
            "Dummy31",
            "Dummy32",
            "Dummy33",
            "Dummy34",
            "Dummy35",
            "Dummy36",
            "Dummy37",
            "Dummy38",
            "Dummy39",
            "Dummy40",
            "Dummy41",
            "Dummy42",
            "Dummy43",
            "Dummy44",
            "Dummy45",
            "Dummy46",
            "Dummy47",
            "Dummy48",
            "Dummy49",
            "Dummy50",
            "Dummy51",
            "Dummy52",
            "Dummy53",
            "Dummy54",
            "Dummy55",
            "Dummy56",
            "Dummy57",
            "Dummy58",
            "Dummy59"
        };

        /// <summary>
        /// A bone in the skeleton.
        /// </summary>
        public class Bone
        {
            /// <summary>
            /// Which bone this is in the file, and also the index into the name arrays (<see cref="BipedBoneNames"/> and <see cref="NonBipedBoneNames"/>)
            /// </summary>
            public uint Index;

            /// <summary>
            /// The index of the parent bone (or equal to <see cref="Index"/> if this bone is a root bone, or 0xFFFFFFFF if the name corresponding to <see cref="Index"/> isn't used in the skeleton.)
            /// </summary>
            public uint ParentIndex;

            /// <summary>
            /// The transform of this Bone relative to its parent Bone.
            /// </summary>
            public Matrix Transform;

            /// <summary>
            /// A List of all the bones that had this Bone's <see cref="Index"/> as their <see cref="ParentIndex"/> when the <see cref="BHDFile"/> was loaded.
            /// </summary>
            public List<Bone> Children = new List<Bone>(); // Not stored in the file, just computed to be helpful.
        }

        /// <summary>
        /// The <see cref="Bone"/>s in this BHDFile.
        /// </summary>
        public List<Bone> Bones = new List<Bone>();

        /// <summary>
        /// The possible names of the bones in this BHDFile.
        /// </summary>
        public string[] NameSlots = null;

        /// <summary>
        /// Creates an empty BHDFile.
        /// </summary>
        public BHDFile()
        {

        }

        /// <summary>
        /// Creates a BHDFile by reading the data from the given file.
        /// </summary>
        /// <param name="filename">The name of the file that data should be read from.</param>
        public BHDFile(string filename)
        {
            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                uint count = reader.ReadUInt32();
                for (uint i = 0; i < count; i++)
                {
                    Bone b = new Bone();
                    b.ParentIndex = reader.ReadUInt32();
                    b.Index = i;
                    b.Transform = Matrix.Identity;
                    Bones.Add(b);
                }
                for (int i = 0; i < count; i++)
                {
                    Bone b = Bones[i];
                    if (b.ParentIndex != 0xFFFFFFFF && b.ParentIndex != i)
                    {
                        Bones[(int)b.ParentIndex].Children.Add(b);
                    }

                    b.Transform.Column1 = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0.0f);
                    b.Transform.Column2 = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0.0f);
                    b.Transform.Column3 = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0.0f);
                    b.Transform.Column4 = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 1.0f);
                }
            }
        }

        /// <summary>
        /// Writes this BHDFile to the given file.
        /// </summary>
        /// <param name="filename">The filename to write to.</param>
        public void Write(string filename)
        {
            using (FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write((uint)Bones.Count);
                foreach (Bone b in Bones)
                {
                    writer.Write(b.ParentIndex);
                }
                foreach (Bone b in Bones)
                {
                    writer.Write(b.Transform.Column1.X);
                    writer.Write(b.Transform.Column1.Y);
                    writer.Write(b.Transform.Column1.Z);
                    writer.Write(b.Transform.Column2.X);
                    writer.Write(b.Transform.Column2.Y);
                    writer.Write(b.Transform.Column2.Z);
                    writer.Write(b.Transform.Column3.X);
                    writer.Write(b.Transform.Column3.Y);
                    writer.Write(b.Transform.Column3.Z);
                    writer.Write(b.Transform.Column4.X);
                    writer.Write(b.Transform.Column4.Y);
                    writer.Write(b.Transform.Column4.Z);
                }
            }
        }
    }
}
