using FluentAssertions;
using NUnit.Framework;
using SAGESharpTests;
using System;
using System.Collections.Generic;
using System.IO;

namespace SAGESharp.SLB.Level.Conversation
{
    class ConversationBinaryAccessorTests
    {
        [TestCase]
        public void Test_Creating_An_Accessor_With_Null_Throws()
        {
            ((Stream)null).Invoking(s => new ConversationBinaryAccessor(s)).Should().Throw<ArgumentNullException>();
        }

        [TestCaseSource(nameof(FileNamesAndConversations))]
        public void Test_Reading_A_File_Successfully(string testFilePath, Func<IList<Character>> expectedProvider)
        {
            var expected = expectedProvider();
            using (var stream = new FileStream(testFilePath, FileMode.Open))
            {
                var result = new ConversationBinaryAccessor(stream).ReadConversation();

                result.Should().BeEquivalentTo(expected);
            }
        }

        [Ignore("Not implemented yet")]
        [TestCaseSource(nameof(FileNamesAndConversations))]
        public void Test_Writing_A_File_Successfully(string expectedOutput, IList<Character> conversation)
        {
            var expected = File.ReadAllBytes(expectedOutput);
            using (var stream = new MemoryStream())
            {
                new ConversationBinaryAccessor(stream)
                    .WriteConversation(conversation as IReadOnlyList<Character> ?? new List<Character>(conversation));

                stream.ToArray().Should().BeEquivalentTo(expected);
            }
        }

        static object[] FileNamesAndConversations() => new ParameterGroup<string, Func<IList<Character>>>()
            .Build();
    }
}
