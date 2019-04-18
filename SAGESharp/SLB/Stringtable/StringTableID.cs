/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using Konvenience;
using SAGESharp.SLB.IO;
using System;
using System.Collections.Generic;

namespace SAGESharp.SLB.Stringtable
{
    public sealed class StringTableID : IEquatable<StringTableID>
    {
        [SLBElement(1)]
        public Identifier Id { get; set; }

        [SLBElement(2)]
        public long Number { get; set; }


        public bool Equals(StringTableID other)
        {
            if (other == null)
            {
                return false;
            }

            return Id == other.Id &&
                Number == other.Number;
        }

        public override string ToString() => $"Id={Id}," +
            $"Number={Number},";

        public override bool Equals(object other)
            => Equals(other as StringTableID);

        public override int GetHashCode()
        {
            int hash = 7877;
            Id.AddHashCodeByVal(ref hash, 4021);
            Number.AddHashCodeByVal(ref hash, 4021);

            return hash;
        }

        public static bool operator ==(StringTableID left, StringTableID right)
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

        public static bool operator !=(StringTableID left, StringTableID right)
            => !(left == right);
    }
}
