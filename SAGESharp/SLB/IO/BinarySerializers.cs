using Konvenience;
using System;
using System.Collections.Generic;
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

    internal sealed class ListBinarySerializer : IBinarySerializer
    {
        private readonly ConstructorInfo constructor;

        private readonly MethodInfo addMethod;

        private readonly IBinarySerializer serializer;

        public ListBinarySerializer(Type type, IBinarySerializer serializer)
        {
            var listType = typeof(List<>).MakeGenericType(type);

            constructor = listType.GetConstructor(Array.Empty<Type>());
            addMethod = listType.GetMethod(nameof(List<object>.Add));
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
}
