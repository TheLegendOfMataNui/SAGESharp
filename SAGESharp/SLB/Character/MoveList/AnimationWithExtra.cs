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

namespace SAGESharp.SLB.Character.MoveList
{
    public sealed class AnimationWithExtra : IEquatable<AnimationWithExtra>
    {
        [SerializableProperty(1)]
        public Animation Animation { get; set; }

        [SerializableProperty(2)]
        public int Extra { get; set; }

        #region ToString
        private static readonly ToStringMethod<AnimationWithExtra> toString = new ToStringMethodBuilder<AnimationWithExtra>()
            .UseProperties()
            .Build();

        public override string ToString() => toString(this);
        #endregion

        #region Equals/GetHashCode
        private static readonly IEqualityComparer<AnimationWithExtra> equalityComparer = MemberwiseEqualityComparer<AnimationWithExtra>
            .ByProperties;

        public bool Equals(AnimationWithExtra other) => equalityComparer.Equals(this, other);

        public override bool Equals(object obj) => Equals(obj as AnimationWithExtra);

        public override int GetHashCode() => equalityComparer.GetHashCode(this);

        public static bool operator ==(AnimationWithExtra left, AnimationWithExtra right) => EqualsOperator(left, right);

        public static bool operator !=(AnimationWithExtra left, AnimationWithExtra right) => !(left == right);
        #endregion
    }
}
