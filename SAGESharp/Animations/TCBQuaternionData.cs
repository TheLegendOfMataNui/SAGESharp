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
    public sealed class TCBQuaternionData : IEquatable<TCBQuaternionData>, IBinarySerializable
    {
        private static readonly float FLOAT_CONVERSION_CONSTANT = 1 / (float)Math.Pow(2, 15);

        private ushort x;
        private ushort y;
        private ushort z;
        private ushort w;

        public short Keyframe { get; set; }

        public float X
        {
            get => x * FLOAT_CONVERSION_CONSTANT;
            set => x = (ushort)(value / FLOAT_CONVERSION_CONSTANT);
        }

        public float Y
        {
            get => y * FLOAT_CONVERSION_CONSTANT;
            set => y = (ushort)(value / FLOAT_CONVERSION_CONSTANT);
        }

        public float Z
        {
            get => z * FLOAT_CONVERSION_CONSTANT;
            set => z = (ushort)(value / FLOAT_CONVERSION_CONSTANT);
        }

        public float W
        {
            get => w * FLOAT_CONVERSION_CONSTANT;
            set => w = (ushort)(value / FLOAT_CONVERSION_CONSTANT);
        }

        #region IBinarySerializable
        public void Read(IBinaryReader binaryReader)
        {
            short keyframe = binaryReader.ReadInt16();
            ushort x = binaryReader.ReadUInt16();
            ushort y = binaryReader.ReadUInt16();
            ushort z = binaryReader.ReadUInt16();
            ushort w = binaryReader.ReadUInt16();

            Keyframe = keyframe;
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public void Write(IBinaryWriter binaryWriter)
        {
            binaryWriter.WriteInt16(Keyframe);
            binaryWriter.WriteUInt16(x);
            binaryWriter.WriteUInt16(y);
            binaryWriter.WriteUInt16(z);
            binaryWriter.WriteUInt16(w);
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
            $"{nameof(Keyframe)}={Keyframe}, " +
            $"{nameof(X)}={X}, " +
            $"{nameof(Y)}={Y}, " +
            $"{nameof(Z)}={Z}, " +
            $"{nameof(W)}={W}" +
        ")";
    }
}
