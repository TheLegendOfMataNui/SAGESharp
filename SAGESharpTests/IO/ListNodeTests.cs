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
        private readonly IBinaryReader binaryReader;

        private readonly IBinaryWriter binaryWriter;

        private readonly IDataNode childNode;

        public ListNodeTests()
        {
            childNode = Substitute.For<IDataNode>();
            binaryReader = Substitute.For<IBinaryReader>();
            binaryWriter = Substitute.For<IBinaryWriter>();
        }

        [SetUp]
        public void Setup()
        {
            childNode.ClearSubstitute();
            binaryReader.ClearSubstitute();
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
        public void Test_Reading_An_Offset()
        {
            uint offset = 0xFFEEDDCC;

            binaryReader.ReadUInt32().Returns(offset);

            uint result = BuildStringListNode().ReadOffset(binaryReader);

            binaryReader.Received().ReadUInt32();

            result.Should().Be(offset);
        }

        [Test]
        public void Test_Reading_An_Offset_From_A_Null_BinaryReader()
        {
            Action action = () => BuildStringListNode().ReadOffset(null);

            action.Should()
                .ThrowArgumentNullException("binaryReader");
        }

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
        public void Test_Writing_A_List_With_An_Offset_Greater_Than_4_bytes()
        {
            Action action = () => BuildStringListNode().Write(binaryWriter, new List<string>());

            binaryWriter.Position.Returns(0xAABBCCDDEE);

            action.Should()
                .ThrowExactly<InvalidOperationException>()
                .WithMessage("Offset is bigger than 4 bytes.");
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

        [Test]
        public void Test_Adding_An_Entry_To_A_List()
        {
            IList<string> list = new List<string>();
            IListNode listNode = BuildStringListNode();
            string value = "value";

            listNode.AddListEntry(list, value);

            list.Should().HaveCount(1);
            list[0].Should().BeSameAs(value);
        }

        [Test]
        public void Test_Adding_An_Entry_To_A_Null_List()
        {
            IListNode listNode = BuildStringListNode();
            Action action = () => listNode.AddListEntry(null, string.Empty);

            action.Should()
                .ThrowArgumentNullException("list");
        }

        [Test]
        public void Test_Adding_A_Null_Entry_To_A_List()
        {
            IListNode listNode = BuildStringListNode();
            Action action = () => listNode.AddListEntry(new List<string>(), null);

            action.Should()
                .ThrowArgumentNullException("value");
        }

        [Test]
        public void Test_Adding_An_Element_To_A_List_Of_Incorrect_Type()
        {
            IList<int> list = new List<int>();
            IListNode listNode = BuildStringListNode();
            Action action = () => listNode.AddListEntry(list, string.Empty);

            action.Should()
                .ThrowExactly<ArgumentException>()
                .WithMessage($"List argument is of type {list.GetType().Name} which should " +
                    $"implement {typeof(IList<string>).Name} but doesn't.");
        }

        [Test]
        public void Test_Adding_An_Element_Of_Invalid_Type_To_A_List()
        {
            IList<string> list = BuildStringList();
            IListNode listNode = BuildStringListNode();
            int value = 1;
            Action action = () => listNode.AddListEntry(list, value);

            action.Should()
                .ThrowExactly<ArgumentException>()
                .WithMessage($"Value should be of type {typeof(string).Name}, " +
                    $"but is of type {value.GetType().Name} instead.");
        }

        [Test]
        public void Test_Creating_A_List_Instance()
        {
            IListNode listNode = BuildStringListNode();

            object result = listNode.CreateList();

            result.Should()
                .BeOfType<List<string>>()
                .Which
                .Should()
                .HaveCount(0);
        }

        [Test]
        public void Test_Reading_Entry_Count()
        {
            IListNode listNode = BuildStringListNode(duplicateEntryCount: false);
            int offset = 20;

            binaryReader.ReadInt32().Returns(offset);

            int result = listNode.ReadEntryCount(binaryReader);

            result.Should().Be(offset);

            binaryReader.Received().ReadInt32();
        }

        [Test]
        public void Test_Reading_Duplicate_Entry_Count()
        {
            IListNode listNode = BuildStringListNode(duplicateEntryCount: true);
            int offset = 20;

            binaryReader.ReadInt32().Returns(offset, offset);

            int result = listNode.ReadEntryCount(binaryReader);

            result.Should().Be(offset);

            binaryReader.Received(2).ReadInt32();
        }
        #endregion

        private IListNode BuildStringListNode(bool duplicateEntryCount = false)
        {
            return new ListNode<string>(childNode, duplicateEntryCount);
        }

        private static IList<string> BuildStringList() => new List<string> { "1", "2", "3" };
    }
}
