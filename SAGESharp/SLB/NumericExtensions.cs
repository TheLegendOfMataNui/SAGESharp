using System;

namespace SAGESharp.SLB
{
    internal static class NumericExtensions
    {
        #region Byte manipulation
        /// <summary>
        /// Gets an individual byte from the input value.
        /// </summary>
        /// 
        /// <remarks>
        /// The starting position is zero.
        /// </remarks>
        /// 
        /// <param name="value">The input value.</param>
        /// <param name="bytePosition">The byte position to get.</param>
        /// 
        /// <returns>The byte in the given position of the input value.</returns>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">If the byte position is out of range.</exception>
        public static byte GetByte(this uint value, byte bytePosition)
        {
            if (bytePosition > 3)
            {
                throw new ArgumentOutOfRangeException();
            }

            return (byte)(value >> (bytePosition * 8));
        }

        /// <summary>
        /// Sets an individual byte in a copy of the input value.
        /// </summary>
        /// 
        /// <remarks>
        /// The starting position is zero.
        /// </remarks>
        /// 
        /// <param name="value">The input value.</param>
        /// <param name="bytePosition">The byte position to set.</param>
        /// <param name="byteValue">The value that will be set.</param>
        /// 
        /// <returns>A copy of the input value with the byte in the given position updated.</returns>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">If the byte position is out of range.</exception>
        public static uint SetByte(this uint value, byte bytePosition, byte byteValue)
        {
            if (bytePosition > 3)
            {
                throw new ArgumentOutOfRangeException();
            }

            var realBytePosition = bytePosition * 8;
            uint zeroes = (uint)~(0xFF << realBytePosition);
            uint finalByte = (uint)(byteValue << realBytePosition);
            return (value & zeroes) | finalByte;
        }
        #endregion

        #region Conversions from byte array
        /// <summary>
        /// Converts the input byte array to an <see cref="int"/> reading 4 bytes starting from <paramref name="startIndex"/>.
        /// </summary>
        /// 
        /// <param name="bytes">The bytes to convert.</param>
        /// <param name="startIndex">The position where the conversion will start.</param>
        /// 
        /// <returns>An <see cref="int"/> with the values from the bytes.</returns>
        public static int ToInt32(this byte[] bytes, int startIndex = 0)
            => BitConverter.ToInt32(bytes, startIndex);

        /// <summary>
        /// Converts the input byte array to an <see cref="uint"/> reading 4 bytes starting from <paramref name="startIndex"/>.
        /// </summary>
        /// 
        /// <param name="bytes">The bytes to convert.</param>
        /// <param name="startIndex">The position where the conversion will start.</param>
        /// 
        /// <returns>An <see cref="uint"/> with the values from the bytes.</returns>
        public static uint ToUInt32(this byte[] bytes, int startIndex = 0)
            => BitConverter.ToUInt32(bytes, startIndex);
        #endregion

        #region Conversions to byte array
        /// <summary>
        /// Gets the bytes from <paramref name="value"/> as an array.
        /// </summary>
        /// 
        /// <param name="value">The value to convert to an array.</param>
        /// 
        /// <returns>The bytes form <paramref name="value"/> in array form.</returns>
        public static byte[] ToByteArray(this int value)
            => BitConverter.GetBytes(value);

        /// <summary>
        /// Gets the bytes from <paramref name="value"/> as an array.
        /// </summary>
        /// 
        /// <param name="value">The value to convert to an array.</param>
        /// 
        /// <returns>The bytes form <paramref name="value"/> in array form.</returns>
        public static byte[] ToByteArray(this uint value)
            => BitConverter.GetBytes(value);
        #endregion
    }
}
