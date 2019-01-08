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
            Assert.That(INPUT_NUMBER.GetByte(2), Is.EqualTo(0xBB));
        }

        [Test]
        public void TestGetByteWithAnInvalidBytePosition()
        {
            Assert.That(() => INPUT_NUMBER.GetByte(4), Throws.InstanceOf(typeof(ArgumentOutOfRangeException)));
        }

        [Test]
        public void TestSetByteWithAValidBytePosition()
        {
            Assert.That(INPUT_NUMBER.SetByte(2, 0xFF), Is.EqualTo(0xAAFFCCDD));
        }

        [Test]
        public void TestSetByteWithAnInvalidBytePosition()
        {
            Assert.That(() => INPUT_NUMBER.SetByte(4, 0xFF), Throws.InstanceOf(typeof(ArgumentOutOfRangeException)));
        }
    }
}
