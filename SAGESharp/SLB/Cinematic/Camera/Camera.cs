using SAGESharp.SLB.IO;
using System;
using System.Collections.Generic;
using System.Text;
using Konvenience;

namespace SAGESharp.SLB.Cinematic.Camera
{
    public sealed class Camera : IEquatable<Camera>
    {

        [SLBElement(1)]
        public float ViewAngle { get; set; }

        [SLBElement(2)]
        public float SpinMaskTimes1 { get; set; }

        [SLBElement(3)]
        public float SpinMaskTimes2 { get; set; }

        [SLBElement(4)]
        public float SpinMaskTimes3 { get; set; }

        [SLBElement(5)]
        public IList<Frame> Frames { get; set; }

        public bool Equals(Camera other)
        {
            if (other == null)
            {
                return false;
            }

            return ViewAngle == other.ViewAngle &&
                SpinMaskTimes1 == other.SpinMaskTimes1 &&
                SpinMaskTimes2 == other.SpinMaskTimes2 &&
                SpinMaskTimes3 == other.SpinMaskTimes3 &&
                Frames.SafeSequenceEquals(other.Frames);
        }

        public override string ToString()
        {
          return $"ViewAngle={ViewAngle}," +
  $"SpinMaskTimes1={SpinMasktimes1}," +
  $"SpinMaskTimes2={SpinMasktimes2}," +
  $"SpinMaskTimes3={SpinMasktimes3}," +
  $"Frames={Frames?.Let(frames => "[(" + string.Join("), (") + ")]") ?? "null"}";
        }

        public override bool Equals(object other)
        {
            return Equals(other as Camera);
        }

        public override int GetHashCode()
        {
            int hash = 6311;
            ViewAngle.AddHashCodeByVal(ref hash, 911);
            SpinMaskTimes1.AddHashCodeByVal(ref hash, 911);
            SpinMaskTimes2.AddHashCodeByVal(ref hash, 911);
            SpinMaskTimes3.AddHashCodeByVal(ref hash, 911);
            Frames.AddHashCodesByRef(ref hash, 911, 9311);

            return hash;
        }

        public static bool operator ==(Camera left, Camera right)
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

        public static bool operator !=(Camera left, Camera right)
        {
            return !(left == right);
        }
    }
}
