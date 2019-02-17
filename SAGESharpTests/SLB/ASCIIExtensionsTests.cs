using FluentAssertions;
using NUnit.Framework;

namespace SAGESharp.SLB
{
    class ASCIIExtensionsTests
    {
        private const byte TEST_BYTE = 0x2B;

        private const char TEST_CHAR = '+';

        [Test]
        public void Test_Converting_A_Byte_To_An_ASCII_Char()
            => TEST_BYTE.ToASCIIChar().Should().Be(TEST_CHAR);

        [Test]
        public void Test_Converting_A_Char_To_An_ASCII_Byte()
            => TEST_CHAR.ToASCIIByte().Should().Be(TEST_BYTE);

        [TestCase(0x34, ExpectedResult = true)] // '4' in ASCII
        [TestCase(0x41, ExpectedResult = false)] // 'A' in ASCII
        public bool Test_IsASCIIDigit(byte value)
            => value.IsASCIIDigit();

        [TestCase(0x41, ExpectedResult = true)] // 'A' in ASCII
        [TestCase(0x61, ExpectedResult = false)] // 'a' in ASCII
        public bool Test_IsASCIIUppercaseLetter(byte value)
            => value.IsASCIIUppercaseLetter();

        [TestCase(0x61, ExpectedResult = true)] // 'a' in ASCII
        [TestCase(0x41, ExpectedResult = false)] // 'A' in ASCII
        public bool Test_IsASCIILowercaseLetter(byte value)
            => value.IsASCIILowercaseLetter();
    }
}
