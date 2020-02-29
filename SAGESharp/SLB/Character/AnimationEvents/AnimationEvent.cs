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

namespace SAGESharp.SLB.Character.AnimationEvents
{
    public sealed class AnimationEvent : IEquatable<AnimationEvent>
    {
        [SerializableProperty(1)]
        public Identifier Id { get; set; }

        [SerializableProperty(2)]
        public Identifier EventArg2 { get; set; }

        [SerializableProperty(3)]
        public Identifier EventArg3 { get; set; }

        [SerializableProperty(4)]
        public Identifier EventArg4 { get; set; }

        [SerializableProperty(5)]
        public double EventArg5 { get; set; }

        [SerializableProperty(6)]
        public int EventArg6 { get; set; }

        [SerializableProperty(7)]
        public int EventArg7 { get; set; }

        [SerializableProperty(8)]
        public int EventArg8 { get; set; }

        [SerializableProperty(9)]
        public int EventArg9 { get; set; }

        [SerializableProperty(10)]
        public int EventArg10 { get; set; }

        [SerializableProperty(11)]
        public int Unknown { get; set; }

        #region ToString
        private static readonly ToStringMethod<AnimationEvent> toString = new ToStringMethodBuilder<AnimationEvent>()
            .UseProperties()
            .Build();

        public override string ToString() => toString(this);
        #endregion

        #region Equals/GetHashCode
        private static readonly IEqualityComparer<AnimationEvent> equalityComparer = MemberwiseEqualityComparer<AnimationEvent>
            .ByProperties;

        public bool Equals(AnimationEvent other) => equalityComparer.Equals(this, other);

        public override bool Equals(object obj) => Equals(obj as AnimationEvent);

        public override int GetHashCode() => equalityComparer.GetHashCode(this);

        public static bool operator ==(AnimationEvent left, AnimationEvent right) => EqualsOperator(left, right);

        public static bool operator !=(AnimationEvent left, AnimationEvent right) => !(left == right);
        #endregion
    }
}
