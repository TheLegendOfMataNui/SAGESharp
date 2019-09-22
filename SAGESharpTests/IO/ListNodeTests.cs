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
    class ListNodeTests
    {
        private readonly IBinaryWriter binaryWriter;

        private readonly IDataNode childNode;

        public ListNodeTests()
        {
            childNode = Substitute.For<IDataNode>();
            binaryWriter = Substitute.For<IBinaryWriter>();
        }

        [SetUp]
        public void Setup()
        {
            childNode.ClearSubstitute();
            binaryWriter.ClearSubstitute();
        }

        [Test]
        public void Test_Creating_A_ListNode_With_A_Null_Child_Node()
        {
            Action action = () => new ListNode<object>(null);

            action.Should()
                .ThrowArgumentNullException("childNode");
        }

        #region IOffsetNode methods
        [Test]
        public void Test_Writing_A_List()
        {
            IList<string> list = BuildStringList();
            IListNode listNode = BuildStringListNode(duplicateEntryCount: false);

            uint offset = 0xFFEEDDCC;
            binaryWriter.Position.Returns(0);
            binaryWriter.WriteInt32(Arg.Do<int>(_ => binaryWriter.Position.Returns(offset)));
            binaryWriter.WriteUInt32(Arg.Do<uint>(_ => binaryWriter.Position.Returns(0)));

            uint result = listNode.Write(binaryWriter, list);

            result.Should().Be(offset);

            Received.InOrder(() =>
            {
                binaryWriter.WriteInt32(list.Count);
                binaryWriter.WriteUInt32(0); // offset placeholder
            });
        }

        [Test]
        public void Test_Writing_A_List_With_Duplicate_Entry_Count()
        {
            IList<string> list = BuildStringList();
            IListNode listNode = BuildStringListNode(duplicateEntryCount: true);

            uint offset = 0xFFEEDDCC;
            binaryWriter.Position.Returns(0);
            binaryWriter.WriteInt32(ArgX.OrderedDo<int>(
                _ => binaryWriter.Position.Returns(0),
                _ => binaryWriter.Position.Returns(offset)
            ));
            binaryWriter.WriteUInt32(Arg.Do<uint>(_ => binaryWriter.Position.Returns(0)));

            uint result = listNode.Write(binaryWriter, list);

            result.Should().Be(offset);

            Received.InOrder(() =>
            {
                binaryWriter.WriteInt32(list.Count);
                binaryWriter.WriteInt32(list.Count);
                binaryWriter.WriteUInt32(0); // offset placeholder
            });
        }

        [Test]
        public void Test_Writing_A_List_With_A_Null_BinaryWriter()
        {
            IList<string> list = BuildStringList();
            Action action = () => BuildStringListNode().Write(null, list);

            action.Should()
                .ThrowArgumentNullException("binaryWriter");
        }

        [Test]
        public void Test_Writing_A_Null_List()
        {
            Action action = () => BuildStringListNode().Write(binaryWriter, null);

            action.Should()
                .ThrowArgumentNullException("value");
        }

        [Test]
        public void Test_Writing_A_List_With_An_Incorrect_Type()
        {
            IList<int> list = new List<int>();
            Action action = () => BuildStringListNode().Write(binaryWriter, list);

            action.Should()
                .ThrowExactly<ArgumentException>()
                .WithMessage($"List argument is of type {list.GetType().Name} which should " +
                    $"implement {typeof(IList<string>).Name} but doesn't.");
        }

        [Test]
        public void Test_Getting_The_Child_Node_Of_A_List_node()
        {
            BuildStringListNode().ChildNode
                .Should()
                .BeSameAs(childNode);
        }
        #endregion

        #region IListNode methods
        [Test]
        public void Test_Getting_The_Count_From_A_List()
        {
            IList<string> list = BuildStringList();

            BuildStringListNode()
                .GetListCount(list)
                .Should()
                .Be(list.Count);
        }

        [Test]
        public void Test_Getting_The_Count_From_A_Null_List()
        {
            Action action = () => BuildStringListNode().GetListCount(null);

            action.Should()
                .ThrowArgumentNullException("list");
        }

        [Test]
        public void Test_Getting_The_Count_From_A_List_Of_Incorrect_Type()
        {
            IList<int> list = new List<int>();
            Action action = () => BuildStringListNode().GetListCount(list);

            action.Should()
                .ThrowExactly<ArgumentException>()
                .WithMessage($"List argument is of type {list.GetType().Name} which should " +
                    $"implement {typeof(IList<string>).Name} but doesn't.");
        }

        [Test]
        public void Test_Getting_The_Entries_From_A_Non_Empty_List()
        {
            IList<string> list = BuildStringList();
            IListNode listNode = BuildStringListNode();

            for (int n = 0; n < list.Count; n++)
            {
                listNode.GetListEntry(list, n)
                    .Should()
                    .BeSameAs(list[n]);
            }
        }

        [Test]
        public void Test_Getting_The_Entries_From_A_Null_List()
        {
            Action action = () => BuildStringListNode().GetListEntry(null, 0);

            action.Should()
                .ThrowArgumentNullException("list");
        }

        [Test]
        public void Test_Getting_The_Entries_From_A_List_Of_Incorrect_Type()
        {
            IList<int> list = new List<int>();
            Action action = () => BuildStringListNode().GetListEntry(list, 0);

            action.Should()
                .ThrowExactly<ArgumentException>()
                .WithMessage($"List argument is of type {list.GetType().Name} which should " +
                    $"implement {typeof(IList<string>).Name} but doesn't.");
        }
        #endregion

        private IListNode BuildStringListNode(bool duplicateEntryCount = false)
        {
            return new ListNode<string>(childNode, duplicateEntryCount);
        }

        private static IList<string> BuildStringList() => new List<string> { "1", "2", "3" };
    }
}
