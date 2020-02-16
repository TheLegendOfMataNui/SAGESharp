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
    /// Static class to function as a simple factory for <see cref="IBinaryWriter"/> instances.
    /// </summary>
    public static class Writer
    {
        /// <summary>
        /// Gets a <see cref="IBinaryWriter"/> for the input <paramref name="stream"/>.
        /// </summary>
        /// 
        /// <param name="stream">The input stream to use in the writer.</param>
        /// 
        /// <returns>A <see cref="IBinaryWriter"/> that writes into the input <paramref name="stream"/>.</returns>
        public static IBinaryWriter ForStream(Stream stream)
            => new BinaryWriterWrapper(stream);

        /// <summary>
        /// Executes <paramref name="action"/> while temporarily moving the writer to <paramref name="position"/>.
        /// </summary>
        /// 
        /// <param name="writer">The writer that whose position will change temporarily.</param>
        /// <param name="position">The new temporal position for the writer.</param>
        /// <param name="action">The action to execute, it receives the original .</param>
        public static void DoAtPosition(this IBinaryWriter writer, long position, Action<long> action)
        {
            var originalPosition = writer.Position;
            writer.Position = position;

            action(originalPosition);
            writer.Position = originalPosition;
        }
    }
}
