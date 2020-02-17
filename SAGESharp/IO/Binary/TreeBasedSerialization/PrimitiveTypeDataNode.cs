/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUtils.Validations;
using System;
using System.Collections.Generic;

namespace SAGESharp.IO.Binary.TreeBasedSerialization
{
    internal sealed class PrimitiveTypeDataNode<T> : IDataNode where T : struct
    {
        private readonly Func<IBinaryReader, object> read;

        private readonly Action<IBinaryWriter, object> write;

        public PrimitiveTypeDataNode()
        {
            if (TypeIs<byte>())
            {
                read = WrapBinaryReaderIfEnum(binaryReader => binaryReader.ReadByte());
                write = (binaryWriter, value) => binaryWriter.WriteByte((byte)value);
            }
            else if (TypeIs<short>())
            {
                read = WrapBinaryReaderIfEnum(binaryReader => binaryReader.ReadInt16());
                write = (binaryWriter, value) => binaryWriter.WriteInt16((short)value);
            }
            else if (TypeIs<ushort>())
            {
                read = WrapBinaryReaderIfEnum(binaryReader => binaryReader.ReadUInt16());
                write = (binaryWriter, value) => binaryWriter.WriteUInt16((ushort)value);
            }
            else if (TypeIs<int>())
            {
                read = WrapBinaryReaderIfEnum(binaryReader => binaryReader.ReadInt32());
                write = (binaryWriter, value) => binaryWriter.WriteInt32((int)value);
            }
            else if (TypeIs<uint>())
            {
                read = WrapBinaryReaderIfEnum(binaryReader => binaryReader.ReadUInt32());
                write = (binaryWriter, value) => binaryWriter.WriteUInt32((uint)value);
            }
            else if (TypeIs<long>())
            {
                read = WrapBinaryReaderIfEnum(binaryReader => binaryReader.ReadInt64());
                write = (binaryWriter, value) => binaryWriter.WriteInt64((long)value);
            }
            else if (TypeIs<ulong>())
            {
                read = WrapBinaryReaderIfEnum(binaryReader => binaryReader.ReadUInt64());
                write = (binaryWriter, value) => binaryWriter.WriteUInt64((ulong)value);
            }
            else if (TypeIs<float>())
            {
                read = (binaryReader) => binaryReader.ReadFloat();
                write = (binaryWriter, value) => binaryWriter.WriteFloat((float)value);
            }
            else if (TypeIs<double>())
            {
                read = (binaryReader) => binaryReader.ReadDouble();
                write = (binaryWriter, value) => binaryWriter.WriteDouble((double)value);
            }
            else if (TypeIs<SLB.Identifier>())
            {
                read = (binaryReader) => (SLB.Identifier)binaryReader.ReadUInt32();
                write = (binaryWriter, value) => binaryWriter.WriteUInt32((SLB.Identifier)value);
            }
            else
            {
                throw BadTypeException.For<T>($"Type {typeof(T).Name} is not a valid primitive.");
            }
        }

        public IReadOnlyList<IEdge> Edges => new List<IEdge>();

        public object Read(IBinaryReader binaryReader)
        {
            Validate.ArgumentNotNull(binaryReader, nameof(binaryReader));

            return read(binaryReader);
        }

        public void Write(IBinaryWriter binaryWriter, object value)
        {
            Validate.ArgumentNotNull(binaryWriter, nameof(binaryWriter));
            Validate.ArgumentNotNull(value, nameof(value));
            Validate.Argument(IsOfType(value), $"Cannot write value of type {value.GetType().Name} as type {typeof(T).Name}.");

            write(binaryWriter, value);
        }

        private static bool TypeIs<U>()
        {
            if (typeof(T).IsEnum)
            {
                return Enum.GetUnderlyingType(typeof(T)) == typeof(U);
            }
            else
            {
                return typeof(T) == typeof(U);
            }
        }

        private static bool IsOfType(object value) => typeof(T) == value.GetType();

        private static Func<IBinaryReader, object> WrapBinaryReaderIfEnum(Func<IBinaryReader, object> function)
        {
            if (!typeof(T).IsEnum)
            {
                return function;
            }
            else
            {
                return (binaryReader) => Enum.ToObject(typeof(T), function(binaryReader));
            }
        }
    }
}
