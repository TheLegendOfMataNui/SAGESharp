/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO;
using System.Collections.Generic;

namespace SAGESharp.SLB
{
    public sealed class PositionTable
    {
        [SerializableProperty(1)]
        [DuplicateEntryCount]
        public IList<Position> Entries { get; set; }
    }

    public sealed class Position
    {
        [SerializableProperty(1)]
        Identifier Id { get; set; }

        [SerializableProperty(2)]
        Point3D Value { get; set; }

        [SerializableProperty(3)]
        int Flags { get; set; }
    }
}
