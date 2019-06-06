/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;

namespace SAGESharp.IO
{
    class StringBinarySerializerTests
    {
        private readonly IBinaryReader reader = Substitute.For<IBinaryReader>();

        private readonly IBinarySerializer<string> serializer = new StringBinarySerializer();

        [SetUp]
        public void Setup()
        {
            reader.ClearSubstitute();
        }

        [Test]
        public void Test_Reading_A_String()
        {
            var expected = "hello world";

            reader.ReadUInt32().Returns((uint)30);
            reader.Position.Returns(50);
            reader.ReadByte().Returns((byte)expected.Length);
            reader.ReadBytes(expected.Length).Returns(new byte[]
            {
                // "hello"
                0x68, 0x65, 0x6C, 0x6C, 0x6F,
                // " "
                0x20,
                // "world"
                0x77, 0x6F, 0x72, 0x6C, 0x64,
            });

            serializer
                .Read(reader)
                .Should()
                .BeOfType<string>()
                .Which
                .Should()
                .Be(expected);

            Received.InOrder(() =>
            {
                reader.ReadUInt32();
                reader.Position = 30;
                reader.ReadByte();
                reader.ReadBytes(expected.Length);
                reader.Position = 50;
            });
        }
    }
}
