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
    internal sealed class TreeWriter
    {
        private class QueueEntry
        {
            public QueueEntry(IDataNode node, object value)
            {
                Node = node;
                Value = value;
            }

            public QueueEntry(IDataNode node, object value, uint offsetPosition) : this(node, value)
            {
                OffsetPosition = offsetPosition;
            }

            public IDataNode Node { get; }

            public object Value { get; }

            public uint? OffsetPosition { get; }
        }

        private readonly Queue<QueueEntry> queue = new Queue<QueueEntry>();

        private readonly List<uint> offsets = new List<uint>();

        public IReadOnlyList<uint> Write(IBinaryWriter binaryWriter, object value, IDataNode rootNode)
        {
            Validate.ArgumentNotNull(binaryWriter, nameof(binaryWriter));
            Validate.ArgumentNotNull(value, nameof(value));
            Validate.ArgumentNotNull(rootNode, nameof(rootNode));

            queue.Clear();
            offsets.Clear();

            Enqueue(rootNode, value);

            while (queue.IsNotEmpty())
            {
                QueueEntry entry = queue.Dequeue();

                ProcessOffset(binaryWriter, entry.OffsetPosition);
                if (entry.Value != null)
                {
                    ProcessDataNode(binaryWriter, entry.Node, entry.Value);
                }
            }

            return offsets;
        }

        private void ProcessOffset(IBinaryWriter binaryWriter, uint? offsetPosition)
        {
            if (!offsetPosition.HasValue)
            {
                return;
            }

            binaryWriter.DoAtPosition(offsetPosition.Value, originalPosition =>
            {
                Validate.Argument(originalPosition <= uint.MaxValue,
                    $"Offset 0x{originalPosition:X} is larger than {sizeof(uint)} bytes.");

                binaryWriter.WriteUInt32((uint)originalPosition);
            });
            offsets.Add(offsetPosition.Value);
        }

        private void ProcessNode(IBinaryWriter binaryWriter, object node, object value)
        {
            if (node is IDataNode dataNode)
            {
                ProcessDataNode(binaryWriter, dataNode, value);
            }
            else if (node is IListNode listNode)
            {
                ProcessListNode(binaryWriter, listNode, value);
            }
            else if (node is IOffsetNode offsetNode)
            {
                ProcessOffsetNode(binaryWriter, offsetNode, value);
            }
            else
            {
                throw new NotImplementedException($"Type {node.GetType().Name} is an unknown node type");
            }
        }

        private void ProcessDataNode(IBinaryWriter binaryWriter, IDataNode node, object value)
        {
            node.Write(binaryWriter, value);

            foreach (IEdge edge in node.Edges)
            {
                object childValue = edge.ExtractChildValue(value);
                ProcessNode(binaryWriter, edge.ChildNode, childValue);
            }
        }

        private void ProcessOffsetNode(IBinaryWriter binaryWriter, IOffsetNode node, object value)
        {
            uint offsetPosition = node.Write(binaryWriter, value);

            Enqueue(node.ChildNode, value, offsetPosition);
        }

        private void ProcessListNode(IBinaryWriter binaryWriter, IListNode node, object list)
        {
            uint offsetPosition = node.Write(binaryWriter, list);
            int count = node.GetListCount(list);
            if (count == 0)
            {
                Enqueue(node.ChildNode, null, offsetPosition);
                return;
            }

            Enqueue(node.ChildNode, node.GetListEntry(list, 0), offsetPosition);

            for (int n = 1; n < count; ++n)
            {
                Enqueue(node.ChildNode, node.GetListEntry(list, n));
            }
        }

        private void Enqueue(IDataNode node, object value)
        {
            queue.Enqueue(new QueueEntry(node, value));
        }

        private void Enqueue(IDataNode node, object value, uint offsetPosition)
        {
            queue.Enqueue(new QueueEntry(node, value, offsetPosition));
        }
    }
}
