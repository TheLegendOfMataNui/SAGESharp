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

            AssertEmptyIdentifier(identifier);
        }

        [Test]
        public void TestCreateIdentifierFromInteger()
        {
            var identifier = new Identifier(0x44434241);

            AssertIdentifierWithTestValue(identifier);
        }

        [Test]
        public void TestCreateIdentifierFromArraySizeSmallerThan4()
        {
            var identifier = new Identifier(new byte[0]);

            AssertEmptyIdentifier(identifier);
        }

        [Test]
        public void TestCreateIdentifierFromArraySize4()
        {
            var identifier = new Identifier(new byte[]
            {
                0x41,
                0x42,
                0x43,
                0x44
            });

            AssertIdentifierWithTestValue(identifier);
        }

        [Test]
        public void TestCreateIdentifierFromArraySizeGreaterThan4()
        {
            var identifier = new Identifier(new byte[]
            {
                0x41,
                0x42,
                0x43,
                0x44,
                0x45,
                0x46,
                0x47,
                0x48
            });

            AssertIdentifierWithTestValue(identifier);
        }

        [Test]
        public void TestCreateIdentifierFromStringSizeSmallerThan4()
        {
            var identifier = new Identifier(string.Empty);

            AssertEmptyIdentifier(identifier);
        }

        [Test]
        public void TestCreateIdentifierFromStringSize4()
        {
            var identifier = new Identifier("ABCD");

            AssertIdentifierWithTestValue(identifier);
        }

        [Test]
        public void TestCreateIdentifierFromStringSizeGreaterThan4()
        {
            var identifier = new Identifier("ABCDEFGH");

            AssertIdentifierWithTestValue(identifier);
        }

        [Test]
        public void TestSettingIdentifierValuesByValue()
        {
            var identifier = new Identifier(0x44434241);

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

        [Test]
        public void TestComparingTwoEqualIdentifiers()
        {
            var a = new Identifier("ABCD");
            var b = new Identifier("ABCD");

            Assert.IsTrue(a.Equals((object)b));
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a == b);
            Assert.IsFalse(a != b);
        }

        [Test]
        public void TestComparingTwoInequalIdentifiers()
        {
            var a = new Identifier("ABCD");
            var b = new Identifier(0xAABBCCDD);

            Assert.IsFalse(a.Equals((object)b));
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [Test]
        public void TestIdentifierHashCode()
        {
            var a = new Identifier(0x11223344);

            Assert.AreEqual(a.GetHashCode(), 0x11223344);
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
            Assert.That(identifier.ToString(), Is.EqualTo("ABCD"));

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
