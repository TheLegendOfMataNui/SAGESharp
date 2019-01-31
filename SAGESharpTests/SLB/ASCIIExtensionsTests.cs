using NUnit.Framework;
using SAGESharp.SLB;

namespace SAGESharpTests.SLB
{
    [TestFixture]
    public class ASCIIExtensionsTests
    {
        private const byte RANDOM_BYTE = 0x2B;

        private const char RANDOM_CHAR = '+';

        [Test]
        public void TestConvertingAByteToAnASCIIChar()
        {
            Assert.That(RANDOM_BYTE.ToASCIIChar(), Is.EqualTo(RANDOM_CHAR));
        }

        [Test]
        public void TestConvertingACharToAnASCIIByte()
        {
            Assert.That(RANDOM_CHAR.ToASCIIByte(), Is.EqualTo(RANDOM_BYTE));
        }

        [Test]
        public void TestIsAnASCIIDigitWithADigit()
        {
            byte value = 0x34;

            Assert.That(value.IsASCIIDigit(), Is.True);
        }

        [Test]
        public void TestIsAnASCIIDigitWithANonDigit()
        {
            Assert.That(RANDOM_BYTE.IsASCIIDigit(), Is.False);
        }

        [Test]
        public void TestIsAnASCIIUppercaseLetterWithAnUppercaseLatter()
        {
            byte value = 0x48;

            Assert.That(value.IsASCIIUppercaseLetter(), Is.True);
        }

        [Test]
        public void TestIsAnASCIIUppercaseLetterWithANonUppercaseLatter()
        {
            Assert.That(RANDOM_BYTE.IsASCIIUppercaseLetter(), Is.False);
        }

        [Test]
        public void TestIsAnASCIILowercaseLetterWithALowercaseLatter()
        {
            byte value = 0x70;

            Assert.That(value.IsASCIILowercaseLetter(), Is.True);
        }

        [Test]
        public void TestIsAnASCIILowerrcaseLetterWithANonUppercaseLatter()
        {
            Assert.That(RANDOM_BYTE.IsASCIILowercaseLetter(), Is.False);
        }
    }
}
