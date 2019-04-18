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
    public sealed class StringTable : IEquatable<StringTable>
    {
        [SLBElement(1)]
        public IList<StringGroup> StringGroups { get; set; }


        public bool Equals(StringTable other)
        {
            if (other == null)
            {
                return false;
            }

            return StringGroups.SafeSequenceEquals(other.StringGroups);
        }

        public override string ToString() => $"StringGroups={StringGroups?.Let(StringGroups => "[(" + string.Join("), (", StringGroups) + ")]") ?? "null"}";

        public override bool Equals(object other)
            => Equals(other as StringGroup);

        public override int GetHashCode()
        {
            int hash = 7723;
            StringGroups.AddHashCodesByRef(ref hash, 9623, 2347);
            return hash;
        }

        public static bool operator ==(StringTable left, StringTable right)
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

        public static bool operator !=(StringTable left, StringTable right)
            => !(left == right);
    }
}
