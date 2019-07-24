/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO;
using System.Collections.Generic;

namespace SAGESharp.SLB
{
    public sealed class EdgeTable
    {
        [SerializableProperty(1)]
        public IList<Edge> Entries { get; set; }
    }

    public sealed class Edge
    {
        [SerializableProperty(1)]
        public Identifier Id { get; set; }

        [SerializableProperty(2)]
        public IList<EdgeData> EdgeData { get; set; }
    }

    public sealed class EdgeData
    {
        [SerializableProperty(1)]
        public float A { get; set; }

        [SerializableProperty(2)]
        public float B { get; set; }
    }
}
