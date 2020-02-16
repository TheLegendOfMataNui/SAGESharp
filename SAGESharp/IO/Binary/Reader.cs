/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using System;
using System.IO;

namespace SAGESharp.IO.Binary
{
    /// <summary>
    /// Static class to function as a simple factory for <see cref="IBinaryReader"/> instances.
    /// </summary>
    public static class Reader
    {
        /// <summary>
        /// Gets a <see cref="IBinaryReader"/> for the intpu <paramref name="stream"/>.
        /// </summary>
        /// 
        /// <param name="stream">The input stream to use in the reader.</param>
        /// 
        /// <returns>A <see cref="IBinaryReader"/> to read the input <paramref name="stream"/>.</returns>
        public static IBinaryReader ForStream(Stream stream)
            => new BinaryReaderWrapper(stream);

        /// <summary>
        /// Executes <paramref name="action"/> while temporarily moving the reader to <paramref name= "position" />.
        /// </summary>
        /// 
        /// <param name="reader">The reader that w</param>
        /// <param name="position">The position where the reader will be moved temporarily.</param>
        /// <param name="action">The action to execute.</param>
        public static void DoAtPosition(this IBinaryReader reader, long position, Action action)
        {
            var originalPosition = reader.Position;
            reader.Position = position;

            action();
            reader.Position = originalPosition;
        }

        /// <summary>
        /// Returns the result of <paramref name="function"/> while temporarily moving the reader to <paramref name= "position" />.
        /// </summary>
        /// 
        /// <typeparam name="TResult">The type that will be returned.</typeparam>
        /// 
        /// <param name="reader">The reader that will be changed temproarily.</param>
        /// <param name="position">The position where the reader will be moved temporarily.</param>
        /// <param name="function">The action to execute.</param>
        /// 
        /// <returns>The result of <paramref name="function"/>.</returns>
        public static TResult DoAtPosition<TResult>(this IBinaryReader reader, long position, Func<TResult> function)
        {
            var originalPosition = reader.Position;
            reader.Position = position;

            var result = function();
            reader.Position = originalPosition;

            return result;
        }
    }
}
