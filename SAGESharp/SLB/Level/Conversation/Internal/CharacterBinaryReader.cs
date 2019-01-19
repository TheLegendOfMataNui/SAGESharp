using SAGESharp.Extensions;
using System;
using System.IO;

namespace SAGESharp.SLB.Level.Conversation.Internal
{
    /// <summary>
    /// Class to read Character objects from binary SLB files.
    /// </summary>
    internal sealed class CharacterBinaryReader : ISLBBinaryReader<Character>
    {
        private readonly Stream stream;

        private readonly ISLBBinaryReader<Identifier> identifierReader;

        private readonly ISLBBinaryReader<Info> infoReader;

        /// <summary>
        /// Crates a new reader with the input objects that will be used to read the character data.
        /// </summary>
        /// 
        /// <param name="stream">The input stream</param>
        /// <param name="identifierReader">An identifier reader</param>
        /// <param name="infoReader">An info reader</param>
        public CharacterBinaryReader(
            Stream stream,
            ISLBBinaryReader<Identifier> identifierReader,
            ISLBBinaryReader<Info> infoReader
        ) {
            this.stream = stream ?? throw new ArgumentNullException("Input stream cannot be null.");
            this.identifierReader = identifierReader ?? throw new ArgumentNullException("Input stream cannot be null.");
            this.infoReader = infoReader ?? throw new ArgumentNullException("Input stream cannot be null.");
        }

        /// <inheritdoc/>
        public Character ReadSLBObject()
        {
            var result = new Character()
            {
                ToaName = identifierReader.ReadSLBObject(),
                CharName = identifierReader.ReadSLBObject(),
                CharCont = identifierReader.ReadSLBObject()
            };

            var infoCount = stream.ForceReadUInt();
            if (infoCount > 0)
            {
                var infoPosition = stream.ForceReadUInt();

                stream.OnPositionDo(infoPosition, () => {
                    for (int n = 0; n < infoCount; ++n)
                    {
                        var info = infoReader.ReadSLBObject();
                        result.Entries.Add(info);
                    }
                });
            }

            return result;
        }
    }
}
