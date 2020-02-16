/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUtils.Extensions;
using NUtils.Validations;
using SAGESharp.IO.Binary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAGESharp.IO
{
    #region Interfaces
    /// <summary>
    /// Represents a node with data (with children nodes) in the tree.
    /// </summary>
    internal interface IDataNode
    {
        /// <summary>
        /// Reads a value from the <paramref name="binaryReader"/>.
        /// </summary>
        /// 
        /// <param name="binaryReader">The reader that will be used to read the value.</param>
        /// 
        /// <returns>The value read from <paramref name="binaryReader"/>.</returns>
        /// 
        /// <exception cref="ArgumentNullException">If <paramref name="binaryReader"/> is null.</exception>
        object Read(IBinaryReader binaryReader);

        /// <summary>
        /// Writes <paramref name="value"/> to the given <paramref name="binaryWriter"/>.
        /// </summary>
        /// 
        /// <param name="binaryWriter">The output binary writer were the object will be written.</param>
        /// <param name="value">The value to be written.</param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If either <paramref name="binaryWriter"/> or <paramref name="value"/> are null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> is not of the expected type.
        /// </exception>
        void Write(IBinaryWriter binaryWriter, object value);

        /// <summary>
        /// Returns the list of edges connecting to child nodes.
        /// </summary>
        IReadOnlyList<IEdge> Edges { get; }
    }

    /// <summary>
    /// Represents a node that will write its contents at a later time.
    /// </summary>
    internal interface IOffsetNode
    {
        /// <summary>
        /// Reads the offset location at which the value of <see cref="ChildNode"/> is located.
        /// </summary>
        /// 
        /// <param name="binaryReader">The input binary reader to read the offset from.</param>
        /// 
        /// <returns>The offset of value of <see cref="ChildNode"/>.</returns>
        uint ReadOffset(IBinaryReader binaryReader);

        /// <summary>
        /// Writes <paramref name="value"/> to the given <paramref name="binaryWriter"/>.
        /// </summary>
        /// 
        /// <param name="binaryWriter">The output binary writer were the object will be written.</param>
        /// <param name="value">The value to be written.</param>
        /// 
        /// <returns>The position where the offset was written.</returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If either <paramref name="binaryWriter"/> or <paramref name="value"/> are null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> is not of the expected type.
        /// </exception>
        uint Write(IBinaryWriter binaryWriter, object value);

        /// <summary>
        /// The actual node that will write the contents of the object.
        /// </summary>
        IDataNode ChildNode { get; }
    }

    /// <summary>
    /// Represents a node with a single child but several entries of the same child node.
    /// </summary>
    internal interface IListNode : IOffsetNode
    {
        /// <summary>
        /// Creates an instance of the corresponding list type.
        /// </summary>
        /// 
        /// <returns>A new instance of the corresponding list type.</returns>
        object CreateList();

        /// <summary>
        /// Reads the amount of entries for the list from <paramref name="binaryReader"/>.
        /// </summary>
        /// 
        /// <param name="binaryReader">The input binary reader to read the count from.</param>
        /// 
        /// <returns>The amount of entries for a list.</returns>
        /// 
        /// <exception cref="ArgumentNullException">If <paramref name="binaryReader"/> is null.</exception>
        int ReadEntryCount(IBinaryReader binaryReader);

        /// <summary>
        /// Retrieves the amount of entries in the given <paramref name="list"/>.
        /// </summary>
        /// 
        /// <param name="list">The input list.</param>
        /// 
        /// <returns>The amount of object ins the input list.</returns>
        /// 
        /// <exception cref="ArgumentNullException">If <paramref name="list"/> is null.</exception>
        int GetListCount(object list);

        /// <summary>
        /// Retrieves the element with <paramref name="index"/> in the given <paramref name="list"/>.
        /// </summary>
        /// 
        /// <param name="list">The input list.</param>
        /// <param name="index">The index of the element.</param>
        /// 
        /// <returns>
        /// The element with the given <paramref name="index"/> in the given <paramref name="list"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">If <paramref name="list"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is not valid for the input list.</exception>
        object GetListEntry(object list, int index);

        /// <summary>
        /// Adds <paramref name="value"/> as a new entry to <paramref name="list"/>.
        /// </summary>
        /// 
        /// <param name="list">The list to add the entry to.</param>
        /// <param name="value">The entry that will be added to the list.</param>
        /// 
        /// <exception cref="ArgumentNullException">If either argument is null.</exception>
        void AddListEntry(object list, object value);
    }

    /// <summary>
    /// Represents an edge connecting a <see cref="IDataNode"/> to
    /// a node (ex: <see cref="IDataNode"/>, <see cref="IOffsetNode"/>).
    /// </summary>
    internal interface IEdge
    {
        /// <summary>
        /// The child node that the edge is connecting.
        /// </summary>
        object ChildNode { get; }

        /// <summary>
        /// Extracts from the given <paramref name="value"/> a child value that is represented by the child node.
        /// </summary>
        /// 
        /// <param name="value">The object represented by the parent node.</param>
        /// 
        /// <returns>The child value represented by the child node.</returns>
        object ExtractChildValue(object value);

        /// <summary>
        /// Sets this edge's corresponding child of <paramref name="value"/> to <paramref name="childValue"/>.
        /// </summary>
        /// 
        /// <param name="value">The value to set the child.</param>
        /// <param name="childValue">The child value that will be set.</param>
        /// 
        /// <exception cref="ArgumentNullException">If either argument is null.</exception>
        void SetChildValue(object value, object childValue);
    }
    #endregion

    #region Implementations
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

    internal sealed class StringDataNode : IDataNode
    {
        internal const byte OFFSET_STRING_MAX_LENGTH = byte.MaxValue - 1;

        private static readonly IReadOnlyList<IEdge> edges = new List<IEdge>();

        private readonly bool inlineString;

        private readonly byte length;

        public StringDataNode()
        {
            length = OFFSET_STRING_MAX_LENGTH;
            inlineString = false;
        }

        public StringDataNode(byte length)
        {
            this.length = length;
            inlineString = true;
        }

        public IReadOnlyList<IEdge> Edges => edges;

        public object Read(IBinaryReader binaryReader)
        {
            Validate.ArgumentNotNull(binaryReader, nameof(binaryReader));

            if (inlineString)
            {
                return ReadInlineString(binaryReader);
            }
            else
            {
                return ReadOfflineString(binaryReader);
            }
        }

        private string ReadInlineString(IBinaryReader binaryReader)
        {
            byte[] bytes = binaryReader.ReadBytes(length);
            int posOfNullCharacter = 0;

            // Get the position of the first null character
            while (posOfNullCharacter < bytes.Length && bytes[posOfNullCharacter] != 0)
            {
                posOfNullCharacter++;
            }

            return bytes.Take(posOfNullCharacter)
                .ToArray()
                .Let(Encoding.ASCII.GetString);
        }

        private string ReadOfflineString(IBinaryReader binaryReader)
        {
            byte length = binaryReader.ReadByte();
            string result = binaryReader.ReadBytes(length)
                .Let(Encoding.ASCII.GetString);

            // Read string termination character
            binaryReader.ReadByte();

            return result;
        }

        public void Write(IBinaryWriter binaryWriter, object value)
        {
            Validate.ArgumentNotNull(binaryWriter, nameof(binaryWriter));
            Validate.ArgumentNotNull(value, nameof(value));
            Validate.Argument(IsString(value), $"Cannot write value of type {value.GetType().Name} as a string.");
            Validate.Argument(IsCorrectLength(value), $"String length is longer than {length}.");

            string valueAsString = value as string;

            if (inlineString)
            {
                binaryWriter.WriteBytes(Encoding.ASCII.GetBytes(valueAsString));

                int diff = length - valueAsString.Length;
                if (diff != 0)
                {
                    binaryWriter.WriteBytes(new byte[diff]);
                }
            }
            else
            {
                binaryWriter.WriteByte((byte)valueAsString.Length);
                binaryWriter.WriteBytes(Encoding.ASCII.GetBytes(valueAsString));
                binaryWriter.WriteByte(0);
            }
        }

        private bool IsCorrectLength(object value) => (value as string).Length <= length;

        private static bool IsString(object value) => typeof(string).Equals(value.GetType());
    }

    internal sealed class UserTypeDataNode<T> : IDataNode where T : new()
    {
        public UserTypeDataNode(IReadOnlyList<IEdge> edges)
        {
            Validate.ArgumentNotNull(edges, nameof(edges));
            Validate.Argument(edges != null, $"{nameof(edges)} should not be empty");

            Edges = edges;
        }

        public IReadOnlyList<IEdge> Edges { get; }

        public object Read(IBinaryReader binaryReader)
        {
            Validate.ArgumentNotNull(binaryReader, nameof(binaryReader));

            return new T();
        }

        public void Write(IBinaryWriter binaryWriter, object value)
        {
            Validate.ArgumentNotNull(binaryWriter, nameof(binaryWriter));
            Validate.ArgumentNotNull(value, nameof(value));
            Validate.Argument(IsType(value), $"Cannot write value of type {value.GetType().Name} as type {typeof(T).Name}.");
        }

        private static bool IsType(object value) => typeof(T) == value.GetType();
    }

    internal sealed class PaddingNode : IDataNode
    {
        private readonly byte[] padding;

        private readonly IDataNode childNode;

        public PaddingNode(byte size, IDataNode childNode)
        {
            Validate.Argument(size > 0, "Padding size cannot be 0.");
            Validate.ArgumentNotNull(childNode, nameof(childNode));

            padding = new byte[size];
            this.childNode = childNode;
        }

        public IReadOnlyList<IEdge> Edges { get => childNode.Edges; }

        public object Read(IBinaryReader binaryReader)
        {
            Validate.ArgumentNotNull(binaryReader, nameof(binaryReader));

            object result = childNode.Read(binaryReader);

            binaryReader.ReadBytes(padding.Length);

            return result;
        }

        public void Write(IBinaryWriter binaryWriter, object value)
        {
            Validate.ArgumentNotNull(binaryWriter, nameof(binaryWriter));
            Validate.ArgumentNotNull(value, nameof(value));

            childNode.Write(binaryWriter, value);

            binaryWriter.WriteBytes(padding);
        }
    }

    internal sealed class Edge<T> : IEdge
    {
        private readonly Func<T, object> extractor;

        private readonly Action<T, object> setter;

        public Edge(Func<T, object> extractor, Action<T, object> setter, object childNode)
        {
            Validate.ArgumentNotNull(extractor, nameof(extractor));
            Validate.ArgumentNotNull(setter, nameof(setter));
            Validate.ArgumentNotNull(childNode, nameof(childNode));

            this.extractor = extractor;
            this.setter = setter;
            ChildNode = childNode;
        }

        public object ChildNode { get; }

        public object ExtractChildValue(object value)
        {
            Validate.ArgumentNotNull(value, nameof(value));
            Validate.Argument(IsType(value), $"Expected {nameof(value)} to be of type {typeof(T).Name} but was of type {value.GetType().Name} instead");

            return extractor((T)value);
        }

        public void SetChildValue(object value, object childValue)
        {
            Validate.ArgumentNotNull(value, nameof(value));
            Validate.ArgumentNotNull(childValue, nameof(childValue));
            Validate.Argument(IsType(value), $"Expected {nameof(value)} to be of type {typeof(T).Name} but was of type {value.GetType().Name} instead");

            setter((T)value, childValue);
        }

        private static bool IsType(object value) => typeof(T) == value.GetType();
    }

    internal abstract class AbstractOffsetNode : IOffsetNode
    {
        protected AbstractOffsetNode(IDataNode childNode)
        {
            Validate.ArgumentNotNull(childNode, nameof(childNode));

            ChildNode = childNode;
        }

        public IDataNode ChildNode { get; }

        public uint ReadOffset(IBinaryReader binaryReader)
        {
            Validate.ArgumentNotNull(binaryReader, nameof(binaryReader));

            return binaryReader.ReadUInt32();
        }

        public abstract uint Write(IBinaryWriter binaryWriter, object value);

        protected uint WriteOffset(IBinaryWriter binaryWriter)
        {
            if (binaryWriter.Position > uint.MaxValue)
            {
                throw new InvalidOperationException("Offset is bigger than 4 bytes.");
            }

            uint offsetPosition = (uint) binaryWriter.Position;

            binaryWriter.WriteUInt32(0);

            return offsetPosition;
        }
    }

    internal sealed class OffsetNode : AbstractOffsetNode
    {
        public OffsetNode(IDataNode childNode) : base(childNode)
        {
        }

        public override uint Write(IBinaryWriter binaryWriter, object value)
        {
            Validate.ArgumentNotNull(binaryWriter, nameof(binaryWriter));
            Validate.ArgumentNotNull(value, nameof(value));

            return WriteOffset(binaryWriter);
        }
    }
    #endregion
}
