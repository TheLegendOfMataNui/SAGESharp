/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO;
using System.Collections.Generic;

namespace SAGESharp.SLB
{
    public sealed class StringGroupTable
    {
        [SerializableProperty(1)]
        public IList<StringGroup> Entries { get; set; }
    }

    public sealed class StringGroup
    {
        [SerializableProperty(1)]
        public Identifier Id { get; set; }

        [SerializableProperty(2)]
        public int Number { get; set; }

        [SerializableProperty(3)]
        public IList<StringGroupEntry> Strings { get; set; }
    }

    public sealed class StringGroupEntry
    {
        [SerializableProperty(1)]
        [OffsetString]
        public string Value { get; set; }
    }
}
