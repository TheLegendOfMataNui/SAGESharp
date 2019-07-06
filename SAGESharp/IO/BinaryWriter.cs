/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using Konvenience;
using System;
using System.IO;

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
        /// Writes an unsigned <see cref="uint"/> (4 bytes).
        /// </summary>
        /// 
        /// <param name="value">The value to write.</param>
        void WriteUInt32(uint value);

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

    /// <summary>
    /// Static class to function as a simple factory for <see cref="IBinaryWriter"/> instances.
    /// </summary>
    public static class Writer
    {
        /// <summary>
        /// Gets a <see cref="IBinaryWriter"/> for the input <paramref name="stream"/>.
        /// </summary>
        /// 
        /// <param name="stream">The input stream to use in the writer.</param>
        /// 
        /// <returns>A <see cref="IBinaryWriter"/> that writes into the input <paramref name="stream"/>.</returns>
        public static IBinaryWriter ForStream(Stream stream)
            => new BinaryWriterWrapper(stream);
    }

    internal sealed class BinaryWriterWrapper : IBinaryWriter
    {
        private readonly BinaryWriter realWriter;

        public BinaryWriterWrapper(Stream stream)
            => realWriter = stream?.Let(s => new BinaryWriter(s)) ?? throw new ArgumentNullException();

        public long Position
        {
            get => realWriter.BaseStream.Position;
            set => realWriter.BaseStream.Position = value;
        }

        public void WriteByte(byte value)
            => realWriter.Write(value);

        public void WriteBytes(byte[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException();
            }

            realWriter.Write(values);
        }

        public void WriteDouble(double value)
            => realWriter.Write(value);

        public void WriteFloat(float value)
            => realWriter.Write(value);

        public void WriteInt16(short value)
            => realWriter.Write(value);

        public void WriteInt32(int value)
            => realWriter.Write(value);

        public void WriteUInt16(ushort value)
            => realWriter.Write(value);

        public void WriteUInt32(uint value)
            => realWriter.Write(value);
    }

    internal static class IBinaryWriterExtensions
    {
        /// <summary>
        /// Executes <paramref name="action"/> while temporarily moving the writer to <paramref name="position"/>.
        /// </summary>
        /// 
        /// <param name="writer">The writer that whose position will change temporarily.</param>
        /// <param name="position">The new temporal position for the writer.</param>
        /// <param name="action">The action to execute, it receives the original .</param>
        public static void DoAtPosition(this IBinaryWriter writer, long position, Action<long> action)
        {
            var originalPosition = writer.Position;
            writer.Position = position;

            action(originalPosition);
            writer.Position = originalPosition;
        }
    }
}
