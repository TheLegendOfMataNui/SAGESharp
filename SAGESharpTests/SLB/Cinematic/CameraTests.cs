/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using SAGESharp.Testing;
using System.Collections.Generic;

namespace SAGESharp.SLB.Cinematic
{
    class CameraTests
    {
        [TestCaseSource(nameof(EqualObjectsTestCases))]
        public void Test_Comparing_Equal_Objects(IComparisionTestCase<Camera> testCase) => testCase.Execute();

        public static IComparisionTestCase<Camera>[] EqualObjectsTestCases() => new IComparisionTestCase<Camera>[]
        {
            ComparisionTestCase.CompareObjectAgainstItself(SampleCamera()),
            ComparisionTestCase.CompareTwoEqualObjects(SampleCamera),
            ComparisionTestCase.CompareNullWithOperators<Camera>()
        };

        [TestCaseSource(nameof(NotEqualObjectsTestCases))]
        public void Test_Comparing_NotEqual_Objects(IComparisionTestCase<Camera> testCase) => testCase.Execute();

        public static IComparisionTestCase<Camera>[] NotEqualObjectsTestCases() => new IComparisionTestCase<Camera>[]
        {
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleCamera, updater: camera => camera.ViewAngle = 0),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleCamera, updater: camera => camera.SpinMaskTimes1 = 0),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleCamera, updater: camera => camera.SpinMaskTimes2 = 0),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleCamera, updater: camera => camera.SpinMaskTimes3 = 0),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleCamera, updater: camera => camera.Frames = null),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleCamera, updater: camera => camera.Frames.Clear()),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleCamera,
                updater: camera => camera.Frames = new List<Frame> { camera.Frames[0] }
            ),
            ComparisionTestCase.CompareNotNullObjectAgainstNull(SampleCamera())
        };

        public static Camera SampleCamera() => new Camera
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
    }
}
