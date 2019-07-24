/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using SAGESharp.Testing;
using System;

using static SAGESharp.SLB.Level.Defaults;

namespace SAGESharp.SLB.Level
{
    class InfoEqualityTests : AbstractEqualityByRefTests<Info>
    {
        protected override Info GetDefault() => DefaultInfo();

        protected override bool EqualsOperator(Info left, Info right) => left == right;

        protected override bool NotEqualsOperator(Info left, Info right) => left != right;

        [TestCaseSource(nameof(InfoModifiers))]
        public void Test_Compare_Default_Object_With_Modified_Object(Action<Info> infoModifier) =>
            TestCompareDefaultObjectWithModifiedObject(infoModifier);

        static object[] InfoModifiers() => new ParameterGroup<Action<Info>>()
            .Parameters(info => info.LineSide = LineSide.None)
            .Parameters(info => info.ConditionStart++)
            .Parameters(info => info.ConditionEnd++)
            .Parameters(info => info.StringLabel = info.StringLabel + 1)
            .Parameters(info => info.StringIndex++)
            .Parameters(info => info.Frames = null)
            .Parameters(info => info.Frames.Clear())
            .Parameters(info => info.Frames[0] = null)
            .Parameters(info => info.Frames[0].ToaAnimation++)
            .Parameters(info => info.Frames.Add(null))
            .Parameters(info => info.Frames.Add(DefaultFrame()))
            .Build();

        [TestCaseSource(nameof(DualInfoModifiers))]
        public void Test_Compare_Modified_Objects(Action<Info> infoModifierA, Action<Info> infoModifierB) =>
            TestCompareModifiedObjects(infoModifierA, infoModifierB);

        static object[] DualInfoModifiers() => new ParameterGroup<Action<Info>, Action<Info>>()
            .Parameters(info => info.Frames = null, info => info.Frames.Clear())
            .Build();
    }
}