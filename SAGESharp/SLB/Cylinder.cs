/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO;

namespace SAGESharp.SLB
{
    internal sealed class Cylinder
    {
        [SerializableProperty(1)]
        public Identifier Id { get; set; }

        [SerializableProperty(2)]
        public CollisionCylinder CollisionCylinder { get; set; }
    }

    internal sealed class CollisionCylinder
    {
        [SerializableProperty(1)]
        Point3D P1 { get; set; }

        [SerializableProperty(2)]
        Point3D P2 { get; set; }
    }
}
