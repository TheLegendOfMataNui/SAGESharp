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

namespace SAGESharp.SLB.Character.MoveList
{
    public sealed class MoveListTable : IEquatable<MoveListTable>
    {
        [SerializableProperty(1)]
        public Identifier Id { get; set; }

        [SerializableProperty(2)]
        [DuplicateEntryCount]
        public IList<Animation> Animations { get; set; }

        [SerializableProperty(3)]
        [DuplicateEntryCount]
        public IList<AnimationWithExtra> AnimationsWithExtra { get; set; }

        #region ToString
        private static readonly ToStringMethod<MoveListTable> toString = new ToStringMethodBuilder<MoveListTable>()
            .UseProperties()
            .Substitute<IList<Animation>>(nameof(Animations), it => it.ListToString())
            .Substitute<IList<AnimationWithExtra>>(nameof(AnimationsWithExtra), it => it.ListToString())
            .Build();

        public override string ToString() => toString(this);
        #endregion

        #region Equals/GetHashCode
        private static readonly IEqualityComparer<MoveListTable> equalityComparer = MemberwiseEqualityComparer<MoveListTable>
            .ByProperties;

        public bool Equals(MoveListTable other) => equalityComparer.Equals(this, other);

        public override bool Equals(object obj) => Equals(obj as MoveListTable);

        public override int GetHashCode() => equalityComparer.GetHashCode(this);

        public static bool operator ==(MoveListTable left, MoveListTable right) => EqualsOperator(left, right);

        public static bool operator !=(MoveListTable left, MoveListTable right) => !(left == right);
        #endregion
    }
}
