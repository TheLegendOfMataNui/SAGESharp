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
