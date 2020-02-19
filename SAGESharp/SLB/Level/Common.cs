﻿/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO;
using System.Collections.Generic;

namespace SAGESharp.SLB.Level
{
    internal sealed class SplinePath
    {
        [SerializableProperty(1)]
        public IList<SplinePoint> Points { get; set; }

        [SerializableProperty(2)]
        public uint SPLine { get; set; }
    }

    internal sealed class SplinePoint
    {
        [SerializableProperty(1)]
        public float Time { get; set; }

        [SerializableProperty(2)]
        public float Value { get; set; }
    }
}
