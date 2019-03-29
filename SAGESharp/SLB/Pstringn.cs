using SAGESharp.SLB.IO;
using System;

namespace SAGESharp.SLB
{
    public sealed class Pstringn : IEquatable<Pstringn>
    {
        [SLBElement(1)]
        public byte Length { get; set; }

        [SLBElement(2)]
        public IList<char> Characters { get; set; }

        [SLBElement(3)]
        public char Nul { get; set; }

        public bool Equals(Pstringn other)
        {
            if (other == null)
            {
                return false;
            }

            return Length == other.Length &&
                Characters.SafeSequenceEquals(other.Characters) &&
                Nul == other.Nul;
        }

        public override string ToString()
            => $"Length={Length}," +
              $"Characters={Characters?.Let(Characters => "[(" + string.Join("), (", Characters) + ")]") ?? "null"}," +
                $"Nul={Nul}";


        public override bool Equals(object other)
            => Equals(other as Pstringn);

        public override int GetHashCode()
        {
            int hash = 2837;
            Length.AddHashCodeByVal(ref hash, 6337);
            Characters.AddHashCodesByRef(ref hash, 6337, 6791);
            Nul.AddHashCodeByRef(ref hash, 6337);

            return hash;
        }

        public static bool operator ==(Pstringn left, Pstringn right)
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

        public static bool operator !=(Pstringn left, Pstringn right)
            => !(left == right);
    }
}
