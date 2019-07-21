/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using System;

namespace SAGESharp.IO
{
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

    /// <summary>
    /// Specifies a property should be followed by a padding.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class RightPaddingAttribute : Attribute
    {
        /// <summary>
        /// Initailizes a new attribute to mark a property with right padding.
        /// </summary>
        /// 
        /// <param name="size">The size of the padding (in bytes).</param>
        public RightPaddingAttribute(byte size) => Size = size;

        /// <summary>
        /// The length of the padding in bytes.
        /// </summary>
        public byte Size { get; }
    }

    /// <summary>
    /// Specifies how a string is stored in binary form.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class BinaryStringAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new attribute to mark a string is located at the given <paramref name="position"/>
        /// and its length is stored as a single byte along with its contents.
        /// </summary>
        /// 
        /// <param name="position">The position for the string.</param>
        public BinaryStringAttribute(StringPosition position)
        {
            Position = position;
            FixedLength = null;
        }

        /// <summary>
        /// Initializes a new attribute to mark a string is located at the given <paramref name="position"/>
        /// and has a fixed length of <paramref name="length"/>.
        /// </summary>
        /// 
        /// <param name="position">The position for the string.</param>
        /// <param name="length">The length for the string.</param>
        public BinaryStringAttribute(StringPosition position, int length)
        {
            Position = position;
            FixedLength = length;
        }

        /// <summary>
        /// The position for the string.
        /// </summary>
        public StringPosition Position { get; }

        /// <summary>
        /// The fixed length for the string, if null it means its length
        /// is stored as a single byte along with its contents.
        /// </summary>
        public int? FixedLength { get; }
    }

    /// <summary>
    /// Where a string is located in a binary file.
    /// </summary>
    public enum StringPosition
    {
        /// <summary>
        /// The string is located in the same position where the string is being read.
        /// </summary>
        Inline,
        /// <summary>
        /// The string is located at an offset that is located where the string is being read.
        /// </summary>
        AtOffset
    }
}
