using NUnit.Framework;
using SAGESharp.SLB;
using SAGESharp.SLB.Level.Conversation;
using System;

using static SAGESharpTests.SLB.Level.Conversation.Defaults;

namespace SAGESharpTests.SLB.Level.Conversation
{
    class CharacterEqualityTests : AbstractEqualityByRefTests<Character>
    {
        protected override Character GetDefault() => DefaultCharacter();

        protected override bool EqualsOperator(Character left, Character right) => left == right;

        protected override bool NotEqualsOperator(Character left, Character right) => left != right;

        [TestCaseSource(nameof(Modifiers))]
        public void Test_Compare_Default_Object_With_Modified_Object(Action<Character> modifier) =>
            TestCompareDefaultObjectWithModifiedObject(modifier);

        static object[] Modifiers() => new ParameterGroup<Action<Character>>()
            .Parameters(character => character.ToaName = new Identifier("TOAX"))
            .Parameters(character => character.CharName = new Identifier("CHAX"))
            .Parameters(character => character.CharCont = new Identifier("CONX"))
            .Parameters(character => character.Entries = null)
            .Parameters(character => character.Entries.Clear())
            .Parameters(character => character.Entries[0].LineSide = LineSide.None)
            .Parameters(character => character.Entries.Add(null))
            .Parameters(character => character.Entries.Add(DefaultInfo()))
            .Build();

        [TestCaseSource(nameof(DualModifiers))]
        public void Test_Compare_Modified_Objects(Action<Character> modifierA, Action<Character> modifierB) =>
            TestCompareModifiedObjects(modifierA, modifierB);

        static object[] DualModifiers() => new ParameterGroup<Action<Character>, Action<Character>>()
            .Parameters(character => character.Entries = null, character => character.Entries.Clear())
            .Build();
    }
}
