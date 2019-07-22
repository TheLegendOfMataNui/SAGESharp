/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SAGESharp.SLB.Level.Conversation
{
    /// <summary>
    /// Class to write a conversation as a binary SLB file.
    /// </summary>
    internal sealed class ConversationBinaryWriter : ISLBBinaryWriter<IList<Character>>
    {
        private readonly Stream stream;

        private readonly ISLBBinaryWriter<Character> characterWriter;

        private readonly ISLBBinaryWriter<Info> infoWriter;

        private readonly ISLBBinaryWriter<Frame> frameWriter;

        private readonly ISLBBinaryWriter<string> stringWriter;

        private readonly ISLBFooterWriter<IList<Character>> footerWriter;

        /// <summary>
        /// Creates a new conversation writer aggregating the input writers.
        /// </summary>
        /// 
        /// <param name="stream">The output stream.</param>
        /// <param name="characterWriter">The writer for individual <see cref="Character"/> entries.</param>
        /// <param name="infoWriter">The writer for individual <see cref="Info"/> entries.</param>
        /// <param name="frameWriter">The writer for individual <see cref="Frame"/> entries.</param>
        /// <param name="stringWriter">The writer for strings.</param>
        /// <param name="footerWriter">The write for the footer of the conversation.</param>
        /// 
        /// <exception cref="ArgumentNullException">If any argument is null.</exception>
        public ConversationBinaryWriter(
            Stream stream,
            ISLBBinaryWriter<Character> characterWriter,
            ISLBBinaryWriter<Info> infoWriter,
            ISLBBinaryWriter<Frame> frameWriter,
            ISLBBinaryWriter<string> stringWriter,
            ISLBFooterWriter<IList<Character>> footerWriter
        ) {
            this.stream = stream ?? throw new ArgumentNullException("The input stream cannot be null.");
            this.characterWriter = characterWriter ?? throw new ArgumentNullException("The character writer cannot be null.");
            this.infoWriter = infoWriter ?? throw new ArgumentNullException("The info writer cannot be null.");
            this.frameWriter = frameWriter ?? throw new ArgumentNullException("The frame writer cannot be null.");
            this.stringWriter = stringWriter ?? throw new ArgumentNullException("The string writer cannot be null.");
            this.footerWriter = footerWriter ?? throw new ArgumentNullException("The footer writer cannot be null.");
        }

        /// <inheritdoc/>
        /// 
        /// <exception cref="ArgumentNullException">If <paramref name="slbObject"/> is null.</exception>
        public void WriteSLBObject(IList<Character> slbObject)
        {
            if (slbObject == null)
            {
                throw new ArgumentNullException();
            }

            stream.WriteInt(slbObject.Count);
            stream.WriteInt(0); // Offset for the list of characters

            foreach (var character in slbObject)
            {
                characterWriter.WriteSLBObject(character);
            }

            var entries = slbObject.SelectMany(c => c.Entries);
            foreach (var info in entries)
            {
                infoWriter.WriteSLBObject(info);
            }

            var frames = entries.SelectMany(i => i.Frames);
            foreach (var frame in frames)
            {
                frameWriter.WriteSLBObject(frame);
            }

            foreach (var conversationSounds in frames.Select(f => f.ConversationSounds))
            {
                stringWriter.WriteSLBObject(conversationSounds);
            }

            footerWriter.WriteFooter(slbObject);
        }
    }
}
