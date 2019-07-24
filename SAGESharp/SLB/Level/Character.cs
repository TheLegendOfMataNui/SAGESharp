/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO;
using System.Collections.Generic;

namespace SAGESharp.SLB.Level
{
    public sealed class CharacterTable
    {
        [SerializableProperty(1)]
        [DuplicateEntryCount]
        public IList<Character> Entries { get; set; }
    }

    public sealed class Character
    {
        [SerializableProperty(1)]
        public Identifier Id { get; set; }

        [SerializableProperty(2)]
        public Point3D Position { get; set; }

        [SerializableProperty(3)]
        public Vector3D Orientation { get; set; }

        [SerializableProperty(4)]
        public float Unkown { get; set; }

        [SerializableProperty(5)]
        [DuplicateEntryCount]
        public IList<Identifier> TriggerBoxes { get; set; }

        [SerializableProperty(6)]
        public IList<SPLinePath> SPLinePaths { get; set; }
    }
}
