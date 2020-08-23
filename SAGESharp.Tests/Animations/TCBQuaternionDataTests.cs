/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NUnit.Framework;
using SAGESharp.Animations;
using System.Reflection;

namespace SAGESharp.Tests.Animations
{
    class TCBQuaternionDataTests
    {
        [TestCaseSource(nameof(ComponentTestCases))]
        public void Test_Getting_The_Value_Of_A_Component(ComponentTestCase testCase)
        {
            TCBQuaternionData quaternion = new TCBQuaternionData();

            testCase.SetPrivateField(quaternion, 0x8000);

            testCase.GetProperty(quaternion).Should().Be(1f);
        }

        [TestCaseSource(nameof(ComponentTestCases))]
        public void Test_Setting_The_Value_Of_A_Component(ComponentTestCase testCase)
        {
            TCBQuaternionData quaternion = new TCBQuaternionData();

            testCase.SetProperty(quaternion, 1f);

            testCase.GetPrivateField(quaternion).Should().Be(0x8000);
        }

        public static ComponentTestCase[] ComponentTestCases() => new ComponentTestCase[]
        {
            new ComponentTestCase(
                description: "For component X",
                fieldName: "x",
                propertyName: nameof(TCBQuaternionData.X)
            ),
            new ComponentTestCase(
                description: "For component Y",
                fieldName: "y",
                propertyName: nameof(TCBQuaternionData.Y)
            ),
            new ComponentTestCase(
                description: "For component Z",
                fieldName: "z",
                propertyName: nameof(TCBQuaternionData.Z)
            ),
            new ComponentTestCase(
                description: "For component W",
                fieldName: "w",
                propertyName: nameof(TCBQuaternionData.W)
            )
        };

        public class ComponentTestCase : AbstractTestCase
        {
            private readonly FieldInfo fieldInfo;

            private readonly PropertyInfo propertyInfo;

            public ComponentTestCase(string description, string fieldName, string propertyName) : base(description)
            {
                fieldInfo = typeof(TCBQuaternionData).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                propertyInfo = typeof(TCBQuaternionData).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            }

            public ushort GetPrivateField(TCBQuaternionData quaternion)
                => (ushort)fieldInfo.GetValue(quaternion);

            public float GetProperty(TCBQuaternionData quaternion)
                => (float)propertyInfo.GetValue(quaternion);

            public void SetPrivateField(TCBQuaternionData quaternion, ushort value)
                => fieldInfo.SetValue(quaternion, value);

            public void SetProperty(TCBQuaternionData quaternion, float value)
                => propertyInfo.SetValue(quaternion, value);
        }

        [TestCaseSource(nameof(EqualObjectsTestCases))]
        public void Test_Comparing_Equal_Objects(IComparisionTestCase<TCBQuaternionData> testCase) => testCase.Execute();

        public static IComparisionTestCase<TCBQuaternionData>[] EqualObjectsTestCases() => new IComparisionTestCase<TCBQuaternionData>[]
        {
            ComparisionTestCase.CompareObjectAgainstItself(SampleTCBQuaternionData()),
            ComparisionTestCase.CompareTwoEqualObjects(SampleTCBQuaternionData),
            ComparisionTestCase.CompareNullWithOperators<TCBQuaternionData>()
        };

        [TestCaseSource(nameof(NotEqualObjectsTestCases))]
        public void Test_Comparing_NotEqual_Objects(IComparisionTestCase<TCBQuaternionData> testCase) => testCase.Execute();

        public static IComparisionTestCase<TCBQuaternionData>[] NotEqualObjectsTestCases() => new IComparisionTestCase<TCBQuaternionData>[]
        {
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleTCBQuaternionData,
                updater: tcbQuaternionData => tcbQuaternionData.Keyframe = 51
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleTCBQuaternionData,
                updater: tcbQuaternionData => tcbQuaternionData.X = 61.32f
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleTCBQuaternionData,
                updater: tcbQuaternionData => tcbQuaternionData.Y = 71.02f
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleTCBQuaternionData,
                updater: tcbQuaternionData => tcbQuaternionData.Z = 12.78f
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleTCBQuaternionData,
                updater: tcbQuaternionData => tcbQuaternionData.W = 85.69f
            ),
            ComparisionTestCase.CompareNotNullObjectAgainstNull(SampleTCBQuaternionData())
        };

        public static TCBQuaternionData SampleTCBQuaternionData() => new TCBQuaternionData
        {
            Keyframe = 0xAA,
            X = 43.23f,
            Y = 36.78f,
            Z = 12.49f,
            W = 78.63f
        };
    }
}
