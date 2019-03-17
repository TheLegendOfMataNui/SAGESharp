﻿using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
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
            reader.ClearReceivedCalls();
            factory.ClearReceivedCalls();
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
