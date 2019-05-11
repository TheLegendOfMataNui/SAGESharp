/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO;
using System;
using System.Collections.Generic;

namespace SAGESharp
{
    public sealed class BKD : IBinarySerializable
    {
        #region Fields
        private IList<BKDEntry> entries = new List<BKDEntry>();
        #endregion

        public ushort Length { get; set; }

        public IList<BKDEntry> Entries
        {
            get => entries;
            set => entries = value ?? throw new ArgumentNullException(nameof(value));
        }

        #region IBinarySerializable
        public void Read(IBinaryReader binaryReader)
        {
            throw new NotImplementedException();
        }

        public void Write(IBinaryWriter binaryWriter)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public sealed class BKDEntry : IBinarySerializable
    {
        #region Fields
        private IList<TCBQuaternionData> tcbQuaternionData = new List<TCBQuaternionData>();

        private IList<TCBInterpolationData> tcbInterpolationData1 = new List<TCBInterpolationData>();

        private IList<TCBInterpolationData> tcbInterpolationData2 = new List<TCBInterpolationData>();
        #endregion

        public ushort Id { get; set; }

        public IList<TCBQuaternionData> TCBQuaternionData
        {
            get => tcbQuaternionData;
            set => tcbQuaternionData = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IList<TCBInterpolationData> TCBInterpolatorData1
        {
            get => tcbInterpolationData1;
            set => tcbInterpolationData1 = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IList<TCBInterpolationData> TCBInterpolatorData2
        {
            get => tcbInterpolationData2;
            set => tcbInterpolationData2 = value ?? throw new ArgumentNullException(nameof(value));
        }

        #region IBinarySerializable
        public void Read(IBinaryReader binaryReader)
        {
            throw new NotImplementedException();
        }

        public void Write(IBinaryWriter binaryWriter)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public sealed class TCBQuaternionData : IBinarySerializable
    {
        public short Short1 { get; set; }

        public short Short2 { get; set; }

        public short Short3 { get; set; }

        public short Short4 { get; set; }

        public short Short5 { get; set; }

        #region IBinarySerializable
        public void Read(IBinaryReader binaryReader)
        {
            throw new NotImplementedException();
        }

        public void Write(IBinaryWriter binaryWriter)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public sealed class TCBInterpolationData : IBinarySerializable
    {
        public int Long1 { get; set; }

        public float Float1 { get; set; }

        public float Float2 { get; set; }

        public float Float3 { get; set; }

        #region IBinarySerializable
        public void Read(IBinaryReader binaryReader)
        {
            throw new NotImplementedException();
        }

        public void Write(IBinaryWriter binaryWriter)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
