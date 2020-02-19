/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using Equ;
using NUtils.MethodBuilders;
using SAGESharp.IO;
using System;
using System.Collections.Generic;

using static SAGESharp.Utils.EqualityUtils;
using static SAGESharp.Utils.ToStringUtils;

namespace SAGESharp.SLB.Level.Conversation
{
    public sealed class Character : IEquatable<Character>
    {
        [SerializableProperty(1)]
        public Identifier ToaName { get; set; }

        [SerializableProperty(2)]
        public Identifier CharName { get; set; }

        [SerializableProperty(3)]
        public Identifier CharCont { get; set; }

        [SerializableProperty(4)]
        public IList<Info> Entries { get; set; }

        #region ToString
        private static readonly ToStringMethod<Character> toString = new ToStringMethodBuilder<Character>()
            .UseProperties()
            .Substitute<IList<Info>>(nameof(Entries), entries => entries.ListToString())
            .Build();

        public override string ToString() => toString(this);
        #endregion

        #region Equals/GetHashCode
        private static readonly IEqualityComparer<Character> equalityComparer = MemberwiseEqualityComparer<Character>
            .ByProperties;

        public bool Equals(Character other) => equalityComparer.Equals(this, other);

        public override bool Equals(object obj) => Equals(obj as Character);

        public override int GetHashCode() => equalityComparer.GetHashCode(this);

        public static bool operator ==(Character left, Character right) => EqualsOperator(left, right);

        public static bool operator !=(Character left, Character right) => !(left == right);
        #endregion
    }
}
