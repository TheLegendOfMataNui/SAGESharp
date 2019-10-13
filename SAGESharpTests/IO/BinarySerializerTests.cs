/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;
using SAGESharp.Testing;
using System;
using System.Collections.Generic;
using System.IO;

namespace SAGESharp.IO
{
    class BinarySerializerTests
    {
        private readonly IBinaryWriter binaryWriter = Substitute.For<IBinaryWriter>();

        [SetUp]
        public void Setup()
        {
            binaryWriter.ClearSubstitute();
        }

        [Test]
        public void Test_OffsetWriter()
        {
            MemoryStream stream = new MemoryStream();
            IBinaryWriter binaryWriter = Writer.ForStream(stream);

            IReadOnlyList<uint> values = new List<uint>
            {
                0x11223344, 0xAABBCCDD, 0x99887766
            };

            binaryWriter.WriteUInt32(values[0]);
            binaryWriter.WriteUInt32(values[1]);
            long offset = binaryWriter.Position;
            binaryWriter.WriteUInt32(0); // Where the offset will be written
            binaryWriter.WriteUInt32(values[2]);

            long endPosition = binaryWriter.Position;

            BinarySerializer.OffsetWriter(binaryWriter, (uint)offset);

            binaryWriter.Position.Should().Be(endPosition);

            IBinaryReader binaryReader = Reader.ForStream(new MemoryStream(stream.ToArray()));

            binaryReader.ReadUInt32().Should().Be(values[0]);
            binaryReader.ReadUInt32().Should().Be(values[1]);
            binaryReader.ReadUInt32().Should().Be((uint)endPosition);
            binaryReader.ReadUInt32().Should().Be(values[2]);
        }

        [Test]
        public void Test_OffsetWriter_With_A_Very_Large_Offset()
        {
            long badOffset = (long)uint.MaxValue + 1;
            Action action = () => BinarySerializer.OffsetWriter(binaryWriter, 0);

            binaryWriter.Position.Returns(badOffset);

            action.Should()
                .ThrowExactly<ArgumentException>()
                .WithMessage($"Offset 0x{badOffset:X} is larger than {sizeof(uint)} bytes.");

            binaryWriter.DidNotReceive().WriteUInt32(Arg.Any<uint>());
        }

        [Test]
        public void Test_Footer_Aligner_When_The_Position_Needs_To_Be_Aligned()
        {
            binaryWriter.Position.Returns(0x01);

            BinarySerializer.FooterAligner(binaryWriter);

            binaryWriter.WriteBytes(Matcher.ForEquivalentArray(new byte[3]));
        }

        [Test]
        public void Test_Footer_Aligner_When_The_Position_Doesnt_Need_To_Be_Aligned()
        {
            binaryWriter.Position.Returns(0x04);

            BinarySerializer.FooterAligner(binaryWriter);

            binaryWriter.DidNotReceive().WriteBytes(Arg.Any<byte[]>());
        }
    }
}
