/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO;
using System.ComponentModel;

namespace SAGESharp.SLB
{
    public sealed class Cylinder : INotifyPropertyChanged
    {
        private Identifier _id;
        [SerializableProperty(1)]
        public Identifier ID
        {
            get => _id;
            set
            {
                _id = value;
                RaisePropertyChanged(nameof(ID));
            }
        }

        private CylinderBounds _bounds;
        [SerializableProperty(2)]
        public CylinderBounds Bounds
        {
            get => _bounds;
            set
            {
                _bounds = value;
                RaisePropertyChanged(nameof(Bounds));
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    /// <summary>
    /// Defines the shape of a vertical cylinder by specifying the corners of the bounding box it occupies.
    /// </summary>
    public sealed class CylinderBounds : INotifyPropertyChanged
    {
        private Point3D _min;
        /// <summary>
        /// The location of the corner of the bounding box with the lowest coordinate values.
        /// </summary>
        [SerializableProperty(1)]
        public Point3D Min
        {
            get => _min;
            set
            {
                _min = value;
                RaisePropertyChanged(nameof(Min));
            }
        }

        private Point3D _max;
        /// <summary>
        /// The location of the corner of the bounding box with the greatest coordinate values.
        /// </summary>
        [SerializableProperty(2)]
        public Point3D Max
        {
            get => _max;
            set
            {
                _max = value;
                RaisePropertyChanged(nameof(Max));
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
