/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using SAGESharp.SLB.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace SAGESharp.SLB.Level.Conversation
{
    class ConversationBinaryWriterTests
    {
        private readonly Stream stream = Substitute.For<Stream>();

        private readonly ISLBBinaryWriter<Character> characterWriter = Substitute.For<ISLBBinaryWriter<Character>>();

        private readonly ISLBBinaryWriter<Info> infoWriter = Substitute.For<ISLBBinaryWriter<Info>>();

        private readonly ISLBBinaryWriter<Frame> frameWriter = Substitute.For<ISLBBinaryWriter<Frame>>();

        private readonly ISLBBinaryWriter<string> stringWriter = Substitute.For<ISLBBinaryWriter<string>>();

        private readonly ISLBFooterWriter<IList<Character>> footerWriter = Substitute.For<ISLBFooterWriter<IList<Character>>>();

        private readonly ISLBBinaryWriter<IList<Character>> conversationWriter;

        public ConversationBinaryWriterTests()
        {
            conversationWriter = new ConversationBinaryWriter(stream, characterWriter, infoWriter, frameWriter, stringWriter, footerWriter);
        }

        [SetUp]
        public void Setup()
        {
            characterWriter.ClearReceivedCalls();
            infoWriter.ClearReceivedCalls();
            frameWriter.ClearReceivedCalls();
            stringWriter.ClearReceivedCalls();
            footerWriter.ClearReceivedCalls();
        }

        [Test]
        public void Test_Creating_A_ConversationBinaryWriter_With_Null_Dependencies()
        {
            this.Invoking(_ => new ConversationBinaryWriter(null, characterWriter, infoWriter, frameWriter, stringWriter, footerWriter))
                .Should().Throw<ArgumentNullException>();

            this.Invoking(_ => new ConversationBinaryWriter(stream, null, infoWriter, frameWriter, stringWriter, footerWriter))
                .Should().Throw<ArgumentNullException>();

            this.Invoking(_ => new ConversationBinaryWriter(stream, characterWriter, null, frameWriter, stringWriter, footerWriter))
                .Should().Throw<ArgumentNullException>();

            this.Invoking(_ => new ConversationBinaryWriter(stream, characterWriter, infoWriter, null, stringWriter, footerWriter))
                .Should().Throw<ArgumentNullException>();

            this.Invoking(_ => new ConversationBinaryWriter(stream, characterWriter, infoWriter, frameWriter, null, footerWriter))
                .Should().Throw<ArgumentNullException>();

            this.Invoking(_ => new ConversationBinaryWriter(stream, characterWriter, infoWriter, frameWriter, stringWriter, null))
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Test_Writing_Conversation()
        {
            var input = new List<Character>
            {
                new Character
                {
                    Entries = new List<Info>
                    {
                        new Info
                        {
                            Frames = new List<Frame>
                            {
                                new Frame { ConversationSounds = "A" },
                                new Frame { ConversationSounds = "B" }
                            }
                        },
                        new Info
                        {
                            Frames = new List<Frame>
                            {
                                new Frame { ConversationSounds = "C" },
                                new Frame { ConversationSounds = "D" }
                            }
                        }
                    }
                }
            };

            conversationWriter.WriteSLBObject(input);

            Received.InOrder(() =>
            {
                characterWriter.WriteSLBObject(input[0]);
                infoWriter.WriteSLBObject(input[0].Entries[0]);
                infoWriter.WriteSLBObject(input[0].Entries[1]);
                frameWriter.WriteSLBObject(input[0].Entries[0].Frames[0]);
                frameWriter.WriteSLBObject(input[0].Entries[0].Frames[1]);
                frameWriter.WriteSLBObject(input[0].Entries[1].Frames[0]);
                frameWriter.WriteSLBObject(input[0].Entries[1].Frames[1]);
                stringWriter.WriteSLBObject("A");
                stringWriter.WriteSLBObject("B");
                stringWriter.WriteSLBObject("C");
                stringWriter.WriteSLBObject("D");
                footerWriter.WriteFooter(input);
            });
        }

        [Test]
        public void Test_Writing_A_Null_Conversation()
            => conversationWriter.Invoking(cw => cw.WriteSLBObject(null)).Should().Throw<ArgumentNullException>();
    }
}
