using System;
using System.Collections.Generic;
using System.IO;

namespace SAGESharp.SLB.Level.Conversation
{
    /// <summary>
    /// Class to read and write level's conversations into binary files (.SLB).
    /// </summary>
    class ConversationBinaryAccessor : IConversationAccessor
    {
        private readonly Stream stream;

        /// <summary>
        /// Creates a new accessor that will read or write to the given stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream that will be used to read or write conversation data.</param>
        /// 
        /// <exception cref="ArgumentNullException">If <paramref name="stream"/> is null.</exception>
        public ConversationBinaryAccessor(Stream stream)
        {
            this.stream = stream ?? throw new ArgumentNullException();
        }

        /// <inheritdoc/>
        public IList<Character> ReadConversation()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void WriteConversation(IReadOnlyList<Character> characters)
        {
            throw new NotImplementedException();
        }
    }
}
