using Konvenience;
using SAGESharp.SLB.IO;
using System;
using System.Collections.Generic;

namespace SAGESharp.SLB.Cinematic.Sound
{
    public sealed class SoundTable : IEquatable<SoundTable>
    {
        [SLBElement(1)]
        public IList<Pstringn> Sounds { get; set; }


        public bool Equals(SoundTable other)
        {
            if (other == null)
            {
                return false;
            }

            return Sounds.SafeSequenceEquals(other.Sounds);
        }

        public override string ToString() =>
            $"Objects={Sounds?.Let(Sounds => "[(" + string.Join("), (", Sounds) + ")]") ?? "null"}";

        public override bool Equals(object other)
            => Equals(other as SoundTable);

        public override int GetHashCode()
        {
            int hash = 8527;
            Sounds.AddHashCodesByRef(ref hash, 5503, 2687);

            return hash;
        }

        public static bool operator ==(SoundTable left, SoundTable right)
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

        public static bool operator !=(SoundTable left, SoundTable right)
            => !(left == right);
    }
}
