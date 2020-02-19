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
    internal sealed class SpotlightTable
    {
        [SerializableProperty(1)]
        [DuplicateEntryCount]
        public IList<Spotlight> Entries { get; set; }
    }

    internal sealed class Spotlight
    {
        [SerializableProperty(1)]
        public Identifier Id { get; set; }

        [SerializableProperty(2)]
        public Point3D Position { get; set; }

        [SerializableProperty(3)]
        public Point3D Vector { get; set; }

        [SerializableProperty(4)]
        public float Intensity { get; set; }

        [SerializableProperty(5)]
        public RGBA Color { get; set; }

        [SerializableProperty(6)]
        public float Theta { get; set; }

        [SerializableProperty(7)]
        public float Phi { get; set; }

        [SerializableProperty(8)]
        public float Range { get; set; }
    }
}
