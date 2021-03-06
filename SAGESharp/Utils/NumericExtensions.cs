/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using System;

namespace SAGESharp.Utils
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
    }
}
