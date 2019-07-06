/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using SAGESharp.Testing;
using System;

namespace SAGESharp
{
    class BKDEqualityTests : AbstractEqualityByRefTests<BKD>
    {
        protected override BKD GetDefault() => new BKD();

        protected override bool EqualsOperator(BKD left, BKD right) => left == right;

        protected override bool NotEqualsOperator(BKD left, BKD right) => left != right;

        [TestCaseSource(nameof(Modifiers))]
        public void Test_Compare_Default_Object_With_Modified_Object(Action<BKD> modifier)
            => TestCompareDefaultObjectWithModifiedObject(modifier);

        static object[] Modifiers() => new ParameterGroup<Action<BKD>>()
            .Parameters(bkd => bkd.Length = 10)
            .Parameters(bkd => bkd.Entries.Add(new BKDEntry()))
            .Build();
    }

    class BKDEntryEqualityTests : AbstractEqualityByRefTests<BKDEntry>
    {
        protected override BKDEntry GetDefault() => new BKDEntry();

        protected override bool EqualsOperator(BKDEntry left, BKDEntry right) => left == right;

        protected override bool NotEqualsOperator(BKDEntry left, BKDEntry right) => left != right;

        [TestCaseSource(nameof(Modifiers))]
        public void Test_Compare_Default_Object_With_Modified_Object(Action<BKDEntry> modifier)
            => TestCompareDefaultObjectWithModifiedObject(modifier);

        static object[] Modifiers() => new ParameterGroup<Action<BKDEntry>>()
            .Parameters(bkdEntry => bkdEntry.Id = 5)
            .Parameters(bkdEntry => bkdEntry.TCBQuaternionData.Add(new TCBQuaternionData()))
            .Parameters(bkdEntry => bkdEntry.TCBInterpolatorData1.Add(new TCBInterpolationData()))
            .Parameters(bkdEntry => bkdEntry.TCBInterpolatorData2.Add(new TCBInterpolationData()))
            .Build();
    }

    class TCBQuaternionDataEqualityTest : AbstractEqualityByRefTests<TCBQuaternionData>
    {
        protected override TCBQuaternionData GetDefault() => new TCBQuaternionData();

        protected override bool EqualsOperator(TCBQuaternionData left, TCBQuaternionData right) => left == right;

        protected override bool NotEqualsOperator(TCBQuaternionData left, TCBQuaternionData right) => left != right;

        [TestCaseSource(nameof(Modifiers))]
        public void Test_Compare_Default_Object_With_Modified_Object(Action<TCBQuaternionData> modifier)
            => TestCompareDefaultObjectWithModifiedObject(modifier);

        static object[] Modifiers() => new ParameterGroup<Action<TCBQuaternionData>>()
            .Parameters(tcbQuaternionData => tcbQuaternionData.Short1 = 5)
            .Parameters(tcbQuaternionData => tcbQuaternionData.Short2 = 6)
            .Parameters(tcbQuaternionData => tcbQuaternionData.Short3 = 7)
            .Parameters(tcbQuaternionData => tcbQuaternionData.Short4 = 8)
            .Parameters(tcbQuaternionData => tcbQuaternionData.Short5 = 9)
            .Build();
    }

    class TCBInterpolationDataEqualityTests : AbstractEqualityByRefTests<TCBInterpolationData>
    {
        protected override TCBInterpolationData GetDefault() => new TCBInterpolationData();

        protected override bool EqualsOperator(TCBInterpolationData left, TCBInterpolationData right) => left == right;

        protected override bool NotEqualsOperator(TCBInterpolationData left, TCBInterpolationData right) => left != right;

        [TestCaseSource(nameof(Modifiers))]
        public void Test_Compare_Default_Object_With_Modified_Object(Action<TCBInterpolationData> modifier)
            => TestCompareDefaultObjectWithModifiedObject(modifier);

        static object[] Modifiers() => new ParameterGroup<Action<TCBInterpolationData>>()
            .Parameters(tcbQuaternionData => tcbQuaternionData.Long1 = 12)
            .Parameters(tcbQuaternionData => tcbQuaternionData.Float1 = 6.9f)
            .Parameters(tcbQuaternionData => tcbQuaternionData.Float2 = 3.2f)
            .Parameters(tcbQuaternionData => tcbQuaternionData.Float3 = 5.4f)
            .Build();
    }
}
