using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using SAGESharp.Testing;
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
        public void Test_DoOnPosition_With_No_Result()
        {
            stream.Position.Returns(0xA0);

            stream.OnPositionDo(0x1C, () => { stream.ReadByte(); });

            Received.InOrder(() =>
            {
                stream.Position = 0x1C;
                stream.ReadByte();
                stream.Position = 0xA0;
            });
        }

        [Test]
        public void Test_DoOnPosition_With_Result()
        {
            stream.Position.Returns(0xA0);
            stream.ReadByte().Returns(1);

            stream.OnPositionDo(0x1C, () => stream.ReadByte()).Should().Be(1);

            Received.InOrder(() =>
            {
                stream.Position = 0x1C;
                stream.ReadByte();
                stream.Position = 0xA0;
            });
        }

        [Test]
        public void Test_ForceReadByte_Succeeds()
        {
            stream.ReadByte().Returns(1);

            stream.ForceReadByte().Should().Be(1);

            stream.Received().ReadByte();
        }

        [Test]
        public void Test_ForceReadByte_Fails()
        {
            stream.ReadByte().Returns(-1);

            stream.Invoking(s => s.ForceReadByte())
                .Should()
                .Throw<EndOfStreamException>();

            stream.Received().ReadByte();
        }

        [Test]
        public void Test_ForceReadBytes_Succeeds()
        {
            var expected = new byte[] { 0x11, 0x22, 0x33 };

            stream
                .Read(Arg.Do<byte[]>(buffer => expected.CopyTo(buffer, 0)), 0, expected.Length)
                .Returns(expected.Length);

            stream.ForceReadBytes(expected.Length).Should().Equal(expected);

            stream.Received().Read(Arg.Any<byte[]>(), 0, expected.Length);
        }

        [Test]
        public void Test_ForceReadBytes_Fails()
        {
            var count = 3;

            stream.Read(Arg.Any<byte[]>(), 0, count).Returns(0);

            stream.Invoking(s => s.ForceReadBytes(count))
                .Should()
                .Throw<EndOfStreamException>();

            stream.Received().Read(Arg.Any<byte[]>(), 0, count);
        }

        [Test]
        public void Test_ForceReadInt_Succeeds()
        {
            stream.ReadByte().Returns(_ => 0x11, _ => 0x22, _ => 0x33, _ => 0x44);

            stream.ForceReadInt().Should().Be(0x44332211);

            stream.Received(4).ReadByte();
        }

        [Test]
        public void Test_ForceReadInt_Fails()
        {
            stream.ReadByte().Returns(-1);

            stream.Invoking(s => s.ForceReadInt())
                .Should()
                .Throw<EndOfStreamException>();

            stream.Received().ReadByte();
        }

        [Test]
        public void Test_ForceReadUInt_Succeeds()
        {
            stream.ReadByte().Returns(_ => 0xAA, _ => 0xBB, _ => 0xCC, _ => 0xDD);

            stream.ForceReadUInt().Should().Be(0xDDCCBBAA);

            stream.Received(4).ReadByte();
        }

        [Test]
        public void Test_ForceReadUInt_Fails()
        {
            stream.ReadByte().Returns(-1);

            stream.Invoking(s => s.ForceReadUInt())
                .Should()
                .Throw<EndOfStreamException>();

            stream.Received().ReadByte();
        }

        [Test]
        public void Test_WriteInt()
        {
            stream.WriteInt(0x44332211);

            stream.Received().Write(Matcher.ForEquivalentArray(new byte[] { 0x11, 0x22, 0x33, 0x44 }), 0, 4);
        }

        [Test]
        public void Test_WriteUInt()
        {
            stream.WriteUInt(0xDDCCBBAA);

            stream.Received().Write(Matcher.ForEquivalentArray(new byte[] { 0xAA, 0xBB, 0xCC, 0xDD }), 0, 4);
        }
    }
}
