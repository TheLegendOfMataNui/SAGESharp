using Moq;
using NUnit.Framework;
using SAGESharp.SLB;
using SAGESharp.SLB.Level.Conversation;
using SAGESharp.Testing;
using System;
using System.IO;

namespace SAGESharpTests.SLB.Level.Conversation
{
    [TestFixture]
    public static class InfoBinaryReaderTests
    {
        [Test]
        public static void TestInfoBinaryReaderConstructor()
        {
            var stream = new Mock<Stream>().Object;
            var identifierReader = new Mock<ISLBBinaryReader<Identifier>>().Object;
            var frameReader = new Mock<ISLBBinaryReader<Frame>>().Object;

            Assert.That(() => new InfoBinaryReader(null, identifierReader, frameReader), Throws.InstanceOf<ArgumentNullException>());
            Assert.That(() => new InfoBinaryReader(stream, null, frameReader), Throws.InstanceOf<ArgumentNullException>());
            Assert.That(() => new InfoBinaryReader(stream, identifierReader, null), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public static void TestReadInfoSlb()
        {
            var streamMock = new Mock<Stream>();
            var identifierReaderMock = new Mock<ISLBBinaryReader<Identifier>>();
            var frameReaderMock = new Mock<ISLBBinaryReader<Frame>>();

            var reader = new InfoBinaryReader(streamMock.Object, identifierReaderMock.Object, frameReaderMock.Object);

            var stringLabel = (Identifier)0x11223300;

            identifierReaderMock
                .SetupSequence(identifierReader => identifierReader.ReadSLBObject())
                .Returns(stringLabel);

            streamMock
                .SetupSequence(stream => stream.ReadByte())
                .ReturnsIntBytes((int)LineSide.Right) // Line side
                .ReturnsIntBytes(0x11223355) // Condition start
                .ReturnsIntBytes(0x11223366) // Condition end
                .ReturnsIntBytes(0x11223377) // String index
                .ReturnsIntBytes(0x02) // Frame count
                .ReturnsIntBytes(0x44); // Frame position

            streamMock
                .Setup(stream => stream.Position)
                .Returns(0x20);

            var frame1 = new Frame();
            var frame2 = new Frame();
            frameReaderMock
                .SetupSequence(infoReader => infoReader.ReadSLBObject())
                .Returns(frame1)
                .Returns(frame2);

            var info = reader.ReadSLBObject();

            Assert.That(info.LineSide, Is.EqualTo(LineSide.Right));
            Assert.That(info.ConditionStart, Is.EqualTo(0x11223355));
            Assert.That(info.ConditionEnd, Is.EqualTo(0x11223366));
            Assert.That(info.StringLabel, Is.EqualTo(stringLabel));
            Assert.That(info.StringIndex, Is.EqualTo(0x11223377));
            Assert.That(info.Frames.Count, Is.EqualTo(2));
            Assert.That(info.Frames, Contains.Item(frame1));
            Assert.That(info.Frames, Contains.Item(frame2));

            streamMock.Verify(stream => stream.ReadByte(), Times.Exactly(24));
            streamMock.VerifyGet(stream => stream.Position, Times.Once);
            streamMock.VerifySet(stream => stream.Position = 0x44, Times.Once);
            streamMock.VerifySet(stream => stream.Position = 0x20, Times.Once);
            streamMock.VerifyNoOtherCalls();

            identifierReaderMock.Verify(identifierReader => identifierReader.ReadSLBObject(), Times.Once);
            identifierReaderMock.VerifyNoOtherCalls();

            frameReaderMock.Verify(frameReader => frameReader.ReadSLBObject(), Times.Exactly(2));
            frameReaderMock.VerifyNoOtherCalls();
        }

        [Test]
        public static void TestReadCharacterSlbWithNoInfo()
        {
            var streamMock = new Mock<Stream>();
            var identifierReaderMock = new Mock<ISLBBinaryReader<Identifier>>();
            var frameReaderMock = new Mock<ISLBBinaryReader<Frame>>();

            var reader = new InfoBinaryReader(streamMock.Object, identifierReaderMock.Object, frameReaderMock.Object);

            var stringLabel = (Identifier)0x11223300;

            identifierReaderMock
                .SetupSequence(identifierReader => identifierReader.ReadSLBObject())
                .Returns(stringLabel);

            streamMock
                .SetupSequence(stream => stream.ReadByte())
                .ReturnsIntBytes((int)LineSide.Right) // Line side
                .ReturnsIntBytes(0x11223355) // Condition start
                .ReturnsIntBytes(0x11223366) // Condition end
                .ReturnsIntBytes(0x11223377) // String index
                .ReturnsIntBytes(0x00); // Frame count

            streamMock
                .Setup(stream => stream.Position)
                .Returns(0x20);

            var frame1 = new Frame();
            var frame2 = new Frame();
            frameReaderMock
                .SetupSequence(infoReader => infoReader.ReadSLBObject())
                .Returns(frame1)
                .Returns(frame2);

            var info = reader.ReadSLBObject();

            Assert.That(info.LineSide, Is.EqualTo(LineSide.Right));
            Assert.That(info.ConditionStart, Is.EqualTo(0x11223355));
            Assert.That(info.ConditionEnd, Is.EqualTo(0x11223366));
            Assert.That(info.StringLabel, Is.EqualTo(stringLabel));
            Assert.That(info.StringIndex, Is.EqualTo(0x11223377));
            Assert.That(info.Frames, Is.Empty);

            streamMock.Verify(stream => stream.ReadByte(), Times.Exactly(20));
            streamMock.VerifyNoOtherCalls();

            identifierReaderMock.Verify(identifierReader => identifierReader.ReadSLBObject(), Times.Once);
            identifierReaderMock.VerifyNoOtherCalls();

            frameReaderMock.VerifyNoOtherCalls();
        }
    }
}
