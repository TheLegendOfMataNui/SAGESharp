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

namespace SAGESharp.SLB.Level.Conversation
{
    class CharacterBinaryWriterTests
    {
        private readonly Stream stream;

        private readonly ISLBBinaryWriter<Character> writer;

        public CharacterBinaryWriterTests()
        {
            stream = Substitute.For<Stream>();
            writer = new CharacterBinaryWriter(stream);
        }

        [SetUp]
        public void Setup()
        {
            stream.ClearReceivedCalls();
        }

        [Test]
        public void Test_Creating_An_CharacterBinaryWriter_With_Null_Dependencies()
        {
            this.Invoking(_ => new CharacterBinaryWriter(null)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Test_Writing_A_Character()
        {
            var input = new Character()
            {
                ToaName = 1,
                CharName = 2,
                CharCont = 3,
                Entries = new List<Info>()
                {
                    new Info(), new Info(),
                    new Info(), new Info()
                }
            };

            var expected = new byte[]
            {
                0x01, 0x00, 0x00, 0x00, // ToaName
                0x02, 0x00, 0x00, 0x00, // CharName
                0x03, 0x00, 0x00, 0x00, // CharCont
                0x04, 0x00, 0x00, 0x00, // Entries count
                0x00, 0x00, 0x00, 0x00  // Entries offset (placeholder)
            };

            writer.WriteSLBObject(input);

            stream.Received().Write(Matcher.ForEquivalentArray(expected), 0, expected.Length);
        }

        [Test]
        public void Test_Writing_A_Null_Character()
            => writer.Invoking(w => w.WriteSLBObject(null)).Should().Throw<ArgumentNullException>();
    }
}
