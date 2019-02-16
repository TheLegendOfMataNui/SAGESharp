using System;
using System.IO;
using System.Linq;
using Konvenience;

namespace SAGESharp.SLB
{
    /// <summary>
    /// Class to read null terminated strings from binary SLB files.
    /// </summary>
    public class StringBinaryReader : ISLBBinaryReader<string>
    {
        private readonly Stream stream;

        /// <summary>
        /// Creates a new reader with the input stream.
        /// </summary>
        /// 
        /// <param name="stream">The input stream.</param>
        /// 
        /// <exception cref="ArgumentNullException">If <paramref name="stream"/> is null.</exception>
        public StringBinaryReader(Stream stream)
            => this.stream = stream ?? throw new ArgumentNullException("Input stream cannot be null.");

        /// <inheritdoc/>
        public string ReadSLBObject() => stream
            .ForceReadByte()
            .Let(length => stream.ForceReadBytes(length))
            .Select(b => b.ToASCIIChar())
            .Let(buffer => string.Concat(buffer));
    }
}
