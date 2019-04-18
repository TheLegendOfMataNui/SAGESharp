using Konvenience;
using SAGESharp.SLB.IO;
using System;
using System.Collections.Generic;

namespace SAGESharp.SLB.Stringtable
{
    public sealed class StringGroup : IEquatable<StringGroup>
    {
        [SLBElement(1)]
        public StringTableID Id { get; set; }

        [SLBElement(2)]
        public IList<string> Strings { get; set; }


        public bool Equals(StringGroup other)
        {
            if (other == null)
            {
                return false;
            }

            return Id == other.Id &&
                StrOffset == other.StrOffset;
        }

        public override string ToString() => $"Id={Id}," +
            $"Strings={Strings?.Let(Strings => "[(" + string.Join("), (", Strings) + ")]") ?? "null"}";

        public override bool Equals(object other)
            => Equals(other as StringGroup);

        public override int GetHashCode()
        {
            int hash = 9857;
            Id.AddHashCodeByVal(ref hash, 4523);
            Strings.AddHashCodesByRef(ref hash, 9623, 2347);

            return hash;
        }

        public static bool operator ==(StringGroup left, StringGroup right)
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

        public static bool operator !=(StringGroup left, StringGroup right)
            => !(left == right);
    }
}
