/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using SAGESharp.SLB;
using SAGESharp.SLB.Level;
using System.Collections.Generic;

namespace SAGESharp.Tests.SLB.Level
{
    class ConversationCharacterTests
    {
        [TestCaseSource(nameof(EqualObjectsTestCases))]
        public void Test_Comparing_Equal_Objects(IComparisionTestCase<ConversationCharacter> testCase) => testCase.Execute();

        public static IComparisionTestCase<ConversationCharacter>[] EqualObjectsTestCases() => new IComparisionTestCase<ConversationCharacter>[]
        {
            ComparisionTestCase.CompareObjectAgainstItself(SampleConversationCharacter()),
            ComparisionTestCase.CompareTwoEqualObjects(SampleConversationCharacter),
            ComparisionTestCase.CompareNullWithOperators<ConversationCharacter>()
        };

        [TestCaseSource(nameof(NotEqualObjectsTestCases))]
        public void Test_Comparing_NotEqual_Objects(IComparisionTestCase<ConversationCharacter> testCase) => testCase.Execute();

        public static IComparisionTestCase<ConversationCharacter>[] NotEqualObjectsTestCases() => new IComparisionTestCase<ConversationCharacter>[]
        {
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleConversationCharacter,
                updater: character => character.ToaName = Identifier.From("TOAX")
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleConversationCharacter,
                updater: character => character.CharName = Identifier.From("CHAX")
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleConversationCharacter,
                updater: character => character.CharCont = Identifier.From("CONX")
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleConversationCharacter,
                updater: character => character.Entries = null
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleConversationCharacter,
                updater: character => character.Entries.Clear()
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleConversationCharacter,
                updater: character => character.Entries[0].LineSide = LineSide.None
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleConversationCharacter,
                updater: character => character.Entries.Add(null)
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: SampleConversationCharacter,
                updater: character => character.Entries.Add(InfoTests.SampleInfo())
            ),
            ComparisionTestCase.CompareNotNullObjectAgainstNull(SampleConversationCharacter())
        };

        public static ConversationCharacter SampleConversationCharacter() => new ConversationCharacter
        {
            ToaName = Identifier.From("TOA1"),
            CharName = Identifier.From("CHA1"),
            CharCont = Identifier.From("CON1"),
            Entries = new List<Info>() { InfoTests.SampleInfo() }
        };
    }
}
