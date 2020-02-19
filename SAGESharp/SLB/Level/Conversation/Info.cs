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
    public sealed class Info : IEquatable<Info>
    {
        [SerializableProperty(1)]
        public LineSide LineSide { get; set; }

        [SerializableProperty(2)]
        public uint ConditionStart { get; set; }

        [SerializableProperty(3)]
        public uint ConditionEnd { get; set; }

        [SerializableProperty(4)]
        public Identifier StringLabel { get; set; }

        [SerializableProperty(5)]
        public int StringIndex { get; set; }

        [SerializableProperty(6)]
        public IList<Frame> Frames { get; set; }

        #region ToString
        private static readonly ToStringMethod<Info> toString = new ToStringMethodBuilder<Info>()
            .UseProperties()
            .Substitute<IList<Frame>>(nameof(Frames), frames => frames.ListToString())
            .Build();

        public override string ToString() => toString(this);
        #endregion

        #region Equals/GetHashCode
        private static readonly IEqualityComparer<Info> equalityComparer = MemberwiseEqualityComparer<Info>
            .ByProperties;

        public bool Equals(Info other) => equalityComparer.Equals(this, other);

        public override bool Equals(object obj) => Equals(obj as Info);

        public override int GetHashCode() => equalityComparer.GetHashCode(this);

        public static bool operator ==(Info left, Info right) => EqualsOperator(left, right);

        public static bool operator !=(Info left, Info right) => !(left == right);
        #endregion
    }
}
