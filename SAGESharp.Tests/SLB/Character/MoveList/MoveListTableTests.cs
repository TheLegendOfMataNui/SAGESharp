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
    class MoveListTableTests
    {
        [TestCaseSource(nameof(EqualObjectsTestCases))]
        public void Test_Comparing_Equal_Objects(IComparisionTestCase<MoveListTable> testCase) => testCase.Execute();

        public static IComparisionTestCase<MoveListTable>[] EqualObjectsTestCases() => new IComparisionTestCase<MoveListTable>[]
        {
            ComparisionTestCase.CompareObjectAgainstItself(SampleMoveListTable()),
            ComparisionTestCase.CompareTwoEqualObjects(SampleMoveListTable),
            ComparisionTestCase.CompareNullWithOperators<MoveListTable>()
        };

        [TestCaseSource(nameof(NotEqualObjectsTestCases))]
        public void Test_Comparing_NotEqual_Objects(IComparisionTestCase<MoveListTable> testCase) => testCase.Execute();

        public static IComparisionTestCase<MoveListTable>[] NotEqualObjectsTestCases() => new IComparisionTestCase<MoveListTable>[]
        {
            ComparisionTestCase.CompareNotNullObjectAgainstNull(SampleMoveListTable()),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleMoveListTable,
                updater: moveListTable => moveListTable.Id = Identifier.From("LIST")
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleMoveListTable,
                updater: moveListTable => moveListTable.Animations.Add(new Animation())
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleMoveListTable,
                updater: moveListTable => moveListTable.Animations.Clear()
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleMoveListTable,
                updater: moveListTable => moveListTable.Animations = null
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleMoveListTable,
                updater: moveListTable => moveListTable.AnimationsWithExtra.Add(new AnimationWithExtra())
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleMoveListTable,
                updater: moveListTable => moveListTable.AnimationsWithExtra.Clear()
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleMoveListTable,
                updater: moveListTable => moveListTable.AnimationsWithExtra = null
            )
        };

        public static MoveListTable SampleMoveListTable() => new MoveListTable
        {
            Id = Identifier.From("MOVE"),
            Animations = new List<Animation>
            {
                AnimationTests.SampleAnimation()
            },
            AnimationsWithExtra = new List<AnimationWithExtra>
            {
                AnimationWithExtraTests.SampleAnimationWithExtra()
            }
        };
    }
}
