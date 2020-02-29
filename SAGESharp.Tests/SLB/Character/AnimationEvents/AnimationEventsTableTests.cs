/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using NUtils.Extensions;
using SAGESharp.SLB.Character.AnimationEvents;
using System.Collections.Generic;

namespace SAGESharp.Tests.SLB.Character.AnimationEvents
{
    class AnimationEventsTableTests
    {
        [TestCaseSource(nameof(EqualObjectsTestCases))]
        public void Test_Comparing_Equal_Objects(IComparisionTestCase<AnimationEventsTable> testCase) => testCase.Execute();

        public static IComparisionTestCase<AnimationEventsTable>[] EqualObjectsTestCases() => new IComparisionTestCase<AnimationEventsTable>[]
        {
            ComparisionTestCase.CompareObjectAgainstItself(SampleAnimationEventsTable()),
            ComparisionTestCase.CompareTwoEqualObjects(SampleAnimationEventsTable),
            ComparisionTestCase.CompareNullWithOperators<AnimationEventsTable>()
        };

        [TestCaseSource(nameof(NotEqualObjectsTestCases))]
        public void Test_Comparing_NotEqual_Objects(IComparisionTestCase<AnimationEventsTable> testCase) => testCase.Execute();

        public static IComparisionTestCase<AnimationEventsTable>[] NotEqualObjectsTestCases() => new IComparisionTestCase<AnimationEventsTable>[]
        {
            ComparisionTestCase.CompareNotNullObjectAgainstNull(SampleAnimationEventsTable()),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimationEventsTable,
                updater: animationEventsTable => animationEventsTable.Entries.Clear()
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimationEventsTable,
                updater: animationEventsTable => animationEventsTable.Entries.Add(AnimationEventTests.SampleAnimationEvent())
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimationEventsTable,
                updater: animationEventsTable => animationEventsTable.Entries.RemoveAt(0)
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimationEventsTable,
                updater: animationEventsTable => animationEventsTable.Entries = null
            )
        };

        public static AnimationEventsTable SampleAnimationEventsTable() => new AnimationEventsTable
        {
            Entries = new List<AnimationEvent>
            {
                AnimationEventTests.SampleAnimationEvent(),
                AnimationEventTests.SampleAnimationEvent().Also(it => it.Id += 1),
                AnimationEventTests.SampleAnimationEvent().Also(it => it.Id += 2)
            }
        };
    }
}
