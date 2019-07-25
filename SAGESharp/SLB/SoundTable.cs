/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO;
using System.Collections.Generic;

namespace SAGESharp.SLB
{
    public sealed class SoundTable
    {
        [SerializableProperty(1)]
        public IList<SoundTableEntry> Entries { get; set; }
    }

    public sealed class SoundTableEntry
    {
        [SerializableProperty(1)]
        [OffsetString]
        public string String { get; set; }

        [SerializableProperty(2)]
        [RightPadding(3)]
        public SoundVariety Variety { get; set; }

        [SerializableProperty(3)]
        public float Volume { get; set; }
    }

    public enum SoundVariety : byte
    {
        None = 0x00,
        Regular = 0x01,
        Pan = 0x02,
        _3D = 0x03
    }
}
