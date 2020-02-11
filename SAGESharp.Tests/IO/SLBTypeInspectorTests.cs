/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;
using NUtils.Extensions;
using SAGESharp.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;

namespace SAGESharp.Tests.IO
{
    class SLBTypeInspectorTests
    {
        private readonly ITypeInspector innerTypeInspector;

        private readonly ITypeInspector typeInspector;

        public SLBTypeInspectorTests()
        {
            innerTypeInspector = Substitute.For<ITypeInspector>();
            typeInspector = new SLBTypeInspector(innerTypeInspector);
        }

        [SetUp]
        public void Setup()
        {
            innerTypeInspector.ClearSubstitute();
        }

        [Test]
        public void Test_GetProperties_With_A_Type_That_Has_SerializablePropertyAttribute()
        {
            Type type = typeof(string);
            string container = string.Empty;
            IReadOnlyList<IPropertyDescriptor> expectedPropertyDescriptors = new List<IPropertyDescriptor>
            {
                DescriptorWithSerializablePropertyAttribute(new SerializablePropertyAttribute(0)),
                DescriptorWithSerializablePropertyAttribute(new SerializablePropertyAttribute(1, "Property")),
            };
            IEnumerable<IPropertyDescriptor> input = expectedPropertyDescriptors.Concat(new List<IPropertyDescriptor>
            {
                Substitute.For<IPropertyDescriptor>()
            });

            innerTypeInspector.GetProperties(type, container).Returns(input);

            var result = typeInspector.GetProperties(type, container);

            innerTypeInspector.Received().GetProperties(type, container);

            result.Should().NotBeNull().And.HaveCount(expectedPropertyDescriptors.Count);

            result.ForEach((propertyDescriptor, index) =>
            {
                var expectedPropertyDescriptor = expectedPropertyDescriptors[index];

                expectedPropertyDescriptor.Name.Should().Be(expectedPropertyDescriptor.Name);
                expectedPropertyDescriptor.Order.Should().Be(expectedPropertyDescriptor.Order);
            });
        }

        [Test]
        public void Test_GetProperties_With_No_SerializablePropertyAttribute()
        {
            Type type = typeof(string);
            string container = string.Empty;

            innerTypeInspector.GetProperties(type, container).Returns(new List<IPropertyDescriptor>
            {
                Substitute.For<IPropertyDescriptor>(),
                Substitute.For<IPropertyDescriptor>()
            });

            var result = typeInspector.GetProperties(type, container);

            innerTypeInspector.Received().GetProperties(type, container);

            result.Should().NotBeNull().And.BeEmpty();
        }

        private static IPropertyDescriptor DescriptorWithSerializablePropertyAttribute(SerializablePropertyAttribute attribute)
        {
            IPropertyDescriptor result = Substitute.For<IPropertyDescriptor>();

            result.GetCustomAttribute<SerializablePropertyAttribute>().Returns(attribute);

            return result;
        }
    }
}
