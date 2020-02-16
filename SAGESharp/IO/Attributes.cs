/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using System;

namespace SAGESharp.IO
{
    /// <summary>
    /// Specifies the value for a string property is stored as an offset and with a
    /// single byte for the string length in the binary file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class OffsetStringAttribute : Attribute
    {
    }

    /// <summary>
    /// Specifies the value for a string property is stored inline (no offset) with a fixed length.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class InlineStringAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new attribute to mark a string property inline (with no offset) and a fixed length.
        /// </summary>
        /// 
        /// <param name="length">The fixed length of the string.</param>
        public InlineStringAttribute(byte length) => Length = length;

        /// <summary>
        /// The fixed length for the string.
        /// </summary>
        public byte Length { get; }
    }

    /// <summary>
    /// Specifies a list (or a list of the given class) should read/write twice its length in a binary file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class DuplicateEntryCountAttribute : Attribute
    {
    }
}
