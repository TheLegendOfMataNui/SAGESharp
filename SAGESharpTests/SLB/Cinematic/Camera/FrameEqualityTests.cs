/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using SAGESharp.Testing;
using System;

namespace SAGESharp.SLB.Cinematic.Camera
{
    class FrameEqualityTests : AbstractEqualityByRefTests<Frame>
    {
        protected override Frame GetDefault() => new Frame
        {
            Time = 1.5f,
            Position = new Point3D { X = 1, Y = 2, Z = 3 },
            Target = new Point3D { X = 4, Y = 5, Z = 6 },
        };

        protected override bool EqualsOperator(Frame left, Frame right)
            => left == right;

        protected override bool NotEqualsOperator(Frame left, Frame right)
            => left != right;

        [TestCaseSource(nameof(Modifiers))]
        public void Test_Compare_Default_Object_With_Modified_Object(Action<Frame> modifier) =>
            TestCompareDefaultObjectWithModifiedObject(modifier);

        static object[] Modifiers() => new ParameterGroup<Action<Frame>>()
            .Parameters(frame => frame.Time = 0)
            .Parameters(frame => frame.Position = null)
            .Parameters(frame => frame.Position.X = 0)
            .Parameters(frame => frame.Target = null)
            .Parameters(frame => frame.Target.X = 0)
            .Build();
    }
}
