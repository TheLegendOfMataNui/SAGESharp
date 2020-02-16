/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using System;

namespace SAGESharp.IO.Binary
{
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
}
