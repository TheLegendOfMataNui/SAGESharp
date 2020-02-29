/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using SAGESharp.SLB.Level.Conversation;
using System.Collections.Generic;

using ConversationCharacter = SAGESharp.SLB.Level.Conversation.Character;

namespace SAGESharp.Tests.SLB.Level.Conversation
{
    class CharacterTableTests
    {
        [TestCaseSource(nameof(EqualObjectsTestCases))]
        public void Test_Comparing_Equal_Objects(IComparisionTestCase<CharacterTable> testCase) => testCase.Execute();

        public static IComparisionTestCase<CharacterTable>[] EqualObjectsTestCases() => new IComparisionTestCase<CharacterTable>[]
        {
            ComparisionTestCase.CompareObjectAgainstItself(TestData.SimpleCharacterTable()),
            ComparisionTestCase.CompareTwoEqualObjects(TestData.SimpleCharacterTable),
            ComparisionTestCase.CompareNullWithOperators<CharacterTable>()
        };

        [TestCaseSource(nameof(NotEqualObjectsTestCases))]
        public void Test_Comparing_NotEqual_Objects(IComparisionTestCase<CharacterTable> testCase) => testCase.Execute();

        public static IComparisionTestCase<CharacterTable>[] NotEqualObjectsTestCases() => new IComparisionTestCase<CharacterTable>[]
        {
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: TestData.SimpleCharacterTable,
                updater: conversation => conversation.Entries = null
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: TestData.SimpleCharacterTable,
                updater: conversation => conversation.Entries.Add(new ConversationCharacter())
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: TestData.SimpleCharacterTable,
                updater: conversation => conversation.Entries = new List<ConversationCharacter>()
            ),
            ComparisionTestCase.CompareNotNullObjectAgainstNull(TestData.SimpleCharacterTable())
        };
    }
}
