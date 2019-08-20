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
    class PrimitiveTypeDataNodeTest
    {
        private readonly IBinaryWriter binaryWriter = Substitute.For<IBinaryWriter>();

        [SetUp]
        public void Setup()
        {
            binaryWriter.ClearSubstitute();
        }

        [Test]
        public void Test_Creating_A_Node_For_A_Not_Valid_Primitive()
        {
            Action action = () => new PrimitiveTypeDataNode<CustomStruct>();

            action.Should()
                .ThrowExactly<BadTypeException>()
                .Where(e => e.Type.Equals(typeof(CustomStruct)))
                .WithMessage($"Type {typeof(CustomStruct).Name} is not a valid primitive.");
        }

        struct CustomStruct
        {
        }

        [TestCaseSource(nameof(SUPPORTED_PRIMITIVES_TEST_CASES))]
        public void Test_Edges_Should_Be_Empty<T>(PrimitiveTypeTestCaseData<T> testCaseData)
            where T : struct
        {
            IDataNode node = new PrimitiveTypeDataNode<T>();

            node.Edges.Should().BeEmpty();
        }

        #region Writing
        [TestCaseSource(nameof(SUPPORTED_PRIMITIVES_TEST_CASES))]
        public void Test_Writing_A_Primitive<T>(PrimitiveTypeTestCaseData<T> testCaseData)
            where T : struct
        {
            IDataNode node = new PrimitiveTypeDataNode<T>();

            node.Write(binaryWriter, testCaseData.Value);

            testCaseData.VerifyWrite(binaryWriter);
        }

        [TestCaseSource(nameof(SUPPORTED_PRIMITIVES_TEST_CASES))]
        public void Test_Writing_With_A_Null_BinaryWriter<T>(PrimitiveTypeTestCaseData<T> testCaseData)
            where T : struct
        {
            IDataNode node = new PrimitiveTypeDataNode<T>();
            Action action = () => node.Write(null, testCaseData.Value);

            action.Should()
                .ThrowArgumentNullException("binaryWriter");
        }

        [TestCaseSource(nameof(SUPPORTED_PRIMITIVES_TEST_CASES))]
        public void Test_Writing_With_A_Null_Value<T>(PrimitiveTypeTestCaseData<T> testCaseData)
            where T : struct
        {
            IDataNode node = new PrimitiveTypeDataNode<T>();
            Action action = () => node.Write(binaryWriter, null);

            action.Should()
                .ThrowArgumentNullException("value");
        }

        [TestCaseSource(nameof(SUPPORTED_PRIMITIVES_TEST_CASES))]
        public void Test_Writing_With_Value_Of_The_Incorrect_Type<T>(PrimitiveTypeTestCaseData<T> testCaseData)
            where T : struct
        {
            IDataNode node = new PrimitiveTypeDataNode<T>();
            object value = new object();
            Action action = () => node.Write(binaryWriter, value);

            action.Should()
                .ThrowExactly<ArgumentException>()
                .WithMessage($"Cannot write value of type {value.GetType().Name} as type {typeof(T).Name}.");
        }
        #endregion

        static object[] SUPPORTED_PRIMITIVES_TEST_CASES() => new object[]
        {
            new PrimitiveTypeTestCaseData<byte>(
                value: 0xFE,
                verifyWrite: (binaryWriter, value) => binaryWriter.Received().WriteByte(value)
            ),
            new PrimitiveTypeTestCaseData<short>(
                value: 0x1234,
                verifyWrite: (binaryWriter, value) => binaryWriter.Received().WriteInt16(value)
            ),
            new PrimitiveTypeTestCaseData<ushort>(
                value: 0xFEDC,
                verifyWrite: (binaryWriter, value) => binaryWriter.Received().WriteUInt16(value)
            ),
            new PrimitiveTypeTestCaseData<int>(
                value: 0x12345678,
                verifyWrite: (binaryWrite, value) => binaryWrite.Received().WriteInt32(value)
            ),
            new PrimitiveTypeTestCaseData<uint>(
                value: 0xFEDCBA98,
                verifyWrite: (binaryWriter, value) => binaryWriter.Received().WriteUInt32(value)
            ),
            new PrimitiveTypeTestCaseData<float>(
                value: 2.5f,
                verifyWrite: (binaryWriter, value) => binaryWriter.Received().WriteFloat(value)
            ),
            new PrimitiveTypeTestCaseData<double>(
                value: 3.2,
                verifyWrite: (binaryWriter, value) => binaryWriter.Received().WriteDouble(value)
            ),
            new PrimitiveTypeTestCaseData<SLB.Identifier>(
                value: SLB.Identifier.From("IDEN"),
                verifyWrite: (binaryWriter, value) => binaryWriter.Received().WriteUInt32(value)
            ),
            new PrimitiveTypeTestCaseData<ByteEnum>(
                value: ByteEnum.Value,
                verifyWrite: (binaryWriter, value) => binaryWriter.Received().WriteByte((byte)ByteEnum.Value)
            ),
            new PrimitiveTypeTestCaseData<ShortEnum>(
                value: ShortEnum.Value,
                verifyWrite: (binaryWriter, value) => binaryWriter.Received().WriteInt16((short)ShortEnum.Value)
            ),
            new PrimitiveTypeTestCaseData<UIntEnum>(
                value: UIntEnum.Value,
                verifyWrite: (binaryWriter, value) => binaryWriter.Received().WriteUInt32((uint)UIntEnum.Value)
            )
        };

        public class PrimitiveTypeTestCaseData<T> : AbstractTestCaseData
        {
            private readonly Action<IBinaryWriter, T> verifyWrite;

            public PrimitiveTypeTestCaseData(T value, Action<IBinaryWriter, T> verifyWrite)
                : base($"Test case for a primitive of type {typeof(T).Name}")
            {
                Value = value;
                this.verifyWrite = verifyWrite;
            }

            public T Value { get; }

            public void VerifyWrite(IBinaryWriter binaryWriter) => verifyWrite(binaryWriter, Value);
        }

        #region Enums
        enum ByteEnum : byte
        {
            Value = 81
        }

        enum ShortEnum : short
        {
            Value = 82
        }

        enum UIntEnum : uint
        {
            Value = 83
        }
        #endregion
    }
}
