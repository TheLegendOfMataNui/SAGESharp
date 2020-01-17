/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NUnit.Framework;
using NUtils.Extensions;
using SAGESharp.Testing;
using System;
using System.IO;

namespace SAGESharp.IO
{
    class BinaryWriterWrapperTests
    {
        private readonly MemoryStream stream = new MemoryStream();

        private readonly IBinaryWriter writer;

        public BinaryWriterWrapperTests()
            => writer = new BinaryWriterWrapper(stream);

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
        public void Test_Setting_The_Position() => writer
            .Also(w => w.Position = 30)
            .Also(w => stream.Position.Should().Be(30));

        [Test]
        public void Test_Getting_The_Position() => stream
            .Also(s => s.Position = 50)
            .Also(s => writer.Position.Should().Be(50));

        [TestCaseSource(nameof(TEST_CASES_DATA))]
        public void Test_Writing_Succesfully<T>(TestCaseData<T> testData)
        {
            testData.ExecuteAction(writer);

            stream
                .ToArray()
                .Should()
                .Equal(testData.ExpectedResult);
        }

        [Test]
        public void Test_Writing_A_Null_Byte_Array()
        {
            writer
                .Invoking(w => w.WriteBytes(null))
                .Should()
                .Throw<ArgumentNullException>();
        }

        static object[] TEST_CASES_DATA() => new object[]
        {
            new TestCaseData<byte>(
                description: "Write a byte",
                input: 0xAB,
                action: (w, v) => w.WriteByte(v),
                expectedResult: new byte[] { 0xAB }
            ),
            new TestCaseData<byte[]>(
                description: "Write a byte array",
                input: new byte[] { 0xAA, 0xBB, 0xCC },
                action: (w, v) => w.WriteBytes(v),
                expectedResult: new byte[] { 0xAA, 0xBB, 0xCC }
            ),
            new TestCaseData<short>(
                description: "Write a signed 16 bits integer",
                input: 0x1122,
                action: (w, v) => w.WriteInt16(v),
                expectedResult: new byte[] { 0x22, 0x11 }
            ),
            new TestCaseData<ushort>(
                description: "Write an unsigned 16 bits integer",
                input: 0xFFEE,
                action: (w, v) => w.WriteUInt16(v),
                expectedResult: new byte[] { 0xEE, 0xFF }
            ),
            new TestCaseData<int>(
                description: "Write a signed 32 bits integer",
                input: 0x11223344,
                action: (w, v) => w.WriteInt32(v),
                expectedResult: new byte[] { 0x44, 0x33, 0x22, 0x11 }
            ),
            new TestCaseData<uint>(
                description: "Write an unsigned 32 bits integer",
                input: 0xFFEEDDCC,
                action: (w, v) => w.WriteUInt32(v),
                expectedResult: new byte[] { 0xCC, 0xDD, 0xEE, 0xFF }
            ),
            new TestCaseData<long>(
                description: "Write a signed 64 bits integer",
                input: 0x1122334455667788,
                action: (w, v) => w.WriteInt64(v),
                expectedResult: new byte[] { 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11 }
            ),
            new TestCaseData<ulong>(
                description: "Write an unsigned 64 bits integer",
                input: 0xFFEEDDCCBBAA9988,
                action: (w, v) => w.WriteUInt64(v),
                expectedResult: new byte[] { 0x88, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF }
            ),
            new TestCaseData<float>(
                description: "Write a 32 bits floating point number",
                input: 2.5f,
                action: (w, v) => w.WriteFloat(v),
                expectedResult: new byte[] { 0x00, 0x00, 0x20, 0x40 }
            ),
            new TestCaseData<double>(
                description: "Write a 64 bits floating point number",
                input: 3.2,
                action: (w, v) => w.WriteDouble(v),
                expectedResult: new byte[] { 0x9A, 0x99, 0x99, 0x99, 0x99, 0x99, 0x09, 0x40 }
            )
        };

        public class TestCaseData<T> : AbstractTestCaseData
        {
            private readonly T input;

            private readonly Action<IBinaryWriter, T> action;

            public TestCaseData(string description, T input, Action<IBinaryWriter, T> action, byte[] expectedResult) : base(description)
            {
                this.input = input;
                this.action = action;
                ExpectedResult = expectedResult;
            }

            public void ExecuteAction(IBinaryWriter writer) => action(writer, input);

            public byte[] ExpectedResult { get; }
        }
    }
}
