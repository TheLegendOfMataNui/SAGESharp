/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO;
using System.Collections.Generic;

namespace SAGESharp.SLB
{
    public sealed class TextureTable
    {
        [SerializableProperty(1)]
        [DuplicateEntryCount]
        public IList<Texture1> Textures1 { get; set; }

        [SerializableProperty(2)]
        [DuplicateEntryCount]
        public IList<Texture2> Textures2 { get; set; }
    }

    public sealed class Texture1
    {
        // string: offset
        // at offset: length + string + null char
        [SerializableProperty(1)]
        [BinaryString(StringPosition.AtOffset)]
        public string Name { get; set; }

        [SerializableProperty(2)]
        public float Transparency { get; set; }

        [SerializableProperty(3)]
        public float OffsetU { get; set; }

        [SerializableProperty(4)]
        public float OffsetV { get; set; }

        [SerializableProperty(5)]
        public int AnimationFrames { get; set; }
    }

    public sealed class Texture2
    {
        [SerializableProperty(1)]
        [BinaryString(StringPosition.AtOffset)]
        public string Name { get; set; }

        [SerializableProperty(2)]
        public float Float1 { get; set; }

        [SerializableProperty(3)]
        public float Float2 { get; set; }

        [SerializableProperty(4)]
        public float Float3 { get; set; }

        [SerializableProperty(5)]
        public float Float4 { get; set; }

        [SerializableProperty(6)]
        public int Int { get; set; }
    }
}
