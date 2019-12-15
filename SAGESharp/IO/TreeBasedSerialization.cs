/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUtils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
                if (!typeof(T).IsEnum)
                {
                    read = (binaryReader) => binaryReader.ReadByte();
                }
                else
                {
                    read = (binaryReader) => Enum.ToObject(typeof(T), binaryReader.ReadByte());
                }

                write = (binaryWriter, value) => binaryWriter.WriteByte((byte)value);
            }
            else if (TypeIs<short>())
            {
                if (!typeof(T).IsEnum)
                {
                    read = (binaryReader) => binaryReader.ReadInt16();
                }
                else
                {
                    read = (binaryReader) => Enum.ToObject(typeof(T), binaryReader.ReadInt16());
                }

                write = (binaryWriter, value) => binaryWriter.WriteInt16((short)value);
            }
            else if (TypeIs<ushort>())
            {
                if (!typeof(T).IsEnum)
                {
                    read = (binaryReader) => binaryReader.ReadUInt16();
                }
                else
                {
                    read = (binaryReader) => Enum.ToObject(typeof(T), binaryReader.ReadUInt16());
                }

                write = (binaryWriter, value) => binaryWriter.WriteUInt16((ushort)value);
            }
            else if (TypeIs<int>())
            {
                if (!typeof(T).IsEnum)
                {
                    read = (binaryReader) => binaryReader.ReadInt32();
                }
                else
                {
                    read = (binaryReader) => Enum.ToObject(typeof(T), binaryReader.ReadInt32());
                }

                write = (binaryWriter, value) => binaryWriter.WriteInt32((int)value);
            }
            else if (TypeIs<uint>())
            {
                if (!typeof(T).IsEnum)
                {
                    read = (binaryReader) => binaryReader.ReadUInt32();
                }
                else
                {
                    read = (binaryReader) => Enum.ToObject(typeof(T), binaryReader.ReadUInt32());
                }

                write = (binaryWriter, value) => binaryWriter.WriteUInt32((uint)value);
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
            Validate.ArgumentNotNull(nameof(binaryReader), binaryReader);

            return read(binaryReader);
        }

        public void Write(IBinaryWriter binaryWriter, object value)
        {
            Validate.ArgumentNotNull(nameof(binaryWriter), binaryWriter);
            Validate.ArgumentNotNull(nameof(value), value);
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
            Validate.ArgumentNotNull(nameof(binaryReader), binaryReader);

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
            Validate.ArgumentNotNull(nameof(binaryWriter), binaryWriter);
            Validate.ArgumentNotNull(nameof(value), value);
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
            Validate.ArgumentNotNull(nameof(edges), edges);
            Validate.Argument(edges != null, $"{nameof(edges)} should not be empty");

            Edges = edges;
        }

        public IReadOnlyList<IEdge> Edges { get; }

        public object Read(IBinaryReader binaryReader)
        {
            Validate.ArgumentNotNull(nameof(binaryReader), binaryReader);

            return new T();
        }

        public void Write(IBinaryWriter binaryWriter, object value)
        {
            Validate.ArgumentNotNull(nameof(binaryWriter), binaryWriter);
            Validate.ArgumentNotNull(nameof(value), value);
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
            Validate.ArgumentNotNull(nameof(childNode), childNode);

            padding = new byte[size];
            this.childNode = childNode;
        }

        public IReadOnlyList<IEdge> Edges { get => childNode.Edges; }

        public object Read(IBinaryReader binaryReader)
        {
            Validate.ArgumentNotNull(nameof(binaryReader), binaryReader);

            object result = childNode.Read(binaryReader);

            binaryReader.ReadBytes(padding.Length);

            return result;
        }

        public void Write(IBinaryWriter binaryWriter, object value)
        {
            Validate.ArgumentNotNull(nameof(binaryWriter), binaryWriter);
            Validate.ArgumentNotNull(nameof(value), value);

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
            Validate.ArgumentNotNull(nameof(extractor), extractor);
            Validate.ArgumentNotNull(nameof(setter), setter);
            Validate.ArgumentNotNull(nameof(childNode), childNode);

            this.extractor = extractor;
            this.setter = setter;
            ChildNode = childNode;
        }

        public object ChildNode { get; }

        public object ExtractChildValue(object value)
        {
            Validate.ArgumentNotNull(nameof(value), value);
            Validate.Argument(IsType(value), $"Expected {nameof(value)} to be of type {typeof(T).Name} but was of type {value.GetType().Name} instead");

            return extractor((T)value);
        }

        public void SetChildValue(object value, object childValue)
        {
            Validate.ArgumentNotNull(nameof(value), value);
            Validate.ArgumentNotNull(nameof(childValue), childValue);
            Validate.Argument(IsType(value), $"Expected {nameof(value)} to be of type {typeof(T).Name} but was of type {value.GetType().Name} instead");

            setter((T)value, childValue);
        }

        private static bool IsType(object value) => typeof(T) == value.GetType();
    }

    internal abstract class AbstractOffsetNode : IOffsetNode
    {
        protected AbstractOffsetNode(IDataNode childNode)
        {
            Validate.ArgumentNotNull(nameof(childNode), childNode);

            ChildNode = childNode;
        }

        public IDataNode ChildNode { get; }

        public uint ReadOffset(IBinaryReader binaryReader)
        {
            Validate.ArgumentNotNull(nameof(binaryReader), binaryReader);

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
            Validate.ArgumentNotNull(nameof(binaryWriter), binaryWriter);
            Validate.ArgumentNotNull(nameof(value), value);

            return WriteOffset(binaryWriter);
        }
    }

    internal sealed class ListNode<T> : AbstractOffsetNode, IListNode
    {
        private readonly bool duplicateEntryCount;

        public ListNode(IDataNode childNode, bool duplicateEntryCount = false) : base(childNode)
        {
            this.duplicateEntryCount = duplicateEntryCount;
        }

        public int GetListCount(object list)
        {
            Validate.ArgumentNotNull(nameof(list), list);
            ValidateIsList(list);

            return (list as IList<T>).Count;
        }

        public object GetListEntry(object list, int index)
        {
            Validate.ArgumentNotNull(nameof(list), list);
            ValidateIsList(list);

            return (list as IList<T>)[index];
        }

        public void AddListEntry(object list, object value)
        {
            Validate.ArgumentNotNull(nameof(list), list);
            Validate.ArgumentNotNull(nameof(value), value);
            ValidateIsList(list);
            Validate.Argument(
                typeof(T).Equals(value.GetType()),
                $"Value should be of type {typeof(string).Name}, but is of type {value.GetType().Name} instead."
            );

            (list as IList<T>).Add((T)value);
        }

        public object CreateList()
        {
            return new List<T>();
        }

        public int ReadEntryCount(IBinaryReader binaryReader)
        {
            Validate.ArgumentNotNull(nameof(binaryReader), binaryReader);

            int result = binaryReader.ReadInt32();

            if (duplicateEntryCount)
            {
                binaryReader.ReadInt32();
            }

            return result;
        }

        public override uint Write(IBinaryWriter binaryWriter, object value)
        {
            Validate.ArgumentNotNull(nameof(binaryWriter), binaryWriter);
            Validate.ArgumentNotNull(nameof(value), value);
            ValidateIsList(value);

            binaryWriter.WriteInt32((value as IList<T>).Count);
            if (duplicateEntryCount)
            {
                binaryWriter.WriteInt32((value as IList<T>).Count);
            }

            return WriteOffset(binaryWriter);
        }

        private static void ValidateIsList(object list) => Validate.Argument(
            typeof(IList<T>).IsAssignableFrom(list.GetType()),
            $"List argument is of type {list.GetType().Name} which should implement {typeof(IList<T>).Name} but doesn't."
        );
    }

    internal static class TreeBuilder
    {
        public static IDataNode BuildTreeForType(Type type)
        {
            Validate.ArgumentNotNull(nameof(type), type);

            return BuildUserTypeDataNode(type);
        }

        private static object BuildPaddedNodeForProperty(PropertyInfo propertyInfo)
        {
            object node = BuildNodeForProperty(propertyInfo);

            RightPaddingAttribute rightPaddingAttribute = propertyInfo
                .GetCustomAttribute<RightPaddingAttribute>();

            if (rightPaddingAttribute is null)
            {
                return node;
            }
            else if (node is IDataNode dataNode)
            {
                return new PaddingNode(rightPaddingAttribute.Size, dataNode);
            }
            else
            {
                throw new BadTypeException(propertyInfo.DeclaringType,
                    $"Property {propertyInfo.Name} cannot be padded.");
            }
        }

        private static object BuildNodeForProperty(PropertyInfo propertyInfo)
        {
            if (IsPrimitiveType(propertyInfo.PropertyType))
            {
                return PrimitiveTypeDataNode(propertyInfo.PropertyType);
            }
            else if (typeof(string) == propertyInfo.PropertyType)
            {
                return StringDataNode(propertyInfo);
            }
            else if (IsListType(propertyInfo.PropertyType))
            {
                return BuildListNode(propertyInfo);
            }
            else
            {
                return BuildUserTypeDataNode(propertyInfo.PropertyType);
            }
        }

        #region PrimitiveTypeDataNode
        private static IDataNode PrimitiveTypeDataNode(Type type)
        {
            return (IDataNode)typeof(PrimitiveTypeDataNode<>)
                .MakeGenericType(type)
                .GetConstructor(Array.Empty<Type>())
                .Invoke(Array.Empty<object>());
        }

        private static bool IsPrimitiveType(Type type)
        {
            return type == typeof(byte) || type == typeof(short) || type == typeof(ushort)
                 || type == typeof(int) || type == typeof(uint) || type == typeof(SLB.Identifier)
                 || type == typeof(float) || type == typeof(double)
                 || (type.IsEnum && IsPrimitiveType(Enum.GetUnderlyingType(type)));
        }
        #endregion

        #region StringDataNode
        private static object StringDataNode(PropertyInfo propertyInfo)
        {
            OffsetStringAttribute offsetStringAttribute = propertyInfo
                .GetCustomAttribute<OffsetStringAttribute>();

            InlineStringAttribute inlineStringAttribute = propertyInfo
                .GetCustomAttribute<InlineStringAttribute>();

            if (offsetStringAttribute != null && inlineStringAttribute != null)
            {
                throw new BadTypeException(propertyInfo.DeclaringType,
                    $"Property {propertyInfo.Name} has duplicate string location attributes.");
            }
            else if (offsetStringAttribute != null)
            {
                return new OffsetNode(new StringDataNode());
            }
            else if (inlineStringAttribute != null)
            {
                return new StringDataNode(inlineStringAttribute.Length);
            }
            else
            {
                throw new BadTypeException(propertyInfo.DeclaringType,
                    $"Property {propertyInfo.Name} is missing a string location attribute.");
            }
        }
        #endregion

        #region UserTypeDataNode
        private static IDataNode BuildUserTypeDataNode(Type type)
        {
            ValidateUserTypeHasConstructor(type);

            IReadOnlyList<IEdge> edges = type.GetProperties()
                .Select(p => SerializableProperty.For(p))
                .Where(p => p != null)
                .OrderBy(p => p.Order)
                .Also(ps => ValidateSerializableProperties(type, ps))
                .Select(p => BuildEdge(p.PropertyInfo))
                .ToList();

            return (IDataNode)typeof(UserTypeDataNode<>)
                .MakeGenericType(type)
                .GetConstructor(new Type[] { typeof(IReadOnlyList<IEdge>) }).Invoke(new object[] { edges });
        }

        private static void ValidateUserTypeHasConstructor(Type type)
        {
            if (type.GetConstructor(Array.Empty<Type>()) is null)
            {
                throw new BadTypeException(type, "Type doesn't have a public parameterless constructor.");
            }
        }

        private static void ValidateSerializableProperties(Type type, IOrderedEnumerable<SerializableProperty> properties)
        {
            int count = properties.Count();
            if (count == 0)
            {
                throw new BadTypeException(type, "Type doesn't have serializable properties.");
            }
            else if (count != properties.Select(p => p.Order).Distinct().Count())
            {
                throw new BadTypeException(type, "Type has two or more properties with the same serialization order.");
            }
        }

        private class SerializableProperty
        {
            private SerializableProperty(PropertyInfo propertyInfo, byte order)
            {
                PropertyInfo = propertyInfo;
                Order = order;
            }

            public PropertyInfo PropertyInfo { get; }

            public byte Order { get; }

            public static SerializableProperty For(PropertyInfo propertyInfo) => propertyInfo
                .GetCustomAttribute<SerializablePropertyAttribute>()
                ?.Let(attribute => new SerializableProperty(propertyInfo, attribute.BinaryOrder));
        }
        #endregion

        #region Edge
        private static IEdge BuildEdge(PropertyInfo propertyInfo)
        {
            Type edgeType = typeof(Edge<>).MakeGenericType(propertyInfo.DeclaringType);
            Type extractorType = typeof(Func<,>).MakeGenericType(propertyInfo.DeclaringType, typeof(object));
            Type setterType = typeof(Action<,>).MakeGenericType(propertyInfo.DeclaringType, typeof(object));

            object extractor = GetExtractorForEdge(propertyInfo);
            object setter = GetSetterForEdge(propertyInfo);
            object childNode = BuildPaddedNodeForProperty(propertyInfo);

            return (IEdge)edgeType.GetConstructor(new Type[] { extractorType, setterType, typeof(object) })
                .Invoke(new object[] { extractor, setter, childNode });
        }

        private static object GetExtractorForEdge(PropertyInfo propertyInfo)
        {
            ParameterExpression valueParameter = Expression.Parameter(propertyInfo.DeclaringType);

            MethodCallExpression methodCall = Expression.Call(valueParameter, propertyInfo.GetMethod);

            UnaryExpression castedMethodCall = Expression.Convert(methodCall, typeof(object));

            LambdaExpression lambdaExpression = Expression.Lambda(castedMethodCall, valueParameter);

            return lambdaExpression.Compile();
        }

        private static object GetSetterForEdge(PropertyInfo propertyInfo)
        {
            ParameterExpression valueParameter = Expression.Parameter(propertyInfo.DeclaringType);
            ParameterExpression childValueParameter = Expression.Parameter(typeof(object));
            UnaryExpression castedChildValueParameter = Expression.Convert(childValueParameter, propertyInfo.PropertyType);

            MethodCallExpression methodCall = Expression.Call(valueParameter, propertyInfo.SetMethod, castedChildValueParameter);

            LambdaExpression lambdaExpression = Expression.Lambda(methodCall, valueParameter, childValueParameter);

            return lambdaExpression.Compile();
        }
        #endregion

        #region ListNode
        private static object BuildListNode(PropertyInfo propertyInfo)
        {
            Type typeOfList = propertyInfo.PropertyType.GenericTypeArguments[0];

            object childNode = BuildUserTypeDataNode(typeOfList);

            bool duplicateEntryCount = propertyInfo
                .GetCustomAttribute<DuplicateEntryCountAttribute>() != null;

            return typeof(ListNode<>)
                .MakeGenericType(typeOfList)
                .GetConstructor(new Type[] { typeof(IDataNode), typeof(bool) })
                .Invoke(new object[] { childNode, duplicateEntryCount });
        }

        private static bool IsListType(Type type)
        {
            return type.IsGenericType && typeof(IList<>) == type.GetGenericTypeDefinition();
        }
        #endregion
    }

    internal sealed class TreeReader
    {
        public object Read(IBinaryReader binaryReader, IDataNode rootNode)
        {
            Validate.ArgumentNotNull(nameof(binaryReader), binaryReader);
            Validate.ArgumentNotNull(nameof(rootNode), rootNode);

            return ProcessDataNode(binaryReader, rootNode);
        }

        private object ProcessNode(IBinaryReader binaryReader, object node)
        {
            if (node is IDataNode dataNode)
            {
                return ProcessDataNode(binaryReader, dataNode);
            }
            else if (node is IListNode listNode)
            {
                return ProcessListNode(binaryReader, listNode);
            }
            else if (node is IOffsetNode offsetNode)
            {
                return ProcessOffsetNode(binaryReader, offsetNode);
            }
            else
            {
                throw new NotImplementedException($"Type {node.GetType().Name} is an unknown node type");
            }
        }

        private object ProcessDataNode(IBinaryReader binaryReader, IDataNode node)
        {
            object value = node.Read(binaryReader);

            foreach (IEdge edge in node.Edges)
            {
                object childValue = ProcessNode(binaryReader, edge.ChildNode);

                edge.SetChildValue(value, childValue);
            }

            return value;
        }

        private object ProcessOffsetNode(IBinaryReader binaryReader, IOffsetNode node)
        {
            uint offset = node.ReadOffset(binaryReader);

            object result = null;
            binaryReader.DoAtPosition(offset, () => result = ProcessDataNode(binaryReader, node.ChildNode));

            return result;
        }

        private object ProcessListNode(IBinaryReader binaryReader, IListNode listNode)
        {
            int count = listNode.ReadEntryCount(binaryReader);
            uint offset = listNode.ReadOffset(binaryReader);
            object list = listNode.CreateList();

            if (count != 0)
            {
                binaryReader.DoAtPosition(offset, () =>
                {
                    for (int n = 0; n < count; ++n)
                    {
                        object entry = ProcessDataNode(binaryReader, listNode.ChildNode);
                        listNode.AddListEntry(list, entry);
                    }
                });
            }

            return list;
        }
    }

    internal sealed class TreeWriter
    {
        private class QueueEntry
        {
            public QueueEntry(IDataNode node, object value)
            {
                Node = node;
                Value = value;
            }

            public QueueEntry(IDataNode node, object value, uint offsetPosition) : this(node, value)
            {
                OffsetPosition = offsetPosition;
            }

            public IDataNode Node { get; }

            public object Value { get; }

            public uint? OffsetPosition { get; }
        }

        private readonly Queue<QueueEntry> queue = new Queue<QueueEntry>();

        private readonly List<uint> offsets = new List<uint>();

        public IReadOnlyList<uint> Write(IBinaryWriter binaryWriter, object value, IDataNode rootNode)
        {
            Validate.ArgumentNotNull(nameof(binaryWriter), binaryWriter);
            Validate.ArgumentNotNull(nameof(value), value);
            Validate.ArgumentNotNull(nameof(rootNode), rootNode);

            queue.Clear();
            offsets.Clear();

            Enqueue(rootNode, value);

            while (queue.IsNotEmpty())
            {
                QueueEntry entry = queue.Dequeue();

                ProcessOffset(binaryWriter, entry.OffsetPosition);
                if (entry.Value != null)
                {
                    ProcessDataNode(binaryWriter, entry.Node, entry.Value);
                }
            }

            return offsets;
        }

        private void ProcessOffset(IBinaryWriter binaryWriter, uint? offsetPosition)
        {
            if (!offsetPosition.HasValue)
            {
                return;
            }

            binaryWriter.DoAtPosition(offsetPosition.Value, originalPosition =>
            {
                Validate.Argument(originalPosition <= uint.MaxValue,
                    $"Offset 0x{originalPosition:X} is larger than {sizeof(uint)} bytes.");

                binaryWriter.WriteUInt32((uint)originalPosition);
            });
            offsets.Add(offsetPosition.Value);
        }

        private void ProcessNode(IBinaryWriter binaryWriter, object node, object value)
        {
            if (node is IDataNode dataNode)
            {
                ProcessDataNode(binaryWriter, dataNode, value);
            }
            else if (node is IListNode listNode)
            {
                ProcessListNode(binaryWriter, listNode, value);
            }
            else if (node is IOffsetNode offsetNode)
            {
                ProcessOffsetNode(binaryWriter, offsetNode, value);
            }
            else
            {
                throw new NotImplementedException($"Type {node.GetType().Name} is an unknown node type");
            }
        }

        private void ProcessDataNode(IBinaryWriter binaryWriter, IDataNode node, object value)
        {
            node.Write(binaryWriter, value);

            foreach (IEdge edge in node.Edges)
            {
                object childValue = edge.ExtractChildValue(value);
                ProcessNode(binaryWriter, edge.ChildNode, childValue);
            }
        }

        private void ProcessOffsetNode(IBinaryWriter binaryWriter, IOffsetNode node, object value)
        {
            uint offsetPosition = node.Write(binaryWriter, value);

            Enqueue(node.ChildNode, value, offsetPosition);
        }

        private void ProcessListNode(IBinaryWriter binaryWriter, IListNode node, object list)
        {
            uint offsetPosition = node.Write(binaryWriter, list);
            int count = node.GetListCount(list);
            if (count == 0)
            {
                Enqueue(node.ChildNode, null, offsetPosition);
                return;
            }

            Enqueue(node.ChildNode, node.GetListEntry(list, 0), offsetPosition);

            for (int n = 1; n < count; ++n)
            {
                Enqueue(node.ChildNode, node.GetListEntry(list, n));
            }
        }

        private void Enqueue(IDataNode node, object value)
        {
            queue.Enqueue(new QueueEntry(node, value));
        }

        private void Enqueue(IDataNode node, object value, uint offsetPosition)
        {
            queue.Enqueue(new QueueEntry(node, value, offsetPosition));
        }
    }
    #endregion
}
