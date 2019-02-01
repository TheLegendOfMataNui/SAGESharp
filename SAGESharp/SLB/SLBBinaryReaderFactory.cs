using SAGESharp.SLB.Level.Conversation;
using System.IO;
using System.Collections.Generic;

namespace SAGESharp.SLB
{
    /// <summary>
    /// Class to build instances of the ISLBBinaryReader interface.
    /// </summary>
    public static class SLBBinaryReaderFactory
    {
        /// <summary>
        /// Builds a new instance of ISLBBinaryReader to read list of Character objects from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The input stream to read</param>
        /// 
        /// <returns>A list of Character objects read from the input stream.</returns>
        public static ISLBBinaryReader<IList<Character>> CharacterConversationReader(Stream stream)
        {
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
            );
        }
    }
}
