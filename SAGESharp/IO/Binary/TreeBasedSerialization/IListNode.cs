/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using System;

namespace SAGESharp.IO.Binary.TreeBasedSerialization
{
    /// <summary>
    /// Represents a node with a single child but several entries of the same child node.
    /// </summary>
    internal interface IListNode : IOffsetNode
    {
        /// <summary>
        /// Creates an instance of the corresponding list type.
        /// </summary>
        /// 
        /// <returns>A new instance of the corresponding list type.</returns>
        object CreateList();

        /// <summary>
        /// Reads the amount of entries for the list from <paramref name="binaryReader"/>.
        /// </summary>
        /// 
        /// <param name="binaryReader">The input binary reader to read the count from.</param>
        /// 
        /// <returns>The amount of entries for a list.</returns>
        /// 
        /// <exception cref="ArgumentNullException">If <paramref name="binaryReader"/> is null.</exception>
        int ReadEntryCount(IBinaryReader binaryReader);

        /// <summary>
        /// Retrieves the amount of entries in the given <paramref name="list"/>.
        /// </summary>
        /// 
        /// <param name="list">The input list.</param>
        /// 
        /// <returns>The amount of object ins the input list.</returns>
        /// 
        /// <exception cref="ArgumentNullException">If <paramref name="list"/> is null.</exception>
        int GetListCount(object list);

        /// <summary>
        /// Retrieves the element with <paramref name="index"/> in the given <paramref name="list"/>.
        /// </summary>
        /// 
        /// <param name="list">The input list.</param>
        /// <param name="index">The index of the element.</param>
        /// 
        /// <returns>
        /// The element with the given <paramref name="index"/> in the given <paramref name="list"/>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">If <paramref name="list"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is not valid for the input list.</exception>
        object GetListEntry(object list, int index);

        /// <summary>
        /// Adds <paramref name="value"/> as a new entry to <paramref name="list"/>.
        /// </summary>
        /// 
        /// <param name="list">The list to add the entry to.</param>
        /// <param name="value">The entry that will be added to the list.</param>
        /// 
        /// <exception cref="ArgumentNullException">If either argument is null.</exception>
        void AddListEntry(object list, object value);
    }
}
