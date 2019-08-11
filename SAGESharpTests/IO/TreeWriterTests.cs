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

        #region Tree with height 1
        [Test]
        public void Test_Writing_An_Instance_Of_A_Tree_With_Height_1()
        {
            IDataNode rootNode = TreeWithHeight1();
            ClassForTreeWithHeight1 value = new ClassForTreeWithHeight1
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

        private class ClassForTreeWithHeight1
        {
            public int Int { get; set; }

            public float Float { get; set; }

            public byte Byte { get; set; }
        }

        private IDataNode TreeWithHeight1() => new BuilderFor.DataNodeSubstitute<ClassForTreeWithHeight1>()
        {
            Edges = new List<IEdge>()
            {
                new BuilderFor.EdgeSubstitute
                {
                    ChildNode = new BuilderFor.DataNodeSubstitute<int>().Build(setup: SetupNodeWriteReturnsNull)
                }.Build(setup: e => SetupEdgeExtractChildValue<ClassForTreeWithHeight1>(e, o => o.Int)),
                new BuilderFor.EdgeSubstitute
                {
                    ChildNode = new BuilderFor.DataNodeSubstitute<float>().Build(setup: SetupNodeWriteReturnsNull)
                }.Build(setup: e => SetupEdgeExtractChildValue<ClassForTreeWithHeight1>(e, o => o.Float)),
                new BuilderFor.EdgeSubstitute
                {
                    ChildNode = new BuilderFor.DataNodeSubstitute<byte>().Build(setup: SetupNodeWriteReturnsNull)
                }.Build(setup: e => SetupEdgeExtractChildValue<ClassForTreeWithHeight1>(e, o => o.Byte))
            }
        }.Build(setup: SetupNodeWriteReturnsNull);

        private void VerifyWriteTreeWithHeight1(IDataNode node, ClassForTreeWithHeight1 value)
        {
            node.Write(binaryWriter, value);
            node.Edges[0].ChildNode.Write(binaryWriter, value.Int);
            node.Edges[1].ChildNode.Write(binaryWriter, value.Float);
            node.Edges[2].ChildNode.Write(binaryWriter, value.Byte);
        }
        #endregion

        #region Tree with height 3
        [Test]
        public void Test_Writing_An_Instance_Of_A_Tree_With_Height_3()
        {
            IDataNode rootNode = TreeWithHeight3();
            ClassForTreeWithHeight3 value = new ClassForTreeWithHeight3
            {
                Child1 = new ClassForTreeWithHeight2
                {
                    Long = 1,
                    Child = new ClassForTreeWithHeight1
                    {
                        Int = 2,
                        Float = 3.5f,
                        Byte = 4
                    }
                },
                Child2 = new ClassForTreeWithHeight2
                {
                    Long = 5,
                    Child = new ClassForTreeWithHeight1
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

        private class ClassForTreeWithHeight2
        {
            public long Long { get; set; }

            public ClassForTreeWithHeight1 Child { get; set; }
        }

        private class ClassForTreeWithHeight3
        {
            public ClassForTreeWithHeight2 Child1 { get; set; }

            public ClassForTreeWithHeight2 Child2 { get; set; }
        }

        private IDataNode TreeWithHeight2() => new BuilderFor.DataNodeSubstitute<ClassForTreeWithHeight2>
        {
            Edges = new List<IEdge>
            {
                new BuilderFor.EdgeSubstitute
                {
                    ChildNode = new BuilderFor.DataNodeSubstitute<long>().Build(setup: SetupNodeWriteReturnsNull)
                }.Build(setup: e => SetupEdgeExtractChildValue<ClassForTreeWithHeight2>(e, o => o.Long)),
                new BuilderFor.EdgeSubstitute
                {
                    ChildNode = TreeWithHeight1()
                }.Build(setup: e => SetupEdgeExtractChildValue<ClassForTreeWithHeight2>(e, o => o.Child))
            }
        }.Build(setup: SetupNodeWriteReturnsNull);

        private IDataNode TreeWithHeight3() => new BuilderFor.DataNodeSubstitute<ClassForTreeWithHeight3>
        {
            Edges = new List<IEdge>
            {
                new BuilderFor.EdgeSubstitute
                {
                    ChildNode = TreeWithHeight2()
                }.Build(setup: e => SetupEdgeExtractChildValue<ClassForTreeWithHeight3>(e, o => o.Child1)),
                new BuilderFor.EdgeSubstitute
                {
                    ChildNode = TreeWithHeight2()
                }.Build(setup: e => SetupEdgeExtractChildValue<ClassForTreeWithHeight3>(e, o => o.Child2))
            }
        }.Build(setup: SetupNodeWriteReturnsNull);

        private void VerifyWriteTreeWithHeight2(IDataNode node, ClassForTreeWithHeight2 value)
        {
            node.Write(binaryWriter, value);
            node.Edges[0].ChildNode.Write(binaryWriter, value.Long);
            VerifyWriteTreeWithHeight1(node.Edges[1].ChildNode as IDataNode, value.Child);
        }

        private void VerifyWriteTreeWithHeight3(IDataNode node, ClassForTreeWithHeight3 value)
        {
            node.Write(binaryWriter, value);
            VerifyWriteTreeWithHeight2(node.Edges[0].ChildNode as IDataNode, value.Child1);
            VerifyWriteTreeWithHeight2(node.Edges[1].ChildNode as IDataNode, value.Child2);
        }
        #endregion

        private void SetupNodeWriteReturnsNull(IDataNode dataNode)
            => dataNode.Write(binaryWriter, Arg.Any<object>()).Returns((uint?)null);

        private static void SetupEdgeExtractChildValue<T>(IEdge edge, Func<T, object> function)
            => edge.ExtractChildValue(Arg.Any<T>()).Returns(args => function((T)args[0]));
    }
}
