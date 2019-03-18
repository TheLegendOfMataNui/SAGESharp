using Konvenience;
using System;
using System.Collections.Generic;

namespace SAGESharp.SLB.IO
{
    /// <summary>
    /// Interface to create instances of <see cref="IBinarySerializer"/>.
    /// </summary>
    public interface IBinarySerializerFactory
    {
        /// <summary>
        /// Creates the corresponding <see cref="IBinarySerializer"/> instance for the given <typeparamref name="T"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// 
        /// <returns>An instance of <see cref="IBinarySerializer" /> for <typeparamref name="T"/>.</returns>
        IBinarySerializer<T> GetSerializerForType<T>();
    }

    class DefaultBinarySerializerFactory : IBinarySerializerFactory
    {
        public IBinarySerializer<T> GetSerializerForType<T>()
        {
            if (typeof(T).IsPrimitive)
            {
                return new PrimitiveBinarySerializer<T>();
            }
            else if (typeof(T) == typeof(Identifier))
            {
                dynamic result = new CastSerializer<Identifier, uint>(new PrimitiveBinarySerializer<uint>());
                return (IBinarySerializer<T>)result;
            }
            else if (typeof(T).IsEnum)
            {
                var underlyingType = Enum.GetUnderlyingType(typeof(T));

                var innerSerializer = typeof(PrimitiveBinarySerializer<>)
                    .MakeGenericType(underlyingType)
                    .GetConstructor(Array.Empty<Type>())
                    .Invoke(Array.Empty<object>());

                var result = typeof(CastSerializer<,>)
                    .MakeGenericType(typeof(T), underlyingType)
                    .GetConstructor(new Type[] { typeof(IBinarySerializer<>).MakeGenericType(underlyingType) })
                    .Invoke(new object[] { innerSerializer });

                return (IBinarySerializer<T>)result;
            }
            else if (typeof(T) == typeof(string))
            {
                dynamic result = new StringBinarySerializer();
                return (IBinarySerializer<T>)result;
            }
            else if (IsListType<T>())
            {
                var elementsType = typeof(T).GetGenericArguments()[0];

                var constructor = typeof(ListBinarySerializer<>)
                    .MakeGenericType(elementsType)
                    .GetConstructor(new Type[] { typeof(IBinarySerializer<>).MakeGenericType(elementsType) });

                var serializer = GetType()
                    .GetMethod(nameof(GetSerializerForType))
                    .MakeGenericMethod(elementsType)
                    .Invoke(this, Array.Empty<object>());

                return (IBinarySerializer<T>)constructor.Invoke(new object[] { serializer });
            }
            else if (IsConcreteClassType<T>())
            {
                return new DefaultBinarySerializer<T>(this);
            }
            else
            {
                throw new BadTypeException(typeof(T), "Type is not a supported serializable type");
            }
        }

        private static bool IsListType<T>() => typeof(T)
            .TakeReferenceIf(t => t.IsGenericType)
            ?.GetGenericTypeDefinition()
            ?.Let(t => t == typeof(List<>) || t == typeof(IList<>) || t == typeof(IReadOnlyList<>))
            ?? false;

        private static bool IsConcreteClassType<T>() => typeof(T)
            .TakeReferenceUnless(t => t.IsValueType)
            ?.TakeReferenceUnless(t => t.IsAbstract)
            ?.TakeReferenceUnless(t => t.IsInterface)
            ?.Let(t => true)
            ?? false;
    }
}
