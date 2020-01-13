/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using SAGESharp.Testing;

namespace SAGESharp.SLB.Level
{
    class FrameTests
    {
        [TestCaseSource(nameof(EqualObjectsTestCases))]
        public void Test_Comparing_Equal_Objects(IComparisionTestCase<Frame> testCase) => testCase.Execute();

        public static IComparisionTestCase<Frame>[] EqualObjectsTestCases() => new IComparisionTestCase<Frame>[]
        {
            ComparisionTestCase.CompareObjectAgainstItself(SampleFrame()),
            ComparisionTestCase.CompareTwoEqualObjects(SampleFrame),
            ComparisionTestCase.CompareNullWithOperators<Frame>()
        };

        [TestCaseSource(nameof(NotEqualObjectsTestCases))]
        public void Test_Comparing_NotEqual_Objects(IComparisionTestCase<Frame> testCase) => testCase.Execute();

        public static IComparisionTestCase<Frame>[] NotEqualObjectsTestCases() => new IComparisionTestCase<Frame>[]
        {
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleFrame, updater: frame => frame.ToaAnimation = 0),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleFrame, updater: frame => frame.CharAnimation = 0),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleFrame, updater: frame => frame.CameraPositionTarget = 0),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleFrame, updater: frame => frame.CameraDistance = 0),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleFrame, updater: frame => frame.StringIndex = 0),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleFrame, updater: frame => frame.ConversationSounds += 'a'),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleFrame, updater: frame => frame.ConversationSounds = null),
            ComparisionTestCase.CompareNotNullObjectAgainstNull(SampleFrame())
        };

        public static Frame SampleFrame() => new Frame
        {
            ToaAnimation = 1,
            CharAnimation = 2,
            CameraPositionTarget = 3,
            CameraDistance = 4,
            StringIndex = 5,
            ConversationSounds = "sounds"
        };
    }
}
