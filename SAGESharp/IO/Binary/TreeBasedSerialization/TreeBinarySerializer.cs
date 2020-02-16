/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUtils.Extensions;
using NUtils.Validations;
using System;
using System.Collections.Generic;

namespace SAGESharp.IO.Binary.TreeBasedSerialization
{
    internal sealed class TreeBinarySerializer<T> : IBinarySerializer<T>
    {
        public delegate object TreeReader(IBinaryReader binaryReader, IDataNode rootNode);

        public delegate IReadOnlyList<uint> TreeWriter(IBinaryWriter binaryWriter, object value, IDataNode rootNode);

        internal const uint FOOTER_MAGIC_NUMBER = 0x00C0FFEE;

        private readonly TreeReader treeReader;

        private readonly TreeWriter treeWriter;

        private readonly IDataNode rootNode;

        private readonly Action<IBinaryWriter> alignFooter;

        public TreeBinarySerializer(
            TreeReader treeReader,
            TreeWriter treeWriter,
            IDataNode rootNode,
            Action<IBinaryWriter> footerAligner
        ) {
            Validate.ArgumentNotNull(treeReader, nameof(treeReader));
            Validate.ArgumentNotNull(treeWriter, nameof(treeWriter));
            Validate.ArgumentNotNull(rootNode, nameof(rootNode));
            Validate.ArgumentNotNull(footerAligner, nameof(footerAligner));

            this.treeReader = treeReader;
            this.treeWriter = treeWriter;
            this.rootNode = rootNode;
            alignFooter = footerAligner;
        }

        public T Read(IBinaryReader binaryReader)
        {
            Validate.ArgumentNotNull(binaryReader, nameof(binaryReader));

            return (T)treeReader(binaryReader, rootNode);
        }

        public void Write(IBinaryWriter binaryWriter, T value)
        {
            Validate.ArgumentNotNull(binaryWriter, nameof(binaryWriter));
            Validate.ArgumentNotNull<object>(value, nameof(value));

            IReadOnlyList<uint> offsets = treeWriter(binaryWriter, value, rootNode);

            alignFooter(binaryWriter);

            // Write footer
            offsets.ForEach(binaryWriter.WriteUInt32);
            binaryWriter.WriteInt32(offsets.Count);
            binaryWriter.WriteUInt32(FOOTER_MAGIC_NUMBER);
        }
    }
}
