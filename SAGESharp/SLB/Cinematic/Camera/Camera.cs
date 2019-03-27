using Konvenience;
using SAGESharp.SLB.IO;
using System;
using System.Collections.Generic;

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

        public override string ToString() => $"ViewAngle={ViewAngle}," +
            $"SpinMaskTimes1={SpinMaskTimes1}," +
            $"SpinMaskTimes2={SpinMaskTimes2}," +
            $"SpinMaskTimes3={SpinMaskTimes3}," +
            $"Frames={Frames?.Let(frames => "[(" + string.Join("), (", frames) + ")]") ?? "null"}";

        public override bool Equals(object other)
            => Equals(other as Camera);

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
            => !(left == right);
    }
}
