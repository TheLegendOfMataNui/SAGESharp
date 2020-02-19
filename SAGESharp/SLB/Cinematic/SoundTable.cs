/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using Equ;
using NUtils.Extensions;
using SAGESharp.IO;
using SAGESharp.IO.Binary;
using System;
using System.Collections.Generic;

namespace SAGESharp.SLB.Cinematic
{
    internal sealed class SoundTable : IEquatable<SoundTable>
    {
        [SerializableProperty(1)]
        [OffsetString]
        public IList<string> Sounds { get; set; }

        public bool Equals(SoundTable other)
            => MemberwiseEqualityComparer<SoundTable>.ByProperties.Equals(this, other);

        public override string ToString() =>
            $"Objects={Sounds?.Let(Sounds => "[(" + string.Join("), (", Sounds) + ")]") ?? "null"}";

        public override bool Equals(object other)
            => Equals(other as SoundTable);

        public override int GetHashCode()
            => MemberwiseEqualityComparer<SoundTable>.ByProperties.GetHashCode(this);

        public static bool operator ==(SoundTable left, SoundTable right)
            => left?.Equals(right) ?? right?.Equals(left) ?? true;

        public static bool operator !=(SoundTable left, SoundTable right)
            => !(left == right);
    }
}
