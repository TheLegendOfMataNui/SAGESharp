/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;
using SAGESharp.SLB;
using System.Collections.Generic;
using System.Linq;

namespace SAGESharp.IO
{
    class BinarySerializersReadingTests
    {
        private readonly IBinaryReader reader = Substitute.For<IBinaryReader>();

        private readonly IBinarySerializerFactory factory = Substitute.For<IBinarySerializerFactory>();

        [SetUp]
        public void Setup()
        {
            reader.ClearSubstitute();
            factory.ClearSubstitute();
        }

        [Test]
        public void Test_Reading_An_Identifier_With_CastSerializer()
        {
            var value = 0xFEDCBA98;
            var innerSerializer = Substitute.For<IBinarySerializer<uint>>();

            innerSerializer.Read(reader).Returns(value);

            new CastSerializer<Identifier, uint>(innerSerializer)
                .Read(reader)
                .Should()
                .Be((Identifier)value);

            innerSerializer.Received().Read(reader);
        }

        [Test]
        public void Test_Reading_An_Enum_With_CastSerializer()
        {
            var innerSerializer = Substitute.For<IBinarySerializer<byte>>();

            innerSerializer.Read(reader).Returns((byte)TestEnum.A);

            new CastSerializer<TestEnum, byte>(innerSerializer)
                .Read(reader)
                .Should()
                .Be(TestEnum.A);

            innerSerializer.Received().Read(reader);
        }

        enum TestEnum : byte
        {
            A = 0xAB,
            B = 0xBB
        }

        [Test]
        public void Test_Reading_A_List()
        {
            var expected = new List<char>() { 'a', 'b', 'c', 'd', 'e' };
            var serializer = Substitute.For<IBinarySerializer<char>>();

            // Returns count and offset
            reader.ReadUInt32().Returns((uint)expected.Count, (uint)70);

            reader.Position.Returns(45);

            // Returns the entire "expected" list from serailizer.Read(reader)
            serializer.Read(reader).Returns(expected[0], expected.Skip(1).Cast<object>().ToArray());

            new ListBinarySerializer<char>(serializer)
                .Read(reader)
                .Should()
                .BeOfType<List<char>>()
                .Which
                .Should()
                .Equal(expected);

            Received.InOrder(() =>
            {
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.Position = 70;
                foreach (var _ in expected)
                {
                    serializer.Read(reader);
                }
                reader.Position = 45;
            });
        }
    }
}
