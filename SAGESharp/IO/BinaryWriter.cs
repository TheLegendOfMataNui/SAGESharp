/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using System;

namespace SAGESharp.IO
{
    /// <summary>
    /// Interface to write chunks of binary data from numbers.
    /// </summary>
    public interface IBinaryWriter
    {
        /// <summary>
        /// Pointer position of the underlying binary data.
        /// </summary>
        long Position { get; set; }

        /// <summary>
        /// Writes a single byte.
        /// </summary>
        /// 
        /// <param name="value">The byte to write.</param>
        void WriteByte(byte value);

        /// <summary>
        /// Writes an array of bytes.
        /// </summary>
        /// 
        /// <param name="values">The bytes to write.</param>
        /// 
        /// <exception cref="ArgumentNullException">If <paramref name="values"/> is null.</exception>
        void WriteBytes(byte[] values);

        /// <summary>
        /// Writes a <see cref="short"/> (2 bytes).
        /// </summary>
        /// 
        /// <param name="value">The value to write.</param>
        void WriteInt16(short value);

        /// <summary>
        /// Writes an <see cref="ushort"/> (2 bytes).
        /// </summary>
        /// 
        /// <param name="value">The value to write.</param>
        void WriteUInt16(ushort value);

        /// <summary>
        /// Writes a <see cref="int"/> (4 bytes).
        /// </summary>
        /// 
        /// <param name="value">The value to write.</param>
        void WriteInt32(int value);

        /// <summary>
        /// Writes an <see cref="uint"/> (4 bytes).
        /// </summary>
        /// 
        /// <param name="value">The value to write.</param>
        void WriteUInt32(uint value);

        /// <summary>
        /// Writes a <see cref="long"/> (8 bytes).
        /// </summary>
        /// 
        /// <param name="value">The value to write.</param>
        void WriteInt64(long value);

        /// <summary>
        /// Writes an <see cref="ulong"/> (8 bytes).
        /// </summary>
        /// 
        /// <param name="value">The value to write.</param>
        void WriteUInt64(ulong value);

        /// <summary>
        /// Writes a <see cref="float"/> (4 bytes).
        /// </summary>
        /// 
        /// <param name="value">The value to write.</param>
        void WriteFloat(float value);

        /// <summary>
        /// Writes a <see cref="double"/> (8 bytes).
        /// </summary>
        /// 
        /// <param name="value">The value to write.</param>
        void WriteDouble(double value);
    }
}
