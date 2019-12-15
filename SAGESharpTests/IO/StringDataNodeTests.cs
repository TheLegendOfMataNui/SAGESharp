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
using System.Text;

namespace SAGESharp.IO
{
    class StringDataNodeTests
    {
        private readonly IBinaryReader binaryReader;

        private readonly IBinaryWriter binaryWriter;

        public StringDataNodeTests()
        {
            binaryReader = Substitute.For<IBinaryReader>();
            binaryWriter = Substitute.For<IBinaryWriter>();
        }

        [SetUp]
        public void Setup()
        {
            binaryReader.ClearSubstitute();
            binaryWriter.ClearSubstitute();
        }

        [Test]
        public void Test_Getting_Edges()
        {
            IDataNode dataNode = new StringDataNode();

            dataNode.Edges
                .Should()
                .BeEmpty();
        }

        [Test]
        public void Test_Reading_An_Offline_String()
        {
            IDataNode dataNode = new StringDataNode();
            string value = "value";

            binaryReader.ReadByte().Returns((byte)value.Length, (byte)0);
            binaryReader.ReadBytes(value.Length).Returns(Encoding.ASCII.GetBytes(value));

            object result = dataNode.Read(binaryReader);

            result.Should().Be(value);

            Received.InOrder(() =>
            {
                binaryReader.ReadByte();
                binaryReader.ReadBytes(value.Length);
                binaryReader.ReadByte();
            });
        }

        [Test]
        public void Test_Reading_An_Inline_String_Of_Exactly_The_Fixed_Size()
        {
            byte length = 20;
            IDataNode dataNode = new StringDataNode(length);
            string value = new string('d', length);

            binaryReader.ReadBytes(length).Returns(Encoding.ASCII.GetBytes(value));

            object result = dataNode.Read(binaryReader);

            result.Should().Be(value);

            binaryReader.Received().ReadBytes(length);
        }

        [Test]
        public void Test_Reading_An_Inline_String_Of_Not_Exactly_The_Fixed_Size()
        {
            byte length = 20;
            IDataNode dataNode = new StringDataNode(length);
            string value = new string('c', length / 2);
            byte[] bytes = new byte[length];

            Array.Copy(Encoding.ASCII.GetBytes(value), bytes, value.Length);
            binaryReader.ReadBytes(length).Returns(bytes);

            object result = dataNode.Read(binaryReader);

            result.Should().Be(value);

            binaryReader.Received().ReadBytes(length);
        }

        [Test]
        public void Test_Reading_A_String_From_A_Null_Reader()
        {
            IDataNode dataNode = new StringDataNode();

            Action action = () => dataNode.Read(null);
            action.Should()
                .ThrowArgumentNullException("binaryReader");
        }

        [Test]
        public void Test_Writing_An_Offline_String()
        {
            IDataNode dataNode = new StringDataNode();
            string value = "my value";

            dataNode.Write(binaryWriter, value);

            Received.InOrder(() =>
            {
                binaryWriter.WriteByte((byte)value.Length);
                binaryWriter.WriteBytes(Matcher.ForEquivalentArray(Encoding.ASCII.GetBytes(value)));
                binaryWriter.WriteByte(0);
            });
        }

        [Test]
        public void Test_Writing_An_Offline_String_With_Invalid_Length()
        {
            IDataNode dataNode = new StringDataNode();
            string value = new string('a', StringDataNode.OFFSET_STRING_MAX_LENGTH + 1);

            Action action = () => dataNode.Write(binaryWriter, value);
            action.Should()
                .ThrowExactly<ArgumentException>()
                .WithMessage($"String length is longer than {StringDataNode.OFFSET_STRING_MAX_LENGTH}.");
        }

        [Test]
        public void Test_Writing_An_Inline_String_Of_Exactly_The_Fixed_Size()
        {
            byte length = 20;
            IDataNode dataNode = new StringDataNode(length);
            string value = new string('b', length);

            dataNode.Write(binaryWriter, value);

            Received.InOrder(() =>
            {
                binaryWriter.WriteBytes(Matcher.ForEquivalentArray(Encoding.ASCII.GetBytes(value)));
            });
        }

        [Test]
        public void Test_Writing_An_Inline_String_Of_Not_Exactly_The_Fixed_Size()
        {
            byte length = 20;
            IDataNode dataNode = new StringDataNode(length);
            string value = new string('c', length / 2);

            dataNode.Write(binaryWriter, value);

            Received.InOrder(() =>
            {
                binaryWriter.WriteBytes(Matcher.ForEquivalentArray(Encoding.ASCII.GetBytes(value)));
                binaryWriter.WriteBytes(Matcher.ForEquivalentArray(new byte[length - value.Length]));
            });
        }

        [Test]
        public void Test_Writing_An_Inline_String_Longer_Than_The_Fixed_Size()
        {
            byte length = 20;
            IDataNode dataNode = new StringDataNode(length);
            string value = new string('a', length + 1);

            Action action = () => dataNode.Write(binaryWriter, value);

            action.Should()
                .ThrowExactly<ArgumentException>()
                .WithMessage($"String length is longer than {length}.");
        }

        [Test]
        public void Test_Writing_A_Non_String_Object()
        {
            IDataNode dataNode = new StringDataNode();
            int value = 1;

            Action action = () => dataNode.Write(binaryWriter, value);
            action.Should()
                .ThrowExactly<ArgumentException>()
                .WithMessage($"Cannot write value of type {value.GetType().Name} as a string.");
        }

        [Test]
        public void Test_Writing_A_String_To_A_Null_BinaryWriter()
        {
            IDataNode dataNode = new StringDataNode();

            Action action = () => dataNode.Write(null, string.Empty);
            action.Should()
                .ThrowArgumentNullException("binaryWriter");
        }

        [Test]
        public void Test_Writing_A_Null_String()
        {
            IDataNode dataNode = new StringDataNode();

            Action action = () => dataNode.Write(binaryWriter, null);
            action.Should()
                .ThrowArgumentNullException("value");
        }
    }
}
