using System;

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
}
