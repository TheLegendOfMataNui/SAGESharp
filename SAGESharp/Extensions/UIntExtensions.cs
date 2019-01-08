using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.Extensions
{
    public static class UIntExtensions
    {
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
