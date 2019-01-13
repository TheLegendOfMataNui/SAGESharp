using Moq;
using NUnit.Framework;
using SAGESharp.SLB.Level.Conversation.IO;
using SAGESharpTests.Util;
using System;
using System.IO;

namespace SAGESharpTests.SLB.Level.Conversation.IO
{
    [TestFixture]
    public static class FrameBinaryReaderTests
    {
        [Test]
        public static void TestFrameBinaryReaderConstructor()
        {
            var stream = new Mock<Stream>().Object;

            Assert.That(() => new FrameBinaryReader(null), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public static void TestReadFrameSlb()
        {
            var streamMock = new Mock<Stream>();

            var reader = new FrameBinaryReader(streamMock.Object);

            var toaAnimation = 0x11223344;
            var charAnimation = 0x11223355;
            var cameraPositionTarget = 0x11223366;
            var cameraDistance = 0x11223377;
            var stringIndex = 0x11223388;
            var conversationSounds = "ABCDE";

            streamMock
                .SetupSequence(stream => stream.ReadByte())
                .ReturnsIntBytes(toaAnimation)
                .ReturnsIntBytes(charAnimation)
                .ReturnsIntBytes(cameraPositionTarget)
                .ReturnsIntBytes(cameraDistance)
                .ReturnsIntBytes(stringIndex)
                // Conversation sounds position
                .ReturnsIntBytes(0x44)
                // Conversation sounds size
                .Returns(conversationSounds.Length)
                .ReturnsASCIIBytes(conversationSounds);

            streamMock
                .Setup(stream => stream.Position)
                .Returns(0x20);

            var frame = reader.ReadSlbObject();

            Assert.AreEqual(frame.ToaAnimation, toaAnimation);
            Assert.AreEqual(frame.CharAnimation, charAnimation);
            Assert.AreEqual(frame.CameraPositionTarget, cameraPositionTarget);
            Assert.AreEqual(frame.CameraDistance, cameraDistance);
            Assert.AreEqual(frame.StringIndex, stringIndex);
            Assert.AreEqual(frame.ConversationSounds, conversationSounds);

            // 6 integers (4 bytes each) + length (1 byte) + length + null character (1 byte)
            streamMock.Verify(stream => stream.ReadByte(), Times.Exactly(26 + conversationSounds.Length));
            streamMock.VerifyGet(stream => stream.Position, Times.Once);
            streamMock.VerifySet(stream => stream.Position = 0x44, Times.Once);
            streamMock.VerifySet(stream => stream.Position = 0x20, Times.Once);
            streamMock.VerifyNoOtherCalls();
        }

        [Test]
        public static void TestReadCharacterSlbWithNoConversationSounds()
        {
            var streamMock = new Mock<Stream>();

            var reader = new FrameBinaryReader(streamMock.Object);

            var toaAnimation = 0x11223344;
            var charAnimation = 0x11223355;
            var cameraPositionTarget = 0x11223366;
            var cameraDistance = 0x11223377;
            var stringIndex = 0x11223388;

            streamMock
                .SetupSequence(stream => stream.ReadByte())
                .ReturnsIntBytes(toaAnimation)
                .ReturnsIntBytes(charAnimation)
                .ReturnsIntBytes(cameraPositionTarget)
                .ReturnsIntBytes(cameraDistance)
                .ReturnsIntBytes(stringIndex)
                // Conversation sounds position
                .ReturnsIntBytes(0x44)
                // Conversation sounds size
                .Returns(0);

            streamMock
                .Setup(stream => stream.Position)
                .Returns(0x20);

            var frame = reader.ReadSlbObject();

            Assert.AreEqual(frame.ToaAnimation, toaAnimation);
            Assert.AreEqual(frame.CharAnimation, charAnimation);
            Assert.AreEqual(frame.CameraPositionTarget, cameraPositionTarget);
            Assert.AreEqual(frame.CameraDistance, cameraDistance);
            Assert.AreEqual(frame.StringIndex, stringIndex);
            Assert.AreEqual(frame.ConversationSounds, string.Empty);

            // 6 integers (4 bytes each) + length (1 byte) + null character (1 byte)
            streamMock.Verify(stream => stream.ReadByte(), Times.Exactly(26));
            streamMock.VerifyGet(stream => stream.Position, Times.Once);
            streamMock.VerifySet(stream => stream.Position = 0x44, Times.Once);
            streamMock.VerifySet(stream => stream.Position = 0x20, Times.Once);
            streamMock.VerifyNoOtherCalls();
        }
    }
}
