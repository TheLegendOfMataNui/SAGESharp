using SAGESharp.SLB.IO;
using System;

namespace SAGESharp.SLB
{
    public sealed class Location : IEquatable<Location>
    {
        [SLBElement(1)]
        public float Time { get; set; }

        [SLBElement(2)]
        public Point3D Position { get; set; }

        [SLBElement(3)]
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
