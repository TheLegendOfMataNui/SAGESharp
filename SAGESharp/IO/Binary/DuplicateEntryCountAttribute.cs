/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using System;

namespace SAGESharp.IO.Binary
{
    /// <summary>
    /// Specifies a list (or a list of the given class) should read/write twice its length in a binary file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class DuplicateEntryCountAttribute : Attribute
    {
    }
}
