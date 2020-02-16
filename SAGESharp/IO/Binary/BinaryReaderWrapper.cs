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
}
