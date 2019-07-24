/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO;
using System.Collections.Generic;

namespace SAGESharp.SLB
{
    public sealed class Sounds
    {
        [SerializableProperty(1)]
        public IList<Sound> Entries { get; set; }
    }

    public sealed class Sound
    {
        [SerializableProperty(1)]
        [BinaryString(StringPosition.AtOffset)]
        public string Filename { get; set; }

        [SerializableProperty(2)]
        public Identifier Identifier { get; set; }

        [SerializableProperty(3)]
        public int Veriety { get; set; }

        [SerializableProperty(4)]
        public uint Priority { get; set; }

        [SerializableProperty(5)]
        public float Volume { get; set; }

        [SerializableProperty(6)]
        public int Pitch { get; set; }

        [SerializableProperty(7)]
        public Point3D Position { get; set; }

        [SerializableProperty(8)]
        public Vector3D Front { get; set; }

        [SerializableProperty(9)]
        public int InsideAngle { get; set; }

        [SerializableProperty(10)]
        public int OutsideAngle { get; set; }

        [SerializableProperty(11)]
        public float OutsideVolume { get; set; }

        [SerializableProperty(12)]
        public float MinDistance { get; set; }

        [SerializableProperty(13)]
        public float MaxDistance { get; set; }
    }
}
