/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;
using SAGESharp.IO;
using System;

namespace SAGESharp.Tests.IO
{
    class BinaryWriterTests
    {
        private readonly BinaryWriterSubstitute binaryWriter = BinaryWriterSubstitute.New();

        [SetUp]
        public void Setup()
        {
            binaryWriter.ClearSubstitute();
        }

        [Test]
        public void Test_DoAtPosition()
        {
            long originalPosition = 70, temporalPosition = 20;
            Action<long> action = Substitute.For<Action<long>>();

            binaryWriter.GetPosition().Returns(originalPosition, temporalPosition);

            binaryWriter.DoAtPosition(temporalPosition, action);

            Received.InOrder(() => binaryWriter.VerifyDoAtPosition(
                originalPosition: originalPosition,
                temporalPosition: temporalPosition,
                action: () => action(originalPosition)
            ));
        }
    }
}
