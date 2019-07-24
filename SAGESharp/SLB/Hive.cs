/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO;
using System.Collections.Generic;

namespace SAGESharp.SLB
{
    public sealed class HiveTable
    {
        [DuplicateEntryCount]
        public IList<Hive> Entries { get; set; }
    }

    public sealed class Hive
    {
        [SerializableProperty(1)]
        public Identifier HiveId { get; set; }

        [SerializableProperty(2)]
        public Point3D Position { get; set; }

        [SerializableProperty(3)]
        public float Orientation { get; set; }

        [SerializableProperty(4)]
        public Point3D CollisionCylinderPoint1 { get; set; }

        [SerializableProperty(5)]
        public Point3D CollisionCylinderPoint2 { get; set; }

        [SerializableProperty(6)]
        public int Health { get; set; }

        [SerializableProperty(7)]
        public Identifier SpawnId { get; set; }

        [SerializableProperty(8)]
        [RightPadding(3)]
        public byte MaxCreatures { get; set; }

        [SerializableProperty(9)]
        public Identifier IdPhysicsGroup { get; set; }
    }
}
