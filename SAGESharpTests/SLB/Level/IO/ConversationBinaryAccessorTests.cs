/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NUnit.Framework;
using SAGESharp.Testing;
using System.Collections.Generic;
using System.IO;

namespace SAGESharp.SLB.Level.IO
{
    class ConversationBinaryAccessorTests
    {
        [TestCaseSource(nameof(FileNamesAndConversations))]
        public void Test_Writing_A_File_Successfully(string testFilePath, IList<ConversationCharacter> conversation)
        {
            var outputFilePath = $"{testFilePath}.tst";
            ConversationBinaryAccessor.WriteConversation(outputFilePath, conversation);

            var actual = File.ReadAllBytes(outputFilePath);
            var expected = File.ReadAllBytes(testFilePath);

            actual.Should().Equal(expected);
        }

        static object[] FileNamesAndConversations() => new ParameterGroup<string, IList<ConversationCharacter>>()
            .Parameters(TestDataPath("EmptyConversation.slb"), TestData.EmptyConversation().Entries)
            .Parameters(TestDataPath("SimpleConversation.slb"), TestData.SimpleConversation().Entries)
            .Parameters(TestDataPath("ComplexConversation.slb"), TestData.ComplexConversation().Entries)
            .Build();

        private static string TestDataPath(string fileName)
            => Path.Combine(TestContext.CurrentContext.TestDirectory, "Test Data", "SLB", "Level", "Conversation", fileName);
    }
}
