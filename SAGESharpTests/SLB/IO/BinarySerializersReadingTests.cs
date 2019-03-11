using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace SAGESharp.SLB.IO
{
    class BinarySerializersReadingTests
    {
        private readonly IBinaryReader reader = Substitute.For<IBinaryReader>();

        [SetUp]
        public void Setup()
        {
            reader.ClearReceivedCalls();
        }

        [TestCase]
        public void Test_Reading_A_String()
        {
            var expected = "hello world";

            reader.ReadUInt32().Returns((uint)30);
            reader.Position.Returns(50);
            reader.ReadByte().Returns((byte)expected.Length);
            reader.ReadBytes(expected.Length).Returns(new byte[]
            {
                // "hello"
                0x68, 0x65, 0x6C, 0x6C, 0x6F,
                // " "
                0x20,
                // "world"
                0x77, 0x6F, 0x72, 0x6C, 0x64,
            });

            new StringBinarySerializer()
                .Read(reader)
                .Should()
                .BeOfType<string>()
                .Which
                .Should()
                .Be(expected);

            Received.InOrder(() =>
            {
                reader.ReadUInt32();
                reader.Position = 30;
                reader.ReadByte();
                reader.ReadBytes(expected.Length);
                reader.Position = 50;
            });
        }
    }
}
