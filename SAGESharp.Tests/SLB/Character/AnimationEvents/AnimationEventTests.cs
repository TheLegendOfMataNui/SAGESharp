/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using SAGESharp.SLB;
using SAGESharp.SLB.Character.AnimationEvents;

namespace SAGESharp.Tests.SLB.Character.AnimationEvents
{
    class AnimationEventTests
    {
        [TestCaseSource(nameof(EqualObjectsTestCases))]
        public void Test_Comparing_Equal_Objects(IComparisionTestCase<AnimationEvent> testCase) => testCase.Execute();

        public static IComparisionTestCase<AnimationEvent>[] EqualObjectsTestCases() => new IComparisionTestCase<AnimationEvent>[]
        {
            ComparisionTestCase.CompareObjectAgainstItself(SampleAnimationEvent()),
            ComparisionTestCase.CompareTwoEqualObjects(SampleAnimationEvent),
            ComparisionTestCase.CompareNullWithOperators<AnimationEvent>()
        };

        [TestCaseSource(nameof(NotEqualObjectsTestCases))]
        public void Test_Comparing_NotEqual_Objects(IComparisionTestCase<AnimationEvent> testCase) => testCase.Execute();

        public static IComparisionTestCase<AnimationEvent>[] NotEqualObjectsTestCases() => new IComparisionTestCase<AnimationEvent>[]
        {
            ComparisionTestCase.CompareNotNullObjectAgainstNull(SampleAnimationEvent()),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimationEvent,
                updater: animationEvent => animationEvent.Id += 1
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimationEvent,
                updater: animationEvent => animationEvent.EventArg2 += 1
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimationEvent,
                updater: animationEvent => animationEvent.EventArg3 += 1
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimationEvent,
                updater: animationEvent => animationEvent.EventArg4 += 1
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimationEvent,
                updater: animationEvent => animationEvent.EventArg5 += 1.1
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimationEvent,
                updater: animationEvent => animationEvent.EventArg6 += 1
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimationEvent,
                updater: animationEvent => animationEvent.EventArg7 += 1
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimationEvent,
                updater: animationEvent => animationEvent.EventArg8 += 1
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimationEvent,
                updater: animationEvent => animationEvent.EventArg9 += 1
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimationEvent,
                updater: animationEvent => animationEvent.EventArg10 += 1
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimationEvent,
                updater: animationEvent => animationEvent.Unknown += 1
            )
        };

        public static AnimationEvent SampleAnimationEvent() => new AnimationEvent
        {
            Id = Identifier.From("TOA1"),
            EventArg2 = Identifier.From("TOA2"),
            EventArg3 = Identifier.From("TOA3"),
            EventArg4 = Identifier.From("TOA4"),
            EventArg5 = 5.0,
            EventArg6 = 6,
            EventArg7 = 7,
            EventArg8 = 8,
            EventArg9 = 9,
            EventArg10 = 10,
            Unknown = 11
        };
    }
}
