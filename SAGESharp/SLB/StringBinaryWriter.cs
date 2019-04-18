/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using Konvenience;
using System;
using System.IO;
using System.Text;

namespace SAGESharp.SLB
{
    /// <summary>
    /// Class to write a string as a SLB binary object.
    /// </summary>
    internal sealed class StringBinaryWriter : ISLBBinaryWriter<string>
    {
        private readonly Stream stream;

        /// <summary>
        /// Creates a new writer using the given input stream.
        /// </summary>
        /// 
        /// <param name="stream">The input stream.</param>
        /// 
        /// <exception cref="ArgumentNullException">If <paramref name="stream"/> is null.</exception>
        public StringBinaryWriter(Stream stream)
        {
            this.stream = stream ?? throw new ArgumentNullException("Output stream cannot be null.");
        }

        /// <inheritdoc/>
        /// 
        /// <exception cref="ArgumentException">If <paramref name="slbObject"/> is longer than 255 characters.</exception>
        public void WriteSLBObject(string slbObject)
        {
            slbObject = slbObject ?? string.Empty; // Ensure the string is never null
            if (slbObject.Length > 255)
            {
                throw new ArgumentException("String cannot be longer than 255 characters.");
            }

            // The size of the string + the string itself + null terminator
            var bufferSize = slbObject.Length + 2;
            var buffer = new byte[bufferSize];

            buffer[0] = (byte)slbObject.Length;
            slbObject
                .ToCharArray()
                .Let(Encoding.ASCII.GetBytes)
                .Also(bytes => bytes.CopyTo(buffer, 1));

            stream.Write(buffer, 0, bufferSize);
        }
    }
}
