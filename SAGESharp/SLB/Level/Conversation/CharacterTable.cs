/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using Equ;
using NUtils.MethodBuilders;
using SAGESharp.IO;
using SAGESharp.Utils;
using System;
using System.Collections.Generic;

using static SAGESharp.Utils.EqualityUtils;
using static SAGESharp.Utils.ToStringUtils;

namespace SAGESharp.SLB.Level.Conversation
{
    public sealed class CharacterTable : IEquatable<CharacterTable>
    {
        [SerializableProperty(1, name: nameof(Conversation))]
        public IList<Character> Entries { get; set; }

        #region ToString
        private static readonly ToStringMethod<CharacterTable> toString = new ToStringMethodBuilder<CharacterTable>()
            .UseProperties()
            .Substitute<IList<Character>>(nameof(Entries), entries => entries.ListToString())
            .Build();

        public override string ToString() => toString(this);
        #endregion

        #region Equals/GetHashCode
        private static readonly IEqualityComparer<CharacterTable> equalityComparer = MemberwiseEqualityComparer<CharacterTable>
            .ByProperties;

        public bool Equals(CharacterTable other) => equalityComparer.Equals(this, other);

        public override bool Equals(object obj) => Equals(obj as CharacterTable);

        public override int GetHashCode() => equalityComparer.GetHashCode(this);

        public static bool operator ==(CharacterTable left, CharacterTable right) => EqualsOperator(left, right);

        public static bool operator !=(CharacterTable left, CharacterTable right) => !(left == right);
        #endregion
    }
}
