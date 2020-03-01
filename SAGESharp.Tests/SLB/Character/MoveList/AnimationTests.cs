/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using SAGESharp.SLB;
using SAGESharp.SLB.Character.MoveList;
using System.Collections.Generic;

namespace SAGESharp.Tests.SLB.Character.MoveList
{
    class AnimationTests
    {
        [TestCaseSource(nameof(EqualObjectsTestCases))]
        public void Test_Comparing_Equal_Objects(IComparisionTestCase<Animation> testCase) => testCase.Execute();

        public static IComparisionTestCase<Animation>[] EqualObjectsTestCases() => new IComparisionTestCase<Animation>[]
        {
            ComparisionTestCase.CompareObjectAgainstItself(SampleAnimation()),
            ComparisionTestCase.CompareTwoEqualObjects(SampleAnimation),
            ComparisionTestCase.CompareNullWithOperators<Animation>()
        };

        [TestCaseSource(nameof(NotEqualObjectsTestCases))]
        public void Test_Comparing_NotEqual_Objects(IComparisionTestCase<Animation> testCase) => testCase.Execute();

        public static IComparisionTestCase<Animation>[] NotEqualObjectsTestCases() => new IComparisionTestCase<Animation>[]
        {
            ComparisionTestCase.CompareNotNullObjectAgainstNull(SampleAnimation()),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimation,
                updater: animation => animation.Id1 += 1
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimation,
                updater: animation => animation.Id2 += 2
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimation,
                updater: animation => animation.Flags1 += 3
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimation,
                updater: animation => animation.Index += 4
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimation,
                updater: animation => animation.Int1 += 5
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimation,
                updater: animation => animation.Float1 += .1f
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimation,
                updater: animation => animation.Float2 += .2f
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimation,
                updater: animation => animation.Flags2 += 6
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimation,
                updater: animation => animation.ReservedCounter += 7
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimation,
                updater: animation => animation.Triggers.Add(new SplitTrigger())
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimation,
                updater: animation => animation.Triggers.Clear()
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimation,
                updater: animation => animation.Triggers = null
            )
        };

        public static Animation SampleAnimation() => new Animation
        {
            Id1 = Identifier.From("ID01"),
            Id2 = Identifier.From("ID02"),
            Flags1 = 0x1122,
            Index = 0x3344,
            Int1 = 0x55667788,
            Float1 = 2.2f,
            Float2 = 3.3f,
            Flags2 = 0x11BBCCDD,
            ReservedCounter = 0x11FF,
            Triggers = new List<SplitTrigger>
            {
                SplitTriggerTests.SampleSplitTrigger()
            }
        };
    }
}
