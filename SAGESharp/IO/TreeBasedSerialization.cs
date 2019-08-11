/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using System;
using System.Collections.Generic;

namespace SAGESharp.IO
{
    #region Interfaces
    /// <summary>
    /// Represents a node in the tree.
    /// </summary>
    internal interface INode
    {
        /// <summary>
        /// The type for the values corresponding to this node.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Writes <paramref name="value"/> to the given <paramref name="binaryWriter"/>.
        /// </summary>
        /// 
        /// <param name="binaryWriter">The output binary writer were the object will be written.</param>
        /// <param name="value">The value to be written, it must be of type <see cref="Type"/>.</param>
        /// 
        /// <returns>The position the offset if any was recorded, null otherwise.</returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If either <paramref name="binaryWriter"/> or <paramref name="value"/> are null.
        /// </exception>
        /// <exception cref="ArgumentException">If <paramref name="value"/> is not of type <see cref="Type"/>.</exception>
        uint? Write(IBinaryWriter binaryWriter, object value);
    }

    /// <summary>
    /// Represents a node with data (with children nodes) in the tree.
    /// </summary>
    internal interface IDataNode : INode
    {
        /// <summary>
        /// Returns the list of edges connecting to child nodes.
        /// </summary>
        IReadOnlyList<IEdge> Edges { get; }
    }

    /// <summary>
    /// Represents a node with a single child but several entries of the same child node.
    /// </summary>
    internal interface IListNode : INode
    {
        /// <summary>
        /// The child data node.
        /// </summary>
        IDataNode ChildNode { get; }

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
    }

    /// <summary>
    /// Represents an edge connecting a <see cref="IDataNode"/> to an <see cref="INode"/>.
    /// </summary>
    internal interface IEdge
    {
        /// <summary>
        /// The child node that the edge is connecting.
        /// </summary>
        INode ChildNode { get; }

        /// <summary>
        /// Extracts from the given <paramref name="value"/> a child value that is represented by the child node.
        /// </summary>
        /// 
        /// <param name="value">The object represented by the parent node.</param>
        /// 
        /// <returns>The child value represented by the child node.</returns>
        object ExtractChildValue(object value);
    }

    /// <summary>
    /// Represents an object that can write a value using an SLB graph.
    /// </summary>
    internal interface ITreeWriter
    {
        /// <summary>
        /// Writes <paramref name="value"/> to <paramref name="binaryWriter"/>
        /// using the SLB graph that starts in the given <paramref name="rootNode"/>.
        /// </summary>
        /// 
        /// <param name="binaryWriter">The binary writer that will be used to write the value.</param>
        /// <param name="value">The value to be written.</param>
        /// <param name="rootNode">The root node of the SLB graph that will be used to write the value.</param>
        /// 
        /// <returns>A list containing the position of the offsets written to <paramref name="binaryWriter"/>.</returns>
        /// 
        /// <exception cref="ArgumentNullException">If any argument is null.</exception>
        IReadOnlyList<uint> Write(IBinaryWriter binaryWriter, object value, IDataNode rootNode);
    }
    #endregion

    #region Implementations
    internal class TreeWriter : ITreeWriter
    {
        public IReadOnlyList<uint> Write(IBinaryWriter binaryWriter, object value, IDataNode rootNode)
        {
            Validate.ArgumentNotNull(nameof(binaryWriter), binaryWriter);
            Validate.ArgumentNotNull(nameof(value), value);
            Validate.ArgumentNotNull(nameof(rootNode), rootNode);

            ProcessNode(binaryWriter, rootNode, value);

            return new List<uint>();
        }

        private void ProcessNode(IBinaryWriter binaryWriter, INode node, object value)
        {
            node.Write(binaryWriter, value);

            if (node is IDataNode dataNode)
            {
                ProcessDataNode(binaryWriter, dataNode, value);
            }
            else
            {
                throw new NotImplementedException($"Type {node.GetType().Name} is an unknown node type");
            }
        }

        private void ProcessDataNode(IBinaryWriter binaryWriter, IDataNode node, object value)
        {
            foreach (IEdge edge in node.Edges)
            {
                object childValue = edge.ExtractChildValue(value);
                ProcessNode(binaryWriter, edge.ChildNode, childValue);
            }
        }
    }
    #endregion
}
