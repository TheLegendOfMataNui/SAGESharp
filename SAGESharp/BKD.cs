/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using Equ;
using NUtils.Extensions;
using SAGESharp.IO;
using System;
using System.Collections.Generic;

namespace SAGESharp
{
    public sealed class BKD : IEquatable<BKD>, IBinarySerializable
    {
        public const int FRAMES_PER_SECOND = 60;

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
            => MemberwiseEqualityComparer<BKD>.ByProperties.Equals(this, other);

        public override bool Equals(object obj)
            => Equals(obj as BKD);

        public override int GetHashCode()
            => MemberwiseEqualityComparer<BKD>.ByProperties.GetHashCode(this);

        public static bool operator ==(BKD left, BKD right)
            => left?.Equals(right) ?? right?.Equals(left) ?? true;

        public static bool operator !=(BKD left, BKD right)
            => !(left == right);
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

        private static IList<T> ReadEntry<T>(IBinaryReader binaryReader, ushort count) where T : class, IBinarySerializable, new()
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
            => MemberwiseEqualityComparer<BKDEntry>.ByProperties.Equals(this, other);

        public override bool Equals(object obj)
            => Equals(obj as BKDEntry);

        public override int GetHashCode()
            => MemberwiseEqualityComparer<BKDEntry>.ByProperties.GetHashCode(this);

        public static bool operator ==(BKDEntry left, BKDEntry right)
            => left?.Equals(right) ?? right?.Equals(left) ?? true;

        public static bool operator !=(BKDEntry left, BKDEntry right)
            => !(left == right);
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
            => MemberwiseEqualityComparer<TCBQuaternionData>.ByProperties.Equals(this, other);

        public override bool Equals(object obj)
            => Equals(obj as TCBQuaternionData);

        public override int GetHashCode()
            => MemberwiseEqualityComparer<TCBQuaternionData>.ByProperties.GetHashCode(this);

        public static bool operator ==(TCBQuaternionData left, TCBQuaternionData right)
            => left?.Equals(right) ?? right?.Equals(left) ?? true;

        public static bool operator !=(TCBQuaternionData left, TCBQuaternionData right)
            => !(left == right);
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
            => MemberwiseEqualityComparer<TCBInterpolationData>.ByProperties.Equals(this, other);

        public override bool Equals(object obj)
            => Equals(obj as TCBInterpolationData);

        public override int GetHashCode()
            => MemberwiseEqualityComparer<TCBInterpolationData>.ByProperties.GetHashCode(this);

        public static bool operator ==(TCBInterpolationData left, TCBInterpolationData right)
            => left?.Equals(right) ?? right?.Equals(left) ?? true;

        public static bool operator !=(TCBInterpolationData left, TCBInterpolationData right)
            => !(left == right);
        #endregion

        public override string ToString() => $"{nameof(TCBInterpolationData)}(" +
            $"{nameof(Long1)}={Long1}, " +
            $"{nameof(Float1)}={Float1}, " +
            $"{nameof(Float2)}={Float2}, " +
            $"{nameof(Float3)}={Float3}" +
        ")";
    }
}
