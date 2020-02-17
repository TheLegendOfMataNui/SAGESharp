/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;
using SAGESharp.IO;
using SAGESharp.IO.Binary.TreeBasedSerialization;
using SAGESharp.Tests.IO.Binary.TreeBasedSerialization.Trees;
using System;
using System.Collections.Generic;

namespace SAGESharp.Tests.IO.Binary.TreeBasedSerialization
{
    class TreeWriterTests
    {
        private readonly BinaryWriterSubstitute binaryWriter;

        private readonly TreeWriter treeWriter;

        public TreeWriterTests()
        {
            binaryWriter = BinaryWriterSubstitute.New();
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

        [Test]
        public void Test_Reading_A_Tree_With_Unkown_Node_Type()
        {
            IDataNode rootNode = TreeWithUnknownNodeType.Build();
            Action action = () => treeWriter.Write(binaryWriter, new object(), rootNode);

            action.Should()
                .ThrowExactly<NotImplementedException>()
                .WithMessage($"Type {typeof(TreeWithUnknownNodeType.UnknownNodeType).Name} is an unknown node type");
        }

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
            uint originalPosition1 = 28, originalPosition2 = 39;
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

            binaryWriter.GetPosition().Returns(originalPosition1, originalPosition2);

            treeWriter.Write(binaryWriter, value, rootNode)
                .Should()
                .Equal(offsetPosition1, offsetPosition2);

            Received.InOrder(() => VerifyWriteTreeWithNestedLists(rootNode, value, originalPosition1, originalPosition2, offsetPosition1, offsetPosition2));
        }

        [Test]
        public void Test_Writing_An_Instance_Of_A_Tree_With_Nested_Empty_Lists()
        {
            uint originalPosition1 = 28, originalPosition2 = 39;
            uint offsetPosition1 = 20, offsetPosition2 = 30;
            IDataNode rootNode = TreeWithNestedLists.Build();
            TreeWithNestedLists.Class value = new TreeWithNestedLists.Class
            {
                List1 = new List<TreeWithHeight1.Class>
                {
                },
                List2 = new List<TreeWithHeight2.Class>
                {
                }
            };

            IOffsetNode list1ChildNode = rootNode.Edges[0].ChildNode as IOffsetNode;
            list1ChildNode
                .Write(binaryWriter, Arg.Any<object>())
                .Returns(offsetPosition1);

            IOffsetNode list2ChildNode = rootNode.Edges[1].ChildNode as IOffsetNode;
            list2ChildNode
                .Write(binaryWriter, Arg.Any<object>())
                .Returns(offsetPosition2);

            binaryWriter.GetPosition().Returns(originalPosition1, originalPosition2);

            treeWriter.Write(binaryWriter, value, rootNode)
                .Should()
                .Equal(offsetPosition1, offsetPosition2);

            list1ChildNode.ChildNode.DidNotReceive().Write(binaryWriter, Arg.Any<object>());
            list2ChildNode.ChildNode.DidNotReceive().Write(binaryWriter, Arg.Any<object>());

            Received.InOrder(() => VerifyWriteTreeWithNestedLists(rootNode, value, originalPosition1, originalPosition2, offsetPosition1, offsetPosition2));
        }

        private void VerifyWriteTreeWithNestedLists(
            IDataNode node,
            TreeWithNestedLists.Class value,
            uint originalPosition1,
            uint originalPosition2,
            uint offsetPosition1,
            uint offsetPosition2
        ) {
            node.Write(binaryWriter, value);

            IListNode listNode1 = node.Edges[0].ChildNode as IListNode;
            listNode1.Write(binaryWriter, value.List1);

            IListNode listNode2 = node.Edges[1].ChildNode as IListNode;
            listNode2.Write(binaryWriter, value.List2);

            VerifyBinaryWriterWritesOffset(originalPosition1, offsetPosition1);
            foreach (var entry in value.List1)
            {
                VerifyWriteTreeWithHeight1(listNode1.ChildNode, entry);
            }

            VerifyBinaryWriterWritesOffset(originalPosition2, offsetPosition2);
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
            uint originalPosition = 43, offset = 15;
            IDataNode rootNode = TreeWithNodeAtOffset.Build();
            TreeWithNodeAtOffset.Class value = new TreeWithNodeAtOffset.Class
            {
                ValueAtOffset = "value",
                ValueInline = 54
            };

            binaryWriter.GetPosition().Returns(originalPosition);

            (rootNode.Edges[0].ChildNode as IOffsetNode)
                .Write(binaryWriter, Arg.Any<object>())
                .Returns(offset);

            treeWriter.Write(binaryWriter, value, rootNode)
                .Should()
                .Equal(offset);

            Received.InOrder(() => VerifyWriteTreeWithNodeAtOffset(rootNode, value, originalPosition, offset));
        }

        private void VerifyWriteTreeWithNodeAtOffset(IDataNode node, TreeWithNodeAtOffset.Class value, uint originalPosition, uint offsetPosition)
        {
            node.Write(binaryWriter, value);

            (node.Edges[0].ChildNode as IOffsetNode).Write(binaryWriter, value.ValueAtOffset);
            (node.Edges[1].ChildNode as IDataNode).Write(binaryWriter, value.ValueInline);

            VerifyBinaryWriterWritesOffset(originalPosition, offsetPosition);
            (node.Edges[0].ChildNode as IOffsetNode).ChildNode.Write(binaryWriter, value.ValueAtOffset);
        }

        [Test]
        public void Test_Writing_An_Instance_Of_A_Tree_With_A_Node_At_A_Large_Offset()
        {
            long originalPosition = (long)uint.MaxValue + 1;
            IDataNode rootNode = TreeWithNodeAtOffset.Build();
            TreeWithNodeAtOffset.Class value = new TreeWithNodeAtOffset.Class
            {
                ValueAtOffset = "offset too large",
                ValueInline = 7
            };
            Action action = () => treeWriter.Write(binaryWriter, value, rootNode);

            binaryWriter.GetPosition().Returns(originalPosition);

            action.Should()
                .ThrowExactly<ArgumentException>()
                .WithMessage($"Offset 0x{originalPosition:X} is larger than {sizeof(uint)} bytes.");

            binaryWriter.DidNotReceive().WriteUInt32(Arg.Any<uint>());
        }
        #endregion

        private void VerifyBinaryWriterWritesOffset(uint originalPosition, uint offset)
        {
            binaryWriter.VerifyDoAtPosition(originalPosition, offset, () => binaryWriter.WriteUInt32(originalPosition));
        }
    }
}
