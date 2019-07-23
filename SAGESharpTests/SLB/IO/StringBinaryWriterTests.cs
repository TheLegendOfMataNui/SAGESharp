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
using System.IO;

namespace SAGESharp.SLB.IO
{
    class StringBinaryWriterTests
    {
        private readonly Stream stream;

        private readonly ISLBBinaryWriter<string> writer;

        public StringBinaryWriterTests()
        {
            stream = null;
            stream = Substitute.For<Stream>();
            writer = new StringBinaryWriter(stream);
        }

        [SetUp]
        public void Setup()
        {
            stream.ClearReceivedCalls();
        }

        [Test]
        public void Test_Creating_A_StringBinaryWriter_With_Null_Dependencies()
        {
            this.Invoking(_ => new StringBinaryWriter(null)).Should().Throw<ArgumentNullException>();
        }

        [TestCaseSource(nameof(StringsWithByteRepresentation))]
        public void Test_Writing_A_String(string input, byte[] expected)
        {
            writer.WriteSLBObject(input);

            stream.Received().Write(Matcher.ForEquivalentArray(expected), 0, expected.Length);
        }

        static object[] StringsWithByteRepresentation() => new ParameterGroup<string, byte[]>()
            .Parameters(null, new byte[] { 0, 0 })
            .Parameters(string.Empty, new byte[] { 0, 0 })
            .Parameters("ABCD", new byte[] { 4, 0x41, 0x42, 0x43, 0x44, 0 })
            .Build();
    }
}
