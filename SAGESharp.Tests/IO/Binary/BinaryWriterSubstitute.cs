/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NSubstitute;
using SAGESharp.IO.Binary;

namespace SAGESharp.Tests.IO.Binary
{
    abstract class BinaryWriterSubstitute : IBinaryWriter, IPositionable
    {
        public static BinaryWriterSubstitute New() => Substitute.ForPartsOf<BinaryWriterSubstitute>();

        public abstract long GetPosition();

        public abstract void SetPosition(long position);

        public long Position { get => GetPosition(); set => SetPosition(value); }

        public abstract void WriteByte(byte value);

        public abstract void WriteBytes(byte[] values);

        public abstract void WriteDouble(double value);

        public abstract void WriteFloat(float value);

        public abstract void WriteInt16(short value);

        public abstract void WriteInt32(int value);

        public abstract void WriteInt64(long value);

        public abstract void WriteUInt16(ushort value);

        public abstract void WriteUInt32(uint value);

        public abstract void WriteUInt64(ulong value);
    }
}
