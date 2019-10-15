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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.IO
{
    class PaddingNodeTests
    {
        private const byte PADDING_SIZE = 23;

        private readonly IDataNode childNode;

        private readonly IBinaryReader binaryReader;

        private readonly IBinaryWriter binaryWriter;

        private readonly IDataNode paddingNode;

        public PaddingNodeTests()
        {
            childNode = Substitute.For<IDataNode>();
            binaryReader = Substitute.For<IBinaryReader>();
            binaryWriter = Substitute.For<IBinaryWriter>();
            paddingNode = new PaddingNode(PADDING_SIZE, childNode);
        }

        [SetUp]
        public void Setup()
        {
            binaryReader.ClearSubstitute();
            binaryWriter.ClearSubstitute();
        }

        [Test]
        public void Test_Creating_A_PaddingNode_With_Size_Zero()
        {
            Action action = () => new PaddingNode(0, childNode);

            action.Should()
                .ThrowExactly<ArgumentException>()
                .WithMessage("Padding size cannot be 0.");
        }

        [Test]
        public void Test_Creating_A_PaddingNode_With_A_Null_Child_Node()
        {
            Action action = () => new PaddingNode(1, null);

            action.Should()
                .ThrowArgumentNullException("childNode");
        }

        [Test]
        public void Test_PaddingNode_Should_Have_The_Edges_Of_The_ChildNode()
        {
            IReadOnlyList<IEdge> edges = new List<IEdge>();

            childNode.Edges.Returns(edges);

            paddingNode.Edges.Should().BeSameAs(edges);
        }

        [Test]
        public void Test_Reading_From_A_BinaryReader()
        {
            string expected = nameof(expected);

            childNode.Read(binaryReader).Returns(expected);

            object result = paddingNode.Read(binaryReader);

            Received.InOrder(() =>
            {
                childNode.Read(binaryReader);
                binaryReader.ReadBytes(PADDING_SIZE);
            });

            result.Should().Be(expected);
        }

        [Test]
        public void Test_Reading_From_A_Null_BinaryReader()
        {
            Action action = () => paddingNode.Read(null);

            action.Should()
                .ThrowArgumentNullException("binaryReader");
        }

        [Test]
        public void Test_Writing_An_Object_To_A_BinaryWriter()
        {
            string expected = nameof(expected);

            paddingNode.Write(binaryWriter, expected);

            Received.InOrder(() =>
            {
                childNode.Write(binaryWriter, expected);
                binaryWriter.WriteBytes(Matcher.ForEquivalentArray(new byte[PADDING_SIZE]));
            });
        }

        [Test]
        public void Test_Writing_An_Object_To_A_Null_BinaryWriter()
        {
            Action action = () => paddingNode.Write(null, string.Empty);

            action.Should()
                .ThrowArgumentNullException("binaryWriter");
        }

        [Test]
        public void Test_Writing_A_Null_Object_To_A_BinaryWriter()
        {
            Action action = () => paddingNode.Write(binaryWriter, null);

            action.Should()
                .ThrowArgumentNullException("value");
        }
    }
}
