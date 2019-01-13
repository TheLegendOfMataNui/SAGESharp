using SAGESharp.Extensions;
using SAGESharp.Slb.IO;
using System;
using System.IO;

namespace SAGESharp.Slb.Level.Conversation.IO
{
    public class CharacterBinaryReader : ISlbReader<Character>
    {
        private readonly Stream stream;

        private readonly ISlbReader<Identifier> identifierReader;

        private readonly ISlbReader<Info> infoReader;

        // TODO: Use the correct info reader class here.
        public CharacterBinaryReader(Stream stream) : this(stream, new IdentifierBinaryReader(stream), null)
        {
        }

        // For unit testing
        internal CharacterBinaryReader(
            Stream stream,
            ISlbReader<Identifier> identifierReader,
            ISlbReader<Info> infoReader
        ) {
            this.stream = stream ?? throw new ArgumentNullException("Input stream cannot be null.");
            this.identifierReader = identifierReader ?? throw new ArgumentNullException("Input stream cannot be null.");
            this.infoReader = infoReader ?? throw new ArgumentNullException("Input stream cannot be null.");
        }

        /// <inheritdoc/>
        public Character ReadSlbObject()
        {
            var result = new Character()
            {
                ToaName = identifierReader.ReadSlbObject(),
                CharName = identifierReader.ReadSlbObject(),
                CharCont = identifierReader.ReadSlbObject()
            };

            var infoCount = stream.ForceReadUInt();
            for (int n = 0; n < infoCount; ++n)
            {
                var info = infoReader.ReadSlbObject();
                result.Entries.Add(info);
            }

            return result;
        }
    }
}
