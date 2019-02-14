﻿using FluentAssertions;
using Moq;
using NSubstitute;
using NUnit.Framework;
using SAGESharp.SLB;
using System.IO;

namespace SAGESharp.SLB
{
    class StreamExtensionsTessts
    {
        private readonly Stream stream = Substitute.For<Stream>();

        [SetUp]
        public void Setup()
        {
            stream.ClearReceivedCalls();
        }

        [Test]
        public void Test_ForceReadBytes_Succeed()
        {
            var expected = new byte[] { 0x11, 0x22, 0x33 };

            stream
                .Read(Arg.Do<byte[]>(buffer => expected.CopyTo(buffer, 0)), 0, expected.Length)
                .Returns(expected.Length);

            stream.ForceReadBytes(expected.Length).Should().Equal(expected);
        }

        [Test]
        public void Test_ForceReadBytes_Fails()
        {
            var count = 3;

            stream.Read(Arg.Any<byte[]>(), 0, count).Returns(0);

            stream.Invoking(s => s.ForceReadBytes(count))
                .Should()
                .Throw<EndOfStreamException>();
        }
    }
}

namespace SAGESharpTests.SLB
{
    [TestFixture]
    public class StreamExtensionsTests
    {
        [Test]
        public void TestDoOnPosition()
        {
            var streamMock = new Mock<Stream>();

            streamMock
                .Setup(stream => stream.Position)
                .Returns(100);

            streamMock.Object.OnPositionDo(50, () => { });

            streamMock.VerifyGet(stream => stream.Position, Times.Once);
            streamMock.VerifySet(stream => stream.Position = 50, Times.Once);
            streamMock.VerifySet(stream => stream.Position = 100, Times.Once);
            streamMock.VerifyNoOtherCalls();
        }

        [Test]
        public void TestDoOnPositionWithResult()
        {
            var streamMock = new Mock<Stream>();

            streamMock
                .Setup(stream => stream.Position)
                .Returns(100);

            var result = streamMock.Object.OnPositionDo(50, () => "ABCD");

            Assert.That(result, Is.EqualTo("ABCD"));

            streamMock.VerifyGet(stream => stream.Position, Times.Once);
            streamMock.VerifySet(stream => stream.Position = 50, Times.Once);
            streamMock.VerifySet(stream => stream.Position = 100, Times.Once);
            streamMock.VerifyNoOtherCalls();
        }

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

        [Test]
        public void TestForceReadUIntSucceeds()
        {
            var streamMock = new Mock<Stream>();

            streamMock
                .SetupSequence(stream => stream.ReadByte())
                .Returns(0xAA)
                .Returns(0xBB)
                .Returns(0xCC)
                .Returns(0xDD);

            Assert.That(streamMock.Object.ForceReadUInt(), Is.EqualTo(0xDDCCBBAA));

            streamMock.Verify(stream => stream.ReadByte(), Times.Exactly(4));
            streamMock.VerifyNoOtherCalls();
        }

        [Test]
        public void TestForceReadUIntFails()
        {
            var streamMock = new Mock<Stream>();

            streamMock
                .SetupSequence(stream => stream.ReadByte())
                .Returns(0xAA)
                .Returns(-1);

            Assert.That(() => streamMock.Object.ForceReadUInt(), Throws.InstanceOf(typeof(EndOfStreamException)));

            streamMock.Verify(stream => stream.ReadByte(), Times.Exactly(2));
            streamMock.VerifyNoOtherCalls();
        }

        [Test]
        public void TestWriteInt()
        {
            var streamMock = new Mock<Stream>();

            streamMock.Object.WriteInt(0x11223344);

            streamMock.Verify(stream => stream.Write(new byte[] { 0x44, 0x33, 0x22, 0x11 }, 0, 4));
            streamMock.VerifyNoOtherCalls();
        }

        [Test]
        public void TestWriteUInt()
        {
            var streamMock = new Mock<Stream>();

            streamMock.Object.WriteUInt(0x11223344);

            streamMock.Verify(stream => stream.Write(new byte[] { 0x44, 0x33, 0x22, 0x11 }, 0, 4));
            streamMock.VerifyNoOtherCalls();
        }
    }
}
