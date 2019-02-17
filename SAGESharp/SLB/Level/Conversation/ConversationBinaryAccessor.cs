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
        /// Reads a level conversation in binary form from the input file.
        /// </summary>
        /// 
        /// <param name="filename">The file name to read for be read.</param>
        /// 
        /// <returns>A conversation (list of <see cref="Character"/> objects) in the stream.</returns>
        public static IList<Character> ReadConversation(string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Open))
            {
                return ReadConversation(stream);
            }
        }

        /// <summary>
        /// Reads a level conversation in binary form from the input stream.
        /// </summary>
        /// 
        /// <param name="stream">The input stream to be read.</param>
        /// 
        /// <returns>A conversation (list of <see cref="Character"/> objects) in the stream.</returns>
        public static IList<Character> ReadConversation(Stream stream)
            => new StringBinaryReader(stream)
                .Let(stringReader => new FrameBinaryReader(stream, stringReader))
                .Let(frameReader => new InfoBinaryReader(stream, frameReader))
                .Let(infoReader => new CharacterBinaryReader(stream, infoReader))
                .Let(characterReader => new ConversationBinaryReader(stream, characterReader))
                .ReadSLBObject();

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
