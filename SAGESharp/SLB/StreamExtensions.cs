using System;
using System.IO;
using Konvenience;

namespace SAGESharp.SLB
{
    internal static class StreamExtensions
    {
        /// <summary>
        /// Writes an integer value to the stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to write</param>
        /// <param name="value">The integer to write in the stream</param>
        public static void WriteInt(this Stream stream, int value)
        {
            stream.Write(BitConverter.GetBytes(value), 0, 4);
        }

        /// <summary>
        /// Writes an unsigned integer value to the stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to write</param>
        /// <param name="value">The unsigned integer to write in the stream</param>
        public static void WriteUInt(this Stream stream, uint value)
        {
            stream.Write(BitConverter.GetBytes(value), 0, 4);
        }
    }
}
