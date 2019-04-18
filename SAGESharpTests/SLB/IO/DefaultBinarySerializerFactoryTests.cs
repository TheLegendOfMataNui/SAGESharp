/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NUnit.Framework;
using SAGESharp.Testing;
using System.Collections.Generic;

namespace SAGESharp.SLB.IO
{
    class DefaultBinarySerializerFactoryTests
    {
        private readonly IBinarySerializerFactory factory = new DefaultBinarySerializerFactory();

        #region Primitive test cases
        [TestCaseSource(nameof(PRIMITIVE_TEST_CASES))]
        public void Test_Get_Serializer_For_Primitive<T>(PrimitiveTestCase<T> testCaseData)
            => factory
                .GetSerializerForType<T>()
                .Should()
                .BeOfType<PrimitiveBinarySerializer<T>>();

        static object[] PRIMITIVE_TEST_CASES() => new object[]
        {
            new PrimitiveTestCase<byte>(),
            new PrimitiveTestCase<short>(),
            new PrimitiveTestCase<ushort>(),
            new PrimitiveTestCase<int>(),
            new PrimitiveTestCase<uint>(),
            new PrimitiveTestCase<float>(),
            new PrimitiveTestCase<double>()
        };

        public class PrimitiveTestCase<T> : AbstractTestCaseData
        {
            public PrimitiveTestCase()
                : base($"Test getting serializer for primitive {typeof(T).Name}")
            {
            }
        }
        #endregion

        #region Casts test cases
        [TestCaseSource(nameof(CASTS_TEST_CASES))]
        public void Test_Get_Serializer_For_Castable_Type<T, U>(CastTestCase<T, U> testcaseData)
            => factory.GetSerializerForType<T>()
                .Should()
                .BeOfType<CastSerializer<T, U>>();

        static object[] CASTS_TEST_CASES() => new object[]
        {
            new CastTestCase<Identifier, uint>(),
            new CastTestCase<ByteEnum, byte>(),
            new CastTestCase<Int32Enum, int>(),
            new CastTestCase<UInt32Enum, uint>()
        };

        enum ByteEnum : byte { }

        enum Int32Enum : int { }

        enum UInt32Enum : uint { }

        public class CastTestCase<T, U> : AbstractTestCaseData
        {
            public CastTestCase()
                : base($"Test getting serializer to read {typeof(U).Name} as {typeof(T).Name}")
            {
            }
        }
        #endregion

        #region String test cases
        [TestCase]
        public void Test_Get_Serializer_For_String() => factory
            .GetSerializerForType<string>()
            .Should()
            .BeOfType<StringBinarySerializer>();
        #endregion

        #region Lists test cases
        [TestCaseSource(nameof(LISTS_TEST_CASES))]
        public void Test_Get_Serializer_For_List_Type<T, U>(ListTestCase<T, U> testCaseData)
            => factory
                .GetSerializerForType<T>()
                .Should()
                .BeOfType<ListBinarySerializer<U>>();

        static object[] LISTS_TEST_CASES() => new object[]
        {
            new ListTestCase<List<int>, int>(),
            new ListTestCase<IList<int>, int>(),
            new ListTestCase<IReadOnlyList<int>, int>()
        };

        public class ListTestCase<T, U> : AbstractTestCaseData
        {
            public ListTestCase() : base($"Test getting serializer to read {typeof(T).Name}")
            {
            }
        }
        #endregion

        #region Class test cases
        [TestCase]
        public void Test_Get_Serialier_For_Class() => factory
            .GetSerializerForType<TestClass>()
            .Should()
            .BeOfType<DefaultBinarySerializer<TestClass>>();

        class TestClass
        {
            [SerializableProperty(0)]
            public int Int { get; set; }
        }
        #endregion

        #region Negative test cases
        [TestCase]
        public void Test_Get_Serializer_For_Struct() => factory
            .Invoking(f => f.GetSerializerForType<TestStruct>())
            .Should()
            .Throw<BadTypeException>()
            .WithMessage("Type is not a supported serializable type")
            .And
            .Type
            .Should()
            .Be(typeof(TestStruct));

        struct TestStruct
        {
            [SerializableProperty(1)]
            public byte Byte { get; set; }
        }

        [TestCase]
        public void Test_Get_Serializer_For_Interface() => factory
            .Invoking(f => f.GetSerializerForType<ITestInterface>())
            .Should()
            .Throw<BadTypeException>()
            .WithMessage("Type is not a supported serializable type")
            .And
            .Type
            .Should()
            .Be(typeof(ITestInterface));

        interface ITestInterface
        {
        }

        [TestCase]
        public void Test_Get_Serializer_For_Abstract_Type() => factory
            .Invoking(f => f.GetSerializerForType<AbstractTestClass>())
            .Should()
            .Throw<BadTypeException>()
            .WithMessage("Type is not a supported serializable type")
            .And
            .Type
            .Should()
            .Be(typeof(AbstractTestClass));

        abstract class AbstractTestClass
        {
        }
        #endregion
    }
}
