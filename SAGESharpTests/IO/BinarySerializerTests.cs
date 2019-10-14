/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;
using SAGESharp.Testing;

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
