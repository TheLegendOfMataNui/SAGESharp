/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using Konvenience;
using SAGESharp.IO;
using System;
using System.Collections.Generic;

namespace SAGESharp.SLB.Cinematic
{
    public sealed class Camera : IEquatable<Camera>
    {
        [SerializableProperty(1)]
        public float ViewAngle { get; set; }

        [SerializableProperty(2)]
        public float SpinMaskTimes1 { get; set; }

        [SerializableProperty(3)]
        public float SpinMaskTimes2 { get; set; }

        [SerializableProperty(4)]
        public float SpinMaskTimes3 { get; set; }

        [SerializableProperty(5)]
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

    public sealed class Frame : IEquatable<Frame>
    {
        [SerializableProperty(1)]
        public float Time { get; set; }

        [SerializableProperty(2)]
        public Point3D Position { get; set; }

        [SerializableProperty(3)]
        public Point3D Target { get; set; }

        public bool Equals(Frame other)
        {
            if (other == null)
            {
                return false;
            }

            return Time == other.Time &&
                Position == other.Position &&
                Target == other.Target;
        }

        public override string ToString()
            => $"Time={Time}, Position={Position}, Target={Target}";

        public override bool Equals(object other)
            => Equals(other as Frame);

        public override int GetHashCode()
        {
            int hash = 7507;
            Time.AddHashCodeByVal(ref hash, 907);
            Position.AddHashCodeByRef(ref hash, 907);
            Target.AddHashCodeByRef(ref hash, 907);

            return hash;
        }

        public static bool operator ==(Frame left, Frame right)
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

        public static bool operator !=(Frame left, Frame right)
            => !(left == right);
    }
}