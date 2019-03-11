using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using SAGESharp.Testing;
using System;
using System.IO;

namespace SAGESharp.SLB.IO
{
    class StreamBinaryReaderTests
    {
        private readonly Stream stream = Substitute.For<Stream>();

        private readonly IBinaryReader reader;

        public StreamBinaryReaderTests()
            => reader = new StreamBinaryReader(stream);

        [SetUp]
        public void Setup()
        {
            stream.ClearReceivedCalls();
        }

        [TestCase]
        public void Test_Create_A_Reader_With_A_Null_Stream() => this
            .Invoking(_ => new StreamBinaryReader(null))
            .Should()
            .Throw<ArgumentNullException>();

        [TestCase]
        public void Test_Getting_The_Position()
        {
            stream.Position.Returns(50);

            reader.Position.Should().Be(50);
        }

        [TestCase]
        public void Test_Setting_The_Position()
        {
            reader.Position = 30;

            stream.Received().Position = 30;
        }

        [TestCaseSource(nameof(TEST_CASES_DATA))]
        public void Test_Reading_Successfully<T>(TestCaseData<T> testData)
        {
            stream
                .Read(Arg.Do<byte[]>(testData.SetBytes), 0, testData.BytesToRead)
                .Returns(testData.BytesToRead);

            testData
                .Function(reader)
                .Should()
                .Be(testData.ExpectedResult);

            stream.Received().Read(Arg.Any<byte[]>(), 0, testData.BytesToRead);
        }

        [TestCase]
        public void Test_Reading_A_Byte_Array()
        {
            var expected = new byte[] { 0x01, 0x02, 0x03 };

            stream
                .Read(Arg.Do<byte[]>(bs => expected.CopyTo(bs, 0)), 0, expected.Length)
                .Returns(expected.Length);

            reader
                .ReadBytes(expected.Length)
                .Should()
                .Equal(expected);

            stream
                .Received()
                .Read(Arg.Any<byte[]>(), 0, expected.Length);
        }

        [TestCase]
        public void Test_Reading_An_Empty_Byte_Array()
        {
            reader
                .ReadBytes(0)
                .Should()
                .BeEmpty();

            stream.DidNotReceiveWithAnyArgs().Read(null, 0, 0);
        }

        [TestCaseSource(nameof(TEST_CASES_DATA))]
        public void Test_Reading_After_EOF<T>(TestCaseData<T> testData)
        {
            stream.Read(Arg.Any<byte[]>(), 0, 0).Returns(0);

            testData
                .Invoking(td => td.Function(reader))
                .Should()
                .Throw<EndOfStreamException>();

            stream.Received().Read(Arg.Any<byte[]>(), 0, testData.BytesToRead);
        }

        [TestCase]
        public void Test_OnPositionDo_With_Result()
        {
            stream.Position.Returns(20);
            stream.ReadByte().Returns(100);

            reader
                .OnPositionDo(40, () => stream.ReadByte())
                .Should()
                .Be(100);

            Received.InOrder(() =>
            {
                stream.Position = 40;
                stream.ReadByte();
                stream.Position = 20;
            });
        }

        [TestCase]
        public void Test_OnPositionDo_Without_Result()
        {
            stream.Position.Returns(20);

            reader.OnPositionDo(40, () => stream.WriteByte(0));

            Received.InOrder(() =>
            {
                stream.Position = 40;
                stream.WriteByte(0);
                stream.Position = 20;
            });
        }

        static object[] TEST_CASES_DATA() => new object[]
        {
            new TestCaseData<byte>(
                description: "Read a byte",
                input: new byte[] { 0xAB },
                function: r => r.ReadByte(),
                expectedResult: 0xAB
            ),
            new TestCaseData<short>(
                description: "Read a signed 16 bits integer",
                input: new byte[] { 0x22, 0x11 },
                function: r => r.ReadInt16(),
                expectedResult: 0x1122
            ),
            new TestCaseData<ushort>(
                description: "Read an unsigned 16 bits integer",
                input: new byte[] { 0xEE, 0xFF },
                function: r => r.ReadUInt16(),
                expectedResult: 0xFFEE
            ),
            new TestCaseData<int>(
                description: "Read a signed 32 bits integer",
                input: new byte[] { 0x44, 0x33, 0x22, 0x11 },
                function: r => r.ReadInt32(),
                expectedResult: 0x11223344
            ),
            new TestCaseData<uint>(
                description: "Read an unsigned 32 bits integer",
                input: new byte[] { 0xCC, 0xDD, 0xEE, 0xFF },
                function: r => r.ReadUInt32(),
                expectedResult: 0xFFEEDDCC
            ),
            new TestCaseData<float>(
                description: "Read a 32 bits floating point number",
                input: new byte[] { 0x00, 0x00, 0x20, 0x40 },
                function: r => r.ReadFloat(),
                expectedResult: 2.5f
            ),
            new TestCaseData<double>(
                description: "Read a 64 bits floating point number",
                input: new byte[] { 0x9A, 0x99, 0x99, 0x99, 0x99, 0x99, 0x09, 0x40 },
                function: r => r.ReadDouble(),
                expectedResult: 3.2
            )
        };

        public class TestCaseData<T> : AbstractTestCaseData
        {
            private readonly byte[] bytes;

            public TestCaseData(string description, byte[] input, Func<IBinaryReader, T> function, T expectedResult) : base(description)
            {
                bytes = input;
                Function = function;
                ExpectedResult = expectedResult;
            }

            public int BytesToRead
            {
                get
                {
                    return bytes.Length;
                }
            }

            public Func<IBinaryReader, T> Function { get; private set; }

            public T ExpectedResult { get; private set; }

            public void SetBytes(byte[] inputBytes)
            {
                inputBytes.Should().HaveCount(bytes.Length,
                    $"input bytes should be of size {bytes.Length} " +
                    $"but they are of size {inputBytes.Length} instead");

                bytes.CopyTo(inputBytes, 0);
            }
        }
    }
}
