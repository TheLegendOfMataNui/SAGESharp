/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using SAGESharp.SLB.Character.MoveList;

namespace SAGESharp.Tests.SLB.Character.MoveList
{
    class AnimationWithExtraTests
    {
        [TestCaseSource(nameof(EqualObjectsTestCases))]
        public void Test_Comparing_Equal_Objects(IComparisionTestCase<AnimationWithExtra> testCase) => testCase.Execute();

        public static IComparisionTestCase<AnimationWithExtra>[] EqualObjectsTestCases() => new IComparisionTestCase<AnimationWithExtra>[]
        {
            ComparisionTestCase.CompareObjectAgainstItself(SampleAnimationWithExtra()),
            ComparisionTestCase.CompareTwoEqualObjects(SampleAnimationWithExtra),
            ComparisionTestCase.CompareNullWithOperators<AnimationWithExtra>()
        };

        [TestCaseSource(nameof(NotEqualObjectsTestCases))]
        public void Test_Comparing_NotEqual_Objects(IComparisionTestCase<AnimationWithExtra> testCase) => testCase.Execute();

        public static IComparisionTestCase<AnimationWithExtra>[] NotEqualObjectsTestCases() => new IComparisionTestCase<AnimationWithExtra>[]
        {
            ComparisionTestCase.CompareNotNullObjectAgainstNull(SampleAnimationWithExtra()),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimationWithExtra,
                updater: animationWithExtra => animationWithExtra.Animation = new Animation()
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimationWithExtra,
                updater: animationWithExtra => animationWithExtra.Animation = null
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleAnimationWithExtra,
                updater: animationWithExtra => animationWithExtra.Extra = 0x55667788
            )
        };

        public static AnimationWithExtra SampleAnimationWithExtra() => new AnimationWithExtra
        {
            Animation = AnimationTests.SampleAnimation(),
            Extra = 0x11223344
        };
    }
}
