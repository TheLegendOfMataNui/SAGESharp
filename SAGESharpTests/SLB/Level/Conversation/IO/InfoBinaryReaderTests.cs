using Moq;
using NUnit.Framework;
using SAGESharp.SLB;
using SAGESharp.SLB.IO;
using SAGESharp.SLB.Level.Conversation;
using SAGESharp.SLB.Level.Conversation.IO;
using SAGESharpTests.Util;
using System;
using System.IO;

namespace SAGESharpTests.SLB.Level.Conversation.IO
{
    [TestFixture]
    public static class InfoBinaryReaderTests
    {
        [Test]
        public static void TestInfoBinaryReaderConstructor()
        {
            var stream = new Mock<Stream>().Object;
            var identifierReader = new Mock<ISlbReader<Identifier>>().Object;
            var frameReader = new Mock<ISlbReader<Frame>>().Object;

            Assert.That(() => new InfoBinaryReader(null, identifierReader, frameReader), Throws.InstanceOf<ArgumentNullException>());
            Assert.That(() => new InfoBinaryReader(stream, null, frameReader), Throws.InstanceOf<ArgumentNullException>());
            Assert.That(() => new InfoBinaryReader(stream, identifierReader, null), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public static void TestReadInfoSlb()
        {
            var streamMock = new Mock<Stream>();
            var identifierReaderMock = new Mock<ISlbReader<Identifier>>();
            var frameReaderMock = new Mock<ISlbReader<Frame>>();

            var reader = new InfoBinaryReader(streamMock.Object, identifierReaderMock.Object, frameReaderMock.Object);

            var stringLabel = new Identifier(0x11223300);

            identifierReaderMock
                .SetupSequence(identifierReader => identifierReader.ReadSlbObject())
                .Returns(stringLabel);

            streamMock
                .SetupSequence(stream => stream.ReadByte())
                .ReturnsIntBytes(0x11223344) // Line side
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
                .SetupSequence(infoReader => infoReader.ReadSlbObject())
                .Returns(frame1)
                .Returns(frame2);

            var info = reader.ReadSlbObject();

            Assert.AreEqual(info.LineSide, 0x11223344);
            Assert.AreEqual(info.ConditionStart, 0x11223355);
            Assert.AreEqual(info.ConditionEnd, 0x11223366);
            Assert.AreEqual(info.StringLabel, stringLabel);
            Assert.AreEqual(info.StringIndex, 0x11223377);
            Assert.AreEqual(info.Frames.Count, 2);
            Assert.IsTrue(info.Frames.Contains(frame1));
            Assert.IsTrue(info.Frames.Contains(frame2));

            streamMock.Verify(stream => stream.ReadByte(), Times.Exactly(24));
            streamMock.VerifyGet(stream => stream.Position, Times.Once);
            streamMock.VerifySet(stream => stream.Position = 0x44, Times.Once);
            streamMock.VerifySet(stream => stream.Position = 0x20, Times.Once);
            streamMock.VerifyNoOtherCalls();

            identifierReaderMock.Verify(identifierReader => identifierReader.ReadSlbObject(), Times.Once);
            identifierReaderMock.VerifyNoOtherCalls();

            frameReaderMock.Verify(frameReader => frameReader.ReadSlbObject(), Times.Exactly(2));
            frameReaderMock.VerifyNoOtherCalls();
        }



        [Test]
        public static void TestReadCharacterSlbWithNoInfo()
        {
            var streamMock = new Mock<Stream>();
            var identifierReaderMock = new Mock<ISlbReader<Identifier>>();
            var frameReaderMock = new Mock<ISlbReader<Frame>>();

            var reader = new InfoBinaryReader(streamMock.Object, identifierReaderMock.Object, frameReaderMock.Object);

            var stringLabel = new Identifier(0x11223300);

            identifierReaderMock
                .SetupSequence(identifierReader => identifierReader.ReadSlbObject())
                .Returns(stringLabel);

            streamMock
                .SetupSequence(stream => stream.ReadByte())
                .ReturnsIntBytes(0x11223344) // Line side
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
                .SetupSequence(infoReader => infoReader.ReadSlbObject())
                .Returns(frame1)
                .Returns(frame2);

            var info = reader.ReadSlbObject();

            Assert.AreEqual(info.LineSide, 0x11223344);
            Assert.AreEqual(info.ConditionStart, 0x11223355);
            Assert.AreEqual(info.ConditionEnd, 0x11223366);
            Assert.AreEqual(info.StringLabel, stringLabel);
            Assert.AreEqual(info.StringIndex, 0x11223377);
            Assert.AreEqual(info.Frames.Count, 0);

            streamMock.Verify(stream => stream.ReadByte(), Times.Exactly(20));
            streamMock.VerifyNoOtherCalls();

            identifierReaderMock.Verify(identifierReader => identifierReader.ReadSlbObject(), Times.Once);
            identifierReaderMock.VerifyNoOtherCalls();

            frameReaderMock.VerifyNoOtherCalls();
        }
    }
}
