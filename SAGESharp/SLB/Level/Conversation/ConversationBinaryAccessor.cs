using System;
using System.Collections.Generic;
using System.IO;

namespace SAGESharp.SLB.Level.Conversation
{
    /// <summary>
    /// Class to read and write level's conversations into binary files (.SLB).
    /// </summary>
    public static class ConversationBinaryAccessor
    {
        /// <summary>
        /// Reads a level conversation in binary form from the input stream.
        /// </summary>
        /// 
        /// <param name="stream">The input stream to be read.</param>
        /// 
        /// <returns>A conversation (list of <see cref="Character"/> objects).</returns>
        /// 
        /// <exception cref="ArgumentNullException">If <paramref name="stream"/> is null.</exception>
        public static IList<Character> ReadConversation(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException();
            }

            ISLBBinaryReader<Identifier> identifierBinaryReader = new IdentifierBinaryReader(stream);

            return new ConversationBinaryReader(
                stream,
                new CharacterBinaryReader(
                    stream,
                    identifierBinaryReader,
                    new InfoBinaryReader(
                        stream,
                        identifierBinaryReader,
                        new FrameBinaryReader(stream)
                    )
                )
            ).ReadSLBObject();
        }

        /// <summary>
        /// Writes a conversation in binary form into the output stream.
        /// </summary>
        /// 
        /// <param name="stream">The output stream.</param>
        /// <param name="characters">The conversation to be writen.</param>
        /// 
        /// <exception cref="ArgumentNullException">If <paramref name="stream"/> is null.</exception>
        public static void WriteConversation(Stream stream, IReadOnlyList<Character> characters)
        {
            throw new NotImplementedException();
        }
    }
}
