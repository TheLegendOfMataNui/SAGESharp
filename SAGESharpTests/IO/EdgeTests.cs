/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using SAGESharp.Testing;
using System;

namespace SAGESharp.IO
{
    class EdgeTests
    {
        private readonly Func<CustomType, object> extractor;

        private readonly Action<CustomType, object> setter;

        private readonly IDataNode childNode;

        private readonly IEdge edge;

        public EdgeTests()
        {
            extractor = Substitute.For<Func<CustomType, object>>();
            setter = Substitute.For<Action<CustomType, object>>();
            childNode = Substitute.For<IDataNode>();
            edge = new Edge<CustomType>(extractor, setter, childNode);
        }

        [Test]
        public void Test_Creating_An_Edge_With_A_Null_Extractor()
        {
            Action action = () => new Edge<CustomType>(null, setter, childNode);

            action
                .Should()
                .ThrowArgumentNullException("extractor");
        }

        [Test]
        public void Test_Creating_An_Edge_With_A_Null_Setter()
        {
            Action action = () => new Edge<CustomType>(extractor, null, childNode);

            action
                .Should()
                .ThrowArgumentNullException("setter");
        }

        [Test]
        public void Test_Creating_An_Edge_With_A_Null_ChildNode()
        {
            Action action = () => new Edge<CustomType>(extractor, setter, null);

            action
                .Should()
                .ThrowArgumentNullException("childNode");
        }

        [Test]
        public void Test_Getting_ChildNode()
        {
            edge.ChildNode
                .Should()
                .BeSameAs(childNode);
        }

        [Test]
        public void Test_Extracting_From_A_Value()
        {
            CustomType value = new CustomType();
            object result = new object();

            extractor.Invoke(Arg.Is(value)).Returns(result);

            edge.ExtractChildValue(value)
                .Should()
                .BeSameAs(result);

            extractor.Received().Invoke(value);
        }

        [Test]
        public void Test_Extracting_From_A_Null_Value()
        {
            Action action = () => edge.ExtractChildValue(null);

            action
                .Should()
                .ThrowArgumentNullException("value");

            extractor.DidNotReceiveWithAnyArgs().Invoke(Arg.Any<CustomType>());
        }

        [Test]
        public void Test_Extracting_From_An_Object_With_Invalid_Type()
        {
            var value = new object();
            Action action = () => edge.ExtractChildValue(value);

            action
                .Should()
                .ThrowExactly<ArgumentException>()
                .WithMessage($"Expected {nameof(value)} to be of type {typeof(CustomType).Name} but was of type {value.GetType().Name} instead");
        }

        [Test]
        public void Test_Setting_A_Child_Value()
        {
            CustomType value = new CustomType();
            string childValue = "value";

            edge.SetChildValue(value, childValue);

            setter.Received().Invoke(value, childValue);
        }

        [Test]
        public void Test_Setting_A_Child_Value_With_A_Null_Value()
        {
            Action action = () => edge.SetChildValue(null, string.Empty);

            action.Should()
                .ThrowArgumentNullException("value");
        }

        [Test]
        public void Test_Setting_A_Null_Child_Value()
        {
            Action action = () => edge.SetChildValue(new CustomType(), null);

            action.Should()
                .ThrowArgumentNullException("childValue");
        }

        private class CustomType
        {
        }
    }
}
