using FluentAssertions;
using Konvenience;
using NUnit.Framework;
using SAGESharp.Testing;
using System;
using System.Reflection;

namespace SAGESharp.SLB.Tests
{
    class IdentifierTests
    {
        [Test]
        public void Test_Cast_Integer_To_Identifier()
        {
            int value = 0x11223344;
            Identifier identifier = value;
            // For the nature of the "Identifier" type
            // we make an exception here testing private members
            // to ensure the correct behavior of the casting operator
            var field = identifier
                .GetType()
                .GetField("value", BindingFlags.NonPublic | BindingFlags.Instance);

            field.GetValue(identifier).Should().Be(value);
        }

        [Test]
        public void Test_Case_Unsigned_Integer_To_Identifier()
        {
            uint value = 0x44434241;
            Identifier identifier = value;
            // For the nature of the "Identifier" type
            // we make an exception here testing private members
            // to ensure the correct behavior of the casting operator
            var field = identifier
                .GetType()
                .GetField("value", BindingFlags.NonPublic | BindingFlags.Instance);

            field.GetValue(identifier).Should().Be(value);
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

        [TestCaseSource(nameof(IdentifierAndBytes))]
        public void Test_Getting_Identifier_Individual_Bytes(Identifier identifier, byte b0, byte b1, byte b2, byte b3)
        {
            identifier.B0.Should().Be(b0);
            identifier.B1.Should().Be(b1);
            identifier.B2.Should().Be(b2);
            identifier.B3.Should().Be(b3);
        }

        static object[] IdentifierAndBytes() => new ParameterGroup<Identifier, byte, byte, byte, byte>()
            .Parameters(0, 0, 0, 0, 0)
            .Parameters(0x44434241, 0x41, 0x42, 0x43, 0x44)
            .Build();

        [TestCaseSource(nameof(IdentifierAndChars))]
        public void Test_Getting_Identifier_Individual_Bytes_As_Chars(Identifier identifier, char c0, char c1, char c2, char c3)
        {
            identifier.C0.Should().Be(c0);
            identifier.C1.Should().Be(c1);
            identifier.C2.Should().Be(c2);
            identifier.C3.Should().Be(c3);
        }

        static object[] IdentifierAndChars() => new ParameterGroup<Identifier, char, char, char, char>()
            .Parameters(0, Identifier.EMPY_CHAR, Identifier.EMPY_CHAR, Identifier.EMPY_CHAR, Identifier.EMPY_CHAR)
            .Parameters(0x44434241, 'A', 'B', 'C', 'D')
            .Build();

        [Test]
        public void Test_Cast_Identifier_To_Integer()
            => ((Identifier)0x11223344).Let(i => (int)i).Should().Be(0x11223344);

        [Test]
        public void Test_Cast_Identifier_To_Unsigned_Integer()
            => ((Identifier)0x44434241).Let(i => (uint)i).Should().Be(0x44434241);

        [TestCaseSource(nameof(IdentifierWithString))]
        public void Test_Identifier_To_String(Identifier identifier, string expected)
            => identifier.ToString().Should().Be(expected);

        static object[] IdentifierWithString() => new ParameterGroup<Identifier, string>()
            .Parameters(0x44434241, "DCBA")
            .Parameters(0, new string(Identifier.EMPY_CHAR, 4))
            .Parameters(0x44434201, $"DCB{Identifier.EMPY_CHAR}")
            .Build();

        [TestCaseSource(nameof(IdentifiersToTestSettingBytes))]
        public void Test_Modify_A_Byte_From_An_Identifier(Func<Identifier, Identifier> function, Identifier expected)
            => Identifier.ZERO.Let(function).Should().Be(expected);

        static object[] IdentifiersToTestSettingBytes() => new ParameterGroup<Func<Identifier, Identifier>, Identifier>()
            .Parameters(i => i.WithB0(0x11), 0x00000011)
            .Parameters(i => i.WithB1(0x11), 0x00001100)
            .Parameters(i => i.WithB2(0x11), 0x00110000)
            .Parameters(i => i.WithB3(0x11), 0x11000000)
            .Build();

        [TestCaseSource(nameof(IdentifiersToTestSettingBytesWithChars))]
        public void Test_Modify_A_Byte_From_An_Identifier_With_A_Char(Func<Identifier, Identifier> function, Identifier expected)
            => Identifier.ZERO.Let(function).Should().Be(expected);

        static object[] IdentifiersToTestSettingBytesWithChars() => new ParameterGroup<Func<Identifier, Identifier>, Identifier>()
            .Parameters(i => i.WithC0('A'), 0x00000041)
            .Parameters(i => i.WithC1('A'), 0x00004100)
            .Parameters(i => i.WithC2('A'), 0x00410000)
            .Parameters(i => i.WithC3('A'), 0x41000000)
            .Build();
    }
}
