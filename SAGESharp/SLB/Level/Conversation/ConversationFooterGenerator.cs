using System;
using System.Collections.Generic;

namespace SAGESharp.SLB.Level.Conversation
{
    /// <summary>
    /// Objects that generates the footer table for a conversation.
    /// </summary>
    class ConversationFooterGenerator : ISLBFooterGenerator<IList<Character>>
    {
        private const int INFO_LENGTH = 28;

        private const int FRAME_LENGTH = 24;

        /// <inheritdoc/>
        /// 
        /// <exception cref="ArgumentNullException">If <paramref name="slbObject"/> is null.</exception>
        public IReadOnlyList<FooterEntry> GenerateFooter(IList<Character> slbObject)
        {
            if (slbObject == null)
            {
                throw new ArgumentNullException();
            }

            var result = new List<FooterEntry>
            {
                // First offset is always at position 4 with value 8
                new FooterEntry { OffsetPosition = 0x00000004, Offset = 0x00000008 }
            };

            // Cursor1 is the position of the offset in the first "Character" object
            var cursor1 = (uint)0x00000018;
            // Cursor2 is the position afte the list of all "Character" objects plus
            // the header of the list (size + offset at the beginning of the file)
            var cursor2 = (uint)(Character.BINARY_SIZE * slbObject.Count) + 8;
            foreach (var character in slbObject)
            {
                result.Add(new FooterEntry { OffsetPosition = cursor1, Offset = cursor2 });

                cursor1 += Character.BINARY_SIZE;
                cursor2 += (uint)(INFO_LENGTH * character.Entries.Count);
            }

            // Move the cursor to the position of the offset in the first "Info" object
            cursor1 += 8;
            foreach (var character in slbObject)
            {
                foreach (var info in character.Entries)
                {
                    result.Add(new FooterEntry { OffsetPosition = cursor1, Offset = cursor2 });

                    cursor1 += INFO_LENGTH;
                    cursor2 += (uint)(FRAME_LENGTH * info.Frames.Count);
                }
            }

            // Move the cursor to the position of the offset in the first "Frame" object
            cursor1 -= 4;
            foreach (var character in slbObject)
            {
                foreach (var info in character.Entries)
                {
                    foreach (var frame in info.Frames)
                    {
                        result.Add(new FooterEntry { OffsetPosition = cursor1, Offset = cursor2 });

                        cursor1 += FRAME_LENGTH;
                        cursor2 += (uint)(frame.ConversationSounds.Length + 2);
                    }
                }
            }

            return result;
        }
    }
}
