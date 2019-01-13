using System;
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

        /// <summary>
        /// Reads a single integer from the stream or throws if the end of the stream was reached.
        /// </summary>
        /// 
        /// <param name="stream">The stream to read</param>
        /// 
        /// <returns>The next integer in the stream.</returns>
        /// 
        /// <exception cref="EndOfStreamException">If the stream was read completely.</exception>
        public static int ForceReadInt(this Stream stream)
        {
            var bytes = new byte[]
            {
                ForceReadByte(stream),
                ForceReadByte(stream),
                ForceReadByte(stream),
                ForceReadByte(stream)
            };

            return BitConverter.ToInt32(bytes, 0);
        }

        /// <summary>
        /// Reads a single unsigned integer from the stream or throws if the end of the stream was reached.
        /// </summary>
        /// 
        /// <param name="stream">Thre stream to read</param>
        /// 
        /// <returns>The next unsigned integer in the stream.</returns>
        /// 
        /// <exception cref="EndOfStreamException">If the stream was read completely.</exception>
        public static uint ForceReadUInt(this Stream stream)
        {
            var bytes = new byte[]
            {
                ForceReadByte(stream),
                ForceReadByte(stream),
                ForceReadByte(stream),
                ForceReadByte(stream)
            };

            return BitConverter.ToUInt32(bytes, 0);
        }
    }
}
