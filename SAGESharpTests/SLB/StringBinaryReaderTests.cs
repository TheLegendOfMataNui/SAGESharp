using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace SAGESharp.SLB
{
    public class StringBinaryReaderTests
    {
        private readonly Stream stream = Substitute.For<Stream>();

        private readonly ISLBBinaryReader<string> reader;

        public StringBinaryReaderTests()
        {
            reader = new StringBinaryReader(stream);
        }

        [SetUp]
        public void Setup()
        {
            stream.ClearReceivedCalls();
        }

        [Test]
        public void Test_Creating_A_StringBinaryReader_With_Null_Dependencies()
            => this
                .Invoking(_ => new StringBinaryReader(null))
                .Should()
                .Throw<ArgumentNullException>();

        [Test]
        public void Test_Reading_A_String()
        {
            var expected = "A string"
                .Select(c => c.ToASCIIByte())
                .ToArray();
            var length = expected.Length;

            stream.ReadByte().Returns(length);
            stream
                .Read(Arg.Do<byte[]>(bytes => expected.CopyTo(bytes, 0)), 0, length)
                .Returns(length);

            reader.ReadSLBObject().Should().Be("A string");
        }
    }
}
