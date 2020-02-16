/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUtils.Validations;
using System;

namespace SAGESharp.IO.Binary.TreeBasedSerialization
{
    internal sealed class TreeReader
    {
        public object Read(IBinaryReader binaryReader, IDataNode rootNode)
        {
            Validate.ArgumentNotNull(binaryReader, nameof(binaryReader));
            Validate.ArgumentNotNull(rootNode, nameof(rootNode));

            return ProcessDataNode(binaryReader, rootNode);
        }

        private object ProcessNode(IBinaryReader binaryReader, object node)
        {
            if (node is IDataNode dataNode)
            {
                return ProcessDataNode(binaryReader, dataNode);
            }
            else if (node is IListNode listNode)
            {
                return ProcessListNode(binaryReader, listNode);
            }
            else if (node is IOffsetNode offsetNode)
            {
                return ProcessOffsetNode(binaryReader, offsetNode);
            }
            else
            {
                throw new NotImplementedException($"Type {node.GetType().Name} is an unknown node type");
            }
        }

        private object ProcessDataNode(IBinaryReader binaryReader, IDataNode node)
        {
            object value = node.Read(binaryReader);

            foreach (IEdge edge in node.Edges)
            {
                object childValue = ProcessNode(binaryReader, edge.ChildNode);

                edge.SetChildValue(value, childValue);
            }

            return value;
        }

        private object ProcessOffsetNode(IBinaryReader binaryReader, IOffsetNode node)
        {
            uint offset = node.ReadOffset(binaryReader);

            object result = null;
            binaryReader.DoAtPosition(offset, () => result = ProcessDataNode(binaryReader, node.ChildNode));

            return result;
        }

        private object ProcessListNode(IBinaryReader binaryReader, IListNode listNode)
        {
            int count = listNode.ReadEntryCount(binaryReader);
            uint offset = listNode.ReadOffset(binaryReader);
            object list = listNode.CreateList();

            if (count != 0)
            {
                binaryReader.DoAtPosition(offset, () =>
                {
                    for (int n = 0; n < count; ++n)
                    {
                        object entry = ProcessDataNode(binaryReader, listNode.ChildNode);
                        listNode.AddListEntry(list, entry);
                    }
                });
            }

            return list;
        }
    }
}
