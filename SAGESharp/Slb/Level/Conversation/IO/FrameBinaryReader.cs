using SAGESharp.Extensions;
using SAGESharp.Slb.IO;
using System;
using System.IO;
using System.Text;

namespace SAGESharp.Slb.Level.Conversation.IO
{
    /// <summary>
    /// Class to read Frame objects from binary SLB files.
    /// </summary>
    public class FrameBinaryReader : ISlbReader<Frame>
    {
        private readonly Stream stream;

        /// <summary>
        /// Creates a new reader with the input stream that will be used to read the frame.
        /// </summary>
        /// 
        /// <param name="stream">The input stream</param>
        public FrameBinaryReader(Stream stream)
        {
            this.stream = stream ?? throw new ArgumentNullException("Input stream cannot be null.");
        }

        /// <inheritdoc/>
        public Frame ReadSlbObject()
        {
            return new Frame()
            {
                ToaAnimation = stream.ForceReadInt(),
                CharAnimation = stream.ForceReadInt(),
                CameraPositionTarget = stream.ForceReadInt(),
                CameraDistance = stream.ForceReadInt(),
                StringIndex = stream.ForceReadInt(),
                ConversationSounds = ReadConversationSounds()
            };
        }

        private string ReadConversationSounds()
        {
            var conversationSoundsPosition = stream.ForceReadInt();

            return stream.OnPositionDo(conversationSoundsPosition, () =>
            {
                var stringSize = stream.ForceReadByte();
                var result = new StringBuilder();
                for (int n = 0; n < stringSize; ++n)
                {
                    result.Append(stream.ForceReadASCIIChar());
                }

                // Read end of string character
                stream.ForceReadByte();

                return result.ToString();
            });
        }
    }
}
