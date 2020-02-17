/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUtils.Extensions;
using NUtils.Validations;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAGESharp.IO.Binary.TreeBasedSerialization
{
    internal sealed class StringDataNode : IDataNode
    {
        internal const byte OFFSET_STRING_MAX_LENGTH = byte.MaxValue - 1;

        private static readonly IReadOnlyList<IEdge> edges = new List<IEdge>();

        private readonly bool inlineString;

        private readonly byte length;

        public StringDataNode()
        {
            length = OFFSET_STRING_MAX_LENGTH;
            inlineString = false;
        }

        public StringDataNode(byte length)
        {
            this.length = length;
            inlineString = true;
        }

        public IReadOnlyList<IEdge> Edges => edges;

        public object Read(IBinaryReader binaryReader)
        {
            Validate.ArgumentNotNull(binaryReader, nameof(binaryReader));

            if (inlineString)
            {
                return ReadInlineString(binaryReader);
            }
            else
            {
                return ReadOfflineString(binaryReader);
            }
        }

        private string ReadInlineString(IBinaryReader binaryReader)
        {
            byte[] bytes = binaryReader.ReadBytes(length);
            int posOfNullCharacter = 0;

            // Get the position of the first null character
            while (posOfNullCharacter < bytes.Length && bytes[posOfNullCharacter] != 0)
            {
                posOfNullCharacter++;
            }

            return bytes.Take(posOfNullCharacter)
                .ToArray()
                .Let(Encoding.ASCII.GetString);
        }

        private string ReadOfflineString(IBinaryReader binaryReader)
        {
            byte length = binaryReader.ReadByte();
            string result = binaryReader.ReadBytes(length)
                .Let(Encoding.ASCII.GetString);

            // Read string termination character
            binaryReader.ReadByte();

            return result;
        }

        public void Write(IBinaryWriter binaryWriter, object value)
        {
            Validate.ArgumentNotNull(binaryWriter, nameof(binaryWriter));
            Validate.ArgumentNotNull(value, nameof(value));
            Validate.Argument(IsString(value), $"Cannot write value of type {value.GetType().Name} as a string.");
            Validate.Argument(IsCorrectLength(value), $"String length is longer than {length}.");

            string valueAsString = value as string;

            if (inlineString)
            {
                binaryWriter.WriteBytes(Encoding.ASCII.GetBytes(valueAsString));

                int diff = length - valueAsString.Length;
                if (diff != 0)
                {
                    binaryWriter.WriteBytes(new byte[diff]);
                }
            }
            else
            {
                binaryWriter.WriteByte((byte)valueAsString.Length);
                binaryWriter.WriteBytes(Encoding.ASCII.GetBytes(valueAsString));
                binaryWriter.WriteByte(0);
            }
        }

        private bool IsCorrectLength(object value) => (value as string).Length <= length;

        private static bool IsString(object value) => typeof(string).Equals(value.GetType());
    }
}
