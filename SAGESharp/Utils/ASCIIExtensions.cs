/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using System.Text;

namespace SAGESharp.Utils
{
    internal static class ASCIIExtensions
    {
        /// <summary>
        /// Converts the input byte to a char based on ASCII.
        /// </summary>
        /// 
        /// <param name="b">The byte to convert.</param>
        /// 
        /// <returns>The byte in char form based on ASCII.</returns>
        public static char ToASCIIChar(this byte b)
        {
            return Encoding.ASCII.GetChars(new[] { b })[0];
        }

        /// <summary>
        /// Converts the input char to a byte based on ASCII.
        /// </summary>
        /// 
        /// <param name="c">The char to convert.</param>
        /// 
        /// <returns>The char in byte form based on ASCII.</returns>
        public static byte ToASCIIByte(this char c)
        {
            return Encoding.ASCII.GetBytes(new[] { c })[0];
        }

        /// <summary>
        /// Returns true if the input byte is an ASCII value representing a digit.
        /// </summary>
        /// 
        /// <param name="b">The input byte.</param>
        /// 
        /// <returns>True if the input byte is a value representing a digit, false otherwise.</returns>
        public static bool IsASCIIDigit(this byte b)
        {
            return 0x30 <= b && b <= 0x39;
        }

        /// <summary>
        /// Returns true if the input byte is an ASCII value representing an uppercase letter.
        /// </summary>
        /// 
        /// <param name="b">The input byte.</param>
        /// 
        /// <returns>True if the input byte is a value representing an uppercase letter, false otherwise.</returns>
        public static bool IsASCIIUppercaseLetter(this byte b)
        {
            return 0x41 <= b && b <= 0x5A;
        }

        /// <summary>
        /// Returns true if the input byte is an ASCII value representing a lower case letter.
        /// </summary>
        /// 
        /// <param name="b">The input byte.</param>
        /// 
        /// <returns>True if the input byte is a value representing a lower letter, false otherwise.</returns>
        public static bool IsASCIILowercaseLetter(this byte b)
        {
            return 0x61 <= b && b <= 0x7A;
        }
    }
}
