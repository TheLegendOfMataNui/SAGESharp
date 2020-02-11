﻿/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;
using NUtils.Extensions;
using SAGESharp.IO;
using System;
using System.Collections.Generic;

namespace SAGESharp.Tests.IO
{
    class TreeBinarySerializerTests
    {
        private readonly TreeBinarySerializer<string>.TreeReader treeReader;

        private readonly TreeBinarySerializer<string>.TreeWriter treeWriter;

        private readonly IDataNode rootNode;

        private readonly Action<IBinaryWriter> footerAligner;

        private readonly IBinaryReader binaryReader;

        private readonly IBinaryWriter binaryWriter;

        private readonly IBinarySerializer<string> serializer;

        public TreeBinarySerializerTests()
        {
            treeReader = Substitute.For<TreeBinarySerializer<string>.TreeReader>();
            treeWriter = Substitute.For<TreeBinarySerializer<string>.TreeWriter>();
            rootNode = Substitute.For<IDataNode>();
            footerAligner = Substitute.For<Action<IBinaryWriter>>();
            binaryReader = Substitute.For<IBinaryReader>();
            binaryWriter = Substitute.For<IBinaryWriter>();
            serializer = new TreeBinarySerializer<string>(treeReader, treeWriter, rootNode, footerAligner);
        }

        [SetUp]
        public void Setup()
        {
            treeReader.ClearSubstitute();
            treeWriter.ClearSubstitute();
            rootNode.ClearSubstitute();
            footerAligner.ClearSubstitute();
            binaryReader.ClearSubstitute();
            binaryWriter.ClearSubstitute();
        }

        [Test]
        public void Test_Creating_A_Serializer_With_Null_Reader()
        {
            Action action = () => new TreeBinarySerializer<string>(null, treeWriter, rootNode, footerAligner);

            action.Should()
                .ThrowArgumentNullException("treeReader");
        }

        [Test]
        public void Test_Creating_A_Serializer_With_Null_Writer()
        {
            Action action = () => new TreeBinarySerializer<string>(treeReader, null, rootNode, footerAligner);

            action.Should()
                .ThrowArgumentNullException("treeWriter");
        }

        [Test]
        public void Test_Creating_A_Serializer_With_Null_RootNode()
        {
            Action action = () => new TreeBinarySerializer<string>(treeReader, treeWriter, null, footerAligner);

            action.Should()
                .ThrowArgumentNullException("rootNode");
        }

        [Test]
        public void Test_Creating_A_Serializer_With_Null_FooterAligner()
        {
            Action action = () => new TreeBinarySerializer<string>(treeReader, treeWriter, rootNode, null);

            action.Should()
                .ThrowArgumentNullException("footerAligner");
        }

        [Test]
        public void Test_Reading_An_Object()
        {
            string value = "value";

            treeReader(binaryReader, rootNode).Returns(value);

            string result = serializer.Read(binaryReader);

            result.Should()
                .BeSameAs(value);
        }

        [Test]
        public void Test_Reading_An_Object_From_A_Null_BinaryReader()
        {
            Action action = () => serializer.Read(null);

            action.Should()
                .ThrowArgumentNullException("binaryReader");
        }

        [Test]
        public void Test_Writing_An_Object()
        {
            string value = "value";
            IReadOnlyList<uint> offsets = new List<uint> { 1, 2, 3 };

            treeWriter(binaryWriter, value, rootNode).Returns(offsets);

            serializer.Write(binaryWriter, value);

            Received.InOrder(() =>
            {
                treeWriter(binaryWriter, value, rootNode);
                footerAligner(binaryWriter);
                offsets.ForEach(binaryWriter.WriteUInt32);
                binaryWriter.WriteInt32(offsets.Count);
                binaryWriter.WriteUInt32(TreeBinarySerializer<object>.FOOTER_MAGIC_NUMBER);
            });
        }

        [Test]
        public void Test_Writing_An_Object_To_A_Null_BinaryWriter()
        {
            Action action = () => serializer.Write(null, string.Empty);

            action.Should()
                .ThrowArgumentNullException("binaryWriter");
        }

        [Test]
        public void Test_Writing_A_Null_Object()
        {
            Action action = () => serializer.Write(binaryWriter, null);

            action.Should()
                .ThrowArgumentNullException("value");
        }
    }
}
