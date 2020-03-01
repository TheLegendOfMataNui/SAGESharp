/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using SAGESharp.SLB;
using SAGESharp.SLB.Character.MoveList;

namespace SAGESharp.Tests.SLB.Character.MoveList
{
    class SplitTriggerTests
    {
        [TestCaseSource(nameof(EqualObjectsTestCases))]
        public void Test_Comparing_Equal_Objects(IComparisionTestCase<SplitTrigger> testCase) => testCase.Execute();

        public static IComparisionTestCase<SplitTrigger>[] EqualObjectsTestCases() => new IComparisionTestCase<SplitTrigger>[]
        {
            ComparisionTestCase.CompareObjectAgainstItself(SampleSplitTrigger()),
            ComparisionTestCase.CompareTwoEqualObjects(SampleSplitTrigger),
            ComparisionTestCase.CompareNullWithOperators<SplitTrigger>()
        };

        [TestCaseSource(nameof(NotEqualObjectsTestCases))]
        public void Test_Comparing_NotEqual_Objects(IComparisionTestCase<SplitTrigger> testCase) => testCase.Execute();

        public static IComparisionTestCase<SplitTrigger>[] NotEqualObjectsTestCases() => new IComparisionTestCase<SplitTrigger>[]
        {
            ComparisionTestCase.CompareNotNullObjectAgainstNull(SampleSplitTrigger()),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleSplitTrigger,
                updater: splitTrigger => splitTrigger.Input += 1
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleSplitTrigger,
                updater: splitTrigger => splitTrigger.Id += 2
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleSplitTrigger,
                updater: splitTrigger => splitTrigger.Float1 += 3
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleSplitTrigger,
                updater: splitTrigger => splitTrigger.Float2 += 4
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleSplitTrigger,
                updater: splitTrigger => splitTrigger.Float3 += 5
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleSplitTrigger,
                updater: splitTrigger => splitTrigger.Flags += 6
            )
        };

        public static SplitTrigger SampleSplitTrigger() => new SplitTrigger
        {
            Input = 0x11223344,
            Id = Identifier.From("ID01"),
            Float1 = 1.2f,
            Float2 = 2.3f,
            Float3 = 3.4f,
            Flags = 0x21,
        };
    }
}
