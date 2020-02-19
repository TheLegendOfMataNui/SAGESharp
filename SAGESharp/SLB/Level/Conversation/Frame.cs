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

namespace SAGESharp.SLB.Level.Conversation
{
    public sealed class Frame : IEquatable<Frame>
    {
        [SerializableProperty(1)]
        public int ToaAnimation { get; set; }

        [SerializableProperty(2)]
        public int CharAnimation { get; set; }

        [SerializableProperty(3)]
        public int CameraPositionTarget { get; set; }

        [SerializableProperty(4)]
        public int CameraDistance { get; set; }

        [SerializableProperty(5)]
        public int StringIndex { get; set; }

        [SerializableProperty(6)]
        [OffsetString]
        public string ConversationSounds { get; set; }

        #region ToString
        private static readonly ToStringMethod<Frame> toString = new ToStringMethodBuilder<Frame>()
            .UseProperties()
            .Build();

        public override string ToString() => toString(this);
        #endregion

        #region Equals/GetHashCode
        private static readonly IEqualityComparer<Frame> equalityComparer = MemberwiseEqualityComparer<Frame>
            .ByProperties;

        public bool Equals(Frame other) => equalityComparer.Equals(this, other);

        public override bool Equals(object obj) => Equals(obj as Frame);

        public override int GetHashCode() => equalityComparer.GetHashCode(this);

        public static bool operator ==(Frame left, Frame right) => EqualsOperator(left, right);

        public static bool operator !=(Frame left, Frame right) => !(left == right);
        #endregion
    }
}
