using Moq;
using NUnit.Framework;
using SAGESharp.Extensions;
using System.IO;

namespace SAGESharpTests.Extensions
{
    [TestFixture]
    public class StreamExtensionsTests
    {
        [Test]
        public void TestForceReadByteSucceeds()
        {
            var streamMock = new Mock<Stream>();

            streamMock
                .Setup(stream => stream.ReadByte())
                .Returns(0xAA);

            Assert.That(streamMock.Object.ForceReadByte(), Is.EqualTo(0xAA));

            streamMock.Verify(stream => stream.ReadByte(), Times.Exactly(1));
            streamMock.VerifyNoOtherCalls();
        }

        [Test]
        public void TestForceReadByteFails()
        {
            var streamMock = new Mock<Stream>();

            streamMock
                .Setup(stream => stream.ReadByte())
                .Returns(-1);

            Assert.That(() => streamMock.Object.ForceReadByte(), Throws.InstanceOf(typeof(EndOfStreamException)));

            streamMock.Verify(stream => stream.ReadByte(), Times.Exactly(1));
            streamMock.VerifyNoOtherCalls();
        }

        [Test]
        public void TestForceReadIntSucceeds()
        {
            var streamMock = new Mock<Stream>();

            streamMock
                .SetupSequence(stream => stream.ReadByte())
                .Returns(0x44)
                .Returns(0x33)
                .Returns(0x22)
                .Returns(0x11);

            Assert.That(streamMock.Object.ForceReadInt(), Is.EqualTo(0x11223344));

            streamMock.Verify(stream => stream.ReadByte(), Times.Exactly(4));
            streamMock.VerifyNoOtherCalls();
        }

        [Test]
        public void TestForceReadIntFails()
        {
            var streamMock = new Mock<Stream>();

            streamMock
                .SetupSequence(stream => stream.ReadByte())
                .Returns(0xAA)
                .Returns(-1);

            Assert.That(() => streamMock.Object.ForceReadInt(), Throws.InstanceOf(typeof(EndOfStreamException)));

            streamMock.Verify(stream => stream.ReadByte(), Times.Exactly(2));
            streamMock.VerifyNoOtherCalls();
        }
    }
}
