/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NUnit.Framework;
using SAGESharp.Animations;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SAGESharp.Tests.Animations
{
    class BKDTests
    {
        private static readonly FieldInfo LENGTH_FIELD = typeof(BKD)
            .GetField("length", BindingFlags.NonPublic | BindingFlags.Instance);

        [Test]
        public void Test_Getting_The_Length_Of_A_BKD_Object()
        {
            BKD bkd = new BKD();

            LENGTH_FIELD.SetValue(bkd, (ushort)3647);

            bkd.Length.Should().Be(60.7833366f);
        }

        [Test]
        public void Test_Setting_The_Length_Of_A_BKD_Object()
        {
            BKD bkd = new BKD
            {
                Length = 60.7833366f
            };

            LENGTH_FIELD.GetValue(bkd).Should().Be(3647);
        }

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
                updater: bkd => bkd.Length *= 4
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleBKD,
                updater: bkd => bkd.Entries.Add(new TransformAnimation())
            ),
            ComparisionTestCase.CompareNotNullObjectAgainstNull(SampleBKD())
        };

        public static BKD SampleBKD() => new BKD
        {
            Length = 1.5f,
            Entries = new List<TransformAnimation>
            {
                BKDEntryTests.SampleBKDEntry()
            }
        };
    }
}
