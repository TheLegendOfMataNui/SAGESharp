/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using SAGESharp.Testing;

namespace SAGESharp.SLB.Cinematic
{
    class FrameTests
    {
        [TestCaseSource(nameof(EqualObjectsTestCases))]
        public void Test_Comparing_Equal_Objects(IComparisionTestCase<Frame> testCase) => testCase.Execute();

        public static IComparisionTestCase<Frame>[] EqualObjectsTestCases() => new IComparisionTestCase<Frame>[]
        {
            ComparisionTestCase.CompareObjectAgainstItself(SampleFrame()),
            ComparisionTestCase.CompareTwoEqualObjects(SampleFrame),
            ComparisionTestCase.CompareNullWithOperators<Frame>()
        };

        [TestCaseSource(nameof(NotEqualObjectsTestCases))]
        public void Test_Comparing_NotEqual_Objects(IComparisionTestCase<Frame> testCase) => testCase.Execute();

        public static IComparisionTestCase<Frame>[] NotEqualObjectsTestCases() => new IComparisionTestCase<Frame>[]
        {
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleFrame, updater: frame => frame.Time = 0),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleFrame, updater: frame => frame.Position = null),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleFrame, updater: frame => frame.Position.X = 0),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleFrame, updater: frame => frame.Target = null),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleFrame, updater: frame => frame.Target.X = 0),
            ComparisionTestCase.CompareNotNullObjectAgainstNull(SampleFrame())
        };

        public static Frame SampleFrame() => new Frame
        {
            Time = 1.5f,
            Position = new Point3D { X = 1, Y = 2, Z = 3 },
            Target = new Point3D { X = 4, Y = 5, Z = 6 },
        };
    }
}
