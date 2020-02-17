/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using System;
using System.Collections.Generic;

namespace SAGESharp.IO.Binary.TreeBasedSerialization
{
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
}
