/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using Equ;
using SAGESharp.IO;
using System;

namespace SAGESharp.SLB.Cinematic
{
    internal sealed class Location : IEquatable<Location>
    {
        [SerializableProperty(1)]
        public float Time { get; set; }

        [SerializableProperty(2)]
        public Point3D Position { get; set; }

        [SerializableProperty(3)]
        public Point3D Orientation { get; set; }

        public bool Equals(Location other)
            => MemberwiseEqualityComparer<Location>.ByProperties.Equals(this, other);

        public override string ToString()
            => $"Time={Time}, Position={Position}, Orientation={Orientation}";

        public override bool Equals(object other)
            => Equals(other as Location);

        public override int GetHashCode()
            => MemberwiseEqualityComparer<Location>.ByProperties.GetHashCode(this);

        public static bool operator ==(Location left, Location right)
            => left?.Equals(right) ?? right?.Equals(left) ?? true;

        public static bool operator !=(Location left, Location right)
            => !(left == right);
    }
}
