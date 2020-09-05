/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO;

namespace SAGESharp.SLB
{
    public sealed class Cylinder
    {
        [SerializableProperty(1)]
        public Identifier ID { get; set; }

        [SerializableProperty(2)]
        public CylinderBounds Bounds { get; set; }
    }

    /// <summary>
    /// Defines the shape of a vertical cylinder by specifying the corners of the bounding box it occupies.
    /// </summary>
    public sealed class CylinderBounds
    {
        /// <summary>
        /// The location of the corner of the bounding box with the lowest coordinate values.
        /// </summary>
        [SerializableProperty(1)]
        public Point3D Min { get; set; }

        /// <summary>
        /// The location of the corner of the bounding box with the greatest coordinate values.
        /// </summary>
        [SerializableProperty(2)]
        public Point3D Max { get; set; }
    }
}
