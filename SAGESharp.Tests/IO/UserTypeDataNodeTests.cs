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
using System;
using System.Collections.Generic;

namespace SAGESharp.Tests.IO
{
    class UserTypeDataNodeTests
    {
        private readonly IDataNode node;

        private readonly IBinaryWriter binaryWriter;

        private readonly IReadOnlyList<IEdge> edges;

        public UserTypeDataNodeTests()
        {
            binaryWriter = Substitute.For<IBinaryWriter>();
            edges = new List<IEdge>
            {
                Substitute.For<IEdge>(),
                Substitute.For<IEdge>(),
                Substitute.For<IEdge>()
            };
            node = new UserTypeDataNode<UserType>(edges);
        }

        [SetUp]
        public void Setup()
        {
            binaryWriter.ClearSubstitute();
        }

        #region Constructor
        public void Test_Create_A_DataNode_With_A_Null_List_Of_Edges()
        {
            Action action = () => new UserTypeDataNode<UserType>(null);

            action
                .Should()
                .ThrowArgumentNullException("edges");
        }

        public void Test_Create_A_DataNode_With_An_Empty_List_Of_Edges()
        {
            Action action = () => new UserTypeDataNode<UserType>(new List<IEdge>());

            action
                .Should()
                .ThrowExactly<ArgumentException>("edges should not be empty");
        }
        #endregion

        [Test]
        public void Test_Get_List_Of_Edges()
        {
            node.Edges
                .Should()
                .Equal(edges);
        }

        #region Reading
        [Test]
        public void Test_Reading_An_Object()
        {
            object result = node.Read(Substitute.For<IBinaryReader>());

            result.Should()
                .BeOfType<UserType>();
        }

        [Test]
        public void Test_Reading_An_Object_From_A_Null_Reader()
        {
            Action action = () => node.Read(null);

            action.Should()
                .ThrowArgumentNullException("binaryReader");
        }
        #endregion

        #region Writing
        [Test]
        public void Test_Writing_An_Objet()
        {
            node.Write(binaryWriter, new UserType());

            binaryWriter.DidNotReceiveWithAnyArgs().WriteByte(Arg.Any<byte>());
            binaryWriter.DidNotReceiveWithAnyArgs().WriteBytes(Arg.Any<byte[]>());
            binaryWriter.DidNotReceiveWithAnyArgs().WriteInt16(Arg.Any<short>());
            binaryWriter.DidNotReceiveWithAnyArgs().WriteUInt16(Arg.Any<ushort>());
            binaryWriter.DidNotReceiveWithAnyArgs().WriteInt32(Arg.Any<int>());
            binaryWriter.DidNotReceiveWithAnyArgs().WriteUInt32(Arg.Any<uint>());
            binaryWriter.DidNotReceiveWithAnyArgs().WriteFloat(Arg.Any<float>());
            binaryWriter.DidNotReceiveWithAnyArgs().WriteDouble(Arg.Any<double>());
        }

        [Test]
        public void Test_Writing_To_A_Null_Binary_Writer()
        {
            Action action = () => node.Write(null, new UserType());

            action
                .Should()
                .ThrowArgumentNullException("binaryWriter");
        }

        [Test]
        public void Test_Writing_A_Null_Value()
        {
            Action action = () => node.Write(binaryWriter, null);

            action
                .Should()
                .ThrowArgumentNullException("value");
        }

        [Test]
        public void Test_Writing_A_Non_Valid_Value()
        {
            var value = new object();
            Action action = () => node.Write(binaryWriter, value);

            action
                .Should()
                .ThrowExactly<ArgumentException>()
                .WithMessage($"Cannot write value of type {value.GetType().Name} as type {typeof(UserType).Name}.");
        }
        #endregion

        private class UserType
        {
        }
    }
}
