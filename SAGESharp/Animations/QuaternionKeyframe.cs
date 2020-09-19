/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using Equ;
using SAGESharp.IO.Binary;
using System;

namespace SAGESharp.Animations
{
    public sealed class QuaternionKeyframe : IEquatable<QuaternionKeyframe>, IBinarySerializable
    {
        private short x;
        private short y;
        private short z;
        private short w;

        public short Frame { get; set; }

        public float X
        {
            get => (float)x / Int16.MaxValue;
            set => x = (short)(value * Int16.MaxValue);
        }

        public float Y
        {
            get => (float)y / Int16.MaxValue;
            set => y = (short)(value * Int16.MaxValue);
        }

        public float Z
        {
            get => (float)z / Int16.MaxValue;
            set => z = (short)(value * Int16.MaxValue);
        }

        public float W
        {
            get => (float)w / Int16.MaxValue;
            set => w = (short)(value * Int16.MaxValue);
        }

        #region IBinarySerializable
        public void Read(IBinaryReader binaryReader)
        {
            short keyframe = binaryReader.ReadInt16();
            short x = binaryReader.ReadInt16();
            short y = binaryReader.ReadInt16();
            short z = binaryReader.ReadInt16();
            short w = binaryReader.ReadInt16();

            Frame = keyframe;
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public void Write(IBinaryWriter binaryWriter)
        {
            binaryWriter.WriteInt16(Frame);
            binaryWriter.WriteInt16(x);
            binaryWriter.WriteInt16(y);
            binaryWriter.WriteInt16(z);
            binaryWriter.WriteInt16(w);
        }
        #endregion

        #region Equality
        public bool Equals(QuaternionKeyframe other)
            => MemberwiseEqualityComparer<QuaternionKeyframe>.ByProperties.Equals(this, other);

        public override bool Equals(object obj)
            => Equals(obj as QuaternionKeyframe);

        public override int GetHashCode()
            => MemberwiseEqualityComparer<QuaternionKeyframe>.ByProperties.GetHashCode(this);

        public static bool operator ==(QuaternionKeyframe left, QuaternionKeyframe right)
            => left?.Equals(right) ?? right?.Equals(left) ?? true;

        public static bool operator !=(QuaternionKeyframe left, QuaternionKeyframe right)
            => !(left == right);
        #endregion

        public override string ToString() => $"{nameof(QuaternionKeyframe)}(" +
            $"{nameof(Frame)}={Frame}, " +
            $"{nameof(X)}={X}, " +
            $"{nameof(Y)}={Y}, " +
            $"{nameof(Z)}={Z}, " +
            $"{nameof(W)}={W}" +
        ")";
    }
}
