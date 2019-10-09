/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;
using SAGESharp.IO.Trees;
using SAGESharp.Testing;
using System;
using System.Collections.Generic;

namespace SAGESharp.IO
{
    class TreeWriterTests
    {
        private readonly IBinaryWriter binaryWriter;

        private readonly ITreeWriter treeWriter;

        private readonly TreeWriter.OffsetWriter offsetWriter;

        public TreeWriterTests()
        {
            binaryWriter = Substitute.For<IBinaryWriter>();
            offsetWriter = Substitute.For<TreeWriter.OffsetWriter>();
            treeWriter = new TreeWriter(offsetWriter);
        }

        [SetUp]
        public void Setup()
        {
            binaryWriter.ClearSubstitute();
            offsetWriter.ClearSubstitute();
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

        #region Tree with height 1
        [Test]
        public void Test_Writing_An_Instance_Of_A_Tree_With_Height_1()
        {
            IDataNode rootNode = TreeWithHeight1.Build();
            TreeWithHeight1.Class value = new TreeWithHeight1.Class
            {
                Int = 1,
                Float = 2.5f,
                Byte = 3
            };

            treeWriter.Write(binaryWriter, value, rootNode)
                .Should()
                .BeEmpty();

            Received.InOrder(() => VerifyWriteTreeWithHeight1(rootNode, value));
        }

        private void VerifyWriteTreeWithHeight1(IDataNode node, TreeWithHeight1.Class value)
        {
            node.Write(binaryWriter, value);
            (node.Edges[0].ChildNode as IDataNode).Write(binaryWriter, value.Int);
            (node.Edges[1].ChildNode as IDataNode).Write(binaryWriter, value.Float);
            (node.Edges[2].ChildNode as IDataNode).Write(binaryWriter, value.Byte);
        }
        #endregion

        #region Tree with height 3
        [Test]
        public void Test_Writing_An_Instance_Of_A_Tree_With_Height_3()
        {
            IDataNode rootNode = TreeWithHeight3.Build();
            TreeWithHeight3.Class value = new TreeWithHeight3.Class
            {
                Child1 = new TreeWithHeight2.Class
                {
                    Long = 1,
                    Child = new TreeWithHeight1.Class
                    {
                        Int = 2,
                        Float = 3.5f,
                        Byte = 4
                    }
                },
                Child2 = new TreeWithHeight2.Class
                {
                    Long = 5,
                    Child = new TreeWithHeight1.Class
                    {
                        Int = 6,
                        Float = 7.5f,
                        Byte = 8
                    }
                }
            };

            treeWriter.Write(binaryWriter, value, rootNode)
                .Should()
                .BeEmpty();

            Received.InOrder(() => VerifyWriteTreeWithHeight3(rootNode, value));
        }

        private void VerifyWriteTreeWithHeight2(IDataNode node, TreeWithHeight2.Class value)
        {
            node.Write(binaryWriter, value);
            (node.Edges[0].ChildNode as IDataNode).Write(binaryWriter, value.Long);
            VerifyWriteTreeWithHeight1(node.Edges[1].ChildNode as IDataNode, value.Child);
        }

        private void VerifyWriteTreeWithHeight3(IDataNode node, TreeWithHeight3.Class value)
        {
            node.Write(binaryWriter, value);
            VerifyWriteTreeWithHeight2(node.Edges[0].ChildNode as IDataNode, value.Child1);
            VerifyWriteTreeWithHeight2(node.Edges[1].ChildNode as IDataNode, value.Child2);
        }
        #endregion

        #region Tree with nested lists
        [Test]
        public void Test_Writing_An_Instance_Of_A_Tree_With_Nested_Lists()
        {
            uint offsetPosition1 = 20, offsetPosition2 = 30;
            IDataNode rootNode = TreeWithNestedLists.Build();
            TreeWithNestedLists.Class value = new TreeWithNestedLists.Class
            {
                List1 = new List<TreeWithHeight1.Class>
                {
                    new TreeWithHeight1.Class
                    {
                        Int = 1,
                        Float = 2.4f,
                        Byte = 3
                    },
                    new TreeWithHeight1.Class
                    {
                        Int = 4,
                        Float = 5.5f,
                        Byte = 6
                    }
                },
                List2 = new List<TreeWithHeight2.Class>
                {
                    new TreeWithHeight2.Class
                    {
                        Long = 7,
                        Child = new TreeWithHeight1.Class
                        {
                            Int = 8,
                            Float = 9.6f,
                            Byte = 10
                        }
                    },
                    new TreeWithHeight2.Class
                    {
                        Long = 11,
                        Child = new TreeWithHeight1.Class
                        {
                            Int = 12,
                            Float = 13.7f,
                            Byte = 14
                        }
                    }
                }
            };

            (rootNode.Edges[0].ChildNode as IOffsetNode)
                .Write(binaryWriter, Arg.Any<object>())
                .Returns(offsetPosition1);

            (rootNode.Edges[1].ChildNode as IOffsetNode)
                .Write(binaryWriter, Arg.Any<object>())
                .Returns(offsetPosition2);

            treeWriter.Write(binaryWriter, value, rootNode)
                .Should()
                .Equal(offsetPosition1, offsetPosition2);

            Received.InOrder(() => VerifyWriteTreeWithNestedLists(rootNode, value, offsetPosition1, offsetPosition2));
        }

        private void VerifyWriteTreeWithNestedLists(
            IDataNode node,
            TreeWithNestedLists.Class value,
            uint offsetPosition1,
            uint offsetPosition2
        ) {
            node.Write(binaryWriter, value);

            IListNode listNode1 = node.Edges[0].ChildNode as IListNode;
            listNode1.Write(binaryWriter, value.List1);

            IListNode listNode2 = node.Edges[1].ChildNode as IListNode;
            listNode2.Write(binaryWriter, value.List2);

            offsetWriter(binaryWriter, offsetPosition1);
            foreach (var entry in value.List1)
            {
                VerifyWriteTreeWithHeight1(listNode1.ChildNode, entry);
            }

            offsetWriter(binaryWriter, offsetPosition2);
            foreach (var entry in value.List2)
            {
                VerifyWriteTreeWithHeight2(listNode2.ChildNode, entry);
            }
        }
        #endregion

        #region Tree with node at offset
        [Test]
        public void Test_Writing_An_Instance_Of_A_Tree_With_A_Node_At_Offset()
        {
            uint offset = 15;
            IDataNode rootNode = TreeWithNodeAtOffset.Build();
            TreeWithNodeAtOffset.Class value = new TreeWithNodeAtOffset.Class
            {
                ValueAtOffset = "value",
                ValueInline = 54
            };

            (rootNode.Edges[0].ChildNode as IOffsetNode)
                .Write(binaryWriter, Arg.Any<object>())
                .Returns(offset);

            treeWriter.Write(binaryWriter, value, rootNode)
                .Should()
                .Equal(offset);

            Received.InOrder(() => VerifyWriteTreeWithNodeAtOffset(rootNode, value, offset));
        }

        private void VerifyWriteTreeWithNodeAtOffset(IDataNode node, TreeWithNodeAtOffset.Class value, uint offsetPosition)
        {
            node.Write(binaryWriter, value);

            (node.Edges[0].ChildNode as IOffsetNode).Write(binaryWriter, value.ValueAtOffset);
            (node.Edges[1].ChildNode as IDataNode).Write(binaryWriter, value.ValueInline);

            offsetWriter(binaryWriter, offsetPosition);
            (node.Edges[0].ChildNode as IOffsetNode).ChildNode.Write(binaryWriter, value.ValueAtOffset);
        }
        #endregion
    }
}
