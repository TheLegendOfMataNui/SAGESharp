using Konvenience;
using SAGESharp.SLB.IO;
using System;
using System.Collections.Generic;

namespace SAGESharp.SLB.Cinematic.Object
{
    public sealed class ObjectTable : IEquatable<ObjectTable>
    {
        [SLBElement(1)]
        public IList<Object> Objects { get; set; }


        public bool Equals(ObjectTable other)
        {
            if (other == null)
            {
                return false;
            }

            return Objects.SafeSequenceEquals(other.Objects);
        }

        public override string ToString() =>
            $"Characters={Objects?.Let(Objects => "[(" + string.Join("), (", Objects) + ")]") ?? "null"}";

        public override bool Equals(object other)
            => Equals(other as ObjectTable);

        public override int GetHashCode()
        {
            int hash = 2179;
            Characters.AddHashCodesByRef(ref hash, 2161, 2791);

            return hash;
        }

        public static bool operator ==(ObjectTable left, ObjectTable right)
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

        public static bool operator !=(ObjectTable left, ObjectTable right)
            => !(left == right);
    }
}
