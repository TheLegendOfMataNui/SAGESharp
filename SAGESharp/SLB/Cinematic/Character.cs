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
    internal sealed class CharacterTable : IEquatable<CharacterTable>
    {
        [SerializableProperty(1)]
        public IList<Character> Entries { get; set; }

        public bool Equals(CharacterTable other)
            => MemberwiseEqualityComparer<CharacterTable>.ByProperties.Equals(this, other);

        public override string ToString() =>
            $"Characters={Entries?.Let(Characters => "[(" + string.Join("), (", Characters) + ")]") ?? "null"}";

        public override bool Equals(object other)
            => Equals(other as CharacterTable);

        public override int GetHashCode()
            => MemberwiseEqualityComparer<CharacterTable>.ByProperties.GetHashCode(this);

        public static bool operator ==(CharacterTable left, CharacterTable right)
            => left?.Equals(right) ?? right?.Equals(left) ?? true;

        public static bool operator !=(CharacterTable left, CharacterTable right)
            => !(left == right);
    }

    internal sealed class Character : IEquatable<Character>
    {
        [SerializableProperty(1)]
        public Identifier AnimHierarchy { get; set; }

        [SerializableProperty(2)]
        public Identifier CharacterId { get; set; }

        [SerializableProperty(3)]
        public Identifier AnimBaked { get; set; }

        [SerializableProperty(4)]
        public float SwitchMaskTimes1 { get; set; }

        [SerializableProperty(5)]
        public float SwitchMaskTimes2 { get; set; }

        [SerializableProperty(6)]
        public IList<Location> Locations { get; set; }

        public bool Equals(Character other)
            => MemberwiseEqualityComparer<Character>.ByProperties.Equals(this, other);

        public override string ToString() => $"AnimHierarchy={AnimHierarchy}," +
            $"Character={CharacterId}," +
            $"AnimBaked={AnimBaked}," +
            $"SwitchMaskTimes1={SwitchMaskTimes1}," +
            $"SwitchMaskTimes2={SwitchMaskTimes2}," +
            $"Locations={Locations?.Let(Locations => "[(" + string.Join("), (", Locations) + ")]") ?? "null"}";

        public override bool Equals(object other)
            => Equals(other as Character);

        public override int GetHashCode()
            => MemberwiseEqualityComparer<Character>.ByProperties.GetHashCode(this);

        public static bool operator ==(Character left, Character right)
            => left?.Equals(right) ?? right?.Equals(left) ?? true;

        public static bool operator !=(Character left, Character right)
            => !(left == right);
    }
}
