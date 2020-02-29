/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using Equ;
using NUtils.MethodBuilders;
using SAGESharp.IO;
using SAGESharp.IO.Binary;
using System;
using System.Collections.Generic;

using static SAGESharp.Utils.EqualityUtils;
using static SAGESharp.Utils.ToStringUtils;

namespace SAGESharp.SLB.Character.AnimationEvents
{
    public sealed class AnimationEventsTable : IEquatable<AnimationEventsTable>
    {
        [SerializableProperty(1, name: nameof(AnimationEvents))]
        [DuplicateEntryCount]
        public IList<AnimationEvent> Entries { get; set; }

        #region ToString
        private static readonly ToStringMethod<AnimationEventsTable> toString = new ToStringMethodBuilder<AnimationEventsTable>()
            .UseProperties()
            .Substitute<IList<AnimationEvent>>(nameof(Entries), entries => entries.ListToString())
            .Build();

        public override string ToString() => toString(this);
        #endregion

        #region Equals/GetHashCode
        private static readonly IEqualityComparer<AnimationEventsTable> equalityComparer = MemberwiseEqualityComparer<AnimationEventsTable>
            .ByProperties;

        public bool Equals(AnimationEventsTable other) => equalityComparer.Equals(this, other);

        public override bool Equals(object obj) => Equals(obj as AnimationEventsTable);

        public override int GetHashCode() => equalityComparer.GetHashCode(this);

        public static bool operator ==(AnimationEventsTable left, AnimationEventsTable right) => EqualsOperator(left, right);

        public static bool operator !=(AnimationEventsTable left, AnimationEventsTable right) => !(left == right);
        #endregion
    }
}
