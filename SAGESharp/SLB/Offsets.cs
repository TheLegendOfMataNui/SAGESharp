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
    public sealed class OffsetTable
    {
        [SerializableProperty(1)]
        public float Unkown1 { get; set; }

        [SerializableProperty(2)]
        public float Unkown2 { get; set; }

        [SerializableProperty(3)]
        public float Unkown3 { get; set; }

        [SerializableProperty(4)]
        public float Unkown4 { get; set; }

        [SerializableProperty(5)]
        public float Unkown5 { get; set; }

        [SerializableProperty(2)]
        [DuplicateEntryCount]
        public IList<Offset> Entries { get; set; }
    }

    public sealed class Offset
    {
        [SerializableProperty(1)]
        public float Unkown1 { get; set; }

        [SerializableProperty(2)]
        public float Unkown2 { get; set; }

        [SerializableProperty(3)]
        public float Unkown3 { get; set; }

        [SerializableProperty(4)]
        public float Unkown4 { get; set; }

        [SerializableProperty(5)]
        public float Unkown5 { get; set; }

        [SerializableProperty(6)]
        public float Unkown6 { get; set; }
    }
}
