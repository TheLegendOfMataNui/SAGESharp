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
    class ConversationCharacterEqualityTests : AbstractEqualityByRefTests<ConversationCharacter>
    {
        protected override ConversationCharacter GetDefault() => DefaultConversationCharacter();

        protected override bool EqualsOperator(ConversationCharacter left, ConversationCharacter right) => left == right;

        protected override bool NotEqualsOperator(ConversationCharacter left, ConversationCharacter right) => left != right;

        [TestCaseSource(nameof(Modifiers))]
        public void Test_Compare_Default_Object_With_Modified_Object(Action<ConversationCharacter> modifier) =>
            TestCompareDefaultObjectWithModifiedObject(modifier);

        static object[] Modifiers() => new ParameterGroup<Action<ConversationCharacter>>()
            .Parameters(character => character.ToaName = Identifier.From("TOAX"))
            .Parameters(character => character.CharName = Identifier.From("CHAX"))
            .Parameters(character => character.CharCont = Identifier.From("CONX"))
            .Parameters(character => character.Entries = null)
            .Parameters(character => character.Entries.Clear())
            .Parameters(character => character.Entries[0].LineSide = LineSide.None)
            .Parameters(character => character.Entries.Add(null))
            .Parameters(character => character.Entries.Add(DefaultInfo()))
            .Build();

        [TestCaseSource(nameof(DualModifiers))]
        public void Test_Compare_Modified_Objects(Action<ConversationCharacter> modifierA, Action<ConversationCharacter> modifierB) =>
            TestCompareModifiedObjects(modifierA, modifierB);

        static object[] DualModifiers() => new ParameterGroup<Action<ConversationCharacter>, Action<ConversationCharacter>>()
            .Parameters(character => character.Entries = null, character => character.Entries.Clear())
            .Build();
    }
}
