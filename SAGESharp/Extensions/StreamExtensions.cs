using System.IO;

namespace SAGESharp.Extensions
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Reads a single byte from the stream or throws if the end of the stream was reached.
        /// </summary>
        /// 
        /// <param name="stream">The stream to read</param>
        /// 
        /// <returns>The next byte in the stream.</returns>
        /// 
        /// <exception cref="EndOfStreamException">If the stream was read completely.</exception>
        public static byte ForceReadByte(this Stream stream)
        {
            var result = stream.ReadByte();
            if (result == -1)
            {
                throw new EndOfStreamException();
            }

            return (byte)result;
        }
    }
}
