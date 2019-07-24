/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using Konvenience;
using SAGESharp.IO;
using System;
using System.Collections.Generic;

namespace SAGESharp.SLB.Cinematic.Text
{
    public sealed class Text : IEquatable<Text>
    {
        [SerializableProperty(1)]
        public Identifier StrLabel { get; set; }

        [SerializableProperty(2)]
        public long StrIdx { get; set; }

        [SerializableProperty(3)]
        public IList<Data> Entries { get; set; }

        public bool Equals(Text other)
        {
            if (other == null)
            {
                return false;
            }

            return StrLabel == other.StrLabel &&
                StrIdx == other.StrIdx &&
                Entries.SafeSequenceEquals(other.Entries);
        }

        public override string ToString() => $"StrLabel={StrLabel}," +
            $"StrIdx={StrIdx}," +
            $"Locations={Entries?.Let(Entries => "[(" + string.Join("), (", Entries) + ")]") ?? "null"}";

        public override bool Equals(object other)
            => Equals(other as Text);

        public override int GetHashCode()
        {
            int hash = 3299;
            StrLabel.AddHashCodeByVal(ref hash, 2957);
            StrIdx.AddHashCodeByVal(ref hash, 2957);
            Entries.AddHashCodesByRef(ref hash, 2957, 4093);

            return hash;
        }

        public static bool operator ==(Text left, Text right)
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

        public static bool operator !=(Text left, Text right)
            => !(left == right);
    }

    public sealed class Data : IEquatable<Data>
    {
        [SerializableProperty(1)]
        public float Time { get; set; }

        [SerializableProperty(2)]
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
