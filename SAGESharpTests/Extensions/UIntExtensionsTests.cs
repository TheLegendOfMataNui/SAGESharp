using NUnit.Framework;
using SAGESharp.Extensions;
using System;

namespace SAGESharpTests.Extensions
{
    [TestFixture]
    public class UIntExtensionsTests
    {
        private const uint INPUT_NUMBER = 0xAABBCCDD;

        [Test]
        public void TestGetByteWithAValidBytePosition()
        {
            Assert.That(INPUT_NUMBER.GetByte(0), Is.EqualTo(0xDD));
            Assert.That(INPUT_NUMBER.GetByte(1), Is.EqualTo(0xCC));
            Assert.That(INPUT_NUMBER.GetByte(2), Is.EqualTo(0xBB));
            Assert.That(INPUT_NUMBER.GetByte(3), Is.EqualTo(0xAA));
        }

        [Test]
        public void TestGetByteWithAnInvalidBytePosition()
        {
            Assert.That(() => INPUT_NUMBER.GetByte(4), Throws.InstanceOf(typeof(ArgumentOutOfRangeException)));
        }

        [Test]
        public void TestSetByteWithAValidBytePosition()
        {
            Assert.That(INPUT_NUMBER.SetByte(0, 0xFF), Is.EqualTo(0xAABBCCFF));
            Assert.That(INPUT_NUMBER.SetByte(1, 0xFF), Is.EqualTo(0xAABBFFDD));
            Assert.That(INPUT_NUMBER.SetByte(2, 0xFF), Is.EqualTo(0xAAFFCCDD));
            Assert.That(INPUT_NUMBER.SetByte(3, 0xFF), Is.EqualTo(0xFFBBCCDD));
        }

        [Test]
        public void TestSetByteWithAnInvalidBytePosition()
        {
            Assert.That(() => INPUT_NUMBER.SetByte(4, 0xFF), Throws.InstanceOf(typeof(ArgumentOutOfRangeException)));
        }
    }
}
