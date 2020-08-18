/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NUnit.Framework;
using NUtils.Extensions;
using SAGESharp.Animations;
using System;
using System.Collections.Generic;

namespace SAGESharp.Tests.Animations
{
    class BKDEntryTests
    {
        [Test]
        public void Test_Setting_A_Null_TCBQuaternion_List()
        {
            BKDEntry entry = new BKDEntry();
            Action action = () => entry.TCBQuaternionData = null;

            action.Should().ThrowArgumentNullException("value");
        }

        [Test]
        public void Test_Setting_A_Null_TCBInpterpolationData1_List()
        {
            BKDEntry entry = new BKDEntry();
            Action action = () => entry.TCBInterpolatorData1 = null;

            action.Should().ThrowArgumentNullException("value");
        }

        [Test]
        public void Test_Setting_A_Null_TCBInpterpolationData2_List()
        {
            BKDEntry entry = new BKDEntry();
            Action action = () => entry.TCBInterpolatorData2 = null;

            action.Should().ThrowArgumentNullException("value");
        }

        [TestCaseSource(nameof(EqualObjectsTestCases))]
        public void Test_Comparing_Equal_Objects(IComparisionTestCase<BKDEntry> testCase) => testCase.Execute();

        public static IComparisionTestCase<BKDEntry>[] EqualObjectsTestCases() => new IComparisionTestCase<BKDEntry>[]
        {
            ComparisionTestCase.CompareObjectAgainstItself(SampleBKDEntry()),
            ComparisionTestCase.CompareTwoEqualObjects(SampleBKDEntry),
            ComparisionTestCase.CompareNullWithOperators<BKDEntry>()
        };

        [TestCaseSource(nameof(NotEqualObjectsTestCases))]
        public void Test_Comparing_NotEqual_Objects(IComparisionTestCase<BKDEntry> testCase) => testCase.Execute();

        public static IComparisionTestCase<BKDEntry>[] NotEqualObjectsTestCases() => new IComparisionTestCase<BKDEntry>[]
        {
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleBKDEntry,
                updater: bkdEntry => bkdEntry.Id++
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleBKDEntry,
                updater: bkdEntry => bkdEntry.TCBQuaternionData.Add(new TCBQuaternionData())
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleBKDEntry,
                updater: bkdEntry => bkdEntry.TCBInterpolatorData1.Add(new TCBInterpolationData())
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleBKDEntry,
                updater: bkdEntry => bkdEntry.TCBInterpolatorData2.Add(new TCBInterpolationData())
            ),
            ComparisionTestCase.CompareNotNullObjectAgainstNull(SampleBKDEntry())
        };

        public static BKDEntry SampleBKDEntry() => new BKDEntry
        {
            Id = 6,
            TCBQuaternionData = new List<TCBQuaternionData>
            {
                TCBQuaternionDataTests.SampleTCBQuaternionData()
            },
            TCBInterpolatorData1 = new List<TCBInterpolationData>
            {
                TCBInterpolationDataTests.SampleTCBInterpolationData()
            },
            TCBInterpolatorData2 = new List<TCBInterpolationData>
            {
                TCBInterpolationDataTests.SampleTCBInterpolationData().Also(o =>
                {
                    o.Keyframe *= 3;
                    o.X *= 5;
                    o.Y *= 2;
                    o.Z *= 4;
                })
            }
        };
    }
}
