using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;
using SAGESharp.Testing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAGESharp.SLB.IO
{
    class BinarySerializersReadingTests
    {
        private readonly IBinaryReader reader = Substitute.For<IBinaryReader>();

        private readonly IBinarySerializerFactory factory = Substitute.For<IBinarySerializerFactory>();

        [SetUp]
        public void Setup()
        {
            reader.ClearSubstitute();
            factory.ClearSubstitute();
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

        [TestCase]
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

        [TestCase]
        public void Test_Reading_An_Identifier_With_CastSerializer()
        {
            var value = 0xFEDCBA98;
            var innerSerializer = Substitute.For<IBinarySerializer<uint>>();

            innerSerializer.Read(reader).Returns(value);

            new CastSerializer<Identifier, uint>(innerSerializer)
                .Read(reader)
                .Should()
                .Be((Identifier)value);

            innerSerializer.Received().Read(reader);
        }

        [TestCase]
        public void Test_Reading_An_Enum_With_CastSerializer()
        {
            var innerSerializer = Substitute.For<IBinarySerializer<byte>>();

            innerSerializer.Read(reader).Returns((byte)TestEnum.A);

            new CastSerializer<TestEnum, byte>(innerSerializer)
                .Read(reader)
                .Should()
                .Be(TestEnum.A);

            innerSerializer.Received().Read(reader);
        }

        enum TestEnum : byte
        {
            A = 0xAB,
            B = 0xBB
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

        [TestCase]
        public void Test_Reading_A_List()
        {
            var expected = new List<char>() { 'a', 'b', 'c', 'd', 'e' };
            var serializer = Substitute.For<IBinarySerializer<char>>();

            // Returns count and offset
            reader.ReadUInt32().Returns((uint)expected.Count, (uint)70);

            reader.Position.Returns(45);

            // Returns the entire "expected" list from serailizer.Read(reader)
            serializer.Read(reader).Returns(expected[0], expected.Skip(1).Cast<object>().ToArray());

            new ListBinarySerializer<char>(serializer)
                .Read(reader)
                .Should()
                .BeOfType<List<char>>()
                .Which
                .Should()
                .Equal(expected);

            Received.InOrder(() =>
            {
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.Position = 70;
                foreach (var _ in expected)
                {
                    serializer.Read(reader);
                }
                reader.Position = 45;
            });
        }

        [TestCase]
        public void Test_Reading_A_Custom_Class()
        {
            var expected = new CustomClass
            {
                Int = 1,
                Short = 2,
                Byte = 3,
                Float = 4.5f,
                Double = 5.5,
                List = new List<char> { 'a', 'b', 'c', 'd' },
                String = "hello world"
            };

            IBinarySerializer<T> setupSerializer<T>(T expectedValue)
            {
                var serializer = Substitute.For<IBinarySerializer<T>>();

                serializer.Read(reader).Returns(expectedValue);
                factory.GetSerializerForType<T>().Returns(serializer);

                return serializer;
            }

            var identifierSerializer = setupSerializer(expected.Identifier);
            var intSerializer = setupSerializer(expected.Int);
            var shortSerializer = setupSerializer(expected.Short);
            var byteSerializer = setupSerializer(expected.Byte);
            var floatSerializer = setupSerializer(expected.Float);
            var doubleSerializer = setupSerializer(expected.Double);
            var listSerializer = setupSerializer(expected.List);
            var stringSerializer = setupSerializer(expected.String);

            new DefaultBinarySerializer<CustomClass>(factory)
                .Read(reader)
                .Should()
                .BeOfType<CustomClass>()
                .Which
                .Should()
                .BeEquivalentTo(expected);

            factory.Received().GetSerializerForType<IList<char>>();
            factory.Received().GetSerializerForType<string>();

            Received.InOrder(() =>
            {
                identifierSerializer.Read(reader);
                intSerializer.Read(reader);
                shortSerializer.Read(reader);
                byteSerializer.Read(reader);
                floatSerializer.Read(reader);
                doubleSerializer.Read(reader);
                listSerializer.Read(reader);
                stringSerializer.Read(reader);
            });
        }

        class CustomClass
        {
            [SLBElement(8)]
            public string String { get; set; }

            [SLBElement(4)]
            public byte Byte { get; set; }

            [SLBElement(3)]
            public short Short { get; set; }

            [SLBElement(2)]
            public int Int { get; set; }

            [SLBElement(5)]
            public float Float { get; set; }

            [SLBElement(6)]
            public double Double { get; set; }

            [SLBElement(1)]
            public Identifier Identifier { get; set; }

            [SLBElement(7)]
            public IList<char> List { get; set; }

            public CustomClass IgnoredValue { get; set; }
        }

        [TestCase]
        public void Test_Reading_A_Class_With_Private_Constructor() => this
            .Invoking(_ => new DefaultBinarySerializer<ClassWithPrivateConstructor>(factory))
            .Should()
            .Throw<BadTypeException>()
            .WithMessage("Type has no public constructor with no arguments")
            .And
            .Type
            .Should()
            .Be(typeof(ClassWithPrivateConstructor));

        class ClassWithPrivateConstructor
        {
            private ClassWithPrivateConstructor()
            {
            }

            [SLBElement(1)]
            public int Int { get; set; }
        }

        [TestCase]
        public void Test_Reading_A_Class_With_No_Annotations() => this
            .Invoking(_ => new DefaultBinarySerializer<ClassWithNoAnnotations>(factory))
            .Should()
            .Throw<BadTypeException>()
            .WithMessage($"Type has no property annotated with {nameof(SLBElementAttribute)}")
            .And
            .Type
            .Should()
            .Be(typeof(ClassWithNoAnnotations));

        class ClassWithNoAnnotations
        {
            public int Int { get; set; }
        }

        [TestCase]
        public void Test_Reading_A_Class_With_An_Annotated_Property_With_No_Setter() => this
            .Invoking(_ => new DefaultBinarySerializer<ClassWithAnnotatedPropertyWithNoSetter>(factory))
            .Should()
            .Throw<BadTypeException>()
            .WithMessage($"Property {nameof(ClassWithAnnotatedPropertyWithNoSetter.Int)} doesn't have a setter")
            .And
            .Type
            .Should()
            .Be(typeof(ClassWithAnnotatedPropertyWithNoSetter));

        class ClassWithAnnotatedPropertyWithNoSetter
        {
            [SLBElement(1)]
            public int Int { get; }
        }

        [TestCase]
        public void Test_Reading_A_Class_With_Properties_With_Duplicated_Attribute_Order() => this
            .Invoking(_ => new DefaultBinarySerializer<ClassWithPropertiesWithDuplicatedAttributeOrder>(factory))
            .Should()
            .Throw<BadTypeException>()
            .WithMessage("Type has more than one property with the same order")
            .And
            .Type
            .Should()
            .Be(typeof(ClassWithPropertiesWithDuplicatedAttributeOrder));

        class ClassWithPropertiesWithDuplicatedAttributeOrder
        {
            [SLBElement(1)]
            public int Int { get;  set; }

            [SLBElement(1)]
            public byte Byte { get;  set; }
        }
    }
}
