﻿/*
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
        /// Initializes a new attribute to mark a serializable property.
        /// </summary>
        /// 
        /// <param name="binaryOrder">The binary order of property (see <see cref="BinaryOrder"/>).</param>
        /// <param name="name">The name of the property in text form (see <see cref="Name"/>).</param>
        public SerializablePropertyAttribute(byte binaryOrder, string name) : this(binaryOrder) => Name = name;

        /// <summary>
        /// The order to serialize/deserialize the property as binary data.
        /// </summary>
        /// 
        /// <remarks>
        /// A single class/struct should not have duplicated values for <see cref="BinaryOrder"/>.
        /// </remarks>
        public byte BinaryOrder { get; }

        /// <summary>
        /// The name of the property when serialized in text form.
        /// </summary>
        public string Name { get; }
    }
}
