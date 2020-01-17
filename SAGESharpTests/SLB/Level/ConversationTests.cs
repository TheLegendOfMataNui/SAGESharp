/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using SAGESharp.Testing;
using System.Collections.Generic;

namespace SAGESharp.SLB.Level
{
    class ConversationTests
    {
        [TestCaseSource(nameof(EqualObjectsTestCases))]
        public void Test_Comparing_Equal_Objects(IComparisionTestCase<Conversation> testCase) => testCase.Execute();

        public static IComparisionTestCase<Conversation>[] EqualObjectsTestCases() => new IComparisionTestCase<Conversation>[]
        {
            ComparisionTestCase.CompareObjectAgainstItself(TestData.SimpleConversation()),
            ComparisionTestCase.CompareTwoEqualObjects(TestData.SimpleConversation),
            ComparisionTestCase.CompareNullWithOperators<Conversation>()
        };

        [TestCaseSource(nameof(NotEqualObjectsTestCases))]
        public void Test_Comparing_NotEqual_Objects(IComparisionTestCase<Conversation> testCase) => testCase.Execute();

        public static IComparisionTestCase<Conversation>[] NotEqualObjectsTestCases() => new IComparisionTestCase<Conversation>[]
        {
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: TestData.SimpleConversation,
                updater: conversation => conversation.Entries = null
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: TestData.SimpleConversation,
                updater: conversation => conversation.Entries.Add(new ConversationCharacter())
            ),
            ComparisionTestCase.CompareTwoNotEqualObjects(
                supplier: TestData.SimpleConversation,
                updater: conversation => conversation.Entries = new List<ConversationCharacter>()
            ),
            ComparisionTestCase.CompareNotNullObjectAgainstNull(TestData.SimpleConversation())
        };
    }
}
