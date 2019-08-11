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

namespace SAGESharp.IO
{
    class TreeWriterTests
    {
        private readonly IBinaryWriter binaryWriter;

        private readonly ITreeWriter treeWriter;

        public TreeWriterTests()
        {
            binaryWriter = Substitute.For<IBinaryWriter>();
            treeWriter = new TreeWriter();
        }

        [SetUp]
        public void Setup()
        {
            binaryWriter.ClearSubstitute();
        }

        #region Null checks
        [Test]
        public void Test_Writing_A_Tree_With_A_Null_BinaryWriter()
        {
            Action action = () => treeWriter.Write(null, new object(), Substitute.For<IDataNode>());

            action
                .Should()
                .ThrowArgumentNullException("binaryWriter");
        }

        [Test]
        public void Test_Writing_A_Tree_With_A_Null_Value()
        {
            Action action = () => treeWriter.Write(binaryWriter, null, Substitute.For<IDataNode>());

            action
                .Should()
                .ThrowArgumentNullException("value");
        }

        [Test]
        public void Test_Writing_A_Tree_With_A_Null_RootNode()
        {
            Action action = () => treeWriter.Write(binaryWriter, new object(), null);

            action
                .Should()
                .ThrowArgumentNullException("rootNode");
        }
        #endregion
    }
}
