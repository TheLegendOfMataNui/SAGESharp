/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;
using System;
using System.Reflection;

namespace SAGESharp.IO
{
    class DefaultPropertyBinarySerializerTests
    {
        private readonly IBinarySerializer<string> serializer = Substitute.For<IBinarySerializer<string>>();

        private readonly PropertyInfo propertyInfo;

        private readonly IPropertyBinarySerializer<Class> propertySerializer;

        public DefaultPropertyBinarySerializerTests()
        {
            propertyInfo = typeof(Class).GetProperty(nameof(Class.Property));
            propertySerializer = new DefaultPropertyBinarySerializer<Class, string>(serializer, propertyInfo);
        }

        [SetUp]
        public void Setup()
        {
            serializer.ClearSubstitute();
        }

        [TestCase]
        public void Test_Reading_A_Property_Into_An_Object()
        {
            var obj = new Class();
            var expected = "expected";
            var reader = Substitute.For<IBinaryReader>();

            serializer.Read(reader).Returns(expected);

            propertySerializer.ReadAndSet(reader, obj);

            obj.Property
                .Should()
                .Be(expected);
        }

        class Class
        {
            public string Property { get; set; }
        }

        [TestCase]
        public void Test_Build_A_DefaultPropertyBinarySerializer_With_A_Null_Serializer()
        {
            Action action = () => new DefaultPropertyBinarySerializer<Class, string>(null, propertyInfo);

            action
                .Should()
                .Throw<ArgumentNullException>()
                .Where(e => e.Message.Contains("serializer"));
        }

        [TestCase]
        public void Test_Build_A_DefaultPropertyBinarySerializer_With_A_Null_PropertyInfo()
        {
            Action action = () => new DefaultPropertyBinarySerializer<Class, string>(serializer, null);

            action
                .Should()
                .Throw<ArgumentNullException>()
                .Where(e => e.Message.Contains("propertyInfo"));
        }
    }
}
