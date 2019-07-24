/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO;
using System;

namespace SAGESharp.SLB.Cinematic
{
    public sealed class Location : IEquatable<Location>
    {
        [SerializableProperty(1)]
        public float Time { get; set; }

        [SerializableProperty(2)]
        public Point3D Position { get; set; }

        [SerializableProperty(3)]
        public Point3D Orientation { get; set; }

        public bool Equals(Location other)
        {
            if (other == null)
            {
                return false;
            }

            return Time == other.Time &&
                Position == other.Position &&
                Orientation == other.Orientation;
        }

        public override string ToString()
            => $"Time={Time}, Position={Position}, Orientation={Orientation}";

        public override bool Equals(object other)
            => Equals(other as Location);

        public override int GetHashCode()
        {
            int hash = 7673;
            Time.AddHashCodeByVal(ref hash, 9103);
            Position.AddHashCodeByRef(ref hash, 9103);
            Orientation.AddHashCodeByRef(ref hash, 9103);

            return hash;
        }

        public static bool operator ==(Location left, Location right)
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

        public static bool operator !=(Location left, Location right)
            => !(left == right);
    }
}
