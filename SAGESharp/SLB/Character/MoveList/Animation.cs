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
    public sealed class Animation : IEquatable<Animation>
    {
        [SerializableProperty(1)]
        public Identifier Id1 { get; set; }

        [SerializableProperty(2)]
        public Identifier Id2 { get; set; }

        [SerializableProperty(3)]
        public short Flags1 { get; set; }

        [SerializableProperty(4)]
        public short Index { get; set; }

        [SerializableProperty(5)]
        public int Int1 { get; set; }

        [SerializableProperty(6)]
        public float Float1 { get; set; }

        [SerializableProperty(7)]
        public float Float2 { get; set; }

        [SerializableProperty(8)]
        public int Flags2 { get; set; }

        [SerializableProperty(9)]
        [RightPadding(2)]
        public short ReservedCounter { get; set; }

        [SerializableProperty(10)]
        [DuplicateEntryCount]
        public IList<SplitTrigger> Triggers { get; set; }

        #region ToString
        private static readonly ToStringMethod<Animation> toString = new ToStringMethodBuilder<Animation>()
            .UseProperties()
            .Substitute<IList<SplitTrigger>>(nameof(Triggers), it => it.ListToString())
            .Build();

        public override string ToString() => toString(this);
        #endregion

        #region Equals/GetHashCode
        private static readonly IEqualityComparer<Animation> equalityComparer = MemberwiseEqualityComparer<Animation>
            .ByProperties;

        public bool Equals(Animation other) => equalityComparer.Equals(this, other);

        public override bool Equals(object obj) => Equals(obj as Animation);

        public override int GetHashCode() => equalityComparer.GetHashCode(this);

        public static bool operator ==(Animation left, Animation right) => EqualsOperator(left, right);

        public static bool operator !=(Animation left, Animation right) => !(left == right);
        #endregion
    }
}
