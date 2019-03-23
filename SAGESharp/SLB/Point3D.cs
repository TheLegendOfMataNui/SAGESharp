using SAGESharp.SLB.IO;
using System;
using System.Collections.Generic;
using System.Text;
using Konvenience;

namespace SAGESharp.SLB
{
      public sealed class Point3D : IEquatable<Point3D>
      {
        /// <summary>
        ///here be X, Y and Z
        /// </summary>

        internal const int  BINARY_SIZE = 12;

        [SLBElement(1)]
        public float X { get; set; }

        [SLBElement(2)]
        public float Y { get; set; }

        [SLBElement(3)]
        public float Z { get; set; }

        public bool Equals(Info other)
        {
            if (other == null)
            {
                return false;
            }

            return X == other.X &&
                Y == other.Y &&
                Z == other.Z;
        }
        public override string ToString()
        {
            return $"X={X}, Y={Y}, Z={Z}";
        }

        public override bool Equals(object other)
        {
          return Equals(other as Point3D);
        }
        public override int GetHashCode()
        {
          int hash = 5381;
          X.AddHashCodeByVal(ref hash, 4243);
          Y.AddHashCodeByVal(ref hash, 4243);
          Z.AddHashCodeByVal(ref hash, 4243);

          return hash;
        }

        public static bool operator ==(Point3D left, Point3D right)
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
        public static bool operator !=(Point3D left, Point3D right)
        {
            return !(left == right);
        }
      }
