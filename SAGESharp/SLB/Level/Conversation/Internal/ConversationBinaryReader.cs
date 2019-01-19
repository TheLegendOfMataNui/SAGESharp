using SAGESharp.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace SAGESharp.SLB.Level.Conversation.Internal
{
    /// <summary>
    /// Class to read a list of Character objects from binary SLB files.
    /// </summary>
    internal sealed class ConversationBinaryReader : ISLBBinaryReader<IList<Character>>
    {
        private readonly Stream stream;

        private readonly ISLBBinaryReader<Character> characterReader;

        /// <summary>
        /// Crates a new reader with the input objects that will be used to read the character data.
        /// </summary>
        /// 
        /// <param name="stream">The input stream</param>
        /// <param name="characterReader">A character reader</param>
        public ConversationBinaryReader(
            Stream stream,
            ISLBBinaryReader<Character> characterReader
        ) {
            this.stream = stream ?? throw new ArgumentNullException("Input stream cannot be null.");
            this.characterReader = characterReader ?? throw new ArgumentNullException("Input stream cannot be null.");
        }

        /// <inheritdoc/>
        public IList<Character> ReadSLBObject()
        {
            var characterCount = stream.ForceReadUInt();

            var result = new List<Character>();
            if (characterCount == 0)
            {
                return result;
            }

            var position = stream.ForceReadUInt();
            stream.OnPositionDo(position, () =>
            {
                for (int n = 0; n < characterCount; ++n)
                {
                    result.Add(characterReader.ReadSLBObject());
                }
            });

            return result;
        }
    }
}
