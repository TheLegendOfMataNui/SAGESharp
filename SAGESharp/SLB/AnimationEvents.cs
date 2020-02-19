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
    internal sealed class AnimationEventsTable
    {
        [DuplicateEntryCount]
        public IList<AnimationEvent> Entries { get; set; }
    }

    internal sealed class AnimationEvent
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
    }
}
