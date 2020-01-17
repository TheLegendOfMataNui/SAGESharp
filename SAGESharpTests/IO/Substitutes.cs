/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NSubstitute;
using System;

namespace SAGESharp.IO
{
    /*
     * This is a very janky solution for a limitation of the substitution
     * framework: you cannot mock/setup several calls over the same property.
     * 
     * This is particularly problematic for the `DoAtPosition` extension
     * methods that are considerably used for (de)serializing objects in binary
     * format.
     * 
     * The way to workaround is to "alias" the `Position` property for
     * `IBinaryReader` and `IBinaryWriter` in abstract classes and that way
     * you can setup the consecutive calls via `GetPosition` and `SetPosition`.
     */
    interface IPositionable
    {
        long GetPosition();

        void SetPosition(long value);
    }

    static class Positionable
    {
        /// <summary>
        /// Verifies <paramref name="action"/> is executed after setting the position of
        /// <paramref name="positionable"/> to <paramref name="temporalPosition"/> and then
        /// gets restored to <paramref name="originalPosition"/>.
        /// </summary>
        /// 
        /// <remarks>
        /// This should be called within a <see cref="Received.InOrder(Action)"/> block.
        /// </remarks>
        /// 
        /// <param name="positionable">The posibionable to verify.</param>
        /// <param name="originalPosition">The expected original position.</param>
        /// <param name="temporalPosition">The expected temporal position.</param>
        /// <param name="action">The action to be executed.</param>
        public static void VerifyDoAtPosition(this IPositionable positionable, long originalPosition, long temporalPosition, Action action)
        {
            positionable.GetPosition();
            positionable.SetPosition(temporalPosition);
            action.Invoke();
            positionable.SetPosition(originalPosition);
        }
    }

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
