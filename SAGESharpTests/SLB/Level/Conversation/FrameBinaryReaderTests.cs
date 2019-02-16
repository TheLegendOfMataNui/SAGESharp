using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.IO;

namespace SAGESharp.SLB.Level.Conversation
{
    class FrameBinaryReaderTests
    {
        private readonly Stream stream = Substitute.For<Stream>();

        private readonly ISLBBinaryReader<string> stringReader = Substitute.For<ISLBBinaryReader<string>>();

        private readonly ISLBBinaryReader<Frame> reader;

        public FrameBinaryReaderTests()
        {
            reader = new FrameBinaryReader(stream, stringReader);
        }

        [SetUp]
        public void Setup()
        {
            stream.ClearReceivedCalls();
        }

        [Test]
        public void Test_Creating_A_FrameBinaryReader_With_Null_Dependencies()
        {
            this.Invoking(_ => new FrameBinaryReader(null, stringReader))
                .Should()
                .Throw<ArgumentNullException>();

            this.Invoking(_ => new FrameBinaryReader(stream, null))
                .Should()
                .Throw<ArgumentNullException>();
        }

        [Test]
        public void Test_Reading_A_Frame()
        {
            var expected = new byte[]
            {
                0x04, 0x03, 0x02, 0x01,
                0x14, 0x13, 0x12, 0x11,
                0x24, 0x23, 0x22, 0x21,
                0x34, 0x33, 0x32, 0x31,
                0x44, 0x43, 0x42, 0x41,
                0x1C, 0x00, 0x00, 0x00
            };

            stream
                .Read(Arg.Do<byte[]>(bytes => expected.CopyTo(bytes, 0)), 0, Frame.BINARY_SIZE)
                .Returns(Frame.BINARY_SIZE);

            stream.Position.Returns(0xA0);

            stringReader.ReadSLBObject().Returns("SOUNDS1");

            reader.ReadSLBObject().Should().Be(new Frame
            {
                ToaAnimation = 0x01020304,
                CharAnimation = 0x11121314,
                CameraPositionTarget = 0x21222324,
                CameraDistance = 0x31323334,
                StringIndex = 0x41424344,
                ConversationSounds = "SOUNDS1"
            });

            Received.InOrder(() =>
            {
                stream.Read(Arg.Any<byte[]>(), 0, Frame.BINARY_SIZE);
                stream.Position = 0x1C;
                stringReader.ReadSLBObject();
                stream.Position = 0xA0;
            });
        }
    }
}
