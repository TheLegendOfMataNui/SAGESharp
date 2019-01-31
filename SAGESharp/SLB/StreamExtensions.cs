using System;
using System.IO;

namespace SAGESharp.SLB
{
    internal static class StreamExtensions
    {
        /// <summary>
        /// Updates the stream to the given position and performs the action,
        /// aftwards restores the original stream position.
        /// </summary>
        /// 
        /// <param name="stream">The stream that will get its positions swapped</param>
        /// <param name="position">The position where the stream will be updated temporary</param>
        /// <param name="action">The action to be executed</param>
        public static void OnPositionDo(this Stream stream, long position, Action action)
        {
            var originalPosition = stream.Position;
            stream.Position = position;

            action();
            stream.Position = originalPosition;
        }

        /// <summary>
        /// Uptas the stream to the given position and performs the input function,
        /// aftwards restors the original stream position and returns the result of the function.
        /// </summary>
        /// 
        /// <typeparam name="TResult">The return type of the function.</typeparam>
        /// 
        /// <param name="stream">The stream that will get its positions swapped</param>
        /// <param name="position">The position where the stream will be updated temporary</param>
        /// <param name="action">The function to be executed</param>
        /// 
        /// <returns>The result of the function</returns>
        public static TResult OnPositionDo<TResult>(this Stream stream, long position, Func<TResult> function)
        {
            var originalPosition = stream.Position;
            stream.Position = position;

            var result = function();
            stream.Position = originalPosition;

            return result;
        }

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
        /// Reads a single ASCII char from the stream or throws if the end of the stream was reached.
        /// </summary>
        /// 
        /// <param name="stream">The stream to read</param>
        /// 
        /// <returns>The next ASCII char in the stream.</returns>
        /// 
        /// <exception cref="EndOfStreamException">If the stream was read completely.</exception>
        public static char ForceReadASCIIChar(this Stream stream)
        {
            return ASCIIExtensions.ToASCIIChar(ForceReadByte(stream));
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

        /// <summary>
        /// Writes an ASCII character to the stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to write</param>
        /// <param name="value">The ASCII character to write in the stream</param>
        public static void WriteASCIIChar(this Stream stream, char value)
        {
            stream.WriteByte(ASCIIExtensions.ToASCIIByte(value));
        }

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
