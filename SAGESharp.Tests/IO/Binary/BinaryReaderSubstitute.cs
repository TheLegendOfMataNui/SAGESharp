/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NSubstitute;
using SAGESharp.IO.Binary;

namespace SAGESharp.Tests.IO.Binary
{
    abstract class BinaryReaderSubstitute : IBinaryReader, IPositionable
    {
        public static BinaryReaderSubstitute New() => Substitute.ForPartsOf<BinaryReaderSubstitute>();

        public abstract long GetPosition();

        public abstract void SetPosition(long position);

        public long Position { get => GetPosition(); set => SetPosition(value); }

        public abstract byte ReadByte();

        public abstract byte[] ReadBytes(int count);

        public abstract double ReadDouble();

        public abstract float ReadFloat();

        public abstract short ReadInt16();

        public abstract int ReadInt32();

        public abstract long ReadInt64();

        public abstract ushort ReadUInt16();

        public abstract uint ReadUInt32();

        public abstract ulong ReadUInt64();
    }
}
