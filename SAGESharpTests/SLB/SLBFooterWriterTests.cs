using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace SAGESharp.SLB
{
    class SLBFooterWriterTests
    {
        private readonly Stream stream;

        private readonly ISLBFooterGenerator<object> footerGenerator;

        private readonly ISLBFooterWriter<object> footerWriter;

        public SLBFooterWriterTests()
        {
            stream = Substitute.For<Stream>();
            footerGenerator = Substitute.For<ISLBFooterGenerator<object>>();
            footerWriter = new SLBFooterWriter<object>(stream, footerGenerator);
        }

        [SetUp]
        public void Setup()
        {
            stream.ClearReceivedCalls();
            footerGenerator.ClearReceivedCalls();
        }

        [Test]
        public void Test_Creating_A_SLBFooterWriter_With_Null_Dependencies()
        {
            this.Invoking(_ => new SLBFooterWriter<object>(null, footerGenerator))
                .Should()
                .Throw<ArgumentNullException>();

            this.Invoking(_ => new SLBFooterWriter<object>(stream, null))
                .Should()
                .Throw<ArgumentNullException>();
        }

        [Test]
        public void Test_Writing_An_Aligned_Footer()
        {
            var dummy = new object();

            footerGenerator.GenerateFooter(dummy).Returns(new List<FooterEntry>()
            {
                new FooterEntry { OffsetPosition = 0x01020304, Offset = 0x0A0B0C0D },
                new FooterEntry { OffsetPosition = 0x11121314, Offset = 0x1A1B1C1D },
                new FooterEntry { OffsetPosition = 0x21222324, Offset = 0x2A2B2C2D }
            });

            footerWriter.WriteFooter(dummy);

            Received.InOrder(() =>
            {
                stream.Position = 0x01020304;
                stream.Write(Matcher.ForEquivalentArray(new byte[] { 0x0D, 0x0C, 0x0B, 0x0A }), 0, 4);

                stream.Position = 0x11121314;
                stream.Write(Matcher.ForEquivalentArray(new byte[] { 0x1D, 0x1C, 0x1B, 0x1A }), 0, 4);

                stream.Position = 0x21222324;
                stream.Write(Matcher.ForEquivalentArray(new byte[] { 0x2D, 0x2C, 0x2B, 0x2A }), 0, 4);

                stream.Seek(0, SeekOrigin.End);

                var footer = new byte[]
                {
                    0x04, 0x03, 0x02, 0x01,
                    0x14, 0x13, 0x12, 0x11,
                    0x24, 0x23, 0x22, 0x21,
                    0x03, 0x00, 0x00, 0x00,
                    0xEE, 0xFF, 0xC0, 0x00
                };

                stream.Write(Matcher.ForEquivalentArray(footer), 0, footer.Length);
            });
        }

        [Test]
        public void Test_Writing_An_Unaligned_Footer()
        {
            var dummy = new object();

            footerGenerator.GenerateFooter(dummy).Returns(new List<FooterEntry>()
            {
                new FooterEntry { OffsetPosition = 0x01020304, Offset = 0x0A0B0C0D },
                new FooterEntry { OffsetPosition = 0x11121314, Offset = 0x1A1B1C1D },
                new FooterEntry { OffsetPosition = 0x21222324, Offset = 0x2A2B2C2D }
            });

            stream.Seek(0, SeekOrigin.End).Returns(29);

            footerWriter.WriteFooter(dummy);

            Received.InOrder(() =>
            {
                stream.Position = 0x01020304;
                stream.Write(Matcher.ForEquivalentArray(new byte[] { 0x0D, 0x0C, 0x0B, 0x0A }), 0, 4);

                stream.Position = 0x11121314;
                stream.Write(Matcher.ForEquivalentArray(new byte[] { 0x1D, 0x1C, 0x1B, 0x1A }), 0, 4);

                stream.Position = 0x21222324;
                stream.Write(Matcher.ForEquivalentArray(new byte[] { 0x2D, 0x2C, 0x2B, 0x2A }), 0, 4);

                stream.Seek(0, SeekOrigin.End);

                // Test for footer alignment, position at the end was 29 + 3 = 32
                stream.Write(Matcher.ForEquivalentArray(new byte[3]), 0, 3);

                var footer = new byte[]
                {
                    0x04, 0x03, 0x02, 0x01,
                    0x14, 0x13, 0x12, 0x11,
                    0x24, 0x23, 0x22, 0x21,
                    0x03, 0x00, 0x00, 0x00,
                    0xEE, 0xFF, 0xC0, 0x00
                };

                stream.Write(Matcher.ForEquivalentArray(footer), 0, footer.Length);
            });
        }
    }
}
