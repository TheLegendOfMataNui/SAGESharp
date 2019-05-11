/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using Konvenience;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SAGESharp.IO
{
    /// <summary>
    /// Represents an object that serializes objects of type <typeparamref name="T"/>.
    /// </summary>
    /// 
    /// <typeparam name="T">Type of the objects to serialize.</typeparam>
    public interface IBinarySerializer<T>
    {
        /// <summary>
        /// Reads an object of type <typeparamref name="T"/> from the input <paramref name="binaryReader"/>.
        /// </summary>
        /// 
        /// <param name="binaryReader">The binary reader used to read the object.</param>
        /// 
        /// <returns>The object read from the <paramref name="binaryReader"/>.</returns>
        T Read(IBinaryReader binaryReader);

        /// <summary>
        /// Writes <paramref name="value"/> to the output <paramref name="binaryWriter"/>.
        /// </summary>
        /// 
        /// <param name="binaryWriter">The output binary writer were the object will be written.</param>
        /// <param name="value">The object to write.</param>
        void Write(IBinaryWriter binaryWriter, T value);
    }

    /// <summary>
    /// Represents an object that can serialize itself.
    /// </summary>
    public interface IBinarySerializable
    {
        /// <summary>
        /// Reads data from the <paramref name="binaryReader"/> into the object itself.
        /// </summary>
        /// 
        /// <param name="binaryReader">The input reader.</param>
        void Read(IBinaryReader binaryReader);

        /// <summary>
        /// Writes data to the <paramref name="binaryWriter"/> from the object itself.
        /// </summary>
        /// 
        /// <param name="binaryWriter">The output writer.</param>
        void Write(IBinaryWriter binaryWriter);
    }

    /// <summary>
    /// The exception that is thrown when a type cannot be serialized.
    /// </summary>
    public class BadTypeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BadTypeException"/>
        /// class with the given <paramref name="type"/>.
        /// </summary>
        /// 
        /// <param name="type">The type that cannot be serialized.</param>
        public BadTypeException(Type type) : base()
            => Type = type;

        /// <summary>
        /// Initializes a new instance of the <see cref="BadTypeException"/>
        /// class with the given <paramref name="type"/> and the given
        /// <paramref name="message"/>.
        /// </summary>
        /// 
        /// <param name="type">The type that cannot be serialized.</param>
        /// <param name="message">The message that describes the error.</param>
        public BadTypeException(Type type, string message) : base(message)
            => Type = type;

        /// <summary>
        /// Initializes a new instance of the <see cref="BadTypeException"/>
        /// class with the given <paramref name="type"/>, the given
        /// <paramref name="message"/> and the given <paramref name="innerException"/>.
        /// </summary>
        /// 
        /// <param name="type">The type that cannot be serialized.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">he exception that is the cause of the current exception.</param>
        public BadTypeException(Type type, string message, Exception innerException)
            : base(message, innerException) => Type = type;

        /// <summary>
        /// The type that cannot be serialized.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Creates a "type safe" <see cref="BadTypeException"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">The type that cannot be serialized.</typeparam>
        /// 
        /// <returns>An instance of <see cref="BadTypeException"/> with type <typeparamref name="T"/>.</returns>
        public static BadTypeException For<T>()
            => new BadTypeException(typeof(T));

        /// <summary>
        /// Creates a "type safe" <see cref="BadTypeException"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">The type that cannot be serialized.</typeparam>
        /// 
        /// <param name="message">The message that describes the error.</param>
        /// 
        /// <returns>
        /// An instance of <see cref="BadTypeException"/> with type <typeparamref name="T"/> and <paramref name="message"/>.
        /// </returns>
        public static BadTypeException For<T>(string message)
            => new BadTypeException(typeof(T), message);

        /// <summary>
        /// Creates a "type safe" <see cref="BadTypeException"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">The type that cannot be serialized.</typeparam>
        /// 
        /// <param name="message">The message that describes the error.</param>
        /// 
        /// <returns>
        /// An instance of <see cref="BadTypeException"/> with type <typeparamref name="T"/>, <paramref name="message"/> and <paramref name="innerException"/>.
        /// </returns>
        public static BadTypeException For<T>(string message, Exception innerException)
            => new BadTypeException(typeof(T), message, innerException);
    }

    /// <summary>
    /// Specifies a property that should be serialized/deserialized.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class SerializablePropertyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new attribute to mark a serializable property.
        /// </summary>
        /// 
        /// <param name="binaryOrder">The binary order of property (see <see cref="BinaryOrder"/>).</param>
        public SerializablePropertyAttribute(byte binaryOrder) => BinaryOrder = binaryOrder;

        /// <summary>
        /// The order to serialize/deserialize the property as binary data.
        /// </summary>
        /// 
        /// <remarks>
        /// A single class/struct should not have duplicated values for <see cref="BinaryOrder"/>.
        /// </remarks>
        public byte BinaryOrder { get; private set; }
    }

    internal sealed class PrimitiveBinarySerializer<T> : IBinarySerializer<T>
    {
        public T Read(IBinaryReader binaryReader) => (T)ReadAsObject(binaryReader);

        private object ReadAsObject(IBinaryReader binaryReader)
        {
            bool isType<U>() => typeof(T) == typeof(U);

            if (isType<byte>())
            {
                return binaryReader.ReadByte();
            }
            else if (isType<short>())
            {
                return binaryReader.ReadInt16();
            }
            else if (isType<ushort>())
            {
                return binaryReader.ReadUInt16();
            }
            else if (isType<int>())
            {
                return binaryReader.ReadInt32();
            }
            else if (isType<uint>())
            {
                return binaryReader.ReadUInt32();
            }
            else if (isType<float>())
            {
                return binaryReader.ReadFloat();
            }
            else if (isType<double>())
            {
                return binaryReader.ReadDouble();
            }
            else
            {
                throw new BadTypeException(typeof(T), "Type is not a supported primitive");
            }
        }

        public void Write(IBinaryWriter binaryWriter, T value)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class CastSerializer<T, U> : IBinarySerializer<T>
    {
        private readonly IBinarySerializer<U> innerSerializer;

        public CastSerializer(IBinarySerializer<U> innerSerializer)
            => this.innerSerializer = innerSerializer;

        public T Read(IBinaryReader binaryReader)
        {
            // I don't like to use dynamic but it seems is
            // the only way to perform the casting here.
            dynamic result = innerSerializer.Read(binaryReader);
            return (T)result;
        }

        public void Write(IBinaryWriter binaryWriter, T value)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class StringBinarySerializer : IBinarySerializer<string>
    {
        public string Read(IBinaryReader binaryReader)
        {
            if (binaryReader == null)
            {
                throw new ArgumentNullException();
            }

            var offset = binaryReader.ReadUInt32();

            return binaryReader.DoAtPosition(offset, () =>
            {
                var count = binaryReader.ReadByte();
                return binaryReader
                    .ReadBytes(count)
                    .Let(Encoding.ASCII.GetChars)
                    .Let(bs => string.Concat(bs));
            });
        }

        public void Write(IBinaryWriter binaryWriter, string value)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class ListBinarySerializer<T> : IBinarySerializer<IList<T>>
    {
        private readonly ConstructorInfo constructor;

        private readonly MethodInfo addMethod;

        private readonly IBinarySerializer<T> serializer;

        public ListBinarySerializer(IBinarySerializer<T> serializer)
        {
            var listType = typeof(List<T>);

            constructor = listType.GetConstructor(Array.Empty<Type>());
            addMethod = listType.GetMethod(nameof(List<T>.Add));
            this.serializer = serializer;
        }

        public IList<T> Read(IBinaryReader binaryReader)
        {
            if (binaryReader == null)
            {
                throw new ArgumentNullException();
            }

            var count = binaryReader.ReadUInt32();
            var offset = binaryReader.ReadUInt32();

            var result = constructor.Invoke(Array.Empty<object>());
            var args = new object[1];
            binaryReader.DoAtPosition(offset, () =>
            {
                for (int n = 0; n < count; ++n)
                {
                    args[0] = serializer.Read(binaryReader);
                    addMethod.Invoke(result, args);
                }
            });

            return (IList<T>)result;
        }

        public void Write(IBinaryWriter binaryWriter, IList<T> value)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class BinarySerializableSerializer<T> : IBinarySerializer<T> where T : IBinarySerializable
    {
        private readonly Func<T> constructor;

        public BinarySerializableSerializer()
        {
            constructor = typeof(T).GetConstructor(Array.Empty<Type>())
                ?.Let<ConstructorInfo, Func<T>>(ci => () => (T)ci.Invoke(Array.Empty<object>()))
                ?? throw new BadTypeException(typeof(T), $"Type {typeof(T).Name} has no public constructor with no arguments");
        }

        public T Read(IBinaryReader binaryReader)
        {
            T result = constructor();

            result.Read(binaryReader);

            return result;
        }

        public void Write(IBinaryWriter binaryWriter, T value)
            => value.Write(binaryWriter);
    }

    /// <summary>
    /// Interface with methods to serialize a property from <typeparamref name="T"/>.
    /// </summary>
    /// 
    /// <typeparam name="T">The type that defines the property.</typeparam>
    internal interface IPropertyBinarySerializer<T>
    {
        /// <summary>
        /// Reads from the <paramref name="reader"/> and sets the value in <paramref name="obj"/>.
        /// </summary>
        /// 
        /// <param name="reader">The reader where the data will be read.</param>
        /// <param name="obj"></param>
        void ReadAndSet(IBinaryReader reader, T obj);
    }

    internal sealed class DefaultPropertyBinarySerializer<T, V> : IPropertyBinarySerializer<T>
    {
        private readonly IBinarySerializer<V> serializer;

        private readonly PropertyInfo propertyInfo;

        public DefaultPropertyBinarySerializer(IBinarySerializer<V> serializer, PropertyInfo propertyInfo)
        {
            this.serializer = serializer;
            this.propertyInfo = propertyInfo;
        }

        public void ReadAndSet(IBinaryReader reader, T obj) => serializer
            .Read(reader)
            .Also(v => propertyInfo.SetValue(obj, v));
    }

    internal sealed class DefaultBinarySerializer<T> : IBinarySerializer<T>
    {
        private readonly Func<T> constructor;

        private readonly IReadOnlyList<IPropertyBinarySerializer<T>> propertyBinarySerializers;

        public DefaultBinarySerializer(IReadOnlyList<IPropertyBinarySerializer<T>> propertyBinarySerializers)
        {
            constructor = typeof(T).GetConstructor(Array.Empty<Type>())
                ?.Let<ConstructorInfo, Func<T>>(ci => () => (T)ci.Invoke(Array.Empty<object>()))
                ?? throw new BadTypeException(typeof(T), $"Type {typeof(T).Name} has no public constructor with no arguments");

            this.propertyBinarySerializers = propertyBinarySerializers;
        }

        public T Read(IBinaryReader binaryReader) => constructor()
            .Also(o => propertyBinarySerializers.ForEach(pbs => pbs.ReadAndSet(binaryReader, o)));

        public void Write(IBinaryWriter binaryWriter, T value)
        {
            throw new NotImplementedException();
        }
    }
}
