using SAGESharp.Extensions;
using SAGESharp.Slb.IO;
using System;
using System.IO;

namespace SAGESharp.Slb.Level.Conversation.IO
{
    /// <summary>
    /// Class to read Character objects from binary SLB files.
    /// </summary>
    public class CharacterBinaryReader : ISlbReader<Character>
    {
        private readonly Stream stream;

        private readonly ISlbReader<Identifier> identifierReader;

        private readonly ISlbReader<Info> infoReader;

        /// <summary>
        /// Crates a new reader with the input objects that will be used to read the character data.
        /// </summary>
        /// 
        /// <param name="stream">The input stream</param>
        /// <param name="identifierReader">An identifier reader</param>
        /// <param name="infoReader">An info reader</param>
        public CharacterBinaryReader(
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
            if (infoCount > 0)
            {
                var infoPosition = stream.ForceReadUInt();

                stream.OnPositionDo(infoPosition, () => {
                    for (int n = 0; n < infoCount; ++n)
                    {
                        var info = infoReader.ReadSlbObject();
                        result.Entries.Add(info);
                    }
                });
            }

            return result;
        }
    }
}
