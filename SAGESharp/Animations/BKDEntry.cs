/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using Equ;
using NUtils.Extensions;
using SAGESharp.IO.Binary;
using System;
using System.Collections.Generic;

namespace SAGESharp.Animations
{
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
}
