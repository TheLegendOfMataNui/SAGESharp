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
    internal sealed class Targets
    {
        [SerializableProperty(1)]
        [DuplicateEntryCount]
        public IList<Target> Entries { get; set; }
    }

    internal sealed class Target
    {
        [SerializableProperty(1)]
        public Identifier Id { get; set; }

        [SerializableProperty(2)]
        public int Index { get; set; }
    }
}
