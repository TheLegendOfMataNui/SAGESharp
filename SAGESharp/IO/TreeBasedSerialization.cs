/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using System;

namespace SAGESharp.IO
{
    #region Interfaces
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
