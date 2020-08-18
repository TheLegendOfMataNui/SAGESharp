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
                updater: tcbQuaternionData => tcbQuaternionData.Keyframe = 51
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleTCBQuaternionData,
                updater: tcbQuaternionData => tcbQuaternionData.X = 61
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleTCBQuaternionData,
                updater: tcbQuaternionData => tcbQuaternionData.Y = 71
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleTCBQuaternionData,
                updater: tcbQuaternionData => tcbQuaternionData.Z = 81
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleTCBQuaternionData,
                updater: tcbQuaternionData => tcbQuaternionData.W = 91
            ),
            ComparisionTestCase.CompareNotNullObjectAgainstNull(SampleTCBQuaternionData())
        };

        public static TCBQuaternionData SampleTCBQuaternionData() => new TCBQuaternionData
        {
            Keyframe = 0xAA,
            X = 0xBB,
            Y = 0xCC,
            Z = 0xDD,
            W = 0xEE
        };
    }
}
