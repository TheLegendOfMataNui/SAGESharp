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
    public sealed class VectorKeyframe : IEquatable<VectorKeyframe>, IBinarySerializable
    {
        public int Frame { get; set; }

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

            Frame = keyframe;
            X = x;
            Y = y;
            Z = z;
        }

        public void Write(IBinaryWriter binaryWriter)
        {
            binaryWriter.WriteInt32(Frame);
            binaryWriter.WriteFloat(X);
            binaryWriter.WriteFloat(Y);
            binaryWriter.WriteFloat(Z);
        }
        #endregion

        #region Equality
        public bool Equals(VectorKeyframe other)
            => MemberwiseEqualityComparer<VectorKeyframe>.ByProperties.Equals(this, other);

        public override bool Equals(object obj)
            => Equals(obj as VectorKeyframe);

        public override int GetHashCode()
            => MemberwiseEqualityComparer<VectorKeyframe>.ByProperties.GetHashCode(this);

        public static bool operator ==(VectorKeyframe left, VectorKeyframe right)
            => left?.Equals(right) ?? right?.Equals(left) ?? true;

        public static bool operator !=(VectorKeyframe left, VectorKeyframe right)
            => !(left == right);
        #endregion

        public override string ToString() => $"{nameof(VectorKeyframe)}(" +
            $"{nameof(Frame)}={Frame}, " +
            $"{nameof(X)}={X}, " +
            $"{nameof(Y)}={Y}, " +
            $"{nameof(Z)}={Z}" +
        ")";
    }
}
