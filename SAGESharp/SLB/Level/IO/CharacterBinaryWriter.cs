/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.SLB.IO;
using System;
using System.IO;

using static System.BitConverter;

namespace SAGESharp.SLB.Level.IO
{
    /// <summary>
    /// Class to write a <see cref="Character"/> as a SLB binary object.
    /// </summary>
    class CharacterBinaryWriter : ISLBBinaryWriter<Character>
    {
        private readonly Stream stream;

        /// <summary>
        /// Creates a new writer using the given input stream.
        /// </summary>
        /// 
        /// <param name="stream">The input stream.</param>
        /// 
        /// <exception cref="ArgumentNullException">If <paramref name="stream"/> is null.</exception>
        public CharacterBinaryWriter(Stream stream)
        {
            this.stream = stream ?? throw new ArgumentNullException("Output stream cannot be null.");
        }

        /// <inheritdoc/>
        /// 
        /// <exception cref="ArgumentNullException">If <paramref name="slbObject"/> is null.</exception>
        public void WriteSLBObject(Character slbObject)
        {
            if (slbObject == null)
            {
                throw new ArgumentNullException();
            }

            var buffer = new byte[Character.BINARY_SIZE];

            GetBytes(slbObject.ToaName).CopyTo(buffer, 0);
            GetBytes(slbObject.CharName).CopyTo(buffer, 4);
            GetBytes(slbObject.CharCont).CopyTo(buffer, 8);
            GetBytes(slbObject.Entries.Count).CopyTo(buffer, 12);

            stream.Write(buffer, 0, Character.BINARY_SIZE);
        }
    }
}
