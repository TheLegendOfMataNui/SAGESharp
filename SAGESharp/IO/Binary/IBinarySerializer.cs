/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using System;

namespace SAGESharp.IO.Binary
{
    /// <summary>
    /// Represents an object that serializes objects of type <typeparamref name="T"/>.
    /// </summary>
    /// 
    /// <typeparam name="T">Type of the objects to serialize.</typeparam>
    public interface IBinarySerializer<T>
    {
        /// <summary>
        /// Reads an object of type <typeparamref name="T"/> from the input <paramref name="binaryReader"/>.
        /// </summary>
        /// 
        /// <param name="binaryReader">The binary reader used to read the object.</param>
        /// 
        /// <returns>The object read from the <paramref name="binaryReader"/>.</returns>
        /// 
        /// <exception cref="ArgumentNullException">If <paramref name="binaryReader"/> is null.</exception>
        T Read(IBinaryReader binaryReader);

        /// <summary>
        /// Writes <paramref name="value"/> to the output <paramref name="binaryWriter"/>.
        /// </summary>
        /// 
        /// <param name="binaryWriter">The output binary writer were the object will be written.</param>
        /// <param name="value">The object to write.</param>
        /// 
        /// <exception cref="ArgumentNullException">If either <paramref name="binaryWriter"/> or <paramref name="value"/> are null.</exception>
        void Write(IBinaryWriter binaryWriter, T value);
    }
}
