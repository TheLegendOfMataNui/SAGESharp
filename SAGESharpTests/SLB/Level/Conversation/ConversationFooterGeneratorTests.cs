using FluentAssertions;
using NUnit.Framework;
using SAGESharp.Testing;
using System;
using System.Collections.Generic;

namespace SAGESharp.SLB.Level.Conversation
{
    class ConversationFooterGeneratorTests
    {
        private readonly ISLBFooterGenerator<IList<Character>> footerGenerator = new ConversationFooterGenerator();

        [Test]
        public void Test_Generating_The_Footer_For_A_Null_Conversation()
            => footerGenerator.Invoking(fg => fg.GenerateFooter(null)).Should().Throw<ArgumentNullException>();

        [TestCaseSource(nameof(ConversationsWithFooter))]
        public void Test_Generating_The_Footer_For_A_Conversation(IList<Character> conversation, IList<FooterEntry> footerTable)
            => footerGenerator.GenerateFooter(conversation).Should().Equal(footerTable);

        static object[] ConversationsWithFooter() => new ParameterGroup<IList<Character>, IList<FooterEntry>>()
            .Parameters(TestData.EmptyConversation(), new List<FooterEntry>
            {
                new FooterEntry { OffsetPosition = 0x00000004, Offset = 0x00000008 }
            })
            .Parameters(TestData.SimpleConversation(), new List<FooterEntry>
            {
                new FooterEntry { OffsetPosition = 0x00000004, Offset = 0x00000008 },
                new FooterEntry { OffsetPosition = 0x00000018, Offset = 0x0000001C },
                new FooterEntry { OffsetPosition = 0x00000034, Offset = 0x00000038 },
                new FooterEntry { OffsetPosition = 0x0000004C, Offset = 0x00000050 }
            })
            .Parameters(TestData.ComplexConversation(), new List<FooterEntry>
            {
                new FooterEntry { OffsetPosition = 0x00000004, Offset = 0x00000008 },
                new FooterEntry { OffsetPosition = 0x00000018, Offset = 0x00000044 },
                new FooterEntry { OffsetPosition = 0x0000002C, Offset = 0x00000098 },
                new FooterEntry { OffsetPosition = 0x00000040, Offset = 0x000000D0 },
                new FooterEntry { OffsetPosition = 0x0000005C, Offset = 0x000000EC },
                new FooterEntry { OffsetPosition = 0x00000078, Offset = 0x00000104 },
                new FooterEntry { OffsetPosition = 0x00000094, Offset = 0x00000134 },
                new FooterEntry { OffsetPosition = 0x000000B0, Offset = 0x0000017C },
                new FooterEntry { OffsetPosition = 0x000000CC, Offset = 0x00000194 },
                new FooterEntry { OffsetPosition = 0x000000E8, Offset = 0x000001AC },
                new FooterEntry { OffsetPosition = 0x00000100, Offset = 0x000001F4 },
                new FooterEntry { OffsetPosition = 0x00000118, Offset = 0x000001FD },
                new FooterEntry { OffsetPosition = 0x00000130, Offset = 0x00000206 },
                new FooterEntry { OffsetPosition = 0x00000148, Offset = 0x0000020F },
                new FooterEntry { OffsetPosition = 0x00000160, Offset = 0x00000218 },
                new FooterEntry { OffsetPosition = 0x00000178, Offset = 0x00000221 },
                new FooterEntry { OffsetPosition = 0x00000190, Offset = 0x0000022A },
                new FooterEntry { OffsetPosition = 0x000001A8, Offset = 0x00000233 },
                new FooterEntry { OffsetPosition = 0x000001C0, Offset = 0x0000023C },
                new FooterEntry { OffsetPosition = 0x000001D8, Offset = 0x00000245 },
                new FooterEntry { OffsetPosition = 0x000001F0, Offset = 0x0000024E }
            })
            .Build();
    }
}
