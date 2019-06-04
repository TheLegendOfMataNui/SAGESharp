/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using Konvenience;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;

namespace SAGESharp.IO
{
    class DefaultBinarySerializerTests
    {
        private readonly IBinaryReader reader = Substitute.For<IBinaryReader>();

        [Test]
        public void Test_Reading_A_Custom_Class()
        {
            var propertySerializers = new List<IPropertyBinarySerializer<CustomClass>>
            {
                Substitute.For<IPropertyBinarySerializer<CustomClass>>().Also(
                    pbs => pbs.ReadAndSet(Arg.Is(reader), Arg.Do<CustomClass>(o => o.String1 = "String1"))
                ),
                Substitute.For<IPropertyBinarySerializer<CustomClass>>().Also(
                    pbs => pbs.ReadAndSet(Arg.Is(reader), Arg.Do<CustomClass>(o => o.String2 = "String2"))
                )
            };

            CustomClass result = new DefaultBinarySerializer<CustomClass>(propertySerializers)
                .Read(reader)
                .Also(o => o.String1.Should().Be("String1"))
                .Also(o => o.String2.Should().Be("String2"))
                .Also(o => o.String3.Should().BeNull())
                .Also(o => propertySerializers.ForEach(pbs => pbs.Received().ReadAndSet(reader, o)));
        }

        public class CustomClass
        {
            public string String1 { get; set; }

            public string String2 { get; set; }

            public string String3 { get; set; }
        }

        [Test]
        public void Test_Reading_A_Class_With_Private_Empty_Constructor() => this
            .Invoking(_ => new DefaultBinarySerializer<ClassWithPrivateConstructor>(new List<IPropertyBinarySerializer<ClassWithPrivateConstructor>> { }))
            .Should()
            .Throw<BadTypeException>()
            .Where(e => e.Message.Contains($"Type {nameof(ClassWithPrivateConstructor)} has no public constructor with no arguments"));

        class ClassWithPrivateConstructor
        {
            private ClassWithPrivateConstructor()
            {
            }
        }
    }
}
