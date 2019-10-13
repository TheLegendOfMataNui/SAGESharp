/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using Konvenience;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;
using SAGESharp.IO.Trees;
using SAGESharp.Testing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAGESharp.IO
{
    class TreeReaderTests
    {
        private readonly IBinaryReader binaryReader;

        private readonly TreeReader.AtOffsetDo atOffsetDo;

        private readonly ITreeReader treeReader;

        public TreeReaderTests()
        {
            binaryReader = Substitute.For<IBinaryReader>();
            atOffsetDo = Substitute.For<TreeReader.AtOffsetDo>();
            treeReader = new TreeReader((binaryReader, offset, action) =>
            {
                atOffsetDo(binaryReader, offset, action);

                action();
            });
        }

        [SetUp]
        public void Setup()
        {
            binaryReader.ClearSubstitute();
            atOffsetDo.ClearSubstitute();
        }

        #region Null checks
        [Test]
        public void Test_Creating_A_TreeReader_With_A_Null_Delegate()
        {
            Action action = () => new TreeReader(null);

            action.Should()
                .ThrowArgumentNullException("atOffsetDo");
        }

        [Test]
        public void Test_Reading_A_Tree_With_A_Null_BinaryReader()
        {
            Action action = () => treeReader.Read(null, Substitute.For<IDataNode>());

            action
                .Should()
                .ThrowArgumentNullException("binaryReader");
        }

        [Test]
        public void Test_Reading_A_Tree_With_A_Null_Root_Node()
        {
            Action action = () => treeReader.Read(binaryReader, null);

            action
                .Should()
                .ThrowArgumentNullException("rootNode");
        }
        #endregion

        [Test]
        public void Test_Reading_A_Tree_With_Unkown_Node_Type()
        {
            IDataNode rootNode = TreeWithUnknownNodeType.Build();
            Action action = () => treeReader.Read(binaryReader, rootNode);

            action.Should()
                .ThrowExactly<NotImplementedException>()
                .WithMessage($"Type {typeof(TreeWithUnknownNodeType.UnknownNodeType).Name} is an unknown node type");
        }

        #region Tree with height 1
        [Test]
        public void Test_Reading_An_Instance_Of_A_Tree_With_Height_1()
        {
            IDataNode rootNode = TreeWithHeight1.Build();
            TreeWithHeight1.Class expected = new TreeWithHeight1.Class
            {
                Int = 1,
                Float = 2.5f,
                Byte = 3
            };

            SetupTreeWithHeight1(rootNode, expected);

            object result = treeReader.Read(binaryReader, rootNode);

            Received.InOrder(() => VerifyReadTreeWithHeight1(rootNode));

            result.Should().BeEquivalentTo(expected);
        }

        private void SetupTreeWithHeight1(IDataNode node, TreeWithHeight1.Class expected, params TreeWithHeight1.Class[] otherExpected)
        {
            node.Read(binaryReader).Returns(_ => new TreeWithHeight1.Class());

            void setupDataNode(IDataNode dataNode, Func<TreeWithHeight1.Class, object> valueExtractor)
            {
                dataNode.Read(binaryReader).Returns(valueExtractor(expected), otherExpected.Select(e => valueExtractor(e)).ToArray());
            }

            setupDataNode(dataNode: node.Edges[0].ChildNode as IDataNode, valueExtractor: e => e.Int);
            setupDataNode(dataNode: node.Edges[1].ChildNode as IDataNode, valueExtractor: e => e.Float);
            setupDataNode(dataNode: node.Edges[2].ChildNode as IDataNode, valueExtractor: e => e.Byte);
        }

        private void VerifyReadTreeWithHeight1(IDataNode node)
        {
            node.Read(binaryReader);
            (node.Edges[0].ChildNode as IDataNode).Read(binaryReader);
            (node.Edges[1].ChildNode as IDataNode).Read(binaryReader);
            (node.Edges[2].ChildNode as IDataNode).Read(binaryReader);
        }
        #endregion

        #region Tree with height 3
        [Test]
        public void Test_Reading_An_Instance_Of_A_Tree_With_Height_3()
        {
            IDataNode rootNode = TreeWithHeight3.Build();
            TreeWithHeight3.Class expected = new TreeWithHeight3.Class
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

            SetupTreeWithHeight3(rootNode, expected);

            object result = treeReader.Read(binaryReader, rootNode);

            Received.InOrder(() => VerifyReadTreeWithHeight3(rootNode));

            result.Should().BeEquivalentTo(expected);
        }

        private void SetupTreeWithHeight2(IDataNode node, TreeWithHeight2.Class expected, params TreeWithHeight2.Class[] otherExpected)
        {
            node.Read(binaryReader).Returns(_ => new TreeWithHeight2.Class());
            (node.Edges[0].ChildNode as IDataNode).Read(binaryReader).Returns(expected.Long, otherExpected.Select(e => (object)e.Long).ToArray());
            SetupTreeWithHeight1(node.Edges[1].ChildNode as IDataNode, expected.Child, otherExpected.Select(e => e.Child).ToArray());
        }

        private void SetupTreeWithHeight3(IDataNode node, TreeWithHeight3.Class expected)
        {
            node.Read(binaryReader).Returns(_ => new TreeWithHeight3.Class());
            SetupTreeWithHeight2(node.Edges[0].ChildNode as IDataNode, expected.Child1);
            SetupTreeWithHeight2(node.Edges[1].ChildNode as IDataNode, expected.Child2);
        }

        private void VerifyReadTreeWithHeight2(IDataNode node)
        {
            node.Read(binaryReader);
            (node.Edges[0].ChildNode as IDataNode).Read(binaryReader);
            VerifyReadTreeWithHeight1(node.Edges[1].ChildNode as IDataNode);
        }

        private void VerifyReadTreeWithHeight3(IDataNode node)
        {
            node.Read(binaryReader);
            VerifyReadTreeWithHeight2(node.Edges[0].ChildNode as IDataNode);
            VerifyReadTreeWithHeight2(node.Edges[1].ChildNode as IDataNode);
        }
        #endregion

        #region Tree with nested lists
        [Test]
        public void Test_Reading_An_Instance_Of_A_Tree_With_Nested_Lists()
        {
            uint offsetPosition1 = 20, offsetPosition2 = 30;
            IDataNode rootNode = TreeWithNestedLists.Build();
            TreeWithNestedLists.Class expected = new TreeWithNestedLists.Class
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

            SetupTreeWithNestedLists(rootNode, expected, offsetPosition1, offsetPosition2);

            object result = treeReader.Read(binaryReader, rootNode);

            Received.InOrder(() => VerifyReadTreeWithNestedLists(rootNode, expected, offsetPosition1, offsetPosition2));

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void Test_Reading_An_Instance_Of_A_Tree_With_Nested_Empty_Lists()
        {
            uint offsetPosition1 = 20, offsetPosition2 = 30;
            IDataNode rootNode = TreeWithNestedLists.Build();
            TreeWithNestedLists.Class expected = new TreeWithNestedLists.Class
            {
                List1 = new List<TreeWithHeight1.Class>
                {
                },
                List2 = new List<TreeWithHeight2.Class>
                {
                }
            };

            SetupTreeWithNestedLists(rootNode, expected, offsetPosition1, offsetPosition2);

            object result = treeReader.Read(binaryReader, rootNode);

            atOffsetDo.DidNotReceive().Invoke(binaryReader, offsetPosition1, Arg.Any<Action>());
            atOffsetDo.DidNotReceive().Invoke(binaryReader, offsetPosition2, Arg.Any<Action>());

            Received.InOrder(() => VerifyReadTreeWithNestedLists(rootNode, expected, offsetPosition1, offsetPosition2));

            result.Should().BeEquivalentTo(expected);
        }

        private void SetupTreeWithNestedLists(IDataNode node, TreeWithNestedLists.Class expected, uint offset1, uint offset2)
        {
            node.Read(binaryReader).Returns(_ => new TreeWithNestedLists.Class());

            void setupListNode<T>(IListNode listNode, IList<T> expectedList, uint offset, Action<IDataNode, T, T[]> setupValues)
            {
                listNode.ReadEntryCount(binaryReader).Returns(expectedList.Count);
                listNode.ReadOffset(binaryReader).Returns(offset);

                if (expectedList.IsNotEmpty())
                {
                    setupValues(listNode.ChildNode, expectedList[0], expectedList.Skip(1).ToArray());
                }
            }

            setupListNode(node.Edges[0].ChildNode as IListNode, expected.List1, offset1, SetupTreeWithHeight1);
            setupListNode(node.Edges[1].ChildNode as IListNode, expected.List2, offset2, SetupTreeWithHeight2);
        }

        private void VerifyReadTreeWithNestedLists(IDataNode node, TreeWithNestedLists.Class value, uint offsetPosition1, uint offsetPosition2)
        {
            node.Read(binaryReader);

            void verifyReadList<T>(IListNode listNode, IList<T> list, uint offset, Action<IDataNode> verifyReadValue)
            {
                listNode.ReadEntryCount(binaryReader);
                listNode.ReadOffset(binaryReader);

                if (list.IsNotEmpty())
                {
                    atOffsetDo(Arg.Is(binaryReader), Arg.Is<long>(offset), Arg.Any<Action>());

                    foreach (var entry in list)
                    {
                        verifyReadValue(listNode.ChildNode);
                    }
                }
            }

            verifyReadList(node.Edges[0].ChildNode as IListNode, value.List1, offsetPosition1, VerifyReadTreeWithHeight1);
            verifyReadList(node.Edges[1].ChildNode as IListNode, value.List2, offsetPosition2, VerifyReadTreeWithHeight2);
        }
        #endregion

        #region Tree with node at offset
        [Test]
        public void Test_Reading_An_Instance_Of_A_Tree_With_A_Node_At_Offset()
        {
            uint offset = 15;
            IDataNode rootNode = TreeWithNodeAtOffset.Build();
            TreeWithNodeAtOffset.Class expected = new TreeWithNodeAtOffset.Class
            {
                ValueAtOffset = "value",
                ValueInline = 54
            };

            SetupTreeWithNodeAtOffset(rootNode, expected, offset);

            object result = treeReader.Read(binaryReader, rootNode);
            
            Received.InOrder(() => VerifyReadTreeWithNodeAtOffset(rootNode, offset));

            result.Should().BeEquivalentTo(expected);
        }

        private void SetupTreeWithNodeAtOffset(IDataNode node, TreeWithNodeAtOffset.Class expected, uint offset)
        {
            node.Read(binaryReader).Returns(_ => new TreeWithNodeAtOffset.Class());

            IOffsetNode offsetNode = node.Edges[0].ChildNode as IOffsetNode;
            offsetNode.ReadOffset(binaryReader).Returns(offset);
            offsetNode.ChildNode.Read(binaryReader).Returns(expected.ValueAtOffset);

            (node.Edges[1].ChildNode as IDataNode).Read(binaryReader).Returns(expected.ValueInline);
        }

        private void VerifyReadTreeWithNodeAtOffset(IDataNode node, uint offsetPosition)
        {
            node.Read(binaryReader);

            IOffsetNode offsetNode = node.Edges[0].ChildNode as IOffsetNode;
            offsetNode.ReadOffset(binaryReader);
            atOffsetDo(Arg.Is(binaryReader), Arg.Is<long>(offsetPosition), Arg.Any<Action>());
            offsetNode.ChildNode.Read(binaryReader);

            (node.Edges[1].ChildNode as IDataNode).Read(binaryReader);
        }
        #endregion
    }
}
