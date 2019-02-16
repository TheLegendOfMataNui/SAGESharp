using FluentAssertions;
using NUnit.Framework;
using SAGESharp.Testing;
using System.Collections.Generic;
using System.IO;

namespace SAGESharp.SLB.Level.Conversation
{
    class ConversationBinaryAccessorTests
    {
        [TestCaseSource(nameof(FileNamesAndConversations))]
        public void Test_Reading_A_File_Successfully(string testFilePath, IList<Character> expected)
            => ConversationBinaryAccessor.ReadConversation(testFilePath).Should().Equal(expected);

        [TestCaseSource(nameof(FileNamesAndConversations))]
        public void Test_Writing_A_File_Successfully(string testFilePath, IList<Character> conversation)
        {
            var outputFilePath = $"{testFilePath}.tst";
            ConversationBinaryAccessor.WriteConversation(outputFilePath, conversation);

            var actual = File.ReadAllBytes(outputFilePath);
            var expected = File.ReadAllBytes(testFilePath);

            actual.Should().Equal(expected);
        }

        static object[] FileNamesAndConversations() => new ParameterGroup<string, IList<Character>>()
            .Parameters(TestDataPath("EmptyConversation.slb"), TestData.EmptyConversation())
            .Parameters(TestDataPath("SimpleConversation.slb"), TestData.SimpleConversation())
            .Parameters(TestDataPath("ComplexConversation.slb"), TestData.ComplexConversation())
            .Build();

        private static string TestDataPath(string fileName)
            => Path.Combine(TestContext.CurrentContext.TestDirectory, "Test Data", "SLB", "Level", "Conversation", fileName);
    }
}
