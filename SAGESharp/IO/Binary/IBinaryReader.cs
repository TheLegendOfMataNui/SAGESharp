/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
namespace SAGESharp.IO.Binary
{
    /// <summary>
    /// Interface to read chunks of binary data as numbers.
    /// </summary>
    public interface IBinaryReader
    {
        /// <summary>
        /// Pointer position of the underlying binary data.
        /// </summary>
        long Position { get; set; }

        /// <summary>
        /// Reads a single byte.
        /// </summary>
        /// 
        /// <returns>A single byte read.</returns>
        byte ReadByte();

        /// <summary>
        /// Reads "<paramref name="count"/>" bytes and returns them as an array.
        /// </summary>
        /// 
        /// <param name="count">The count of bytes to read.</param>
        /// 
        /// <returns>A byte array with the "<paramref name="count"/>" of bytes.</returns>
        byte[] ReadBytes(int count);

        /// <summary>
        /// Reads a <see cref="short"/> number.
        /// </summary>
        /// 
        /// <returns>A <see cref="short"/> number.</returns>
        short ReadInt16();

        /// <summary>
        /// Reads an <see cref="ushort"/> number.
        /// </summary>
        /// 
        /// <returns>An <see cref="ushort"/> number.</returns>
        ushort ReadUInt16();

        /// <summary>
        /// Reads an <see cref="int"/> number.
        /// </summary>
        /// 
        /// <returns>An <see cref="int"/> number.</returns>
        int ReadInt32();

        /// <summary>
        /// Reads an <see cref="uint"/> number.
        /// </summary>
        /// 
        /// <returns>An <see cref="uint"/> number.</returns>
        uint ReadUInt32();

        /// <summary>
        /// Reads a <see cref="long"/> number.
        /// </summary>
        /// 
        /// <returns>A <see cref="long"/> number.</returns>
        long ReadInt64();

        /// <summary>
        /// Reads an <see cref="ulong"/> number.
        /// </summary>
        /// 
        /// <returns>An <see cref="ulong"/> number.</returns>
        ulong ReadUInt64();

        /// <summary>
        /// Reads a <see cref="float"/> number.
        /// </summary>
        /// 
        /// <returns>A <see cref="float"/> number.</returns>
        float ReadFloat();

        /// <summary>
        /// Reads a <see cref="double"/> number.
        /// </summary>
        /// 
        /// <returns>A <see cref="double"/> number.</returns>
        double ReadDouble();
    }
}
