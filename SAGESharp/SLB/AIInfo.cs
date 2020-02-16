/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO;
using SAGESharp.IO.Binary;

namespace SAGESharp.SLB
{
    public sealed class AIInfo
    {
        [SerializableProperty(1)]
        [RightPadding(3)]
        public bool Flying { get; set; }

        [SerializableProperty(2)]
        public float Range { get; set; }

        [SerializableProperty(3)]
        public long PatrolRandomness { get; set; }

        [SerializableProperty(4)]
        public TimerValues TimerValuesIdle { get; set; }

        [SerializableProperty(5)]
        public TimerValues TimerValuesPatrol { get; set; }

        [SerializableProperty(6)]
        public short Toughness { get; set; }

        [SerializableProperty(7)]
        public AttackType AttackType { get; set; }

        [SerializableProperty(8)]
        public bool Benign { get; set; }

        [SerializableProperty(9)]
        [InlineString(32)]
        public string ProjectileSprite { get; set; }
    }

    public sealed class TimerValues
    {
        [SerializableProperty(1)]
        public float A { get; set; }

        [SerializableProperty(2)]
        public float B { get; set; }
    }

    public enum AttackType : short
    {
        Melee = 0,
        Kamikaze = 1,
        Jump = 2,
        Ranged = 3,
        MoveToAttack = 4,
        Miniboss = 5
    }
}
