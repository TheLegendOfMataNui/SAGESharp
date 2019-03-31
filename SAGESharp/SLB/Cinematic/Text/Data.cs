using Konvenience;
using SAGESharp.SLB.IO;
using System;
using System.Collections.Generic;

namespace SAGESharp.SLB.Cinematic.Text
{
    public sealed class Data : IEquatable<Data>
    {
        [SLBElement(1)]
        public float Time { get; set; }

        [SLBElement(2)]
        public long StrOffset { get; set; }


        public bool Equals(Data other)
        {
            if (other == null)
            {
                return false;
            }

            return Time == other.Time &&
                StrOffset == other.StrOffset;
        }

        public override string ToString() => $"Time={Time}," +
            $"StrOffset={StrOffset},";

        public override bool Equals(object other)
            => Equals(other as Data);

        public override int GetHashCode()
        {
            int hash = 9857;
            Time.AddHashCodeByVal(ref hash, 4523);
            StrOffset.AddHashCodeByVal(ref hash, 4523);

            return hash;
        }

        public static bool operator ==(Data left, Data right)
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

        public static bool operator !=(Data left, Data right)
            => !(left == right);
    }
}
