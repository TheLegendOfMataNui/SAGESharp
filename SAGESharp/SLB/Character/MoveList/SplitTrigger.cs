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

namespace SAGESharp.SLB.Character.MoveList
{
    public sealed class SplitTrigger : IEquatable<SplitTrigger>
    {
        [SerializableProperty(1)]
        public int Input { get; set; }

        [SerializableProperty(2)]
        public Identifier Id { get; set; }

        [SerializableProperty(3)]
        public float Float1 { get; set; }

        [SerializableProperty(4)]
        public float Float2 { get; set; }

        [SerializableProperty(5)]
        public float Float3 { get; set; }

        [SerializableProperty(6)]
        [RightPadding(3)]
        public byte Flags { get; set; }

        #region ToString
        private static readonly ToStringMethod<SplitTrigger> toString = new ToStringMethodBuilder<SplitTrigger>()
            .UseProperties()
            .Build();

        public override string ToString() => toString(this);
        #endregion

        #region Equals/GetHashCode
        private static readonly IEqualityComparer<SplitTrigger> equalityComparer = MemberwiseEqualityComparer<SplitTrigger>
            .ByProperties;

        public bool Equals(SplitTrigger other) => equalityComparer.Equals(this, other);

        public override bool Equals(object obj) => Equals(obj as SplitTrigger);

        public override int GetHashCode() => equalityComparer.GetHashCode(this);

        public static bool operator ==(SplitTrigger left, SplitTrigger right) => EqualsOperator(left, right);

        public static bool operator !=(SplitTrigger left, SplitTrigger right) => !(left == right);
        #endregion
    }
}
