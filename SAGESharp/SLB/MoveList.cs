/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO;
using SAGESharp.IO.Binary;
using System.Collections.Generic;

namespace SAGESharp.SLB
{
    public sealed class MoveList
    {
        [SerializableProperty(1)]
        public Identifier Id { get; set; }

        [SerializableProperty(2)]
        public IList<Animation> Animations { get; set; }

        [SerializableProperty(3)]
        public IList<AnimationWithExtra> AnimationsWithExtra { get; set; }
    }

    public sealed class Animation
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
    }

    public sealed class AnimationWithExtra
    {
        [SerializableProperty(1)]
        public Animation Animation { get; set; }

        [SerializableProperty(2)]
        public int Extra { get; set; }
    }

    public sealed class SplitTrigger
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
    }
}
