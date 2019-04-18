/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using SAGESharp.Testing;
using System;
using System.IO;

namespace SAGESharp.SLB.Level.Conversation
{
    class FrameBinaryWriterTests
    {
        private readonly Stream stream;

        private readonly ISLBBinaryWriter<Frame> writer;

        public FrameBinaryWriterTests()
        {
            stream = Substitute.For<Stream>();
            writer = new FrameBinaryWriter(stream);
        }

        [SetUp]
        public void Setup()
        {
            stream.ClearReceivedCalls();
        }

        [Test]
        public void Test_Creating_A_FrameBinaryWriter_With_Null_Dependencies()
        {
            this.Invoking(_ => new FrameBinaryWriter(null)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Test_Writing_A_Frame()
        {
            var input = new Frame()
            {
                ToaAnimation = 0x14131211,
                CharAnimation = 0x24232221,
                CameraPositionTarget = 0x34333231,
                CameraDistance = 0x44434241,
                StringIndex = 0x54535251
            };

            var expected = new byte[]
            {
                0x11, 0x12, 0x13, 0x14, // ToaAnimation
                0x21, 0x22, 0x23, 0x24, // CharAnimation
                0x31, 0x32, 0x33, 0x34, // CameraPositionTarget
                0x41, 0x42, 0x43, 0x44, // CameraDistance
                0x51, 0x52, 0x53, 0x54, // StringIndex
                0x00, 0x00, 0x00, 0x00  // ConversationSounds offset (placeholder)
            };

            writer.WriteSLBObject(input);

            stream.Received().Write(Matcher.ForEquivalentArray(expected), 0, expected.Length);
        }

        [Test]
        public void Test_Writing_A_Null_Frame()
            => writer.Invoking(w => w.WriteSLBObject(null)).Should().Throw<ArgumentNullException>();
    }
}
