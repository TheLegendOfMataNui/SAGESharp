using System;
using System.Collections.Generic;
using System.IO;

namespace SAGESharp.SLB.Level.Conversation
{
    /// <summary>
    /// Class to read Info objects from binary SLB files.
    /// </summary>
    internal sealed class InfoBinaryReader : ISLBBinaryReader<Info>
    {
        private readonly Stream stream;

        private readonly ISLBBinaryReader<Frame> frameReader;

        /// <summary>
        /// Crates a new reader with the input objects that will be used to read the info.
        /// </summary>
        /// 
        /// <param name="stream">The input stream</param>
        /// <param name="frameReader">A frame reader</param>
        public InfoBinaryReader(Stream stream, ISLBBinaryReader<Frame> frameReader) {
            this.stream = stream ?? throw new ArgumentNullException("Input stream cannot be null.");
            this.frameReader = frameReader ?? throw new ArgumentNullException("Frame reader cannot be null.");
        }

        /// <inheritdoc/>
        public Info ReadSLBObject()
        {
            var buffer = stream.ForceReadBytes(Info.BINARY_SIZE);

            var result = new Info
            {
                LineSide = (LineSide)buffer.ToInt32(),
                ConditionStart = buffer.ToUInt32(4),
                ConditionEnd = buffer.ToUInt32(8),
                StringLabel = buffer.ToInt32(12),
                StringIndex = buffer.ToInt32(16),
                Frames = new List<Frame>()
            };

            var frameCount = buffer.ToInt32(20);
            if (frameCount > 0)
            {
                var framesPosition = buffer.ToInt32(24);

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
