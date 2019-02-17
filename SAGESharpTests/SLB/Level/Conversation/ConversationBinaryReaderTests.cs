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

            // Character count
            var callNo = 0;
            stream.Read(Arg.Do<byte[]>(bytes =>
            {
                // Use 0x03 (the amount of entires) for the first call
                // Use 0x1C (the offset of the entires) for all the other calls
                bytes[0] = (byte)((callNo++ == 0) ? 0x03 : 0x1C);
                bytes[1] = 0x00;
                bytes[2] = 0x00;
                bytes[3] = 0x00;
            }), 0, 4).Returns(04);

            stream.Position.Returns(0xA0);

            characterReader.ReadSLBObject().Returns(expected[0], expected[1], expected[2]);

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
            // Character count
            stream.Read(Arg.Do<byte[]>(bytes =>
            {
                bytes[0] = 0x00;
                bytes[1] = 0x00;
                bytes[2] = 0x00;
                bytes[3] = 0x00;
            }), 0, 4).Returns(04);

            reader.ReadSLBObject().Should().BeEmpty();

            stream.DidNotReceive().Position = Arg.Any<long>();
            characterReader.DidNotReceive().ReadSLBObject();
        }

    }
}
