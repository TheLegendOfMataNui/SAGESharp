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
    public class BinarySerializersReadingTests
    {
        private readonly IBinaryReader binaryReader = Substitute.For<IBinaryReader>();

        [Test]
        public void Test_Reading_A_Valid_Class_With()
        {
            var propertySerializers = new List<IPropertyBinarySerializer<ValidClass>>
            {
                Substitute.For<IPropertyBinarySerializer<ValidClass>>().Also(
                    pbs => pbs.ReadAndSet(Arg.Is(binaryReader), Arg.Do<ValidClass>(o => o.String1 = "String1"))
                ),
                Substitute.For<IPropertyBinarySerializer<ValidClass>>().Also(
                    pbs => pbs.ReadAndSet(Arg.Is(binaryReader), Arg.Do<ValidClass>(o => o.String2 = "String2"))
                )
            };

            ValidClass result = new DefaultBinarySerializer<ValidClass>(propertySerializers)
                .Read(binaryReader)
                .Also(o => o.String1.Should().Be("String1"))
                .Also(o => o.String2.Should().Be("String2"))
                .Also(o => o.String3.Should().BeNull())
                .Also(o => propertySerializers.ForEach(pbs => pbs.Received().ReadAndSet(binaryReader, o)));
        }

        public class ValidClass
        {
            public string String1 { get; set; }

            public string String2 { get; set; }

            public string String3 { get; set; }
        }

        [Test]
        public void Test_Reading_A_Class_With_Private_Empty_Constructor()
            => this.Invoking(_ => new DefaultBinarySerializer<InvalidClass>(new List<IPropertyBinarySerializer<InvalidClass>> { }))
                .Should()
                .Throw<BadTypeException>()
                .Where(e => e.Message.Contains($"Type {nameof(InvalidClass)} has no public constructor with no arguments"));

        public class InvalidClass
        {
            private InvalidClass()
            {
            }
        }
    }
}
