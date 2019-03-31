using Konvenience;
using SAGESharp.SLB.IO;
using System;
using System.Collections.Generic;

namespace SAGESharp.SLB.Cinematic.Character
{
    public sealed class CharTable : IEquatable<CharTable>
    {
        [SLBElement(1)]
        public IList<Character> Characters { get; set; }


        public bool Equals(CharTable other)
        {
            if (other == null)
            {
                return false;
            }

            return Characters.SafeSequenceEquals(other.Characters);
        }

        public override string ToString() =>
            $"Characters={Characters?.Let(Characters => "[(" + string.Join("), (", Characters) + ")]") ?? "null"}";

        public override bool Equals(object other)
            => Equals(other as CharTable);

        public override int GetHashCode()
        {
            int hash = 739;
            Characters.AddHashCodesByRef(ref hash, 3217, 5563);

            return hash;
        }

        public static bool operator ==(CharTable left, CharTable right)
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

        public static bool operator !=(CharTable left, CharTable right)
            => !(left == right);
    }
}
