/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using Konvenience;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;
using SAGESharp.SLB;
using SAGESharp.Testing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAGESharp.IO
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
        public void Test_Reading_A_Custom_Class()
        {
            var propertySerializers = new List<IPropertyBinarySerializer<CustomClass>>
            {
                Substitute.For<IPropertyBinarySerializer<CustomClass>>().Also(
                    pbs => pbs.ReadAndSet(Arg.Is(reader), Arg.Do<CustomClass>(o => o.String1 = "String1"))
                ),
                Substitute.For<IPropertyBinarySerializer<CustomClass>>().Also(
                    pbs => pbs.ReadAndSet(Arg.Is(reader), Arg.Do<CustomClass>(o => o.String2 = "String2"))
                )
            };

            CustomClass result = new DefaultBinarySerializer<CustomClass>(propertySerializers)
                .Read(reader)
                .Also(o => o.String1.Should().Be("String1"))
                .Also(o => o.String2.Should().Be("String2"))
                .Also(o => o.String3.Should().BeNull())
                .Also(o => propertySerializers.ForEach(pbs => pbs.Received().ReadAndSet(reader, o)));
        }

        public class CustomClass
        {
            public string String1 { get; set; }

            public string String2 { get; set; }

            public string String3 { get; set; }
        }

        [Test]
        public void Test_Reading_A_Class_With_Private_Empty_Constructor() => this
            .Invoking(_ => new DefaultBinarySerializer<ClassWithPrivateConstructor>(new List<IPropertyBinarySerializer<ClassWithPrivateConstructor>> { }))
            .Should()
            .Throw<BadTypeException>()
            .Where(e => e.Message.Contains($"Type {nameof(ClassWithPrivateConstructor)} has no public constructor with no arguments"));

        class ClassWithPrivateConstructor
        {
            private ClassWithPrivateConstructor()
            {
            }
        }
    }
}
