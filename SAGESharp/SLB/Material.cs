/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO;
using System.Collections.Generic;

namespace SAGESharp.SLB
{
    public sealed class MaterialTable
    {
        [SerializableProperty(1)]
        public IList<Material> Entries { get; set; }
    }

    public sealed class Material
    {
        [SerializableProperty(1)]
        public int Index { get; set; }

        [SerializableProperty(2)]
        public int Flags { get; set; }
    }
}
