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
        public short Keyframe { get; set; }

        public short X { get; set; }

        public short Y { get; set; }

        public short Z { get; set; }

        public short W { get; set; }

        #region IBinarySerializable
        public void Read(IBinaryReader binaryReader)
        {
            short keyframe = binaryReader.ReadInt16();
            short x = binaryReader.ReadInt16();
            short y = binaryReader.ReadInt16();
            short z = binaryReader.ReadInt16();
            short w = binaryReader.ReadInt16();

            Keyframe = keyframe;
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public void Write(IBinaryWriter binaryWriter)
        {
            binaryWriter.WriteInt16(Keyframe);
            binaryWriter.WriteInt16(X);
            binaryWriter.WriteInt16(Y);
            binaryWriter.WriteInt16(Z);
            binaryWriter.WriteInt16(W);
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
