using System;
using System.Collections.Generic;
using System.IO;

namespace SAGESharp.SLB.Level.Conversation
{
    /// <summary>
    /// Class to read Character objects from binary SLB files.
    /// </summary>
    internal sealed class CharacterBinaryReader : ISLBBinaryReader<Character>
    {
        private readonly Stream stream;

        private readonly ISLBBinaryReader<Info> infoReader;

        /// <summary>
        /// Crates a new reader with the input objects that will be used to read the character data.
        /// </summary>
        /// 
        /// <param name="stream">The input stream</param>
        /// <param name="infoReader">An info reader</param>
        public CharacterBinaryReader(Stream stream, ISLBBinaryReader<Info> infoReader) {
            this.stream = stream ?? throw new ArgumentNullException("Input stream cannot be null.");
            this.infoReader = infoReader ?? throw new ArgumentNullException("Input stream cannot be null.");
        }

        /// <inheritdoc/>
        public Character ReadSLBObject()
        {
            var buffer = stream.ForceReadBytes(Character.BINARY_SIZE);

            var result = new Character()
            {
                ToaName = buffer.ToInt32(),
                CharName = buffer.ToInt32(4),
                CharCont = buffer.ToInt32(8),
                Entries = new List<Info>()
            };

            var infoCount = buffer.ToInt32(12);
            if (infoCount > 0)
            {
                var infoPosition = buffer.ToInt32(16);

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
