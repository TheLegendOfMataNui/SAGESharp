using System.Collections.Generic;

namespace SAGESharp.SLB.Level.Conversation
{
    /// <summary>
    /// Interface to write or read conversation objects.
    /// </summary>
    public interface IConversationAccessor
    {
        /// <summary>
        /// Reads a conversation from the underlying storage.
        /// </summary>
        /// 
        /// <returns>A list of characters that represent the read conversation.</returns>
        IList<Character> ReadConversation();

        /// <summary>
        /// Writes the given characters as a conversation in the underlying storage.
        /// </summary>
        /// 
        /// <returns>A list of characters that represent the conversation to write.</returns>
        void WriteConversation(IReadOnlyList<Character> characters);
    }
}
