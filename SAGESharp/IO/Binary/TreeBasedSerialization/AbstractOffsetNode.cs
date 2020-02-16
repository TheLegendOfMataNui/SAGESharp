/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUtils.Validations;
using System;

namespace SAGESharp.IO.Binary.TreeBasedSerialization
{
    internal abstract class AbstractOffsetNode : IOffsetNode
    {
        protected AbstractOffsetNode(IDataNode childNode)
        {
            Validate.ArgumentNotNull(childNode, nameof(childNode));

            ChildNode = childNode;
        }

        public IDataNode ChildNode { get; }

        public uint ReadOffset(IBinaryReader binaryReader)
        {
            Validate.ArgumentNotNull(binaryReader, nameof(binaryReader));

            return binaryReader.ReadUInt32();
        }

        public abstract uint Write(IBinaryWriter binaryWriter, object value);

        protected uint WriteOffset(IBinaryWriter binaryWriter)
        {
            if (binaryWriter.Position > uint.MaxValue)
            {
                throw new InvalidOperationException("Offset is bigger than 4 bytes.");
            }

            uint offsetPosition = (uint) binaryWriter.Position;

            binaryWriter.WriteUInt32(0);

            return offsetPosition;
        }
    }
}
