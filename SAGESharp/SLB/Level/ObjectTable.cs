/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO;
using SAGESharp.IO.Binary;
using System.Collections.Generic;

namespace SAGESharp.SLB.Level
{
    public sealed class ObjectTable
    {
        [SerializableProperty(1)]
        [System.Obsolete("This is not used.")]
        public Identifier Id { get; set; }

        [SerializableProperty(2)]
        [DuplicateEntryCount]
        public IList<Object> Objects { get; set; }
    }

    /// <summary>
    /// An object placed inside a level.
    /// </summary>
    /// <remarks>
    /// This corresponds to the POSITION structure in SLB_level_obj.bt.
    /// </remarks>
    public sealed class Object : System.ComponentModel.INotifyPropertyChanged
    {
        private Identifier id;
        [SerializableProperty(1)]
        public Identifier ID
        {
            get => id;
            set
            {
                id = value;
                RaisePropertyChanged(nameof(ID));
            }
        }

        [SerializableProperty(2)]
        public Point3D Location { get; set; }

        [SerializableProperty(3)]
        public Point3D Orientation { get; set; }

        private float unknown;
        [SerializableProperty(4)]
        public float Unknown
        {
            get => unknown;
            set
            {
                unknown = value;
                RaisePropertyChanged(nameof(Unknown));
            }
        }

        [SerializableProperty(5)]
        public Point3D CollisionPoint1 { get; set; }

        [SerializableProperty(6)]
        public Point3D CollisionPoint2 { get; set; }

        private int flags;
        [SerializableProperty(7)]
        public int Flags
        {
            get => flags;
            set
            {
                flags = value;
                RaisePropertyChanged(nameof(Flags));
            }
        }

        #region INotifyPropertyChanged
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
