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
    public sealed class ConversationCharacterInfoTable
    {
        [SerializableProperty(1)]
        public IList<ConversationCharacterInfo> Entries { get; set; }
    }

    public sealed class ConversationCharacterInfo
    {
        [SerializableProperty(1)]
        public Identifier Id1 { get; set; }

        [SerializableProperty(2)]
        public Identifier Id2 { get; set; }

        [SerializableProperty(3)]
        public float Float1 { get; set; }

        [SerializableProperty(4)]
        public float Float2 { get; set; }

        [SerializableProperty(5)]
        public float Float3 { get; set; }

        [SerializableProperty(6)]
        public float Float4 { get; set; }

        [SerializableProperty(7)]
        public float Float5 { get; set; }

        [SerializableProperty(8)]
        public float Float6 { get; set; }

        [SerializableProperty(9)]
        public float Float7 { get; set; }

        [SerializableProperty(10)]
        [RightPadding(3)]
        byte Byte1 { get; set; }
    }
}
