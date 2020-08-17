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
}
