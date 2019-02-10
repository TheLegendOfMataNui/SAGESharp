using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using SAGESharpTests;
using System;
using System.IO;
using System.Linq;

namespace SAGESharp.SLB
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

            stream.Received().Write(Arg.Is<byte[]>(bytes => bytes.SequenceEqual(expected)), 0, expected.Length);
        }

        static object[] StringsWithByteRepresentation() => new ParameterGroup<string, byte[]>()
            .Parameters(null, new byte[] { 0, 0 })
            .Parameters(string.Empty, new byte[] { 0, 0 })
            .Parameters("ABCD", new byte[] { 4, 0x41, 0x42, 0x43, 0x44, 0 })
            .Build();
    }
}
