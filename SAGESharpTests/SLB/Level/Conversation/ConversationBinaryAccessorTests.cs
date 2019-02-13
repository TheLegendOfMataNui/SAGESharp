using FluentAssertions;
using NUnit.Framework;
using SAGESharpTests;
using System.Collections.Generic;
using System.IO;

namespace SAGESharp.SLB.Level.Conversation
{
    class ConversationBinaryAccessorTests
    {
        [TestCaseSource(nameof(FileNamesAndConversations))]
        public void Test_Reading_A_File_Successfully(string testFilePath, IList<Character> expected)
        {
            using (var stream = new FileStream(testFilePath, FileMode.Open))
            {
                var result = ConversationBinaryAccessor.ReadConversation(stream);

                result.Should().BeEquivalentTo(expected);
            }
        }

        [TestCaseSource(nameof(FileNamesAndConversations))]
        public void Test_Writing_A_File_Successfully(string expectedOutput, IList<Character> conversation)
        {
            var expected = File.ReadAllBytes(expectedOutput);
            using (var stream = new MemoryStream())
            {
                ConversationBinaryAccessor
                    .WriteConversation(stream, conversation);

                stream.ToArray().Should().BeEquivalentTo(expected);
            }
        }

        static object[] FileNamesAndConversations() => new ParameterGroup<string, IList<Character>>()
            .Parameters(TestDataPath("EmptyConversation.slb"), TestData.EmptyConversation())
            .Parameters(TestDataPath("SimpleConversation.slb"), TestData.SimpleConversation())
            .Parameters(TestDataPath("ComplexConversation.slb"), TestData.ComplexConversation())
            .Build();

        private static string TestDataPath(string fileName)
            => $@"{TestContext.CurrentContext.TestDirectory}\Test Data\SLB\Level\Conversation\{fileName}";
    }
}
