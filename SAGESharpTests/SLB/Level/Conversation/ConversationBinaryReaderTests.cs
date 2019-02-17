using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace SAGESharp.SLB.Level.Conversation
{
    class ConversationBinaryReaderTests
    {
        private readonly Stream stream = Substitute.For<Stream>();

        private readonly ISLBBinaryReader<Character> characterReader = Substitute.For<ISLBBinaryReader<Character>>();

        private readonly ISLBBinaryReader<IList<Character>> reader;

        public ConversationBinaryReaderTests()
        {
            reader = new ConversationBinaryReader(stream, characterReader);
        }

        [SetUp]
        public void Setup()
        {
            stream.ClearReceivedCalls();
            characterReader.ClearReceivedCalls();
        }

        [Test]
        public void Creating_A_ConversationBinaryReader_With_Null_Dependencies()
        {
            this.Invoking(_ => new ConversationBinaryReader(null, characterReader))
                .Should()
                .Throw<ArgumentNullException>();

            this.Invoking(_ => new ConversationBinaryReader(stream, null))
                .Should()
                .Throw<ArgumentNullException>();
        }

        [Test]
        public void Test_Reading_A_Non_Empty_Conversation()
        {
            var expected = new List<Character>()
            {
                new Character { ToaName = 1 },
                new Character { ToaName = 2 },
                new Character { ToaName = 3 }
            };

            // Amount of entries
            stream.ReadByte().Returns(
                _ => 0x03, _ => 0x00, _ => 0x00, _ => 0x00, // Character count
                _ => 0x1C, _ => 0x00, _ => 0x00, _ => 0x00  // Offset of first character
            );

            stream.Position.Returns(0xA0);

            characterReader.ReadSLBObject().Returns(
                _ => expected[0],
                _ => expected[1],
                _ => expected[2]
            );

            reader.ReadSLBObject().Should().Equal(expected);

            Received.InOrder(() =>
            {
                stream.Position = 0x1C;
                characterReader.ReadSLBObject();
                characterReader.ReadSLBObject();
                characterReader.ReadSLBObject();
                stream.Position = 0xA0;
            });
        }

        [Test]
        public void Test_Reading_An_Empty_Conversation()
        {
            // Three entries located at 0x1C
            stream.ReadByte().Returns(
                _ => 0x00, _ => 0x00, _ => 0x00, _ => 0x00
            );

            reader.ReadSLBObject().Should().BeEmpty();

            stream.DidNotReceive().Position = Arg.Any<long>();
            characterReader.DidNotReceive().ReadSLBObject();
        }

    }
}
