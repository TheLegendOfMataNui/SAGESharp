/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO.Binary;
using System;
using System.Collections.Generic;

namespace SAGESharp.IO
{
    #region Interfaces
    /// <summary>
    /// Represents a node with data (with children nodes) in the tree.
    /// </summary>
    internal interface IDataNode
    {
        /// <summary>
        /// Reads a value from the <paramref name="binaryReader"/>.
        /// </summary>
        /// 
        /// <param name="binaryReader">The reader that will be used to read the value.</param>
        /// 
        /// <returns>The value read from <paramref name="binaryReader"/>.</returns>
        /// 
        /// <exception cref="ArgumentNullException">If <paramref name="binaryReader"/> is null.</exception>
        object Read(IBinaryReader binaryReader);

        /// <summary>
        /// Writes <paramref name="value"/> to the given <paramref name="binaryWriter"/>.
        /// </summary>
        /// 
        /// <param name="binaryWriter">The output binary writer were the object will be written.</param>
        /// <param name="value">The value to be written.</param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If either <paramref name="binaryWriter"/> or <paramref name="value"/> are null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> is not of the expected type.
        /// </exception>
        void Write(IBinaryWriter binaryWriter, object value);

        /// <summary>
        /// Returns the list of edges connecting to child nodes.
        /// </summary>
        IReadOnlyList<IEdge> Edges { get; }
    }

    /// <summary>
    /// Represents a node that will write its contents at a later time.
    /// </summary>
    internal interface IOffsetNode
    {
        /// <summary>
        /// Reads the offset location at which the value of <see cref="ChildNode"/> is located.
        /// </summary>
        /// 
        /// <param name="binaryReader">The input binary reader to read the offset from.</param>
        /// 
        /// <returns>The offset of value of <see cref="ChildNode"/>.</returns>
        uint ReadOffset(IBinaryReader binaryReader);

        /// <summary>
        /// Writes <paramref name="value"/> to the given <paramref name="binaryWriter"/>.
        /// </summary>
        /// 
        /// <param name="binaryWriter">The output binary writer were the object will be written.</param>
        /// <param name="value">The value to be written.</param>
        /// 
        /// <returns>The position where the offset was written.</returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If either <paramref name="binaryWriter"/> or <paramref name="value"/> are null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> is not of the expected type.
        /// </exception>
        uint Write(IBinaryWriter binaryWriter, object value);

        /// <summary>
        /// The actual node that will write the contents of the object.
        /// </summary>
        IDataNode ChildNode { get; }
    }

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

    /// <summary>
    /// Represents an edge connecting a <see cref="IDataNode"/> to
    /// a node (ex: <see cref="IDataNode"/>, <see cref="IOffsetNode"/>).
    /// </summary>
    internal interface IEdge
    {
        /// <summary>
        /// The child node that the edge is connecting.
        /// </summary>
        object ChildNode { get; }

        /// <summary>
        /// Extracts from the given <paramref name="value"/> a child value that is represented by the child node.
        /// </summary>
        /// 
        /// <param name="value">The object represented by the parent node.</param>
        /// 
        /// <returns>The child value represented by the child node.</returns>
        object ExtractChildValue(object value);

        /// <summary>
        /// Sets this edge's corresponding child of <paramref name="value"/> to <paramref name="childValue"/>.
        /// </summary>
        /// 
        /// <param name="value">The value to set the child.</param>
        /// <param name="childValue">The child value that will be set.</param>
        /// 
        /// <exception cref="ArgumentNullException">If either argument is null.</exception>
        void SetChildValue(object value, object childValue);
    }
    #endregion
}
