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
    public sealed class BKD : IEquatable<BKD>, IBinarySerializable
    {
        public const int FRAMES_PER_SECOND = 60;

        #region Fields
        private IList<TransformAnimation> entries = new List<TransformAnimation>();
        private ushort length;
        #endregion

        public float Length
        {
            get => length / (float)FRAMES_PER_SECOND;
            set => length = (ushort)(value * FRAMES_PER_SECOND);
        }

        public IList<TransformAnimation> Entries
        {
            get => entries;
            set => entries = value ?? throw new ArgumentNullException(nameof(value));
        }

        #region IBinarySerializable
        public void Read(IBinaryReader binaryReader)
        {
            ushort newLength = binaryReader.ReadUInt16();
            ushort newEntriesCount = binaryReader.ReadUInt16();

            IList<TransformAnimation> newEntries = new List<TransformAnimation>(newEntriesCount);
            for (int n = 0; n < newEntriesCount; n++)
            {
                TransformAnimation entry = new TransformAnimation();
                entry.Read(binaryReader);
                newEntries.Add(entry);
            }

            length = newLength;
            Entries = newEntries;
        }

        public void Write(IBinaryWriter binaryWriter)
        {
            void WriteObject(IBinarySerializable binarySerializable)
            {
                binarySerializable.Write(binaryWriter);
            }

            binaryWriter.WriteUInt16(length);
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
                entry.RotationKeyframes.ForEach(WriteObject);

                offsetPosition += 4;
                binaryWriter.DoAtPosition(offsetPosition, offset => binaryWriter.WriteUInt32((uint)offset));
                entry.TranslationKeyframes.ForEach(WriteObject);

                offsetPosition += 4;
                binaryWriter.DoAtPosition(offsetPosition, offset => binaryWriter.WriteUInt32((uint)offset));
                entry.ScaleKeyframes.ForEach(WriteObject);
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
}
