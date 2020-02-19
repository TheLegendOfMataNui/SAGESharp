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
    internal sealed class ObjectTable : IEquatable<ObjectTable>
    {
        [SerializableProperty(1)]
        public IList<Object> Entries { get; set; }

        public bool Equals(ObjectTable other)
            => MemberwiseEqualityComparer<ObjectTable>.ByProperties.Equals(this, other);

        public override string ToString() =>
            $"Objects={Entries?.Let(Objects => "[(" + string.Join("), (", Objects) + ")]") ?? "null"}";

        public override bool Equals(object other)
            => Equals(other as ObjectTable);

        public override int GetHashCode()
            => MemberwiseEqualityComparer<ObjectTable>.ByProperties.GetHashCode(this);

        public static bool operator ==(ObjectTable left, ObjectTable right)
            => left?.Equals(right) ?? right?.Equals(left) ?? true;

        public static bool operator !=(ObjectTable left, ObjectTable right)
            => !(left == right);
    }

    internal sealed class Object : IEquatable<Object>
    {
        [SerializableProperty(1)]
        public Identifier Instance { get; set; }

        [SerializableProperty(2)]
        public Identifier Type { get; set; }

        [SerializableProperty(3)]
        public IList<Location> Locations { get; set; }

        public bool Equals(Object other)
            => MemberwiseEqualityComparer<Object>.ByProperties.Equals(this, other);

        public override string ToString() => $"Instance={Instance}," +
            $"Type={Type}," +
            $"Locations={Locations?.Let(Locations => "[(" + string.Join("), (", Locations) + ")]") ?? "null"}";

        public override bool Equals(object other)
            => Equals(other as Object);

        public override int GetHashCode()
            => MemberwiseEqualityComparer<Object>.ByProperties.GetHashCode(this);

        public static bool operator ==(Object left, Object right)
            => left?.Equals(right) ?? right?.Equals(left) ?? true;

        public static bool operator !=(Object left, Object right)
            => !(left == right);
    }
}
