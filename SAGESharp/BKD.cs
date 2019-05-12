/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using Konvenience;
using SAGESharp.IO;
using System;
using System.Collections.Generic;

namespace SAGESharp
{
    public sealed class BKD : IEquatable<BKD>, IBinarySerializable
    {
        #region Fields
        private IList<BKDEntry> entries = new List<BKDEntry>();
        #endregion

        public ushort Length { get; set; }

        public IList<BKDEntry> Entries
        {
            get => entries;
            set => entries = value ?? throw new ArgumentNullException(nameof(value));
        }

        #region IBinarySerializable
        public void Read(IBinaryReader binaryReader)
        {
            ushort newLength = binaryReader.ReadUInt16();
            ushort newEntriesCount = binaryReader.ReadUInt16();

            IList<BKDEntry> newEntries = new List<BKDEntry>(newEntriesCount);
            for (int n = 0; n < newEntriesCount; n++)
            {
                BKDEntry entry = new BKDEntry();
                entry.Read(binaryReader);
                newEntries.Add(entry);
            }

            Length = newLength;
            Entries = newEntries;
        }

        public void Write(IBinaryWriter binaryWriter)
        {
            void WriteObject(IBinarySerializable binarySerializable)
            {
                binarySerializable.Write(binaryWriter);
            }

            binaryWriter.WriteUInt16(Length);
            binaryWriter.WriteUInt16((ushort)Entries.Count);

            long[] offsetPositions = new long[Entries.Count];
            Entries.ForEach((entry, n) =>
            {
                offsetPositions[n] = binaryWriter.Position + 8;
                WriteObject(entry);
            });

            Entries.ForEach((entry, n) =>
            {
                long offsetPosition = offsetPositions[n];
                binaryWriter.DoAtPosition(offsetPosition, offset => binaryWriter.WriteUInt32((uint)offset));
                entry.TCBQuaternionData.ForEach(WriteObject);

                offsetPosition += 4;
                binaryWriter.DoAtPosition(offsetPosition, offset => binaryWriter.WriteUInt32((uint)offset));
                entry.TCBInterpolatorData1.ForEach(WriteObject);

                offsetPosition += 4;
                binaryWriter.DoAtPosition(offsetPosition, offset => binaryWriter.WriteUInt32((uint)offset));
                entry.TCBInterpolatorData2.ForEach(WriteObject);
            });
        }
        #endregion

        #region Equality
        public bool Equals(BKD other)
        {
            if (other == null)
            {
                return false;
            }

            return Length == other.Length &&
                Entries.SafeSequenceEquals(other.Entries);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BKD);
        }

        public override int GetHashCode()
        {
            int hash = 427477;
            Length.AddHashCodeByVal(ref hash, 446503);
            Entries.AddHashCodesByRef(ref hash, 446503, 542891);

            return hash;
        }

        public static bool operator ==(BKD left, BKD right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            else if (left is null)
            {
                return right.Equals(left);
            }
            else
            {
                return left.Equals(right);
            }
        }

        public static bool operator !=(BKD left, BKD right)
        {
            return !(left == right);
        }
        #endregion

