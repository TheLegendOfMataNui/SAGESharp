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
    public sealed class BKD : IEquatable<BKD>, IBinarySerializable
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

        #region Equality
        public bool Equals(BKD other)
        {
            if (other == null)
            {
                return false;
            }

            return Length == other.Length &&
                Entries.SafeSequenceEquals(other.Entries);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BKD);
        }

        public override int GetHashCode()
        {
            int hash = 427477;
            Length.AddHashCodeByVal(ref hash, 446503);
            Entries.AddHashCodesByRef(ref hash, 446503, 542891);

            return hash;
        }

        public static bool operator ==(BKD left, BKD right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            else if (left is null)
            {
                return right.Equals(left);
            }
            else
            {
                return left.Equals(right);
            }
        }

        public static bool operator !=(BKD left, BKD right)
        {
            return !(left == right);
        }
        #endregion

        public override string ToString() => $"{nameof(BKD)}(" +
            $"{nameof(Length)}={Length}, " +
            $"{nameof(Entries)}=[{string.Join(", ", entries)}]" +
        ")";
    }

    public sealed class BKDEntry : IEquatable<BKDEntry>, IBinarySerializable
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

        #region Equality
        public bool Equals(BKDEntry other)
        {
            if (other == null)
            {
                return false;
            }

            return Id == other.Id &&
                TCBQuaternionData.SafeSequenceEquals(other.TCBQuaternionData) &&
                TCBInterpolatorData1.SafeSequenceEquals(other.TCBInterpolatorData1) &&
                TCBInterpolatorData2.SafeSequenceEquals(other.TCBInterpolatorData2);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BKDEntry);
        }

        public override int GetHashCode()
        {
            int hash = 216761;
            Id.AddHashCodeByVal(ref hash, 114371);
            TCBQuaternionData.AddHashCodesByRef(ref hash, 114371, 527929);
            TCBInterpolatorData1.AddHashCodesByRef(ref hash, 114371, 322891);
            TCBInterpolatorData2.AddHashCodesByRef(ref hash, 114371, 15053);

            return hash;
        }

        public static bool operator ==(BKDEntry left, BKDEntry right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            else if (left is null)
            {
                return right.Equals(left);
            }
            else
            {
                return left.Equals(right);
            }
        }

        public static bool operator !=(BKDEntry left, BKDEntry right)
        {
            return !(left == right);
        }
        #endregion

        public override string ToString() => $"{nameof(BKDEntry)}(" +
            $"{nameof(Id)}={Id}, " +
            $"{nameof(TCBQuaternionData)}=[{string.Join(", ", TCBQuaternionData)}], " +
            $"{nameof(TCBInterpolatorData1)}=[{string.Join(", ", TCBInterpolatorData1)}], " +
            $"{nameof(TCBInterpolatorData2)}=[{string.Join(", ", TCBInterpolatorData2)}]"
        + ")";
    }

    public sealed class TCBQuaternionData : IEquatable<TCBQuaternionData>, IBinarySerializable
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

        #region Equality
        public bool Equals(TCBQuaternionData other)
        {
            if (other == null)
            {
                return false;
            }

            return Short1 == other.Short1 &&
                Short2 == other.Short2 &&
                Short3 == other.Short3 &&
                Short4 == other.Short4 &&
                Short5 == other.Short5;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TCBQuaternionData);
        }

        public override int GetHashCode()
        {
            int hash = 494917;
            Short1.AddHashCodeByVal(ref hash, 417881);
            Short2.AddHashCodeByVal(ref hash, 417881);
            Short3.AddHashCodeByVal(ref hash, 417881);
            Short4.AddHashCodeByVal(ref hash, 417881);
            Short5.AddHashCodeByVal(ref hash, 417881);

            return hash;
        }

        public static bool operator ==(TCBQuaternionData left, TCBQuaternionData right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            else if (left is null)
            {
                return right.Equals(left);
            }
            else
            {
                return left.Equals(right);
            }
        }

        public static bool operator !=(TCBQuaternionData left, TCBQuaternionData right)
        {
            return !(left == right);
        }
        #endregion

        public override string ToString() => $"{nameof(TCBQuaternionData)}(" +
            $"{nameof(Short1)}={Short1}, " +
            $"{nameof(Short2)}={Short2}, " +
            $"{nameof(Short3)}={Short3}, " +
            $"{nameof(Short4)}={Short4}, " +
            $"{nameof(Short5)}={Short5})" +
        ")";
    }

    public sealed class TCBInterpolationData : IEquatable<TCBInterpolationData>, IBinarySerializable
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

        #region Equality
        public bool Equals(TCBInterpolationData other)
        {
            if (other == null)
            {
                return false;
            }

            return Long1 == other.Long1 &&
                Float1 == other.Float1 &&
                Float2 == other.Float2 &&
                Float3 == other.Float3;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TCBInterpolationData);
        }

        public override int GetHashCode()
        {
            int hash = 324871;
            Long1.AddHashCodeByVal(ref hash, 25667);
            Float1.AddHashCodeByVal(ref hash, 25667);
            Float2.AddHashCodeByVal(ref hash, 25667);
            Float3.AddHashCodeByVal(ref hash, 25667);

            return hash;
        }

        public static bool operator ==(TCBInterpolationData left, TCBInterpolationData right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            else if (left is null)
            {
                return right.Equals(left);
            }
            else
            {
                return left.Equals(right);
            }
        }

        public static bool operator !=(TCBInterpolationData left, TCBInterpolationData right)
        {
            return !(left == right);
        }
        #endregion

        public override string ToString() => $"{nameof(TCBInterpolationData)}(" +
            $"{nameof(Long1)}={Long1}, " +
            $"{nameof(Float1)}={Float1}, " +
            $"{nameof(Float2)}={Float2}, " +
            $"{nameof(Float3)}={Float3}" +
        ")";
    }
}
