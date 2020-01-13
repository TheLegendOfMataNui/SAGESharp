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
}
