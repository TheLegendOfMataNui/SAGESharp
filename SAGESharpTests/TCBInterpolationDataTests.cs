/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using SAGESharp.Testing;

namespace SAGESharp
{
    class TCBInterpolationDataTests
    {
        [TestCaseSource(nameof(EqualObjectsTestCases))]
        public void Test_Comparing_Equal_Objects(IComparisionTestCase<TCBInterpolationData> testCase) => testCase.Execute();

        public static IComparisionTestCase<TCBInterpolationData>[] EqualObjectsTestCases() => new IComparisionTestCase<TCBInterpolationData>[]
        {
            ComparisionTestCase.CompareObjectAgainstItself(SampleTCBInterpolationData()),
            ComparisionTestCase.CompareTwoEqualObjects(SampleTCBInterpolationData),
            ComparisionTestCase.CompareNullWithOperators<TCBInterpolationData>()
        };

        [TestCaseSource(nameof(NotEqualObjectsTestCases))]
        public void Test_Comparing_NotEqual_Objects(IComparisionTestCase<TCBInterpolationData> testCase) => testCase.Execute();

        public static IComparisionTestCase<TCBInterpolationData>[] NotEqualObjectsTestCases() => new IComparisionTestCase<TCBInterpolationData>[]
        {
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleTCBInterpolationData,
                updater: tcbInterpolationData => tcbInterpolationData.Long1 = 12
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleTCBInterpolationData,
                updater: tcbInterpolationData => tcbInterpolationData.Float1 = 6.9f
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleTCBInterpolationData,
                updater: tcbInterpolationData => tcbInterpolationData.Float2 = 3.2f
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleTCBInterpolationData,
                updater: tcbInterpolationData => tcbInterpolationData.Float3 = 5.4f
            ),
            ComparisionTestCase.CompareNotNullObjectAgainstNull(SampleTCBInterpolationData())
        };

        public static TCBInterpolationData SampleTCBInterpolationData() => new TCBInterpolationData
        {
            Long1 = 0xABCD,
            Float1 = 2.2f,
            Float2 = 3.5f,
            Float3 = 4.9f
        };
    }
}
