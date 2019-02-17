using System;
using System.IO;

using static System.BitConverter;

namespace SAGESharp.SLB.Level.Conversation
{
    /// <summary>
    /// Class to write an <see cref="Info"/> as a SLB binary object.
    /// </summary>
    class InfoBinaryWriter : ISLBBinaryWriter<Info>
    {
        private readonly Stream stream;

        /// <summary>
        /// Creates a new writer using the given input stream.
        /// </summary>
        /// 
        /// <param name="stream">The input stream.</param>
        /// 
        /// <exception cref="ArgumentNullException">If <paramref name="stream"/> is null.</exception>
        public InfoBinaryWriter(Stream stream)
        {
            this.stream = stream ?? throw new ArgumentNullException("Output stream cannot be null.");
        }
        
        /// <inheritdoc/>
        /// 
        /// <exception cref="ArgumentNullException">If <paramref name="slbObject"/> is null.</exception>
        public void WriteSLBObject(Info slbObject)
        {
            if (slbObject == null)
            {
                throw new ArgumentNullException();
            }

            var buffer = new byte[Info.BINARY_SIZE];

            GetBytes((int)slbObject.LineSide).CopyTo(buffer, 0);
            GetBytes(slbObject.ConditionStart).CopyTo(buffer, 4);
            GetBytes(slbObject.ConditionEnd).CopyTo(buffer, 8);
            GetBytes(slbObject.StringLabel).CopyTo(buffer, 12);
            GetBytes(slbObject.StringIndex).CopyTo(buffer, 16);
            GetBytes(slbObject.Frames.Count).CopyTo(buffer, 20);

            stream.Write(buffer, 0, Info.BINARY_SIZE);
        }
    }
}
