/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO;

namespace SAGESharp.SLB
{
    internal sealed class Light
    {
        [SerializableProperty(1)]
        public Identifier Id { get; set; }

        [SerializableProperty(2)]
        public Point3D Position { get; set; }

        [SerializableProperty(3)]
        public float Intensity { get; set; }

        [SerializableProperty(4)]
        public float Range { get; set; }

        [SerializableProperty(5)]
        public RGBA RGBA { get; set; }

        [SerializableProperty(6)]
        public int Flags { get; set; }
    }

    internal sealed class RGBA
    {
        [SerializableProperty(1)]
        public float Red { get; set; }

        [SerializableProperty(2)]
        public float Green { get; set; }

        [SerializableProperty(3)]
        public float Blue { get; set; }

        [SerializableProperty(4)]
        public float Alpha { get; set; }
    }
}
