using SAGESharp.Extensions;
using System;
using System.IO;

namespace SAGESharp.SLB.Level.Conversation.Internal
{
    /// <summary>
    /// Class to read Info objects from binary SLB files.
    /// </summary>
    internal sealed class InfoBinaryReader : ISLBBinaryReader<Info>
    {
        private readonly Stream stream;

        private readonly ISLBBinaryReader<Identifier> identifierReader;

        private readonly ISLBBinaryReader<Frame> frameReader;

        /// <summary>
        /// Crates a new reader with the input objects that will be used to read the info.
        /// </summary>
        /// 
        /// <param name="stream">The input stream</param>
        /// <param name="identifierReader">An identifier reader</param>
        /// <param name="frameReader">A frame reader</param>
        // TODO: Use the correct frame reader class here.
        public InfoBinaryReader(
            Stream stream,
            ISLBBinaryReader<Identifier> identifierReader,
            ISLBBinaryReader<Frame> frameReader
        ) {
            this.stream = stream ?? throw new ArgumentNullException("Input stream cannot be null.");
            this.identifierReader = identifierReader ?? throw new ArgumentNullException("Identifier reader cannot be null.");
            this.frameReader = frameReader ?? throw new ArgumentNullException("Frame reader cannot be null.");
        }

        /// <inheritdoc/>
        public Info ReadSLBObject()
        {
            var result = new Info()
            {
                LineSide = (LineSide)stream.ForceReadInt(),
                ConditionStart = stream.ForceReadUInt(),
                ConditionEnd = stream.ForceReadUInt(),
                StringLabel = identifierReader.ReadSLBObject(),
                StringIndex = stream.ForceReadInt()
            };

            var frameCount = stream.ForceReadInt();
            if (frameCount > 0)
            {
                var framesPosition = stream.ForceReadInt();

                stream.OnPositionDo(framesPosition, () =>
                {
                    for (int n = 0; n < frameCount; ++n)
                    {
                        var frame = frameReader.ReadSLBObject();
                        result.Frames.Add(frame);
                    }
                });
            }

            return result;
        }
    }
}
