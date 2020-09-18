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
    /// <summary>
    /// Stores the animation of a transformation by storing separate translation, rotation, and scale keyframes.
    /// </summary>
    public sealed class TransformAnimation : IEquatable<TransformAnimation>, IBinarySerializable
    {
        #region Fields
        private IList<QuaternionKeyframe> rotationKeyframes = new List<QuaternionKeyframe>();

        private IList<VectorKeyframe> translationKeyframes = new List<VectorKeyframe>();

        private IList<VectorKeyframe> scaleKeyframes = new List<VectorKeyframe>();
        #endregion

        /// <summary>
        /// The ID of the bone that this animation animates.
        /// </summary>
        public ushort BoneID { get; set; }

        /// <summary>
        /// The keyframes for the rotation of this transform.
        /// </summary>
        public IList<QuaternionKeyframe> RotationKeyframes
        {
            get => rotationKeyframes;
            set => rotationKeyframes = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// The keyframes for the translation of this transform.
        /// </summary>
        public IList<VectorKeyframe> TranslationKeyframes
        {
            get => translationKeyframes;
            set => translationKeyframes = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// The keyframes for the scale of this transform.
        /// </summary>
        public IList<VectorKeyframe> ScaleKeyframes
        {
            get => scaleKeyframes;
            set => scaleKeyframes = value ?? throw new ArgumentNullException(nameof(value));
        }

        #region IBinarySerializable
        public void Read(IBinaryReader binaryReader)
        {
            ushort newId = binaryReader.ReadUInt16();
            ushort rotationsCount = binaryReader.ReadUInt16();
            ushort translationsCount = binaryReader.ReadUInt16();
            ushort scalesCount = binaryReader.ReadUInt16();

            IList<QuaternionKeyframe> rotationData = ReadEntries<QuaternionKeyframe>(binaryReader, rotationsCount);

            IList<VectorKeyframe> translationData = ReadEntries<VectorKeyframe>(binaryReader, translationsCount);

            IList<VectorKeyframe> scaleData = ReadEntries<VectorKeyframe>(binaryReader, scalesCount);

            BoneID = newId;
            RotationKeyframes = rotationData;
            TranslationKeyframes = translationData;
            ScaleKeyframes = scaleData;
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
            binaryWriter.WriteUInt16(BoneID);
            binaryWriter.WriteUInt16((ushort)RotationKeyframes.Count);
            binaryWriter.WriteUInt16((ushort)TranslationKeyframes.Count);
            binaryWriter.WriteUInt16((ushort)ScaleKeyframes.Count);
            binaryWriter.WriteUInt32(0); // RotationData offset
            binaryWriter.WriteUInt32(0); // TranslationData offset
            binaryWriter.WriteUInt32(0); // ScalingData offset
        }
        #endregion

        #region Equality
        public bool Equals(TransformAnimation other)
            => MemberwiseEqualityComparer<TransformAnimation>.ByProperties.Equals(this, other);

        public override bool Equals(object obj)
            => Equals(obj as TransformAnimation);

        public override int GetHashCode()
            => MemberwiseEqualityComparer<TransformAnimation>.ByProperties.GetHashCode(this);

        public static bool operator ==(TransformAnimation left, TransformAnimation right)
            => left?.Equals(right) ?? right?.Equals(left) ?? true;

        public static bool operator !=(TransformAnimation left, TransformAnimation right)
            => !(left == right);
        #endregion

        public override string ToString() => $"{nameof(TransformAnimation)}(" +
            $"{nameof(BoneID)}={BoneID}, " +
            $"{nameof(RotationKeyframes)}=[{string.Join(", ", RotationKeyframes)}], " +
            $"{nameof(TranslationKeyframes)}=[{string.Join(", ", TranslationKeyframes)}], " +
            $"{nameof(ScaleKeyframes)}=[{string.Join(", ", ScaleKeyframes)}]"
        + ")";
    }
}
