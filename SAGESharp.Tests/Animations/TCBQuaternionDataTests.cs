/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using SAGESharp.Animations;

namespace SAGESharp.Tests.Animations
{
    class TCBQuaternionDataTests
    {
        [TestCaseSource(nameof(EqualObjectsTestCases))]
        public void Test_Comparing_Equal_Objects(IComparisionTestCase<TCBQuaternionData> testCase) => testCase.Execute();

        public static IComparisionTestCase<TCBQuaternionData>[] EqualObjectsTestCases() => new IComparisionTestCase<TCBQuaternionData>[]
        {
            ComparisionTestCase.CompareObjectAgainstItself(SampleTCBQuaternionData()),
            ComparisionTestCase.CompareTwoEqualObjects(SampleTCBQuaternionData),
            ComparisionTestCase.CompareNullWithOperators<TCBQuaternionData>()
        };

        [TestCaseSource(nameof(NotEqualObjectsTestCases))]
        public void Test_Comparing_NotEqual_Objects(IComparisionTestCase<TCBQuaternionData> testCase) => testCase.Execute();

        public static IComparisionTestCase<TCBQuaternionData>[] NotEqualObjectsTestCases() => new IComparisionTestCase<TCBQuaternionData>[]
        {
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleTCBQuaternionData,
                updater: tcbQuaternionData => tcbQuaternionData.Short1 = 51
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleTCBQuaternionData,
                updater: tcbQuaternionData => tcbQuaternionData.Short2 = 61
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleTCBQuaternionData,
                updater: tcbQuaternionData => tcbQuaternionData.Short3 = 71
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleTCBQuaternionData,
                updater: tcbQuaternionData => tcbQuaternionData.Short4 = 81
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleTCBQuaternionData,
                updater: tcbQuaternionData => tcbQuaternionData.Short5 = 91
            ),
            ComparisionTestCase.CompareNotNullObjectAgainstNull(SampleTCBQuaternionData())
        };

        public static TCBQuaternionData SampleTCBQuaternionData() => new TCBQuaternionData
        {
            Short1 = 0xAA,
            Short2 = 0xBB,
            Short3 = 0xCC,
            Short4 = 0xDD,
            Short5 = 0xEE
        };
    }
}
