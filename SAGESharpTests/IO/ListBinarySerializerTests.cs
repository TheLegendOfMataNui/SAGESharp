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
using System.Collections.Generic;
using System.Linq;

namespace SAGESharp.IO
{
    class ListBinarySerializerTests
    {
        private readonly IBinaryReader reader;

        private readonly IBinarySerializer<string> stringSerializer;

        private readonly IBinarySerializer<IList<string>> serializer;

        public ListBinarySerializerTests()
        {
            reader = Substitute.For<IBinaryReader>();
            stringSerializer = Substitute.For<IBinarySerializer<string>>();
            serializer = new ListBinarySerializer<string>(stringSerializer);
        }

        [SetUp]
        public void Setup()
        {
            reader.ClearSubstitute();
            stringSerializer.ClearSubstitute();
        }

        [Test]
        public void Test_Reading_A_List()
        {
            IList<string> expected = new List<string>() { "a", "b", "c", "d", "e" };

            // Returns count and offset
            reader.ReadUInt32().Returns((uint)expected.Count, (uint)70);

            reader.Position.Returns(45);

            // Returns the entire "expected" list from serailizer.Read(reader)
            stringSerializer.Read(reader).Returns(expected[0], expected.Skip(1).Cast<object>().ToArray());

            serializer
                .Read(reader)
                .Should()
                .BeOfType<List<string>>()
                .Which
                .Should()
                .Equal(expected);

            Received.InOrder(() =>
            {
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.Position = 70;
                for (int n = 0; n < expected.Count; ++n)
                {
                    stringSerializer.Read(reader);
                }
                reader.Position = 45;
            });
        }

        [Test]
        public void Test_Building_A_ListBinarySerializer_With_A_Null_BinarySerializer()
        {
            Action action = () => new ListBinarySerializer<object>(null);

            action
                .Should()
                .Throw<ArgumentNullException>()
                .Where(e => e.Message.Contains("serializer"));
        }

        [Test]
        public void Test_Test_Reading_From_A_Null_Reader()
        {
            Action action = () => serializer.Read(null);

            action
                .Should()
                .Throw<ArgumentNullException>()
                .Where(e => e.Message.Contains("binaryReader"));
        }
    }
}
