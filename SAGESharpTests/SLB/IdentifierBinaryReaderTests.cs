using Moq;
using NUnit.Framework;
using SAGESharp.SLB;
using SAGESharpTests.Util;
using System;
using System.IO;

namespace SAGESharpTests.SLB
{
    [TestFixture]
    public class IdentifierBinaryReaderTests
    {
        [Test]
        public void TestCreatingAnIdentifierBinaryReaderWithANullStream()
        {
            Assert.That(() => new IdentifierBinaryReader(null), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void TestReadingAnIdentifierSuccessfully()
        {
            var streamMock = new Mock<Stream>();
            ISLBBinaryReader<Identifier> reader = new IdentifierBinaryReader(streamMock.Object);

            streamMock
                .SetupSequence(stream => stream.ReadByte())
                .ReturnsIntBytes(0x44434241);

            var identifier = reader.ReadSLBObject();

            Assert.That(identifier.ToInteger(), Is.EqualTo(0x44434241));

            streamMock.Verify(stream => stream.ReadByte(), Times.Exactly(4));
        }
    }
}
