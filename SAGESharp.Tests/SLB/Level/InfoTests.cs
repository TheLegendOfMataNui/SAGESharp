/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using NUtils.Extensions;
using SAGESharp.SLB.Level;
using System.Collections.Generic;

namespace SAGESharp.Tests.SLB.Level
{
    class InfoTests
    {
        [TestCaseSource(nameof(EqualObjectsTestCases))]
        public void Test_Comparing_Equal_Objects(IComparisionTestCase<Info> testCase) => testCase.Execute();

        public static IComparisionTestCase<Info>[] EqualObjectsTestCases() => new IComparisionTestCase<Info>[]
        {
            ComparisionTestCase.CompareObjectAgainstItself(SampleInfo()),
            ComparisionTestCase.CompareTwoEqualObjects(SampleInfo),
            ComparisionTestCase.CompareNullWithOperators<Info>()
        };

        [TestCaseSource(nameof(NotEqualObjectsTestCases))]
        public void Test_Comparing_NotEqual_Objects(IComparisionTestCase<Info> testCase) => testCase.Execute();

        public static IComparisionTestCase<Info>[] NotEqualObjectsTestCases() => new IComparisionTestCase<Info>[]
        {
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleInfo, updater: info => info.LineSide = LineSide.None),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleInfo, updater: info => info.ConditionStart++),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleInfo, updater: info => info.ConditionEnd++),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleInfo, updater: info => info.StringLabel += 1),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleInfo, updater: info => info.StringIndex++),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleInfo, updater: info => info.Frames = null),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleInfo, updater: info => info.Frames.Clear()),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleInfo, updater: info => info.Frames[0] = null),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleInfo, updater: info => info.Frames[0].ToaAnimation++),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleInfo, updater: info => info.Frames.Add(null)),
            ComparisionTestCase.CompareTwoNotEqualObjects(supplier: SampleInfo, updater: info => info.Frames.Add(FrameTests.SampleFrame())),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: () => SampleInfo().Also(info => info.Frames.Clear()),
                updater: info => info.Frames = null
            ),
            ComparisionTestCase.CompareNotNullObjectAgainstNull(SampleInfo())
        };

        public static Info SampleInfo() => new Info
        {
            LineSide = LineSide.Right,
            ConditionStart = 1,
            ConditionEnd = 2,
            StringLabel = 3,
            StringIndex = 4,
            Frames = new List<Frame>() { FrameTests.SampleFrame() }
        };
    }
}
