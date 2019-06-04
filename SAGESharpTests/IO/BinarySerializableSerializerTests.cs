/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;
using System;

namespace SAGESharp.IO
{
    class BinarySerializableSerializerTests
    {
        private readonly IBinaryReader reader = Substitute.For<IBinaryReader>();

        private readonly IBinaryWriter writer = Substitute.For<IBinaryWriter>();

        private readonly IBinarySerializer<BinarySerializable> serializer = new BinarySerializableSerializer<BinarySerializable>();

        [SetUp]
        public void Setup()
        {
            reader.ClearReceivedCalls();
            writer.ClearSubstitute();
        }

        [Test]
        public void Test_Reading_An_IBinarySerializable_Class()
        {
            uint value = 0xFFEECCDD;

            reader.ReadUInt32().Returns(value);

            serializer.Read(reader).Value.Should().Be(value);

            reader.Received().ReadUInt32();
        }

        [Test]
        public void Test_Writing_An_IBinarySerializable_Class()
        {
            BinarySerializable serializable = new BinarySerializable()
            {
                Value = 0xFFEECCDD
            };

            serializer.Write(writer, serializable);

            writer.Received().WriteUInt32(serializable.Value);
        }

        [Test]
        public void Test_Reading_From_A_Null_BinaryReader()
        {
            Action action = () => serializer.Read(null);

            action
                .Should()
                .Throw<ArgumentNullException>()
                .Where(e => e.Message.Contains("binaryReader"));
        }

        [Test]
        public void Test_Writing_To_A_Null_BinaryWriter()
        {
            Action action = () => serializer.Write(null, new BinarySerializable());

            action.Should()
                .Throw<ArgumentNullException>()
                .Where(e => e.Message.Contains("binaryWriter"));
        }

        [Test]
        public void Test_Writing_A_Null_Object()
        {
            Action action = () => serializer.Write(writer, null);

            action.Should()
                .Throw<ArgumentNullException>()
                .Where(e => e.Message.Contains("value"));
        }

        [Test]
        public void Test_Creating_A_BinarySerializableSerializer_For_A_Type_With_No_Public_Empty_Constructor()
        {
            this.Invoking(_ => new BinarySerializableSerializer<BinarySerializableWithPrivateConstructor>())
                .Should()
                .Throw<BadTypeException>()
                .Where(e => e.Message.Contains($"Type {nameof(BinarySerializableWithPrivateConstructor)} has no public constructor with no arguments"));
        }

        class BinarySerializable : IBinarySerializable
        {
            public uint Value { get; set; }

            public void Read(IBinaryReader binaryReader) => Value = binaryReader.ReadUInt32();

            public void Write(IBinaryWriter binaryWriter) => binaryWriter.WriteUInt32(Value);
        }

        class BinarySerializableWithPrivateConstructor : IBinarySerializable
        {
            private BinarySerializableWithPrivateConstructor()
            {
            }

            public void Read(IBinaryReader binaryReader) => throw new NotImplementedException();

            public void Write(IBinaryWriter binaryWriter) => throw new NotImplementedException();
        }
    }
}
