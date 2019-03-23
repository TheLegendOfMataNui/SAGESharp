using SAGESharp.SLB.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAGESharp.SLB.Cinematic.Camera
{
    public sealed class Frame : IEquatable<Frame>
    {

        internal const int BINARY_SIZE = 28;

        [SLBElement(1)]
        public float Time { get; set; }

        [SLBElement(2)]
        public Point3D Position { get; set; }

        [SLBElement(3)]
        public Point3D Target { get; set; }

        public bool Equals(Frame other)
        {
            if (other == null)
            {
                return false;
            }

            return Time == other.Time &&
                Position == other.Position &&
                Target == other.Target;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat("Time={0}", Time).Append(", ");
            result.AppendFormat("Position={0}", Position).Append(", ");
            result.AppendFormat("Target={0}", Target).Append(", ");

            return result.ToString();
        }

        public override bool Equals(object other)
        {
            return Equals(other as Frame);
        }

        public override int GetHashCode()
        {
            int hash = 7507;
            Time.AddHashCodeByVal(ref hash, 907);
            Position.AddHashCodeByVal(ref hash, 907);
            Target.AddHashCodeByVal(ref hash, 907);

            return hash;
        }

        public static bool operator ==(Frame left, Frame right)
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

        public static bool operator !=(Frame left, Frame right)
        {
            return !(left == right);
        }
    }
}
