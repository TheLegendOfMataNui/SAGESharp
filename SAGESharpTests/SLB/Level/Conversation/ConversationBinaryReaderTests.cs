using Moq;
using NUnit.Framework;
using SAGESharp.SLB;
using SAGESharp.SLB.Level.Conversation;
using SAGESharpTests.Util;
using System;
using System.IO;

namespace SAGESharpTests.SLB.Level.Conversation
{
    public static class ConversationBinaryReaderTests
    {
        [Test]
        public static void TestConversationBinaryReaderConstructor()
        {
            var stream = new Mock<Stream>().Object;
            var characterReader = new Mock<ISLBBinaryReader<Character>>().Object;

            Assert.That(() => new ConversationBinaryReader(null, characterReader), Throws.InstanceOf<ArgumentNullException>());
            Assert.That(() => new ConversationBinaryReader(stream, null), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public static void TestReadCharacterSlb()
        {
            var streamMock = new Mock<Stream>();
            var characterReaderMock = new Mock<ISLBBinaryReader<Character>>();

            var reader = new ConversationBinaryReader(streamMock.Object, characterReaderMock.Object);

            streamMock
                .SetupSequence(stream => stream.ReadByte())
                // Character entry count
                .ReturnsIntBytes(2)
                // Character entries position
                .ReturnsIntBytes(0x44);

            streamMock
                .Setup(stream => stream.Position)
                .Returns(0x20);

            var character1 = new Character();
            var character2 = new Character();
            characterReaderMock
                .SetupSequence(infoReader => infoReader.ReadSLBObject())
                .Returns(character1)
                .Returns(character2);

            var characters = reader.ReadSLBObject();

            Assert.AreEqual(characters.Count, 2);
            Assert.IsTrue(characters.Contains(character1));
            Assert.IsTrue(characters.Contains(character2));

            streamMock.Verify(stream => stream.ReadByte(), Times.Exactly(8));
            streamMock.VerifyGet(stream => stream.Position, Times.Once);
            streamMock.VerifySet(stream => stream.Position = 0x44, Times.Once);
            streamMock.VerifySet(stream => stream.Position = 0x20, Times.Once);
            streamMock.VerifyNoOtherCalls();
        }

        [Test]
        public static void TestReadCharacterSlbWithNoInfo()
        {

            var streamMock = new Mock<Stream>();
            var characterReaderMock = new Mock<ISLBBinaryReader<Character>>();

            var reader = new ConversationBinaryReader(streamMock.Object, characterReaderMock.Object);

            streamMock
                .SetupSequence(stream => stream.ReadByte())
                .ReturnsIntBytes(0);

            var characters = reader.ReadSLBObject();

            Assert.AreEqual(characters.Count, 0);

            streamMock.Verify(stream => stream.ReadByte(), Times.Exactly(4));
            streamMock.VerifyNoOtherCalls(); ;
        }
    }
}
