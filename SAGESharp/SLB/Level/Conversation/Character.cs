/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.SLB.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAGESharp.SLB.Level.Conversation
{
    public sealed class Character : IEquatable<Character>
    {
        /// <summary>
        /// The size of a <see cref="Character"/> entry in a binary SLB file.
        /// </summary>
        /// 
        /// This includes only <see cref="ToaName"/>, <see cref="CharName"/>,
        /// <see cref="CharCont"/>, the size of <see cref="Entries"/> and the
        /// position (offset) for all the entries (not represented in <see cref="Character"/>)
        /// in that order.
        internal const int BINARY_SIZE = 20;

        [SLBElement(1)]
        public Identifier ToaName { get; set; }

        [SLBElement(2)]
        public Identifier CharName { get; set; }

        [SLBElement(3)]
        public Identifier CharCont { get; set; }

        [SLBElement(4)]
        public IList<Info> Entries { get; set; }

        public bool Equals(Character other)
        {
            if (other is null)
            {
                return false;
            }

            return ToaName == other.ToaName &&
                CharName == other.CharName &&
                CharCont == other.CharCont &&
                Entries.SafeSequenceEquals(other.Entries);
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat("ToaName={0}", ToaName).Append(", ");
            result.AppendFormat("CharName={0}", ToaName).Append(", ");
            result.AppendFormat("CharCont={0}", ToaName).Append(", ");
            if (Entries == null)
            {
                result.Append("Entries=null");
            }
            else if (Entries.Count != 0)
            {
                result.AppendFormat("Entries=[({0})]", string.Join("), (", Entries));
            }
            else
            {
                result.Append("Entries=[]");
            }

            return result.ToString();
        }

        public override bool Equals(object other)
        {
            return Equals(other as Character);
        }

        public override int GetHashCode()
        {
            int hash = 19;
            ToaName.AddHashCodeByVal(ref hash, 89);
            CharName.AddHashCodeByVal(ref hash, 89);
            CharCont.AddHashCodeByVal(ref hash, 89);
            Entries.AddHashCodesByRef(ref hash, 89, 53);

            return hash;
        }

        public static bool operator ==(Character left, Character right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            else if(left is null)
            {
                return right.Equals(left);
            }
            else
            {
                return left.Equals(right);
            }
        }

        public static bool operator !=(Character left, Character right)
        {
            return !(left == right);
        }
    }
}
