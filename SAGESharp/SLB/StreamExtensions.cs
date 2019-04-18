/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using System;
using System.IO;
using Konvenience;

namespace SAGESharp.SLB
{
    internal static class StreamExtensions
    {
        /// <summary>
        /// Writes an integer value to the stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to write</param>
        /// <param name="value">The integer to write in the stream</param>
        public static void WriteInt(this Stream stream, int value)
        {
            stream.Write(BitConverter.GetBytes(value), 0, 4);
        }

        /// <summary>
        /// Writes an unsigned integer value to the stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to write</param>
        /// <param name="value">The unsigned integer to write in the stream</param>
        public static void WriteUInt(this Stream stream, uint value)
        {
            stream.Write(BitConverter.GetBytes(value), 0, 4);
        }
    }
}
