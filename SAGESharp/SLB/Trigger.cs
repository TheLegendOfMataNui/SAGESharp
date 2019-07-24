/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO;
using System.Collections.Generic;

namespace SAGESharp.SLB
{
    public sealed class Trigger
    {
        [SerializableProperty(1)]
        [DuplicateEntryCount]
        public IList<Box> Boxes { get; set; }

        [SerializableProperty(2)]
        [DuplicateEntryCount]
        public IList<Plane> Planes { get; set; }
    }

    public sealed class Box
    {
        [SerializableProperty(1)]
        public Identifier Id { get; set; }

        [SerializableProperty(2)]
        public Point3D Point1 { get; set; }

        [SerializableProperty(3)]
        public Point3D Point2 { get; set; }
    }

    public sealed class Plane
    {
        [SerializableProperty(1)]
        public Identifier Id { get; set; }

        [SerializableProperty(2)]
        public Point3D Point1 { get; set; }

        [SerializableProperty(3)]
        public Point3D Point2 { get; set; }

        [SerializableProperty(4)]
        public Point3D Point3 { get; set; }

        [SerializableProperty(5)]
        public Point3D Point4 { get; set; }

        [SerializableProperty(6)]
        public Vector3D Vector { get; set; }

        [SerializableProperty(7)]
        public Identifier IdOther1 { get; set; }

        [SerializableProperty(8)]
        public Identifier IdOther2 { get; set; }

        [SerializableProperty(9)]
        public Identifier IdOther3 { get; set; }
    }
}
