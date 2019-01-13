using Moq;
using NUnit.Framework;
using SAGESharp.Slb.Level.Conversation;
using System.IO;

namespace SAGESharpTests.Slb.Level.Conversation
{
    [TestFixture]
    public class FrameTests
    {
        [Test]
        public void TestReadingAFrameFromAStream()
        {
            var frame = new Frame();
            var streamMock = new Mock<Stream>();

            streamMock
                .SetupSequence(stream => stream.ReadByte())
                // ToaAnimation (int)
                .Returns(0x44)
                .Returns(0x33)
                .Returns(0x22)
                .Returns(0x11)
                // CharAnimation (int)
                .Returns(0x55)
                .Returns(0x33)
                .Returns(0x22)
                .Returns(0x11)
                // CameraPositionTarget (int)
                .Returns(0x66)
                .Returns(0x33)
                .Returns(0x22)
                .Returns(0x11)
                // CameraDistance (int)
                .Returns(0x77)
                .Returns(0x33)
                .Returns(0x22)
                .Returns(0x11)
                // StringIndex (int)
                .Returns(0x88)
                .Returns(0x33)
                .Returns(0x22)
                .Returns(0x11)
                // Offset (int, ignored)
                .Returns(0x00)
                .Returns(0x00)
                .Returns(0x00)
                .Returns(0x00)
                // ConversationSounds (string)
                .Returns(0x05) // Size
                .Returns(0x41) // 'A'
                .Returns(0x42) // 'B'
                .Returns(0x43) // 'C'
                .Returns(0x44) // 'D'
                .Returns(0x45) // 'E'
                .Returns(0x00) // Null character
                ;

            frame.ReadFrom(streamMock.Object);

            // It should read 6 integers (4 bytes each) + string size + 5 characters + null terminator
            streamMock.Verify(stream => stream.ReadByte(), Times.Exactly((6 * 4) + 7));

            Assert.That(frame.ToaAnimation, Is.EqualTo(0x11223344));
            Assert.That(frame.CharAnimation, Is.EqualTo(0x11223355));
            Assert.That(frame.CameraPositionTarget, Is.EqualTo(0x11223366));
            Assert.That(frame.CameraDistance, Is.EqualTo(0x11223377));
            Assert.That(frame.StringIndex, Is.EqualTo(0x11223388));
            Assert.That(frame.ConversationSounds, Is.EqualTo("ABCDE"));
        }
    }
}
