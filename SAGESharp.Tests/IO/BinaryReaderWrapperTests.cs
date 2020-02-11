/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NUnit.Framework;
using SAGESharp.IO;
using System;
using System.IO;
using System.Linq;

namespace SAGESharp.Tests.IO
{
    class BinaryReaderWrapperTests
    {
        private readonly Stream stream = new MemoryStream();

        private readonly IBinaryReader reader;

        public BinaryReaderWrapperTests()
            => reader = new BinaryReaderWrapper(stream);

        [SetUp]
        public void Setup()
        {
            stream.SetLength(0);
            stream.Position = 0;
        }

        [Test]
        public void Test_Create_A_Reader_With_A_Null_Stream() => this
            .Invoking(_ => new BinaryReaderWrapper(null))
            .Should()
            .Throw<ArgumentNullException>();

        [Test]
        public void Test_Getting_The_Position()
        {
            stream.Position = 50;

            reader.Position.Should().Be(50);
        }

        [Test]
        public void Test_Setting_The_Position()
        {
            reader.Position = 30;

            stream.Position.Should().Be(30);
        }

        [TestCaseSource(nameof(TEST_CASES_DATA))]
        public void Test_Reading_Successfully<T>(TestCaseData<T> testData)
        {
            SetupStreamWithArray(testData.Bytes);

            testData
                .Function(reader)
                .Should()
                .Be(testData.ExpectedResult);

            stream.Position.Should().Be(testData.Bytes.Length);
        }

        [Test]
        public void Test_Reading_A_Byte_Array()
        {
            var expected = new byte[] { 0x01, 0x02, 0x03 };

            SetupStreamWithArray(expected);

            reader
                .ReadBytes(expected.Length)
                .Should()
                .Equal(expected);

            stream.Position.Should().Be(expected.Length);
        }

        [Test]
        public void Test_Reading_An_Empty_Byte_Array()
        {
            reader
                .ReadBytes(0)
                .Should()
                .BeEmpty();

            stream.Position.Should().Be(0);
        }

        [TestCaseSource(nameof(TEST_CASES_DATA))]
        public void Test_Reading_After_EOF<T>(TestCaseData<T> testData)
        {
            var subArray = testData.Bytes.Skip(1).ToArray();
            SetupStreamWithArray(subArray);

            testData
                .Invoking(td => td.Function(reader))
                .Should()
                .Throw<EndOfStreamException>();

            stream.Position.Should().Be(subArray.Length);
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
            new TestCaseData<long>(
                description: "Read a signed 64 bits integer",
                input: new byte[] { 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11 },
                function: r => r.ReadInt64(),
                expectedResult: 0x1122334455667788
            ),
            new TestCaseData<ulong>(
                description: "Read an unsigned 64 bits integer",
                // 0123456789ABCDEF
                input: new byte[] { 0x88, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF },
                function: r => r.ReadUInt64(),
                expectedResult: 0xFFEEDDCCBBAA9988
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

        public class TestCaseData<T> : AbstractTestCase
        {
            public TestCaseData(string description, byte[] input, Func<IBinaryReader, T> function, T expectedResult) : base(description)
            {
                Bytes = input;
                Function = function;
                ExpectedResult = expectedResult;
            }

            public byte[] Bytes { get; }

            public Func<IBinaryReader, T> Function { get; private set; }

            public T ExpectedResult { get; private set; }
        }

        private void SetupStreamWithArray(byte[] array, int position = 0)
        {
            stream.SetLength(position + array.Length);
            stream.Position = position;
            stream.Write(array, 0, array.Length);
            stream.Position = 0;
        }
    }
}
