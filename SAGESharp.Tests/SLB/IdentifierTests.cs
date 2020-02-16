/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NUnit.Framework;
using NUtils.Extensions;
using SAGESharp.SLB;
using System;
using System.Reflection;

namespace SAGESharp.Tests.SLB
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

        [Test]
        public void Test_Create_Identifier_From_Byte_Array()
        {
            byte[] input = new byte[] { 0x11, 0x22, 0x33, 0x44 };

            Identifier result = Identifier.From(input);

            result.Should().Be((Identifier)0x44332211);
        }

        [TestCaseSource(nameof(IncorrectByteArrayTestCases))]
        public void Test_Creating_An_Identifier_From_An_incorrect_Byte_Array(IncorrectByteArrayTestCase testCase)
        {
            Action action = () => Identifier.From(testCase.Values);

            action.Should()
                .ThrowExactly<ArgumentException>()
                .WithMessage("Input is not 4 bytes long.");
        }

        static IncorrectByteArrayTestCase[] IncorrectByteArrayTestCases() => new IncorrectByteArrayTestCase[]
        {
            new IncorrectByteArrayTestCase(
                values: new byte[0],
                description: "Test creating an Identifier from an empty array."
            ),
            new IncorrectByteArrayTestCase(
                values: new byte[1],
                description: "Test creating an Identifier from an array with only one element."
            ),
            new IncorrectByteArrayTestCase(
                values: new byte[5],
                description: "Test creating an Identifier from an array with more than four elements."
            )
        };

        public class IncorrectByteArrayTestCase : AbstractTestCase
        {
            public IncorrectByteArrayTestCase(byte[] values, string description) : base(description)
            {
                Values = values;
            }

            public byte[] Values { get; }
        }

        [Test]
        public void Test_Create_Identifier_From_Null_Byte_Array_Should_Throw_ArgumentNullException()
        {
            Action action = () => Identifier.From((byte[])null);

            action.Should()
                .ThrowArgumentNullException("values");
        }

        [TestCaseSource(nameof(IdentifierFromStingTestCases))]
        public void Test_Create_Identifier_From_String(IdentifierFromStringTestCase testCase)
        {
            Identifier result = Identifier.From(testCase.Value);

            result.Should().Be(testCase.Expected);
        }

        static IdentifierFromStringTestCase[] IdentifierFromStingTestCases() => new IdentifierFromStringTestCase[]
        {
            new IdentifierFromStringTestCase(
                value: "Id01",
                expected: 0x49643031,
                description: "Creating an identifier form a string with a digit character"
            ),
            new IdentifierFromStringTestCase(
                value: "val|0xab|",
                expected: 0x76616CAB,
                description: "Creating an identifier form a string with an escaped byte as a lowercase hexadecimal"
            ),
            new IdentifierFromStringTestCase(
                value: "val|0xEF|",
                expected: 0x76616CEF,
                description: "Creating an identifier form a string with an escaped byte as an uppercase hexadecimal"
            ),
            new IdentifierFromStringTestCase(
                value: "val|1|",
                expected: 0x76616C01,
                description: "Creating an identifier form a string with an escaped byte as decimal"
            ),
            new IdentifierFromStringTestCase(
                value: "|0x10||0x23||0x7B||0xB6|",
                expected: 0x10237BB6,
                description: "Creating an identifier form a string with only escaped characters"
            )
        };

        public class IdentifierFromStringTestCase : AbstractTestCase
        {
            public IdentifierFromStringTestCase(string value, Identifier expected, string description) : base(description)
            {
                Value = value;
                Expected = expected;
            }

            public string Value { get; }

            public Identifier Expected { get; }
        }

        [TestCaseSource(nameof(IdentifierFromInvalidStringTestCases))]
        public void Test_Create_Identifier_From_Invalid_String(IdentifierFromInvalidStringTestCase testCase)
        {
            Action action = () => Identifier.From(testCase.Value);

            action.Should()
                .ThrowExactly<ArgumentException>()
                .WithMessage($"\"{testCase.Value}\" is not a valid Identifier.");
        }

        static IdentifierFromInvalidStringTestCase[] IdentifierFromInvalidStringTestCases() => new IdentifierFromInvalidStringTestCase[]
        {
            new IdentifierFromInvalidStringTestCase(
                value: string.Empty,
                description: "Creating an identifier form a string with no characters"
            ),
            new IdentifierFromInvalidStringTestCase(
                value: "A",
                description: "Creating an identifier from a string with a single character"
            ),
            new IdentifierFromInvalidStringTestCase(
                value: "FEDCBA",
                description: "Creating an identifier from a string with more than four characters"
            ),
            new IdentifierFromInvalidStringTestCase(
                value: "FED|0",
                description: "Creating an identifier from a string with with bad escaping"
            )
        };

        public class IdentifierFromInvalidStringTestCase : AbstractTestCase
        {
            public IdentifierFromInvalidStringTestCase(string value, string description) : base(description)
            {
                Value = value;
            }

            public string Value { get; }
        }

        [Test]
        public void Test_Create_Identifier_From_Null_String_Should_Throw_ArgumentNullException()
        {
            Action action = () => Identifier.From((string)null);

            action.Should()
                .ThrowArgumentNullException("value");
        }

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

        [TestCaseSource(nameof(ToStringTestCases))]
        public void Test_Identifier_ToString(ToStringTestCase testCase)
        {
            string result = testCase.Value.ToString();

            result.Should().Be(testCase.Expected);
        }

        static ToStringTestCase[] ToStringTestCases() => new ToStringTestCase[]
        {
            new ToStringTestCase(
                value: 0x546F6130,
                expected: "Toa0",
                description: "Test converting an identifier with only alphanumerical characters to a string"
            ),
            new ToStringTestCase(
                value: 0x546F617B,
                expected: "Toa|0x7B|",
                description: "Test converting an identifier with alphanumerical and symbols to a string"
            ),
            new ToStringTestCase(
                value: 0x101F9AEF,
                expected: "|0x10||0x1F||0x9A||0xEF|",
                description: "Test converting an identifier with only symbols to a string"
            ),
            new ToStringTestCase(
                value: Identifier.ZERO,
                expected: "|0x00||0x00||0x00||0x00|",
                description: "Test converting an identifier with value 0 to a string"
            )
        };

        public class ToStringTestCase : AbstractTestCase
        {
            public ToStringTestCase(Identifier value, string expected, string description) : base(description)
            {
                Value = value;
                Expected = expected;
            }

            public Identifier Value { get; }

            public string Expected { get; }
        }

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

        [TestCaseSource(nameof(EqualObjectsTestCases))]
        public void Test_Comparing_Equal_Objects(IComparisionTestCase<Identifier> testCase) => testCase.Execute();

        public static IComparisionTestCase<Identifier>[] EqualObjectsTestCases() => new IComparisionTestCase<Identifier>[]
        {
            ComparisionTestCase.CompareObjectAgainstItself(SampleIdentifier()),
            ComparisionTestCase.CompareTwoEqualObjects(SampleIdentifier)
        };

        [TestCaseSource(nameof(NotEqualObjectsTestCases))]
        public void Test_Comparing_NotEqual_Objects(IComparisionTestCase<Identifier> testCase) => testCase.Execute();

        public static IComparisionTestCase<Identifier>[] NotEqualObjectsTestCases() => new IComparisionTestCase<Identifier>[]
        {
            ComparisionTestCase.CompareTwoNotEqualObjects(SampleIdentifier(), SampleIdentifier().WithB0(0x01)),
            ComparisionTestCase.CompareTwoNotEqualObjects(SampleIdentifier(), SampleIdentifier().WithB1(0x01)),
            ComparisionTestCase.CompareTwoNotEqualObjects(SampleIdentifier(), SampleIdentifier().WithB2(0x01)),
            ComparisionTestCase.CompareTwoNotEqualObjects(SampleIdentifier(), SampleIdentifier().WithB3(0x01)),
            ComparisionTestCase.CompareTwoNotEqualObjects(SampleIdentifier(), SampleIdentifier().WithC0('A')),
            ComparisionTestCase.CompareTwoNotEqualObjects(SampleIdentifier(), SampleIdentifier().WithC1('B')),
            ComparisionTestCase.CompareTwoNotEqualObjects(SampleIdentifier(), SampleIdentifier().WithC2('C')),
            ComparisionTestCase.CompareTwoNotEqualObjects(SampleIdentifier(), SampleIdentifier().WithC3('D')),
            ComparisionTestCase.CompareTwoNotEqualObjects(SampleIdentifier(), (Identifier)1),
            ComparisionTestCase.CompareTwoNotEqualObjects(SampleIdentifier(), Identifier.From(new byte[] { 0x01, 0x02, 0x03, 0x04 })),
            ComparisionTestCase.CompareTwoNotEqualObjects(SampleIdentifier(), Identifier.From("BCDA")),
            ComparisionTestCase.CompareTwoNotEqualObjects((Identifier)0x11223344, (Identifier)0x11121314)
        };

        public static Identifier SampleIdentifier() => 0xAABBCCDD;
    }
}
