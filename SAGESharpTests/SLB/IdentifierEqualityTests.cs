using NUnit.Framework;
using SAGESharp.SLB;
using SAGESharpTests.Util;
using System;

namespace SAGESharpTests.SLB
{
    class IdentifierEqualityTests : AbstractEqualityTests<Identifier>
    {
        protected override Identifier GetDefault() => new Identifier("ABCD");

        protected override bool EqualsOperator(Identifier left, Identifier right) => left == right;

        protected override bool NotEqualsOperator(Identifier left, Identifier right) => left != right;

        [TestCaseSource(nameof(Modifiers))]
        public void Test_Compare_Default_Object_With_Modified_Object(Action<Identifier> modifier) =>
            TestCompareDefaultObjectWithModifiedObject(modifier);

        static object[] Modifiers() => new ParameterGroup<Action<Identifier>>()
            .Parameters(identifier => identifier.B0 = 0x00)
            .Parameters(identifier => identifier.B1 = 0x00)
            .Parameters(identifier => identifier.B2 = 0x00)
            .Parameters(identifier => identifier.B3 = 0x00)
            .Parameters(identifier => identifier.C0 = 'X')
            .Parameters(identifier => identifier.C1 = 'Y')
            .Parameters(identifier => identifier.C2 = 'Z')
            .Parameters(identifier => identifier.C3 = 'S')
            .Parameters(identifier => identifier.SetFrom(identifier.ToInteger() + 1))
            .Parameters(identifier => identifier.SetFrom(new byte[] { 0x01, 0x02, 0x03, 0x04 }))
            .Parameters(identifier => identifier.SetFrom("DCBA"))
            .Build();

        [TestCaseSource(nameof(DualModifiers))]
        public void Test_Compare_Modified_Objects(Action<Identifier> modifierA, Action<Identifier> modifierB) =>
            TestCompareModifiedObjects(modifierA, modifierB);

        static object[] DualModifiers() => new ParameterGroup<Action<Identifier>, Action<Identifier>>()
            .Parameters(identifier => identifier.SetFrom(0x11223344), identifier => identifier.SetFrom(0x11121314))
            .Build();
    }
}
