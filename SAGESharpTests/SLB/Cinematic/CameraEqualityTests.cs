/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using SAGESharp.Testing;
using System;
using System.Collections.Generic;

namespace SAGESharp.SLB.Cinematic
{
    class CameraEqualityTests : AbstractEqualityByRefTests<Camera>
    {
        protected override Camera GetDefault() => new Camera
        {
            ViewAngle = 1.1f,
            SpinMaskTimes1 = 1.2f,
            SpinMaskTimes2 = 1.3f,
            SpinMaskTimes3 = 1.4f,
            Frames = new List<Frame>
            {
                new Frame()
                {
                    Time = 1.5f,
                    Position = new Point3D { X = 2, Y = 3, Z = 4 },
                    Target = new Point3D { X = 5, Y = 6, Z = 7 }
                },
                new Frame()
                {
                    Time = 1.5f,
                    Position = new Point3D { X = 2, Y = 3, Z = 4 },
                    Target = new Point3D { X = 5, Y = 6, Z = 7 }
                }
            }
        };

        protected override bool EqualsOperator(Camera left, Camera right)
            => left == right;

        protected override bool NotEqualsOperator(Camera left, Camera right)
            => left != right;

        [TestCaseSource(nameof(Modifiers))]
        public void Test_Compare_Default_Object_With_Modified_Object(Action<Camera> modifier) =>
            TestCompareDefaultObjectWithModifiedObject(modifier);

        static object[] Modifiers() => new ParameterGroup<Action<Camera>>()
            .Parameters(camera => camera.ViewAngle = 0)
            .Parameters(camera => camera.SpinMaskTimes1 = 0)
            .Parameters(camera => camera.SpinMaskTimes2 = 0)
            .Parameters(camera => camera.SpinMaskTimes3 = 0)
            .Parameters(camera => camera.Frames = null)
            .Parameters(camera => camera.Frames.Clear())
            .Parameters(camera => camera.Frames = new List<Frame> { camera.Frames[0] })
            .Build();

        [TestCaseSource(nameof(DualModifiers))]
        public void Test_Compare_Modified_Objects(Action<Camera> modifierA, Action<Camera> modifierB) =>
            TestCompareModifiedObjects(modifierA, modifierB);

        static object[] DualModifiers() => new ParameterGroup<Action<Camera>, Action<Camera>>()
            .Parameters(camera => camera.Frames = null, camera => camera.Frames.Clear())
            .Build();
    }
}
