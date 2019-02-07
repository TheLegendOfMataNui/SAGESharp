using FluentAssertions;
using Konvenience;
using NUnit.Framework;
using SAGESharp.SLB;
using SAGESharpTests;
using System;
using System.Reflection;

namespace SAGESharp.SLB.Tests
{
    class IdentifierTests
    {
        [Test]
        public void Test_Cast_Integer_To_Identifier()
        {
            var identifier = (Identifier)0x11223344;
            // For the nature of the "Identifier" type
            // we make an exception here testing private members
            // to ensure the correct behavior of the casting operator
            var field = identifier
                .GetType()
                .GetField("value", BindingFlags.NonPublic | BindingFlags.Instance);

            field.GetValue(identifier).Should().Be(0x11223344);
        }

        [TestCaseSource(nameof(ByteArraysAndIdentifiers))]
        public void Test_Create_Identifier_From_Byte_Array(byte[] values, Identifier expected)
            => Identifier.From(values).Should().Be(expected);

        static object[] ByteArraysAndIdentifiers() => new ParameterGroup<byte[], Identifier>()
            .Parameters(new byte[0], 0)
            .Parameters(new byte[] { 0x11 }, 0x11)
            .Parameters(new byte[] { 0x11, 0x22, 0x33, 0x44 }, 0x44332211)
            .Parameters(new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66 }, 0x44332211)
            .Build();

        [Test]
        public void Test_Create_Identifier_From_Null_Byte_Array_Should_Throw_ArgumentNullException()
            => ((byte[])null).Invoking(nullByteArray => Identifier.From(nullByteArray)).Should().Throw<ArgumentNullException>();

        [TestCaseSource(nameof(StringsAndIdentifiers))]
        public void Test_Create_Identifier_From_String(string value, Identifier expected)
            => Identifier.From(value).Should().Be(expected);

        static object[] StringsAndIdentifiers() => new ParameterGroup<string, Identifier>()
            .Parameters(string.Empty, Identifier.ZERO)
            .Parameters("A", 0x41)
            .Parameters("DCBA", 0x44434241)
            .Parameters("FEDCBA", 0x44434241)
            .Build();

        [Test]
        public void Test_Create_Identifier_From_Null_String_Should_Throw_ArgumentNullException()
            => ((string)null).Invoking(nullString => Identifier.From(nullString)).Should().Throw<ArgumentNullException>();

        [Test]
        public void Test_Cast_Identifier_To_Integer()
            => ((Identifier)0x11223344).Let(i => (int)i).Should().Be(0x11223344);
    }
}

namespace SAGESharpTests.SLB
{
    [TestFixture]
    public class IdentifierTests
    {
        [Test]
        public void TestCreateEmptyIdentifier()
        {
            var identifier = new Identifier();

            AssertEmptyIdentifier(identifier);
        }

        [Test]
        public void TestSettingIdentifierValuesByChars()
        {
            var identifier = new Identifier
            {
                C0 = 'A',
                C1 = 'B',
                C2 = 'C',
                C3 = 'D'
            };

            AssertIdentifierWithTestValue(identifier);
        }

        [Test]
        public void TestSettingIdentifierValuesByBytes()
        {
            var identifier = new Identifier
            {
                B0 = 0x41,
                B1 = 0x42,
                B2 = 0x43,
                B3 = 0x44
            };

            AssertIdentifierWithTestValue(identifier);
        }

        [Test]
        public void TestSettingInvalidCharsShouldReturnEmptyChar()
        {
            var identifier = new Identifier();

            identifier.B0 = 0x7B; // This is ASCII for '{'
            Assert.That(identifier.C0, Is.EqualTo(Identifier.EMPY_CHAR));

            identifier.B0 = 0x7C; // This is ASCII for '|'
            Assert.That(identifier.C0, Is.EqualTo(Identifier.EMPY_CHAR));

            identifier.B0 = 0x7D; // This is ASCII for '}'
            Assert.That(identifier.C0, Is.EqualTo(Identifier.EMPY_CHAR));

            identifier.B0 = 0x7E; // This is ASCII for '~'
            Assert.That(identifier.C0, Is.EqualTo(Identifier.EMPY_CHAR));

            var expectedString = new string(Identifier.EMPY_CHAR, 4);
            Assert.That(identifier.ToString(), Is.EqualTo(expectedString));
        }

        private static void AssertEmptyIdentifier(Identifier identifier)
        {
            var expectedString = new string(Identifier.EMPY_CHAR, 4);

            Assert.That(identifier.ToInteger(), Is.EqualTo(0));
            Assert.That(identifier.ToString(), Is.EqualTo(expectedString));

            Assert.That(identifier.C0, Is.EqualTo(Identifier.EMPY_CHAR));
            Assert.That(identifier.C1, Is.EqualTo(Identifier.EMPY_CHAR));
            Assert.That(identifier.C2, Is.EqualTo(Identifier.EMPY_CHAR));
            Assert.That(identifier.C3, Is.EqualTo(Identifier.EMPY_CHAR));

            Assert.That(identifier.B0, Is.EqualTo(0));
            Assert.That(identifier.B1, Is.EqualTo(0));
            Assert.That(identifier.B2, Is.EqualTo(0));
            Assert.That(identifier.B3, Is.EqualTo(0));
        }

        private static void AssertIdentifierWithTestValue(Identifier identifier)
        {
            // Test value is the equivalent of:
            // C0 = 'A', C1 = 'B', C2 = 'C', C3 = 'D'

            Assert.That(identifier.ToInteger(), Is.EqualTo(0x44434241));
            Assert.That(identifier.ToString(), Is.EqualTo("DCBA"));

            Assert.That(identifier.C0, Is.EqualTo('A'));
            Assert.That(identifier.C1, Is.EqualTo('B'));
            Assert.That(identifier.C2, Is.EqualTo('C'));
            Assert.That(identifier.C3, Is.EqualTo('D'));

            Assert.That(identifier.B0, Is.EqualTo(0x41));
            Assert.That(identifier.B1, Is.EqualTo(0x42));
            Assert.That(identifier.B2, Is.EqualTo(0x43));
            Assert.That(identifier.B3, Is.EqualTo(0x44));
        }
    }
}
