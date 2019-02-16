using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace SAGESharp.SLB.Level.Conversation
{
    class InfoBinaryReaderTests
    {
        private readonly Stream stream = Substitute.For<Stream>();

        private readonly ISLBBinaryReader<Frame> frameReader = Substitute.For<ISLBBinaryReader<Frame>>();

        private readonly ISLBBinaryReader<Info> reader;

        public InfoBinaryReaderTests()
        {
            reader = new InfoBinaryReader(stream, frameReader);
        }

        [SetUp]
        public void Setup()
        {
            stream.ClearReceivedCalls();
            frameReader.ClearReceivedCalls();
        }

        [Test]
        public void Test_Creating_An_InfoBinaryReader_With_Null_Dependency()
        {
            this.Invoking(_ => new InfoBinaryReader(null, frameReader)).Should().Throw<ArgumentNullException>();
            this.Invoking(_ => new InfoBinaryReader(stream, null)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Test_Reading_An_Info_Object_With_Frames()
        {
            var expected = new byte[]
            {
                0x02, 0x00, 0x00, 0x00,
                0x04, 0x03, 0x02, 0x01,
                0x14, 0x13, 0x12, 0x11,
                0x24, 0x23, 0x22, 0x21,
                0x34, 0x33, 0x32, 0x31,
                0x02, 0x00, 0x00, 0x00,
                0x1C, 0x00, 0x00, 0x00
            };

            stream
                .Read(Arg.Do<byte[]>(bytes => expected.CopyTo(bytes, 0)), 0, Info.BINARY_SIZE)
                .Returns(Info.BINARY_SIZE);

            stream.Position.Returns(0xA0);

            frameReader.ReadSLBObject().Returns(_ => new Frame());

            reader.ReadSLBObject().Should().Be(new Info
            {
                LineSide = LineSide.Left,
                ConditionStart = 0x01020304,
                ConditionEnd = 0x11121314,
                StringLabel = 0x21222324,
                StringIndex = 0x31323334,
                Frames = new List<Frame> { new Frame(), new Frame() }
            });

            Received.InOrder(() =>
            {
                stream.Position = 0x1C;
                frameReader.ReadSLBObject();
                frameReader.ReadSLBObject();
                stream.Position = 0xA0;
            });
        }

        [Test]
        public void Test_Reading_An_Info_Object_Without_Frames()
        {
            var expected = new byte[]
            {
                0x02, 0x00, 0x00, 0x00,
                0x04, 0x03, 0x02, 0x01,
                0x14, 0x13, 0x12, 0x11,
                0x24, 0x23, 0x22, 0x21,
                0x34, 0x33, 0x32, 0x31,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00
            };

            stream
                .Read(Arg.Do<byte[]>(bytes => expected.CopyTo(bytes, 0)), 0, Info.BINARY_SIZE)
                .Returns(Info.BINARY_SIZE);

            reader.ReadSLBObject().Should().Be(new Info
            {
                LineSide = LineSide.Left,
                ConditionStart = 0x01020304,
                ConditionEnd = 0x11121314,
                StringLabel = 0x21222324,
                StringIndex = 0x31323334,
                Frames = new List<Frame> { }
            });

            stream.DidNotReceive().Position = Arg.Any<long>();
            frameReader.DidNotReceive().ReadSLBObject();
        }
    }
}
