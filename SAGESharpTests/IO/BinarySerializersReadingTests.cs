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
    }
}