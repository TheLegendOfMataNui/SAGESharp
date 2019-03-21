using System.Collections.Generic;
using System.IO;
using Konvenience;

namespace SAGESharp.SLB.Level.Conversation
{
    /// <summary>
    /// Class to read and write level's conversations into binary files (.SLB).
    /// </summary>
    public static class ConversationBinaryAccessor
    {
        /// <summary>
        /// Writes a level conversation in binary form into the output file.
        /// </summary>
        /// 
        /// <param name="filename">The output file name.</param>
        /// <param name="characters">The conversation to be writen.</param>
        public static void WriteConversation(string filename, IList<Character> characters)
        {
            using (var stream = new FileStream(filename, FileMode.Create))
            {
                WriteConversation(stream, characters);
            }
        }

        /// <summary>
        /// Writes a conversation in binary form into the output stream.
        /// </summary>
        /// 
        /// <param name="stream">The output stream.</param>
        /// <param name="characters">The conversation to be writen.</param>
        public static void WriteConversation(Stream stream, IList<Character> characters)
            => new ConversationBinaryWriter(
                stream,
                new CharacterBinaryWriter(stream),
                new InfoBinaryWriter(stream),
                new FrameBinaryWriter(stream),
                new StringBinaryWriter(stream),
                new SLBFooterWriter<IList<Character>>(stream, new ConversationFooterGenerator())
            ).WriteSLBObject(characters);
    }
}
