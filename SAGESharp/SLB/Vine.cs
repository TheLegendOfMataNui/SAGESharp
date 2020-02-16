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
    public sealed class VineTable
    {
        [SerializableProperty(1)]
        public IList<Vine> Entries { get; set; }
    }

    public sealed class Vine
    {
        [SerializableProperty(1)]
        public Point3D Position { get; set; }

        [SerializableProperty(2)]
        public float Orientation { get; set; }

        [SerializableProperty(3)]
        [RightPadding(3)]
        public bool Bool1 { get; set; }

        [SerializableProperty(4)]
        [RightPadding(3)]
        public bool Bool2 { get; set; }

        [SerializableProperty(5)]
        public float Float { get; set; }
    }
}
