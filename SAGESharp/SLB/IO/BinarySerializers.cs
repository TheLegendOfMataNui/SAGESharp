using Konvenience;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SAGESharp.SLB.IO
{
    /// <summary>
    /// Represents an object that can read other objects from a binary reader.
    /// </summary>
    public interface IBinarySerializer
    {
        /// <summary>
        /// Reads an objects from the input <paramref name="binaryReader"/>.
        /// </summary>
        /// 
        /// <param name="binaryReader">The binary reader used to read the object.</param>
        /// 
        /// <returns>The objec read from the <paramref name="binaryReader"/>.</returns>
        object Read(IBinaryReader binaryReader);
    }

    /// <summary>
    /// Specifies a property that should be serialized/deserialized for an SLB object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class SLBElementAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new attribute to mark SLB elements.
        /// </summary>
        /// 
        /// <param name="order">The order of the SLB property.</param>
        public SLBElementAttribute(byte order) => Order = order;

        /// <summary>
        /// The order of the SLB property.
        /// </summary>
        /// 
        /// <remarks>
        /// A single class/struct should not have two or more properties with the same value for Order.
        /// </remarks>
        public byte Order { get; private set; }
    }

    internal sealed class FuncBasedBinarySerializer<T> : IBinarySerializer
    {
        public FuncBasedBinarySerializer(Func<IBinaryReader, T> readFunction)
        {
            ReadFunction = readFunction;
        }

        private Func<IBinaryReader, T> ReadFunction { get; }

        public object Read(IBinaryReader binaryReader) => ReadFunction(binaryReader);
    }

    internal sealed class StringBinarySerializer : IBinarySerializer
    {
        public object Read(IBinaryReader binaryReader)
        {
            if (binaryReader == null)
            {
                throw new ArgumentNullException();
            }

            var offset = binaryReader.ReadUInt32();

            return binaryReader.OnPositionDo(offset, () =>
            {
                var count = binaryReader.ReadByte();
                return binaryReader
                    .ReadBytes(count)
                    .Let(Encoding.ASCII.GetChars)
                    .Let(bs => string.Concat(bs));
            });
        }
    }

    internal sealed class ListBinarySerializer<T> : IBinarySerializer
    {
        private readonly ConstructorInfo constructor;

        private readonly MethodInfo addMethod;

        private readonly IBinarySerializer serializer;

        public ListBinarySerializer(IBinarySerializer serializer)
        {
            var listType = typeof(List<T>);

            constructor = listType.GetConstructor(Array.Empty<Type>());
            addMethod = listType.GetMethod(nameof(List<T>.Add));
            this.serializer = serializer;
        }

        public object Read(IBinaryReader binaryReader)
        {
            if (binaryReader == null)
            {
                throw new ArgumentNullException();
            }

            var count = binaryReader.ReadUInt32();
            var offset = binaryReader.ReadUInt32();

            var result = constructor.Invoke(Array.Empty<object>());
            var args = new object[1];
            binaryReader.OnPositionDo(offset, () =>
            {
                for (int n = 0; n < count; ++n)
                {
                    args[0] = serializer.Read(binaryReader);
                    addMethod.Invoke(result, args);
                }
            });

            return result;
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
    internal sealed class DefaultBinarySerializer<T> : IBinarySerializer
    {
        private readonly ConstructorInfo constructor;

        private readonly PropertySetter[] setters;

        public DefaultBinarySerializer(IBinarySerializerFactory factory)
        {
            var type = typeof(T);

            constructor = type.GetConstructor(Array.Empty<Type>())
                ?? throw new ArgumentException($"Type {type.Name} lacks a public constructor with no arguments");
            setters = type.GetProperties()
                .Select(p => PropertySetter.From(p, factory))
                .Where(ss => ss != null)
                .OrderBy(ss => ss.Order)
                .ToArray()
                .Also(ss => AssertValidSetters(ss, type));
        }

        public object Read(IBinaryReader binaryReader)
        {
            var result = constructor.Invoke(Array.Empty<object>());

            foreach (var setter in setters)
            {
                setter.ReadAndSet(binaryReader, result);
            }

            return result;
        }

        private static void AssertValidSetters(PropertySetter[] setters, Type type)
        {
            if (setters.Length == 0)
            {
                throw new ArgumentException($"Type {type.Name} doesn't have any attribute marked with {nameof(SLBElementAttribute)}");
            }

            if (setters.Length != setters.Select(s => s.Order).Distinct().Count())
            {
                throw new ArgumentException($"Type {type.Name} has more than one {nameof(SLBElementAttribute)} with the same order");
            }
        }

        private class PropertySetter
        {
            private static readonly MethodInfo GET_SERIALIZER_FOR_TYPE_METHOD =
                typeof(IBinarySerializerFactory)
                .GetMethod(nameof(IBinarySerializerFactory.GetSerializerForType));

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
                .GetCustomAttribute<SLBElementAttribute>()
                ?.Also(_ => AssertWritableProperty(property))
                ?.Let(a => new PropertySetter(property, a.Order, GetReadFunction(property.PropertyType, factory)));

            private static void AssertWritableProperty(PropertyInfo property)
            {
                if (property.GetSetMethod() == null)
                {
                    throw new ArgumentException($"Property {property.Name} in type " +
                        property.DeclaringType.Name + " doesn't have a setter");
                }
            }

            private static Func<IBinaryReader, object> GetReadFunction(Type type, IBinarySerializerFactory factory)
            {
                var serializer = (IBinarySerializer)GET_SERIALIZER_FOR_TYPE_METHOD
                    .MakeGenericMethod(type)
                    .Invoke(factory, Array.Empty<object>());

                return r => serializer.Read(r);
            }
        }
    }
}
