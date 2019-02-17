using System;
using System.IO;
using Konvenience;

namespace SAGESharp.SLB.Level.Conversation
{
    /// <summary>
    /// Class to read Frame objects from binary SLB files.
    /// </summary>
    internal sealed class FrameBinaryReader : ISLBBinaryReader<Frame>
    {
        private readonly Stream stream;

        private readonly ISLBBinaryReader<string> stringReader;

        /// <summary>
        /// Creates a new reader with the input stream that will be used to read the frame.
        /// </summary>
        /// 
        /// <param name="stream">The input stream</param>
        /// <param name="stringReader">A reader of binary SLB strings.</param>
        /// 
        /// <exception cref="ArgumentNullException">If any argument is null.</exception>
        public FrameBinaryReader(Stream stream, ISLBBinaryReader<string> stringReader)
        {
            this.stream = stream ?? throw new ArgumentNullException("Input stream cannot be null.");
            this.stringReader = stringReader ?? throw new ArgumentNullException("The string reader cannot be null.");
        }

        /// <inheritdoc/>
        public Frame ReadSLBObject() => stream
            .ForceReadBytes(Frame.BINARY_SIZE)
            .Let(buffer => new Frame
            {
                ToaAnimation = buffer.ToInt32(),
                CharAnimation = buffer.ToInt32(4),
                CameraPositionTarget = buffer.ToInt32(8),
                CameraDistance = buffer.ToInt32(12),
                StringIndex = buffer.ToInt32(16),
                ConversationSounds = buffer.ToInt32(20).Let(ReadConversationSounds)
            });

        private string ReadConversationSounds(int offset)
            => stream.OnPositionDo(offset, () => stringReader.ReadSLBObject());
    }
}
