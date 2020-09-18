/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using SAGESharp.Animations;

namespace SAGESharp.Tests.Animations
{
    class TCBInterpolationDataTests
    {
        [TestCaseSource(nameof(EqualObjectsTestCases))]
        public void Test_Comparing_Equal_Objects(IComparisionTestCase<VectorKeyframe> testCase) => testCase.Execute();

        public static IComparisionTestCase<VectorKeyframe>[] EqualObjectsTestCases() => new IComparisionTestCase<VectorKeyframe>[]
        {
            ComparisionTestCase.CompareObjectAgainstItself(SampleTCBInterpolationData()),
            ComparisionTestCase.CompareTwoEqualObjects(SampleTCBInterpolationData),
            ComparisionTestCase.CompareNullWithOperators<VectorKeyframe>()
        };

        [TestCaseSource(nameof(NotEqualObjectsTestCases))]
        public void Test_Comparing_NotEqual_Objects(IComparisionTestCase<VectorKeyframe> testCase) => testCase.Execute();

        public static IComparisionTestCase<VectorKeyframe>[] NotEqualObjectsTestCases() => new IComparisionTestCase<VectorKeyframe>[]
        {
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleTCBInterpolationData,
                updater: tcbInterpolationData => tcbInterpolationData.Frame = 12
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleTCBInterpolationData,
                updater: tcbInterpolationData => tcbInterpolationData.X = 6.9f
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleTCBInterpolationData,
                updater: tcbInterpolationData => tcbInterpolationData.Y = 3.2f
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleTCBInterpolationData,
                updater: tcbInterpolationData => tcbInterpolationData.Z = 5.4f
            ),
            ComparisionTestCase.CompareNotNullObjectAgainstNull(SampleTCBInterpolationData())
        };

        public static VectorKeyframe SampleTCBInterpolationData() => new VectorKeyframe
        {
            Frame = 0xABCD,
            X = 2.2f,
            Y = 3.5f,
            Z = 4.9f
        };
    }
}
