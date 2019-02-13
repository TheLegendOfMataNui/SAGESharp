using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.BitConverter;

namespace SAGESharp.SLB
{
    /// <summary>
    /// Interface that writes the footer for a given SLB object.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the SLB object.</typeparam>
    internal interface ISLBFooterWriter<T>
    {
        /// <summary>
        /// Writes the footer for the given SLB object.
        /// </summary>
        /// 
        /// <param name="slbObject">The SLB object to write.</param>
        void WriteFooter(T slbObject);
    }

    /// <summary>
    /// Interface that generates the footer for the given SLB object.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the SLB object </typeparam>
    internal interface ISLBFooterGenerator<T>
    {
        /// <summary>
        /// Generates the a footer from the input object.
        /// </summary>
        /// 
        /// <param name="slbObject">The SLB object that will be used to generate the footer.</param>
        /// 
        /// <returns>The footer generated from the input object.</returns>
        IReadOnlyList<FooterEntry> GenerateFooter(T slbObject);
    }

    /// <summary>
    /// Object that represents an entry in the footer table.
    /// </summary>
    internal struct FooterEntry
    {
        /// <summary>
        /// The position of the offset.
        /// </summary>
        public uint OffsetPosition { get; set; }

        /// <summary>
        /// The offset value.
        /// </summary>
        public uint Offset { get; set; }
    }

    /// <summary>
    /// Implementation of the <see cref="ISLBFooterWriter{T}"/> interface.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the SLB object.</typeparam>
    internal class SLBFooterWriter<T> : ISLBFooterWriter<T>
    {
        private readonly Stream stream;

        private readonly ISLBFooterGenerator<T> footerGenerator;

        /// <summary>
        /// Creates a new SLBFooterWriter.
        /// </summary>
        /// 
        /// <param name="stream">The output stream.</param>
        /// <param name="footerGenerator">The object that will generate the footer to write.</param>
        /// 
        /// <exception cref="ArgumentNullException">If any argument is null.</exception>
        public SLBFooterWriter(Stream stream, ISLBFooterGenerator<T> footerGenerator)
        {
            this.stream = stream ?? throw new ArgumentNullException("The ouput stream cannot be null.");
            this.footerGenerator = footerGenerator ?? throw new ArgumentNullException("The footer generator cannot be null");
        }

        /// <inheritdoc />
        /// 
        /// <exception cref="ArgumentNullException">If the input object is null.</exception>
        public void WriteFooter(T slbObject)
        {
            if (slbObject == null)
            {
                throw new ArgumentNullException();
            }

            var offsets = footerGenerator.GenerateFooter(slbObject);

            foreach (var entry in offsets)
            {
                stream.Position = entry.OffsetPosition;
                stream.WriteUInt(entry.Offset);
            }

            // Jump to the end and align the footer
            var end = (int)stream.Seek(0, SeekOrigin.End);
            // We get the amount of bytes that are unaligned
            // by getting bytes after the last aligned integer (end % 4)
            // and counting the amount of additional bytes needed
            // to be aligned (4 - bytes after las aligned integer)
            int bytesToFill = 4 - (end % 4);
            if (bytesToFill != 4)
            {
                stream.Write(new byte[bytesToFill], 0, bytesToFill);
            }

            // Entries in the table (4 bytes/1 32 bit number per entry)
            // + table size (4 bytes/1 32 bit number)
            // + magic number (4 bytes/1 32 bit number)
            var footerSize = (offsets.Count * 4) + 8;
            var footer = new byte[footerSize];

            var pos = 0;
            foreach (var offset in offsets.Select(o => o.OffsetPosition))
            {
                GetBytes(offset).CopyTo(footer, pos);
                pos += 4;
            }

            GetBytes(offsets.Count).CopyTo(footer, pos);
            pos += 4;

            // Write magic numbers
            footer[pos++] = 0xEE;
            footer[pos++] = 0xFF;
            footer[pos++] = 0xC0;

            stream.Write(footer, 0, footerSize);
        }
    }
}
