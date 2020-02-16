/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUtils.Extensions;
using System;
using System.IO;

namespace SAGESharp.IO.Binary
{
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

        public void WriteInt64(long value)
            => realWriter.Write(value);

        public void WriteUInt16(ushort value)
            => realWriter.Write(value);

        public void WriteUInt32(uint value)
            => realWriter.Write(value);

        public void WriteUInt64(ulong value)
            => realWriter.Write(value);
    }
}