        public override string ToString() => $"{nameof(BKD)}(" +
            $"{nameof(Length)}={Length}, " +
            $"{nameof(Entries)}=[{string.Join(", ", entries)}]" +
        ")";
    }

    public sealed class BKDEntry : IEquatable<BKDEntry>, IBinarySerializable
    {
        #region Fields
        private IList<TCBQuaternionData> tcbQuaternionData = new List<TCBQuaternionData>();

        private IList<TCBInterpolationData> tcbInterpolationData1 = new List<TCBInterpolationData>();

        private IList<TCBInterpolationData> tcbInterpolationData2 = new List<TCBInterpolationData>();
        #endregion

        public ushort Id { get; set; }

        public IList<TCBQuaternionData> TCBQuaternionData
        {
            get => tcbQuaternionData;
            set => tcbQuaternionData = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IList<TCBInterpolationData> TCBInterpolatorData1
        {
            get => tcbInterpolationData1;
            set => tcbInterpolationData1 = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IList<TCBInterpolationData> TCBInterpolatorData2
        {
            get => tcbInterpolationData2;
            set => tcbInterpolationData2 = value ?? throw new ArgumentNullException(nameof(value));
        }

        #region IBinarySerializable
        public void Read(IBinaryReader binaryReader)
        {
            ushort newId = binaryReader.ReadUInt16();
            ushort newTCBQuaternionDataCount = binaryReader.ReadUInt16();
            ushort newTCBInterpolatorData1Count = binaryReader.ReadUInt16();
            ushort newTCBInterpolatorData2Count = binaryReader.ReadUInt16();

            IList<TCBQuaternionData> newTCBQuaternionData = ReadEntry<TCBQuaternionData>(binaryReader, newTCBQuaternionDataCount);

            IList<TCBInterpolationData> newTCBInterpolatorData1 = ReadEntry<TCBInterpolationData>(binaryReader, newTCBInterpolatorData1Count);

            IList<TCBInterpolationData> newTCBInterpolatorData2 = ReadEntry<TCBInterpolationData>(binaryReader, newTCBInterpolatorData2Count);

            Id = newId;
            TCBQuaternionData = newTCBQuaternionData;
            TCBInterpolatorData1 = newTCBInterpolatorData1;
            TCBInterpolatorData2 = newTCBInterpolatorData2;
        }

        private static IList<T> ReadEntry<T>(IBinaryReader binaryReader, ushort count) where T : IBinarySerializable, new()
        {
            uint offset = binaryReader.ReadUInt32();
            IList <T> result = new List<T>(count);
            if (count != 0)
            {
                binaryReader.DoAtPosition(offset, () =>
                {
                    for (int n = 0; n < count; ++n)
                    {
                        result.Add(new T().Also(e => e.Read(binaryReader)));
                    }
                });
            }

            return result;
        }

        public void Write(IBinaryWriter binaryWriter)
        {
            binaryWriter.WriteUInt16(Id);
            binaryWriter.WriteUInt16((ushort)TCBQuaternionData.Count);
            binaryWriter.WriteUInt16((ushort)TCBInterpolatorData1.Count);
            binaryWriter.WriteUInt16((ushort)TCBInterpolatorData2.Count);
            binaryWriter.WriteUInt32(0); // TCBQuaternionData offset
            binaryWriter.WriteUInt32(0); // TCBInterpolatorData1 offset
            binaryWriter.WriteUInt32(0); // TCBInterpolatorData2 offset
        }
        #endregion

        #region Equality
        public bool Equals(BKDEntry other)
        {
            if (other == null)
            {
                return false;
            }

            return Id == other.Id &&
                TCBQuaternionData.SafeSequenceEquals(other.TCBQuaternionData) &&
                TCBInterpolatorData1.SafeSequenceEquals(other.TCBInterpolatorData1) &&
                TCBInterpolatorData2.SafeSequenceEquals(other.TCBInterpolatorData2);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BKDEntry);
        }

        public override int GetHashCode()
        {
            int hash = 216761;
            Id.AddHashCodeByVal(ref hash, 114371);
            TCBQuaternionData.AddHashCodesByRef(ref hash, 114371, 527929);
            TCBInterpolatorData1.AddHashCodesByRef(ref hash, 114371, 322891);
            TCBInterpolatorData2.AddHashCodesByRef(ref hash, 114371, 15053);

            return hash;
        }

        public static bool operator ==(BKDEntry left, BKDEntry right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            else if (left is null)
            {
                return right.Equals(left);
            }
            else
            {
                return left.Equals(right);
            }
        }

        public static bool operator !=(BKDEntry left, BKDEntry right)
        {
            return !(left == right);
        }
        #endregion

        public override string ToString() => $"{nameof(BKDEntry)}(" +
            $"{nameof(Id)}={Id}, " +
            $"{nameof(TCBQuaternionData)}=[{string.Join(", ", TCBQuaternionData)}], " +
            $"{nameof(TCBInterpolatorData1)}=[{string.Join(", ", TCBInterpolatorData1)}], " +
            $"{nameof(TCBInterpolatorData2)}=[{string.Join(", ", TCBInterpolatorData2)}]"
        + ")";
    }

    public sealed class TCBQuaternionData : IEquatable<TCBQuaternionData>, IBinarySerializable
    {
        public short Short1 { get; set; }

        public short Short2 { get; set; }

        public short Short3 { get; set; }

        public short Short4 { get; set; }

        public short Short5 { get; set; }

        #region IBinarySerializable
        public void Read(IBinaryReader binaryReader)
        {
            short newShort1 = binaryReader.ReadInt16();
            short newShort2 = binaryReader.ReadInt16();
            short newShort3 = binaryReader.ReadInt16();
            short newShort4 = binaryReader.ReadInt16();
            short newShort5 = binaryReader.ReadInt16();

            Short1 = newShort1;
            Short2 = newShort2;
            Short3 = newShort3;
            Short4 = newShort4;
            Short5 = newShort5;
        }

        public void Write(IBinaryWriter binaryWriter)
        {
            binaryWriter.WriteInt16(Short1);
            binaryWriter.WriteInt16(Short2);
            binaryWriter.WriteInt16(Short3);
            binaryWriter.WriteInt16(Short4);
            binaryWriter.WriteInt16(Short5);
        }
        #endregion

        #region Equality
        public bool Equals(TCBQuaternionData other)
        {
            if (other == null)
            {
                return false;
            }

            return Short1 == other.Short1 &&
                Short2 == other.Short2 &&
                Short3 == other.Short3 &&
                Short4 == other.Short4 &&
                Short5 == other.Short5;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TCBQuaternionData);
        }

        public override int GetHashCode()
        {
            int hash = 494917;
            Short1.AddHashCodeByVal(ref hash, 417881);
            Short2.AddHashCodeByVal(ref hash, 417881);
            Short3.AddHashCodeByVal(ref hash, 417881);
            Short4.AddHashCodeByVal(ref hash, 417881);
            Short5.AddHashCodeByVal(ref hash, 417881);

            return hash;
        }

        public static bool operator ==(TCBQuaternionData left, TCBQuaternionData right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            else if (left is null)
            {
                return right.Equals(left);
            }
            else
            {
                return left.Equals(right);
            }
        }

        public static bool operator !=(TCBQuaternionData left, TCBQuaternionData right)
        {
            return !(left == right);
        }
        #endregion

        public override string ToString() => $"{nameof(TCBQuaternionData)}(" +
            $"{nameof(Short1)}={Short1}, " +
            $"{nameof(Short2)}={Short2}, " +
            $"{nameof(Short3)}={Short3}, " +
            $"{nameof(Short4)}={Short4}, " +
            $"{nameof(Short5)}={Short5}" +
        ")";
    }

    public sealed class TCBInterpolationData : IEquatable<TCBInterpolationData>, IBinarySerializable
    {
        public int Long1 { get; set; }

        public float Float1 { get; set; }

        public float Float2 { get; set; }

        public float Float3 { get; set; }

        #region IBinarySerializable
        public void Read(IBinaryReader binaryReader)
        {
            int newLong1 = binaryReader.ReadInt32();
            float newFloat1 = binaryReader.ReadFloat();
            float newFloat2 = binaryReader.ReadFloat();
            float newFloat3 = binaryReader.ReadFloat();

            Long1 = newLong1;
            Float1 = newFloat1;
            Float2 = newFloat2;
            Float3 = newFloat3;
        }

        public void Write(IBinaryWriter binaryWriter)
        {
            binaryWriter.WriteInt32(Long1);
            binaryWriter.WriteFloat(Float1);
            binaryWriter.WriteFloat(Float2);
            binaryWriter.WriteFloat(Float3);
        }
        #endregion

        #region Equality
        public bool Equals(TCBInterpolationData other)
        {
            if (other == null)
            {
                return false;
            }

            return Long1 == other.Long1 &&
                Float1 == other.Float1 &&
                Float2 == other.Float2 &&
                Float3 == other.Float3;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TCBInterpolationData);
        }

        public override int GetHashCode()
        {
            int hash = 324871;
            Long1.AddHashCodeByVal(ref hash, 25667);
            Float1.AddHashCodeByVal(ref hash, 25667);
            Float2.AddHashCodeByVal(ref hash, 25667);
            Float3.AddHashCodeByVal(ref hash, 25667);

            return hash;
        }

        public static bool operator ==(TCBInterpolationData left, TCBInterpolationData right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            else if (left is null)
            {
                return right.Equals(left);
            }
            else
            {
                return left.Equals(right);
            }
        }

        public static bool operator !=(TCBInterpolationData left, TCBInterpolationData right)
        {
            return !(left == right);
        }
        #endregion

        public override string ToString() => $"{nameof(TCBInterpolationData)}(" +
            $"{nameof(Long1)}={Long1}, " +
            $"{nameof(Float1)}={Float1}, " +
            $"{nameof(Float2)}={Float2}, " +
            $"{nameof(Float3)}={Float3}" +
        ")";
    }
}
