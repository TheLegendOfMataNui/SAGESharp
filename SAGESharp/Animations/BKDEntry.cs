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
        private IList<TCBQuaternionData> rotationData = new List<TCBQuaternionData>();

        private IList<TCBInterpolationData> translationData = new List<TCBInterpolationData>();

        private IList<TCBInterpolationData> scalingData = new List<TCBInterpolationData>();
        #endregion

        public ushort Id { get; set; }

        public IList<TCBQuaternionData> RotationData
        {
            get => rotationData;
            set => rotationData = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IList<TCBInterpolationData> TranslationData
        {
            get => translationData;
            set => translationData = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IList<TCBInterpolationData> ScalingData
        {
            get => scalingData;
            set => scalingData = value ?? throw new ArgumentNullException(nameof(value));
        }

        #region IBinarySerializable
        public void Read(IBinaryReader binaryReader)
        {
            ushort newId = binaryReader.ReadUInt16();
            ushort rotationsCount = binaryReader.ReadUInt16();
            ushort translationsCount = binaryReader.ReadUInt16();
            ushort scalesCount = binaryReader.ReadUInt16();

            IList<TCBQuaternionData> rotationData = ReadEntries<TCBQuaternionData>(binaryReader, rotationsCount);

            IList<TCBInterpolationData> translationData = ReadEntries<TCBInterpolationData>(binaryReader, translationsCount);

            IList<TCBInterpolationData> scaleData = ReadEntries<TCBInterpolationData>(binaryReader, scalesCount);

            Id = newId;
            RotationData = rotationData;
            TranslationData = translationData;
            ScalingData = scaleData;
        }

        private static IList<T> ReadEntries<T>(IBinaryReader binaryReader, ushort count) where T : class, IBinarySerializable, new()
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
            binaryWriter.WriteUInt16((ushort)RotationData.Count);
            binaryWriter.WriteUInt16((ushort)TranslationData.Count);
            binaryWriter.WriteUInt16((ushort)ScalingData.Count);
            binaryWriter.WriteUInt32(0); // RotationData offset
            binaryWriter.WriteUInt32(0); // TranslationData offset
            binaryWriter.WriteUInt32(0); // ScalingData offset
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
            $"{nameof(RotationData)}=[{string.Join(", ", RotationData)}], " +
            $"{nameof(TranslationData)}=[{string.Join(", ", TranslationData)}], " +
            $"{nameof(ScalingData)}=[{string.Join(", ", ScalingData)}]"
        + ")";
    }
}
