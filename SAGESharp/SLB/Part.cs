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
    public sealed class PartVector
    {
        [SerializableProperty(1)]
        [DuplicateEntryCount]
        public IList<Part> Entries { get; set; }
    }

    public sealed class Part
    {
        [SerializableProperty(1)]
        public float Float1 { get; set; }

        [SerializableProperty(2)]
        public float Float2 { get; set; }

        [SerializableProperty(3)]
        public float Float3 { get; set; }

        [SerializableProperty(4)]
        public float Float4 { get; set; }

        [SerializableProperty(5)]
        public int Int1 { get; set; }

        [SerializableProperty(6)]
        public int Int2 { get; set; }
    }
}
