using NUnit.Framework;
using SAGESharp.Slb;

namespace SAGESharpTests.Slb
{
    [TestFixture]
    public class IdentifierTests
    {
        [Test]
        public void TestCreateEmptyIdentifier()
        {
            var identifier = new Identifier();
            var expectedString = new string(Identifier.EMPY_CHAR, 4);

            Assert.That(identifier.Value, Is.EqualTo(0));
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

        [Test]
        public void TestCreateIdentifierFromInteger()
        {
            var identifier = new Identifier(0x44434241);

            AssertIdentifierWithTestValue(identifier);
        }

        [Test]
        public void TestSettingIdentifierValuesByValue()
        {
            var identifier = new Identifier
            {
                Value = 0x44434241
            };

            AssertIdentifierWithTestValue(identifier);
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

        private static void AssertIdentifierWithTestValue(Identifier identifier)
        {
            // Test value is the equivalent of:
            // C0 = 'A', C1 = 'B', C2 = 'C', C3 = 'D'

            Assert.That(identifier.Value, Is.EqualTo(0x44434241));
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
