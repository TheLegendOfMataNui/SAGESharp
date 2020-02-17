/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUtils.Validations;
using System.Collections.Generic;

namespace SAGESharp.IO.Binary.TreeBasedSerialization
{
    internal sealed class PaddingNode : IDataNode
    {
        private readonly byte[] padding;

        private readonly IDataNode childNode;

        public PaddingNode(byte size, IDataNode childNode)
        {
            Validate.Argument(size > 0, "Padding size cannot be 0.");
            Validate.ArgumentNotNull(childNode, nameof(childNode));

            padding = new byte[size];
            this.childNode = childNode;
        }

        public IReadOnlyList<IEdge> Edges { get => childNode.Edges; }

        public object Read(IBinaryReader binaryReader)
        {
            Validate.ArgumentNotNull(binaryReader, nameof(binaryReader));

            object result = childNode.Read(binaryReader);

            binaryReader.ReadBytes(padding.Length);

            return result;
        }

        public void Write(IBinaryWriter binaryWriter, object value)
        {
            Validate.ArgumentNotNull(binaryWriter, nameof(binaryWriter));
            Validate.ArgumentNotNull(value, nameof(value));

            childNode.Write(binaryWriter, value);

            binaryWriter.WriteBytes(padding);
        }
    }
}
