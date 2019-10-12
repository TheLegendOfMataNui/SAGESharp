/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NUnit.Framework;
using System;

namespace SAGESharp.Utils
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
    }
}
