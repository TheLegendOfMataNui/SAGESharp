/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using Equ;
using NUtils.Extensions;
using SAGESharp.IO;
using System;
using System.Collections.Generic;

namespace SAGESharp.SLB.Cinematic
{
    internal sealed class Text : IEquatable<Text>
    {
        [SerializableProperty(1)]
        public Identifier StrLabel { get; set; }

        [SerializableProperty(2)]
        public long StrIdx { get; set; }

        [SerializableProperty(3)]
        public IList<Data> Entries { get; set; }

        public bool Equals(Text other)
            => MemberwiseEqualityComparer<Text>.ByProperties.Equals(this, other);

        public override string ToString() => $"StrLabel={StrLabel}," +
            $"StrIdx={StrIdx}," +
            $"Locations={Entries?.Let(Entries => "[(" + string.Join("), (", Entries) + ")]") ?? "null"}";

        public override bool Equals(object other)
            => Equals(other as Text);

        public override int GetHashCode()
            => MemberwiseEqualityComparer<Text>.ByProperties.GetHashCode(this);

        public static bool operator ==(Text left, Text right)
            => left?.Equals(right) ?? right?.Equals(left) ?? true;

        public static bool operator !=(Text left, Text right)
            => !(left == right);
    }

    internal sealed class Data : IEquatable<Data>
    {
        [SerializableProperty(1)]
        public float Time { get; set; }

        [SerializableProperty(2)]
        public long StrOffset { get; set; }

        public bool Equals(Data other)
            => MemberwiseEqualityComparer<Data>.ByProperties.Equals(this, other);

        public override string ToString() => $"Time={Time}," +
            $"StrOffset={StrOffset},";

        public override bool Equals(object other)
            => Equals(other as Data);

        public override int GetHashCode()
            => MemberwiseEqualityComparer<Data>.ByProperties.GetHashCode(this);

        public static bool operator ==(Data left, Data right)
            => left?.Equals(right) ?? right?.Equals(left) ?? true;

        public static bool operator !=(Data left, Data right)
            => !(left == right);
    }
}
