/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace SAGESharp.Tests
{
    class BKDTests
    {
        [Test]
        public void Test_Setting_A_Null_Entry_List_To_A_BKD_Object()
        {
            BKD bkd = new BKD();
            Action action = () => bkd.Entries = null;

            action.Should().ThrowArgumentNullException("value");
        }

        [TestCaseSource(nameof(EqualObjectsTestCases))]
        public void Test_Comparing_Equal_Objects(IComparisionTestCase<BKD> testCase) => testCase.Execute();

        public static IComparisionTestCase<BKD>[] EqualObjectsTestCases() => new IComparisionTestCase<BKD>[]
        {
            ComparisionTestCase.CompareObjectAgainstItself(SampleBKD()),
            ComparisionTestCase.CompareTwoEqualObjects(SampleBKD),
            ComparisionTestCase.CompareNullWithOperators<BKD>()
        };

        [TestCaseSource(nameof(NotEqualObjectsTestCases))]
        public void Test_Comparing_NotEqual_Objects(IComparisionTestCase<BKD> testCase) => testCase.Execute();

        public static IComparisionTestCase<BKD>[] NotEqualObjectsTestCases() => new IComparisionTestCase<BKD>[]
        {
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleBKD,
                updater: bkd => bkd.Length *= 2
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleBKD,
                updater: bkd => bkd.Entries.Add(new BKDEntry())
            ),
            ComparisionTestCase.CompareNotNullObjectAgainstNull(SampleBKD())
        };

        public static BKD SampleBKD() => new BKD
        {
            Length = 5,
            Entries = new List<BKDEntry>
            {
                BKDEntryTests.SampleBKDEntry()
            }
        };
    }
}
