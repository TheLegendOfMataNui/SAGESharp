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
using System.IO;
using System.Reflection;

namespace SAGESharp.IO
{
    class BinarySerializerTests
    {
        private readonly IBinaryWriter binaryWriter = Substitute.For<IBinaryWriter>();

        private readonly IBinarySerializer<string> binarySerializer = Substitute.For<IBinarySerializer<string>>();

        [SetUp]
        public void Setup()
        {
            binaryWriter.ClearSubstitute();
        }

        [Test]
        public void Test_Footer_Aligner_When_The_Position_Needs_To_Be_Aligned()
        {
            binaryWriter.Position.Returns(0x01);

            BinarySerializer.AlignFooter(binaryWriter);

            binaryWriter.WriteBytes(Matcher.ForEquivalentArray(new byte[3]));
        }

        [Test]
        public void Test_Footer_Aligner_When_The_Position_Doesnt_Need_To_Be_Aligned()
        {
            binaryWriter.Position.Returns(0x04);

            BinarySerializer.AlignFooter(binaryWriter);

            binaryWriter.DidNotReceive().WriteBytes(Arg.Any<byte[]>());
        }

        [Test]
        public void Test_Reading_With_A_BinarySerializer_Directly_From_A_Stream()
        {
            string expected = nameof(expected);
            WithDummyStream(stream =>
            {
                binarySerializer.Read(Arg.Any<IBinaryReader>()).Returns(expected);

                string result = binarySerializer.Read(stream);

                binarySerializer.Received().Read(Arg.Do<IBinaryReader>(binaryReader =>
                {
                    binaryReader.Should().BeOfType<BinaryReaderWrapper>();

                    typeof(BinaryReaderWrapper)
                        .GetField("realReader", BindingFlags.Instance | BindingFlags.NonPublic)
                        .GetValue(binaryReader)
                        .Should()
                        .BeSameAs(stream);
                }));

                result.Should().Be(expected);
            });
        }

        [Test]
        public void Test_Reading_With_A_Null_BinarySerializer_Directly_From_A_Stream()
        {
            IBinarySerializer<string> binarySerializer = null;
            WithDummyStream(stream =>
            {
                Action action = () => binarySerializer.Read(stream);

                action.Should()
                    .ThrowArgumentNullException("binarySerializer");
            });
        }

        [Test]
        public void Test_Reading_With_A_BinarySerializer_Directly_From_A_Null_Stream()
        {
            Stream stream = null;
            Action action = () => binarySerializer.Read(stream);

            action.Should()
                .ThrowArgumentNullException("stream");
        }

        [Test]
        public void Test_Writing_With_A_BinarySerializer_Directly_To_A_Stream()
        {
            string value = nameof(value);
            WithDummyStream(stream =>
            {
                binarySerializer.Write(stream, value);

                binarySerializer.Received().Write(Arg.Do<IBinaryWriter>(binaryReader =>
                {
                    binaryReader.Should().BeOfType<BinaryWriterWrapper>();

                    typeof(BinaryWriterWrapper)
                        .GetField("realReader", BindingFlags.Instance | BindingFlags.NonPublic)
                        .GetValue(binaryReader)
                        .Should()
                        .BeSameAs(stream);
                }), value);
            });
        }

        [Test]
        public void Test_Writing_With_A_Null_BinarySerializer_Directly_To_A_Stream()
        {
            IBinarySerializer<string> binarySerializer = null;
            WithDummyStream(stream =>
            {
                Action action = () => binarySerializer.Write(stream, string.Empty);

                action.Should()
                    .ThrowArgumentNullException("binarySerializer");
            });
        }

        [Test]
        public void Test_Writing_With_A_BinarySerializer_Directly_To_A_Null_Stream()
        {
            Stream stream = null;
            Action action = () => binarySerializer.Write(stream, string.Empty);

            action.Should()
                .ThrowArgumentNullException("stream");
        }

        [Test]
        public void Test_Writing_With_A_BinarySerializer_Directly_To_A_Stream_A_Null_Value()
        {
            WithDummyStream(stream =>
            {
                Action action = () => binarySerializer.Write(stream, null);

                action.Should()
                    .ThrowArgumentNullException("value");
            });
        }

        private static void WithDummyStream(Action<Stream> action)
        {
            using (Stream stream = new MemoryStream())
            {
                action(stream);
            }
        }
    }
}
