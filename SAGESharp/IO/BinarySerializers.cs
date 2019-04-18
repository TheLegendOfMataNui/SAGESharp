/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using Konvenience;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SAGESharp.IO
{
    /// <summary>
    /// Represents an object that serialize objects of type <typeparamref name="T"/>.
    /// </summary>
    /// 
    /// <typeparam name="T">Type of the objects to serialize.</typeparam>
    public interface IBinarySerializer<out T>
    {
        /// <summary>
        /// Reads an objects from the input <paramref name="binaryReader"/>.
        /// </summary>
        /// 
        /// <param name="binaryReader">The binary reader used to read the object.</param>
        /// 
        /// <returns>The objec read from the <paramref name="binaryReader"/>.</returns>
        T Read(IBinaryReader binaryReader);
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
    }

    internal sealed class ListBinarySerializer<T> : IBinarySerializer<List<T>>
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

        public List<T> Read(IBinaryReader binaryReader)
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

            return (List<T>)result;
        }
    }

    /*
     * This class deserves some documentation:
     * 
     * The way it works is:
     *     - From the input type get the default constructor.
     *     - For each property in the type, create a `PropertySetter` with
     *       `PropertySetter.From` unless the property is not annotated
     *       with `SLBElementAttribute` in that case it will be null.
     *     - Filter all the null `PropertySetter`
     *     - Order by `SLBElementAttribute.Order`
     *     - Create an array from there
     *     - Assert that:
     *         1. The input type has at least one property with `SLBElementAttribute`
     *         2. No two properties have the same value for `SLBElementAttribute.Order`
     * 
     * At the time of reading (`Read` method) from the `IBinaryReader`,
     * the `PropertySetter` is used to use the correct method (or object)
     * in the reader and set the value in the property of the object result
     * of `Read`.
     */
    internal sealed class DefaultBinarySerializer<T> : IBinarySerializer<T>
    {
        private readonly ConstructorInfo constructor;

        private readonly PropertySetter[] setters;

        public DefaultBinarySerializer(IBinarySerializerFactory factory)
        {
            var type = typeof(T);

            constructor = type.GetConstructor(Array.Empty<Type>())
                ?? throw new BadTypeException(type, "Type has no public constructor with no arguments");
            setters = type.GetProperties()
                .Select(p => PropertySetter.From(p, factory))
                .Where(ss => ss != null)
                .OrderBy(ss => ss.Order)
                .ToArray()
                .Also(ss => AssertValidSetters(ss, type));
        }

        public T Read(IBinaryReader binaryReader)
        {
            var result = constructor.Invoke(Array.Empty<object>());

            foreach (var setter in setters)
            {
                setter.ReadAndSet(binaryReader, result);
            }

            return (T)result;
        }

        private static void AssertValidSetters(PropertySetter[] setters, Type type)
        {
            if (setters.Length == 0)
            {
                throw new BadTypeException(type, $"Type has no property annotated with {nameof(SerializablePropertyAttribute)}");
            }

            if (setters.Length != setters.Select(s => s.Order).Distinct().Count())
            {
                throw new BadTypeException(type, "Type has more than one property with the same order");
            }
        }

        private class PropertySetter
        {
            private static readonly MethodInfo GET_SERIALIZER_FOR_TYPE_METHOD =
                typeof(IBinarySerializerFactory)
                .GetMethod(nameof(IBinarySerializerFactory.GetSerializerForType));

            private static readonly Type[] SERIALIZER_READ_METHOD_ARGUMENT_TYPES = new Type[]
            {
                typeof(IBinaryReader)
            };

            private PropertySetter(PropertyInfo property, int order, Func<IBinaryReader, object> readFunction)
            {
                Property = property;
                Order = order;
                ReadFunction = readFunction;
            }

            private PropertyInfo Property { get; }

            public int Order { get; }

            private Func<IBinaryReader, object> ReadFunction { get; }

            public void ReadAndSet(IBinaryReader reader, object obj)
            {
                var value = ReadFunction(reader);
                Property.SetValue(obj, value);
            }

            public static PropertySetter From(PropertyInfo property, IBinarySerializerFactory factory) => property
                .GetCustomAttribute<SerializablePropertyAttribute>()
                ?.Also(_ => AssertWritableProperty(property))
                ?.Let(a => new PropertySetter(property, a.BinaryOrder, GetReadFunction(property.PropertyType, factory)));

            private static void AssertWritableProperty(PropertyInfo property)
            {
                if (property.GetSetMethod() == null)
                {
                    throw new BadTypeException(property.DeclaringType, $"Property {property.Name} doesn't have a setter");
                }
            }

            private static Func<IBinaryReader, object> GetReadFunction(Type type, IBinarySerializerFactory factory)
            {
                var readMethod = typeof(IBinarySerializer<>)
                    .MakeGenericType(type)
                    .GetMethod(nameof(Read), SERIALIZER_READ_METHOD_ARGUMENT_TYPES);

                var serializer = GET_SERIALIZER_FOR_TYPE_METHOD
                    .MakeGenericMethod(type)
                    .Invoke(factory, Array.Empty<object>());

                return r => readMethod.Invoke(serializer, new object[] { r });
            }
        }
    }
}
