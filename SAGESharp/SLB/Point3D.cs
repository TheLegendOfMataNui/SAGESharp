/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.SLB.IO;
using System;

namespace SAGESharp.SLB
{
    public sealed class Point3D : IEquatable<Point3D>
    {
        [SerializableProperty(1)]
        public float X { get; set; }

        [SerializableProperty(2)]
        public float Y { get; set; }

        [SerializableProperty(3)]
        public float Z { get; set; }

        public bool Equals(Point3D other)
        {
            if (other == null)
            {
                return false;
            }

            return X == other.X &&
                Y == other.Y &&
                Z == other.Z;
        }

        public override string ToString()
            => $"X={X}, Y={Y}, Z={Z}";

        public override bool Equals(object other)
            => Equals(other as Point3D);

        public override int GetHashCode()
        {
            int hash = 5381;
            X.AddHashCodeByVal(ref hash, 4243);
            Y.AddHashCodeByVal(ref hash, 4243);
            Z.AddHashCodeByVal(ref hash, 4243);

            return hash;
        }

        public static bool operator ==(Point3D left, Point3D right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            else if (left is null)
            {
                return right.Equals(left);
            }
            else
            {
                return left.Equals(right);
            }
        }

        public static bool operator !=(Point3D left, Point3D right)
            => !(left == right);
    }
}
