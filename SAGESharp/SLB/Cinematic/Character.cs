/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUtils.Extensions;
using SAGESharp.IO;
using System;
using System.Collections.Generic;

namespace SAGESharp.SLB.Cinematic
{
    public sealed class CharacterTable : IEquatable<CharacterTable>
    {
        [SerializableProperty(1)]
        public IList<Character> Entries { get; set; }

        public bool Equals(CharacterTable other)
        {
            if (other == null)
            {
                return false;
            }

            return Entries.SafeSequenceEquals(other.Entries);
        }

        public override string ToString() =>
            $"Characters={Entries?.Let(Characters => "[(" + string.Join("), (", Characters) + ")]") ?? "null"}";

        public override bool Equals(object other)
            => Equals(other as CharacterTable);

        public override int GetHashCode()
        {
            int hash = 739;
            Entries.AddHashCodesByRef(ref hash, 3217, 5563);

            return hash;
        }

        public static bool operator ==(CharacterTable left, CharacterTable right)
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

        public static bool operator !=(CharacterTable left, CharacterTable right)
            => !(left == right);
    }

    public sealed class Character : IEquatable<Character>
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
        {
            if (other == null)
            {
                return false;
            }

            return AnimHierarchy == other.AnimHierarchy &&
                CharacterId == other.CharacterId &&
                AnimBaked == other.AnimBaked &&
                SwitchMaskTimes1 == other.SwitchMaskTimes1 &&
                SwitchMaskTimes2 == other.SwitchMaskTimes2 &&
                Locations.SafeSequenceEquals(other.Locations);
        }

        public override string ToString() => $"AnimHierarchy={AnimHierarchy}," +
            $"Character={CharacterId}," +
            $"AnimBaked={AnimBaked}," +
            $"SwitchMaskTimes1={SwitchMaskTimes1}," +
            $"SwitchMaskTimes2={SwitchMaskTimes2}," +
            $"Locations={Locations?.Let(Locations => "[(" + string.Join("), (", Locations) + ")]") ?? "null"}";

        public override bool Equals(object other)
            => Equals(other as Character);

        public override int GetHashCode()
        {
            int hash = 7639;
            AnimHierarchy.AddHashCodeByVal(ref hash, 4539);
            CharacterId.AddHashCodeByVal(ref hash, 4539);
            AnimBaked.AddHashCodeByVal(ref hash, 4539);
            SwitchMaskTimes1.AddHashCodeByVal(ref hash, 4539);
            SwitchMaskTimes2.AddHashCodeByVal(ref hash, 4539);
            Locations.AddHashCodesByRef(ref hash, 4539, 8219);

            return hash;
        }

        public static bool operator ==(Character left, Character right)
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

        public static bool operator !=(Character left, Character right)
            => !(left == right);
    }
}
