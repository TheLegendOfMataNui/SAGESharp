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
        private byte _flying;
        [SerializableProperty(1)]
        [RightPadding(3)]
        public byte Flying
        {
            get => _flying;
            set
            {
                _flying = value;
                RaisePropertyChanged(nameof(Flying));
            }
        }

        private float _range;
        [SerializableProperty(2)]
        public float Range
        {
            get => _range;
            set
            {
                _range = value;
                RaisePropertyChanged(nameof(Range));
            }
        }

        private long _patrolRandomness;
        [SerializableProperty(3)]
        public long PatrolRandomness
        {
            get => _patrolRandomness;
            set
            {
                _patrolRandomness = value;
                RaisePropertyChanged(nameof(PatrolRandomness));
            }
        }

        private TimerValues _timerValuesIdle;
        [SerializableProperty(4)]
        public TimerValues TimerValuesIdle
        {
            get => _timerValuesIdle;
            set
            {
                _timerValuesIdle = value;
                RaisePropertyChanged(nameof(TimerValuesIdle));
            }
        }

        private TimerValues _timerValuesPatrol;
        [SerializableProperty(5)]
        public TimerValues TimerValuesPatrol
        {
            get => _timerValuesPatrol;
            set
            {
                _timerValuesPatrol = value;
                RaisePropertyChanged(nameof(TimerValuesPatrol));
            }
        }

        private short _toughness;
        [SerializableProperty(6)]
        public short Toughness
        {
            get => _toughness;
            set
            {
                _toughness = value;
                RaisePropertyChanged(nameof(Toughness));
            }
        }

        private AttackType _attackType;
        [SerializableProperty(7)]
        public AttackType AttackType
        {
            get => _attackType;
            set
            {
                _attackType = value;
                RaisePropertyChanged(nameof(AttackType));
            }
        }

        private byte _benign;
        [SerializableProperty(8)]
        public byte Benign
        {
            get => _benign;
            set
            {
                _benign = value;
                RaisePropertyChanged(nameof(Benign));
            }
        }

        private string _projectileSprite;
        [SerializableProperty(9)]
        [InlineString(32)]
        public string ProjectileSprite
        {
            get => _projectileSprite;
            set
            {
                _projectileSprite = value;
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
        private float _a;
        [SerializableProperty(1)]
        public float A
        {
            get => _a;
            set
            {
                _a = value;
                RaisePropertyChanged(nameof(A));
            }
        }

        private float _b;
        [SerializableProperty(2)]
        public float B
        {
            get => _b;
            set
            {
                _b = value;
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
