/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using Equ;
using NUtils.Extensions;
using SAGESharp.IO;
using System;
using System.Collections.Generic;

namespace SAGESharp.SLB.Cinematic
{
    internal sealed class Camera : IEquatable<Camera>
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
            => MemberwiseEqualityComparer<Camera>.ByProperties.Equals(this, other);

        public override string ToString() => $"ViewAngle={ViewAngle}," +
            $"SpinMaskTimes1={SpinMaskTimes1}," +
            $"SpinMaskTimes2={SpinMaskTimes2}," +
            $"SpinMaskTimes3={SpinMaskTimes3}," +
            $"Frames={Frames?.Let(frames => "[(" + string.Join("), (", frames) + ")]") ?? "null"}";

        public override bool Equals(object other)
            => Equals(other as Camera);

        public override int GetHashCode()
            => MemberwiseEqualityComparer<Camera>.ByProperties.GetHashCode(this);

        public static bool operator ==(Camera left, Camera right)
            => left?.Equals(right) ?? right?.Equals(left) ?? true;

        public static bool operator !=(Camera left, Camera right)
            => !(left == right);
    }

    internal sealed class Frame : IEquatable<Frame>
    {
        [SerializableProperty(1)]
        public float Time { get; set; }

        [SerializableProperty(2)]
        public Point3D Position { get; set; }

        [SerializableProperty(3)]
        public Point3D Target { get; set; }

        public bool Equals(Frame other)
            => MemberwiseEqualityComparer<Frame>.ByProperties.Equals(this, other);

        public override string ToString()
            => $"Time={Time}, Position={Position}, Target={Target}";

        public override bool Equals(object other)
            => Equals(other as Frame);

        public override int GetHashCode()
            => MemberwiseEqualityComparer<Frame>.ByProperties.GetHashCode(this);

        public static bool operator ==(Frame left, Frame right)
            => left?.Equals(right) ?? right?.Equals(left) ?? true;

        public static bool operator !=(Frame left, Frame right)
            => !(left == right);
    }
}
