/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;
using SAGESharp.SLB;
using System;
using System.Reflection;

namespace SAGESharp.IO
{
    class IdentifierPropertyBinarySerializerTests
    {

        private readonly PropertyInfo propertyInfo;

        private readonly IPropertyBinarySerializer<Class> propertySerializer;

        private readonly IBinaryReader reader = Substitute.For<IBinaryReader>();

        public IdentifierPropertyBinarySerializerTests()
        {
            propertyInfo = typeof(Class).GetProperty(nameof(Class.Property));
            propertySerializer = new IdentifierPropertyBinarySerializer<Class>(propertyInfo);
        }

        [SetUp]
        public void Setup()
        {
            reader.ClearSubstitute();
        }

        [Test]
        public void Test_Reading_An_Identifier_Property_Into_An_Object()
        {
            Class obj = new Class();
            Identifier expected = 0xAABBCCDD;
            IBinaryReader reader = Substitute.For<IBinaryReader>();

            reader.ReadUInt32().Returns((uint)expected);

            propertySerializer.ReadAndSet(reader, obj);

            obj.Property.Should().Be(expected);

            reader.Received().ReadUInt32();
        }

        [Test]
        public void Test_Build_A_DefaultPropertyBinarySerializer_With_A_Null_PropertyInfo()
        {
            Action action = () => new IdentifierPropertyBinarySerializer<Class>(null);

            action
                .Should()
                .Throw<ArgumentNullException>()
                .Where(e => e.Message.Contains("propertyInfo"));
        }

        [Test]
        public void Test_Read_From_A_Null_Reader()
        {
            Action action = () => propertySerializer.ReadAndSet(null, new Class());

            action
                .Should()
                .Throw<ArgumentNullException>()
                .Where(e => e.Message.Contains("reader"));
        }

        [Test]
        public void Test_Read_To_A_Null_Object()
        {
            Action action = () => propertySerializer.ReadAndSet(reader, null);

            action
                .Should()
                .Throw<ArgumentNullException>()
                .Where(e => e.Message.Contains("obj"));
        }

        class Class
        {
            public Identifier Property { get; set; }
        }
    }
}
