/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUtils.Extensions;
using System;
using System.IO;

namespace SAGESharp.IO
{
    #region Interface
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

    /// <summary>
    /// Static class to function as a simple factory for <see cref="IBinaryReader"/> instances.
    /// </summary>
    public static class Reader
    {
        /// <summary>
        /// Gets a <see cref="IBinaryReader"/> for the intpu <paramref name="stream"/>.
        /// </summary>
        /// 
        /// <param name="stream">The input stream to use in the reader.</param>
        /// 
        /// <returns>A <see cref="IBinaryReader"/> to read the input <paramref name="stream"/>.</returns>
        public static IBinaryReader ForStream(Stream stream)
            => new BinaryReaderWrapper(stream);

        /// <summary>
        /// Executes <paramref name="action"/> while temporarily moving the reader to <paramref name= "position" />.
        /// </summary>
        /// 
        /// <param name="reader">The reader that w</param>
        /// <param name="position">The position where the reader will be moved temporarily.</param>
        /// <param name="action">The action to execute.</param>
        public static void DoAtPosition(this IBinaryReader reader, long position, Action action)
        {
            var originalPosition = reader.Position;
            reader.Position = position;

            action();
            reader.Position = originalPosition;
        }

        /// <summary>
        /// Returns the result of <paramref name="function"/> while temporarily moving the reader to <paramref name= "position" />.
        /// </summary>
        /// 
        /// <typeparam name="TResult">The type that will be returned.</typeparam>
        /// 
        /// <param name="reader">The reader that will be changed temproarily.</param>
        /// <param name="position">The position where the reader will be moved temporarily.</param>
        /// <param name="function">The action to execute.</param>
        /// 
        /// <returns>The result of <paramref name="function"/>.</returns>
        public static TResult DoAtPosition<TResult>(this IBinaryReader reader, long position, Func<TResult> function)
        {
            var originalPosition = reader.Position;
            reader.Position = position;

            var result = function();
            reader.Position = originalPosition;

            return result;
        }
    }
    #endregion

    #region Implementation
    internal sealed class BinaryReaderWrapper : IBinaryReader
    {
        private readonly BinaryReader realReader;

        public BinaryReaderWrapper(Stream stream)
            => realReader = stream?.Let(s => new BinaryReader(s)) ?? throw new ArgumentNullException();

        public long Position
        {
            get => realReader.BaseStream.Position;
            set => realReader.BaseStream.Position = value;
        }

        public byte ReadByte()
            => realReader.ReadByte();

        public byte[] ReadBytes(int count)
            => realReader.ReadBytes(count);

        public short ReadInt16()
            => realReader.ReadInt16();

        public ushort ReadUInt16()
            => realReader.ReadUInt16();

        public int ReadInt32()
            => realReader.ReadInt32();

        public uint ReadUInt32()
            => realReader.ReadUInt32();

        public long ReadInt64()
            => realReader.ReadInt64();

        public ulong ReadUInt64()
            => realReader.ReadUInt64();

        public float ReadFloat()
            => realReader.ReadSingle();

        public double ReadDouble()
            => realReader.ReadDouble();
    }
    #endregion
}
