/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO;
using System.Collections.Generic;

namespace SAGESharp.SLB
{
    public sealed class InstanceTable
    {
        [SerializableProperty(1)]
        public Identifier Id { get; set; }

        [SerializableProperty(2)]
        [DuplicateEntryCount]
        public IList<Instance> Entries { get; set; }
    }

    public sealed class Instance
    {
        [SerializableProperty(1)]
        public Identifier Id1 { get; set; }

        [SerializableProperty(2)]
        public Identifier Id2 { get; set; }

        [SerializableProperty(3)]
        [RightPadding(16)]
        public Point3D Position { get; set; }
    }
}
