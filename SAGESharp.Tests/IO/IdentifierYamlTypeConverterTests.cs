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
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

using Identifier = SAGESharp.SLB.Identifier;

namespace SAGESharp.Tests.IO
{
    class IdentifierYamlTypeConverterTests
    {
        private static readonly Type IDENTIFIER_TYPE = typeof(Identifier);

        private readonly IYamlTypeConverter converter = new IdentifierYamlTypeConverter();

        private readonly IEmitter emitter = Substitute.For<IEmitter>();

        private readonly IParser parser = Substitute.For<IParser>();

        [SetUp]
        public void Setup()
        {
            emitter.ClearSubstitute();
            parser.ClearSubstitute();
        }

        [Test]
        public void Test_Should_Accept_Identifier_Type()
        {
            bool result = converter.Accepts(IDENTIFIER_TYPE);

            result.Should().BeTrue();
        }

        [TestCase(typeof(int))]
        [TestCase(typeof(string))]
        [TestCase(typeof(IdentifierYamlTypeConverter))]
        public void Test_Should_Not_Accept_Non_Identifier_Types(Type type)
        {
            bool result = converter.Accepts(type);

            result.Should().BeFalse();
        }

        [Test]
        public void Test_Reading_A_Valid_Identifier_String()
        {
            string value = "TOA1";
            Identifier identifier = Identifier.From(value);
            Scalar scalar = new Scalar(value);

            parser.Current.Returns(scalar);

            object result = converter.ReadYaml(parser, IDENTIFIER_TYPE);

            result.Should().Be(identifier);
        }

        [Test]
        public void Test_Reading_From_A_Null_Parser()
        {
            Action action = () => converter.ReadYaml(null, IDENTIFIER_TYPE);

            action.Should()
                .ThrowArgumentNullException("parser");
        }

        [Test]
        public void Test_Reading_With_A_Null_Type()
        {
            Action action = () => converter.ReadYaml(parser, null);

            action.Should()
                .ThrowArgumentNullException("type");
        }

        [Test]
        public void Test_Reading_With_An_Invalid_Type()
        {
            Type type = typeof(string);
            Action action = () => converter.ReadYaml(parser, type);

            action.Should()
                .ThrowExactly<ArgumentException>()
                .WithMessage($"Was expecting type {IDENTIFIER_TYPE.Name} but found {type.Name}");
        }

        [Test]
        public void Test_Writing_A_Valid_Identifier()
        {
            string identifier = "TOA2";

            converter.WriteYaml(emitter, Identifier.From(identifier), IDENTIFIER_TYPE);

            emitter.Received().Emit(Arg.Do<Scalar>(scalar =>
            {
                scalar.Tag.Should().BeNull();

                scalar.Value.Should().Be(identifier);
            }));
        }

        [Test]
        public void Test_Writing_To_A_Null_Emitter()
        {
            Action action = () => converter.WriteYaml(null, Identifier.ZERO, IDENTIFIER_TYPE);

            action.Should()
                .ThrowArgumentNullException("emitter");
        }

        [Test]
        public void Test_Writing_A_Null_Identifier()
        {
            Action action = () => converter.WriteYaml(emitter, null, IDENTIFIER_TYPE);

            action.Should()
                .ThrowArgumentNullException("value");
        }

        [Test]
        public void Test_Writing_With_A_Null_Type()
        {
            Action action = () => converter.WriteYaml(emitter, Identifier.ZERO, null);

            action.Should()
                .ThrowArgumentNullException("type");
        }

        [Test]
        public void Test_Writing_With_An_Invalid_Type()
        {
            Type type = typeof(string);
            Action action = () => converter.WriteYaml(emitter, Identifier.ZERO, type);

            action.Should()
                .ThrowExactly<ArgumentException>()
                .WithMessage($"Was expecting type {IDENTIFIER_TYPE.Name} but found {type.Name}");
        }

        [Test]
        public void Test_Writing_A_Non_Identifier_Value()
        {
            string value = string.Empty;
            Action action = () => converter.WriteYaml(emitter, value, IDENTIFIER_TYPE);

            action.Should()
                .ThrowExactly<ArgumentException>()
                .WithMessage($"The input value is type {value.GetType().Name}, was expecting {IDENTIFIER_TYPE.Name} instead.");
        }
    }
}
