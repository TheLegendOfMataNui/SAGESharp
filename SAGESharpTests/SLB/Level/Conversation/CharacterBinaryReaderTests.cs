using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using SAGESharp.Testing;
using System;
using System.Collections.Generic;
using System.IO;

namespace SAGESharp.SLB.Level.Conversation
{
    class CharacterBinaryReaderTests
    {
        private readonly Stream stream = Substitute.For<Stream>();

        private readonly ISLBBinaryReader<Info> infoReader = Substitute.For<ISLBBinaryReader<Info>>();

        private readonly ISLBBinaryReader<Character> reader;

        public CharacterBinaryReaderTests()
        {
            reader = new CharacterBinaryReader(stream, infoReader);
        }

        [SetUp]
        public void Setup()
        {
            stream.ClearReceivedCalls();
            infoReader.ClearReceivedCalls();
        }

        [Test]
        public void Test_Creating_A_CharacterBinaryReader_With_Null_Dependencies()
        {
            this.Invoking(_ => new CharacterBinaryReader(null, infoReader)).Should().Throw<ArgumentNullException>();
            this.Invoking(_ => new CharacterBinaryReader(stream, null)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Test_Reading_A_Character_With_Info_Entries()
        {
            var expected = new byte[]
            {
                0x04, 0x03, 0x02, 0x01, // ToaName
                0x14, 0x13, 0x12, 0x11, // CharName
                0x24, 0x23, 0x22, 0x21, // CharCont
                0x02, 0x00, 0x00, 0x00, // Info entries count
                0x1C, 0x00, 0x00, 0x00  // Info entries position
            };

            stream
                .Read(Arg.Do<byte[]>(bytes => expected.CopyTo(bytes, 0)), 0, Character.BINARY_SIZE)
                .Returns(Character.BINARY_SIZE);

            stream.Position.Returns(0x0A);

            infoReader.ReadSLBObject().Returns(_ => new Info());

            reader.ReadSLBObject().Should().Be(new Character
            {
                ToaName = 0x01020304,
                CharName = 0x11121314,
                CharCont = 0x21222324,
                Entries = new List<Info> { new Info(), new Info() }
            });

            Received.InOrder(() =>
            {
                stream.Position = 0x1C;
                infoReader.ReadSLBObject();
                infoReader.ReadSLBObject();
                stream.Position = 0x0A;
            });
        }

        [Test]
        public void Test_Reading_A_Character_Without_Info_Entries()
        {
            var expected = new byte[]
            {
                0x04, 0x03, 0x02, 0x01, // ToaName
                0x14, 0x13, 0x12, 0x11, // CharName
                0x24, 0x23, 0x22, 0x21, // CharCont
                0x00, 0x00, 0x00, 0x00, // Info entries count
                0x00, 0x00, 0x00, 0x00  // Info entries position
            };

            stream
                .Read(Arg.Do<byte[]>(bytes => expected.CopyTo(bytes, 0)), 0, Character.BINARY_SIZE)
                .Returns(Character.BINARY_SIZE);

            reader.ReadSLBObject().Should().Be(new Character
            {
                ToaName = 0x01020304,
                CharName = 0x11121314,
                CharCont = 0x21222324,
                Entries = new List<Info> { }
            });

            stream.DidNotReceive().Position = Arg.Any<long>();
            infoReader.DidNotReceive().ReadSLBObject();
        }
    }
}
