/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO;
using SAGESharp.IO.Binary;
using System.ComponentModel;

namespace SAGESharp.SLB
{
    public sealed class AIInfo : INotifyPropertyChanged
    {
        private byte flying;
        [SerializableProperty(1)]
        [RightPadding(3)]
        public byte Flying
        {
            get => flying;
            set
            {
                flying = value;
                RaisePropertyChanged(nameof(Flying));
            }
        }

        private float range;
        [SerializableProperty(2)]
        public float Range
        {
            get => range;
            set
            {
                range = value;
                RaisePropertyChanged(nameof(Range));
            }
        }

        private long patrolRandomness;
        [SerializableProperty(3)]
        public long PatrolRandomness
        {
            get => patrolRandomness;
            set
            {
                patrolRandomness = value;
                RaisePropertyChanged(nameof(PatrolRandomness));
            }
        }

        private TimerValues timerValuesIdle;
        [SerializableProperty(4)]
        public TimerValues TimerValuesIdle
        {
            get => timerValuesIdle;
            set
            {
                timerValuesIdle = value;
                RaisePropertyChanged(nameof(TimerValuesIdle));
            }
        }

        private TimerValues timerValuesPatrol;
        [SerializableProperty(5)]
        public TimerValues TimerValuesPatrol
        {
            get => timerValuesPatrol;
            set
            {
                timerValuesPatrol = value;
                RaisePropertyChanged(nameof(TimerValuesPatrol));
            }
        }

        private short toughness;
        [SerializableProperty(6)]
        public short Toughness
        {
            get => toughness;
            set
            {
                toughness = value;
                RaisePropertyChanged(nameof(Toughness));
            }
        }

        private AttackType attackType;
        [SerializableProperty(7)]
        public AttackType AttackType
        {
            get => attackType;
            set
            {
                attackType = value;
                RaisePropertyChanged(nameof(AttackType));
            }
        }

        private byte benign;
        [SerializableProperty(8)]
        public byte Benign
        {
            get => benign;
            set
            {
                benign = value;
                RaisePropertyChanged(nameof(Benign));
            }
        }

        private string projectileSprite;
        [SerializableProperty(9)]
        [InlineString(32)]
        public string ProjectileSprite
        {
            get => projectileSprite;
            set
            {
                projectileSprite = value;
                RaisePropertyChanged(nameof(ProjectileSprite));
            }
        }

        public AIInfo()
        {

        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public sealed class TimerValues : INotifyPropertyChanged
    {
        private float a;
        [SerializableProperty(1)]
        public float A
        {
            get => a;
            set
            {
                a = value;
                RaisePropertyChanged(nameof(A));
            }
        }

        private float b;
        [SerializableProperty(2)]
        public float B
        {
            get => b;
            set
            {
                b = value;
                RaisePropertyChanged(nameof(B));
            }
        }

        public TimerValues()
        {

        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
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
