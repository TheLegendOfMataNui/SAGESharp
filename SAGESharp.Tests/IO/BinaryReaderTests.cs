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

namespace SAGESharp.Tests.IO
{
    class BinaryReaderTests
    {
        private readonly BinaryReaderSubstitute binaryReader = BinaryReaderSubstitute.New();

        [SetUp]
        public void Setup()
        {
            binaryReader.ClearSubstitute();
        }

        [Test]
        public void Test_DoAtPosition_With_Result()
        {
            long originalPosition = 20, temporalPosition = 30;
            string expected = nameof(expected);
            Func<string> function = Substitute.For<Func<string>>();

            function.Invoke().Returns(expected);

            binaryReader.GetPosition().Returns(originalPosition, temporalPosition);

            string result = binaryReader.DoAtPosition(temporalPosition, function);

            result.Should().Be(expected);

            Received.InOrder(() => binaryReader.VerifyDoAtPosition(
                originalPosition: originalPosition,
                temporalPosition: temporalPosition,
                action: () => function()
            ));
        }

        [Test]
        public void Test_DoAtPosition_Without_Result()
        {
            long originalPosition = 20, temporalPosition = 30;
            string expected = nameof(expected);
            Action action = Substitute.For<Action>();

            binaryReader.GetPosition().Returns(originalPosition, temporalPosition);

            binaryReader.DoAtPosition(temporalPosition, action);

            Received.InOrder(() => binaryReader.VerifyDoAtPosition(
                originalPosition: originalPosition,
                temporalPosition: temporalPosition,
                action: action
            ));
        }
    }
}
