/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using SAGESharp.SLB.IO;
using SAGESharp.Testing;
using System;
using System.Collections.Generic;
using System.IO;

namespace SAGESharp.SLB.Level.IO
{
    class InfoBinaryWriterTests
    {
        private readonly Stream stream;

        private readonly ISLBBinaryWriter<Info> writer;

        public InfoBinaryWriterTests()
        {
            stream = Substitute.For<Stream>();
            writer = new InfoBinaryWriter(stream);
        }

        [SetUp]
        public void Setup()
        {
            stream.ClearReceivedCalls();
        }

        [Test]
        public void Test_Creating_An_InfoBinaryWriter_With_Null_Dependencies()
        {
            this.Invoking(_ => new InfoBinaryWriter(null)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Test_Writing_An_Info_Object()
        {
            var input = new Info()
            {
                LineSide = LineSide.Right,
                ConditionStart = 2,
                ConditionEnd = 3,
                StringLabel = 4,
                StringIndex = 5,
                Frames = new List<Frame>()
                {
                    new Frame(), new Frame(), new Frame(),
                    new Frame(), new Frame(), new Frame()
                }
            };

            var expected = new byte[]
            {
                0x01, 0x00, 0x00, 0x00, // LineSide
                0x02, 0x00, 0x00, 0x00, // ConditionStart
                0x03, 0x00, 0x00, 0x00, // ConditionEnd
                0x04, 0x00, 0x00, 0x00, // StringLabel
                0x05, 0x00, 0x00, 0x00, // StringIndex
                0x06, 0x00, 0x00, 0x00, // Frames count
                0x00, 0x00, 0x00, 0x00  // Frames offset (placeholder)
            };

            writer.WriteSLBObject(input);

            stream.Received().Write(Matcher.ForEquivalentArray(expected), 0, expected.Length);
        }

        [Test]
        public void Test_Writing_A_Null_Info_Object()
            => writer.Invoking(w => w.WriteSLBObject(null)).Should().Throw<ArgumentNullException>();
    }
}
