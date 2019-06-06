/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;
using SAGESharp.Testing;
using System;

namespace SAGESharp.IO
{
    class PrimitiveBinarySerializerTests
    {
        private readonly IBinaryReader reader = Substitute.For<IBinaryReader>();

        [SetUp]
        public void Setup()
        {
            reader.ClearSubstitute();
        }

        [TestCaseSource(nameof(SUPPORTED_PRIMITIVES_TEST_CASES))]
        public void Test_Reading_A_Primitive<T>(PrimitiveReadingTestCaseData<T> testCaseData)
        {
            testCaseData.Setup(reader);

            new PrimitiveBinarySerializer<T>()
                .Read(reader)
                .Should()
                .BeOfType<T>()
                .Which
                .Should()
                .Be(testCaseData.Value);

            testCaseData.Verify(reader);
        }

        static object[] SUPPORTED_PRIMITIVES_TEST_CASES() => new object[]
        {
            new PrimitiveReadingTestCaseData<byte>(
                setup: (r, v) => r.ReadByte().Returns(v),
                value: 0xFE,
                verify: r => r.Received().ReadByte()
            ),
            new PrimitiveReadingTestCaseData<short>(
                setup: (r, v) => r.ReadInt16().Returns(v),
                value: 0x1234,
                verify: r => r.Received().ReadInt16()
            ),
            new PrimitiveReadingTestCaseData<ushort>(
                setup: (r, v) => r.ReadUInt16().Returns(v),
                value: 0xFEDC,
                verify: r => r.Received().ReadUInt16()
            ),
            new PrimitiveReadingTestCaseData<int>(
                setup: (r, v) => r.ReadInt32().Returns(v),
                value: 0x12345678,
                verify: r => r.Received().ReadInt32()
            ),
            new PrimitiveReadingTestCaseData<uint>(
                setup: (r, v) => r.ReadUInt32().Returns(v),
                value: 0xFEDCBA98,
                verify: r => r.Received().ReadUInt32()
            ),
            new PrimitiveReadingTestCaseData<float>(
                setup: (r, v) => r.ReadFloat().Returns(v),
                value: 2.5f,
                verify: r => r.Received().ReadFloat()
            ),
            new PrimitiveReadingTestCaseData<double>(
                setup: (r, v) => r.ReadDouble().Returns(v),
                value: 3.2,
                verify: r => r.Received().ReadDouble()
            )
        };

        public class PrimitiveReadingTestCaseData<T> : AbstractTestCaseData
        {
            private readonly Action<IBinaryReader, T> setup;

            private readonly Action<IBinaryReader> verify;

            public PrimitiveReadingTestCaseData(Action<IBinaryReader, T> setup, T value, Action<IBinaryReader> verify)
                : base($"Test reading a primitive of type {typeof(T).Name}")
            {
                this.setup = setup;
                Value = value;
                this.verify = verify;
            }

            public T Value { get; }

            public void Setup(IBinaryReader reader)
            {
                setup(reader, Value);
            }

            public void Verify(IBinaryReader reader)
            {
                verify(reader);
            }
        }

        [Test]
        public void Test_Reading_An_Unsupported_Primitive()
            => new PrimitiveBinarySerializer<char>()
                .Invoking(s => s.Read(reader))
                .Should()
                .Throw<BadTypeException>()
                .WithMessage("Type is not a supported primitive")
                .And
                .Type
                .Should()
                .Be(typeof(char));
    }
}
