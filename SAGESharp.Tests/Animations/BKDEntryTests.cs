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
            TransformAnimation entry = new TransformAnimation();
            Action action = () => entry.RotationKeyframes = null;

            action.Should().ThrowArgumentNullException("value");
        }

        [Test]
        public void Test_Setting_A_Null_TCBInpterpolationData1_List()
        {
            TransformAnimation entry = new TransformAnimation();
            Action action = () => entry.TranslationKeyframes = null;

            action.Should().ThrowArgumentNullException("value");
        }

        [Test]
        public void Test_Setting_A_Null_TCBInpterpolationData2_List()
        {
            TransformAnimation entry = new TransformAnimation();
            Action action = () => entry.ScaleKeyframes = null;

            action.Should().ThrowArgumentNullException("value");
        }

        [TestCaseSource(nameof(EqualObjectsTestCases))]
        public void Test_Comparing_Equal_Objects(IComparisionTestCase<TransformAnimation> testCase) => testCase.Execute();

        public static IComparisionTestCase<TransformAnimation>[] EqualObjectsTestCases() => new IComparisionTestCase<TransformAnimation>[]
        {
            ComparisionTestCase.CompareObjectAgainstItself(SampleBKDEntry()),
            ComparisionTestCase.CompareTwoEqualObjects(SampleBKDEntry),
            ComparisionTestCase.CompareNullWithOperators<TransformAnimation>()
        };

        [TestCaseSource(nameof(NotEqualObjectsTestCases))]
        public void Test_Comparing_NotEqual_Objects(IComparisionTestCase<TransformAnimation> testCase) => testCase.Execute();

        public static IComparisionTestCase<TransformAnimation>[] NotEqualObjectsTestCases() => new IComparisionTestCase<TransformAnimation>[]
        {
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleBKDEntry,
                updater: bkdEntry => bkdEntry.BoneID++
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleBKDEntry,
                updater: bkdEntry => bkdEntry.RotationKeyframes.Add(new QuaternionKeyframe())
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleBKDEntry,
                updater: bkdEntry => bkdEntry.TranslationKeyframes.Add(new VectorKeyframe())
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleBKDEntry,
                updater: bkdEntry => bkdEntry.ScaleKeyframes.Add(new VectorKeyframe())
            ),
            ComparisionTestCase.CompareNotNullObjectAgainstNull(SampleBKDEntry())
        };

        public static TransformAnimation SampleBKDEntry() => new TransformAnimation
        {
            BoneID = 6,
            RotationKeyframes = new List<QuaternionKeyframe>
            {
                TCBQuaternionDataTests.SampleTCBQuaternionData()
            },
            TranslationKeyframes = new List<VectorKeyframe>
            {
                TCBInterpolationDataTests.SampleTCBInterpolationData()
            },
            ScaleKeyframes = new List<VectorKeyframe>
            {
                TCBInterpolationDataTests.SampleTCBInterpolationData().Also(o =>
                {
                    o.Frame *= 3;
                    o.X *= 5;
                    o.Y *= 2;
                    o.Z *= 4;
                })
            }
        };
    }
}
