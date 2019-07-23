/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.SLB.IO;
using System;
using System.IO;

using static System.BitConverter;

namespace SAGESharp.SLB.Level.Conversation.IO
{
    /// <summary>
    /// Class to write a Frame as a SLB binary object.
    /// </summary>
    internal class FrameBinaryWriter : ISLBBinaryWriter<Frame>
    {
        private readonly Stream stream;

        /// <summary>
        /// Creates a new writer using the given input stream.
        /// </summary>
        /// 
        /// <param name="stream">The input stream.</param>
        /// 
        /// <exception cref="ArgumentNullException">If <paramref name="stream"/> is null.</exception>
        public FrameBinaryWriter(Stream stream)
        {
            this.stream = stream ?? throw new ArgumentNullException("Output stream cannot be null.");
        }

        /// <inheritdoc/>
        /// 
        /// <exception cref="ArgumentNullException">If <paramref name="slbObject"/> is null.</exception>
        public void WriteSLBObject(Frame slbObject)
        {
            if (slbObject == null)
            {
                throw new ArgumentNullException();
            }

            var buffer = new byte[Frame.BINARY_SIZE];

            GetBytes(slbObject.ToaAnimation).CopyTo(buffer, 0);
            GetBytes(slbObject.CharAnimation).CopyTo(buffer, 4);
            GetBytes(slbObject.CameraPositionTarget).CopyTo(buffer, 8);
            GetBytes(slbObject.CameraDistance).CopyTo(buffer, 12);
            GetBytes(slbObject.StringIndex).CopyTo(buffer, 16);

            stream.Write(buffer, 0, Frame.BINARY_SIZE);
        }
    }
}
