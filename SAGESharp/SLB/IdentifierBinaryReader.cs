using System;
using System.IO;

namespace SAGESharp.SLB
{
    /// <summary>
    /// Class to read an binary Identifier from a stream.
    /// </summary>
    internal sealed class IdentifierBinaryReader : ISLBBinaryReader<Identifier>
    {
        private readonly Stream stream;

        /// <summary>
        /// Creates a new reader with the given input.
        /// </summary>
        /// 
        /// <param name="stream">The input to read, cannot be null</param>
        public IdentifierBinaryReader(Stream stream)
        {
            this.stream = stream ?? throw new ArgumentNullException("Input stream cannot be null.");
        }

        /// <inheritdoc/>
        public Identifier ReadSLBObject()
        {
            return Identifier.From(new byte[]
            {
                stream.ForceReadByte(),
                stream.ForceReadByte(),
                stream.ForceReadByte(),
                stream.ForceReadByte()
            });
        }
    }
}
