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
        public int Keyframe { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        #region IBinarySerializable
        public void Read(IBinaryReader binaryReader)
        {
            int keyframe = binaryReader.ReadInt32();
            float x = binaryReader.ReadFloat();
            float y = binaryReader.ReadFloat();
            float z = binaryReader.ReadFloat();

            Keyframe = keyframe;
            X = x;
            Y = y;
            Z = z;
        }

        public void Write(IBinaryWriter binaryWriter)
        {
            binaryWriter.WriteInt32(Keyframe);
            binaryWriter.WriteFloat(X);
            binaryWriter.WriteFloat(Y);
            binaryWriter.WriteFloat(Z);
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
            $"{nameof(Keyframe)}={Keyframe}, " +
            $"{nameof(X)}={X}, " +
            $"{nameof(Y)}={Y}, " +
            $"{nameof(Z)}={Z}" +
        ")";
    }
}
