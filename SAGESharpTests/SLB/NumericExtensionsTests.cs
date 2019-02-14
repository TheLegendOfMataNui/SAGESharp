using FluentAssertions;
using NUnit.Framework;
using System;

namespace SAGESharp.SLB
{
    class NumericExtensionsTests
    {
        [TestCase(0xAABBCCDD, 0, ExpectedResult = 0xDD)]
        [TestCase(0xAABBCCDD, 1, ExpectedResult = 0xCC)]
        [TestCase(0xAABBCCDD, 2, ExpectedResult = 0xBB)]
        [TestCase(0xAABBCCDD, 3, ExpectedResult = 0xAA)]
        public byte Test_GetByte_With_A_Valid_Byte_Position(uint value, byte position)
            => value.GetByte(position);

        [Test]
        public void Test_GetByte_With_An_Invalid_Byte_Position()
            => default(uint).Invoking(n => n.GetByte(5)).Should().Throw<ArgumentOutOfRangeException>();

        [TestCase(0xAABBCCDD, 0, ExpectedResult = 0xAABBCCFF)]
        [TestCase(0xAABBCCDD, 1, ExpectedResult = 0xAABBFFDD)]
        [TestCase(0xAABBCCDD, 2, ExpectedResult = 0xAAFFCCDD)]
        [TestCase(0xAABBCCDD, 3, ExpectedResult = 0xFFBBCCDD)]
        public uint Test_SetByte_With_A_Valid_Byte_Position(uint value, byte position)
            => value.SetByte(position, 0xFF);

        [Test]
        public void Test_SetByte_With_An_Invalid_Byte_Position()
            => default(uint).Invoking(n => n.SetByte(5, 0)).Should().Throw<ArgumentOutOfRangeException>();

        [TestCase(0, ExpectedResult = 0x13141516)]
        [TestCase(1, ExpectedResult = 0x12131415)]
        [TestCase(2, ExpectedResult = 0x11121314)]
        [TestCase(3, ExpectedResult = 0x10111213)]
        public int Test_Convert_An_Array_To_An_Int32(int startIndex)
            => new byte[] { 0x16, 0x15, 0x14, 0x13, 0x12, 0x11, 0x10 }.ToInt32(startIndex);

        [TestCase(0, ExpectedResult = 0xFCFDFEFF)]
        [TestCase(1, ExpectedResult = 0xFBFCFDFE)]
        [TestCase(2, ExpectedResult = 0xFAFBFCFD)]
        [TestCase(3, ExpectedResult = 0xF9FAFBFC)]
        public uint Test_Convert_An_Array_To_An_UInt32(int startIndex)
            => new byte[] { 0xFF, 0xFE, 0xFD, 0xFC, 0xFB, 0xFA, 0xF9 }.ToUInt32(startIndex);
    }
}
