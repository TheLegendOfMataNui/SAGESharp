/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUtils.Validations;
using System.Collections.Generic;

namespace SAGESharp.IO.Binary.TreeBasedSerialization
{
    internal sealed class ListNode<T> : AbstractOffsetNode, IListNode
    {
        private readonly bool duplicateEntryCount;

        public ListNode(IDataNode childNode, bool duplicateEntryCount = false) : base(childNode)
        {
            this.duplicateEntryCount = duplicateEntryCount;
        }

        public int GetListCount(object list)
        {
            Validate.ArgumentNotNull(list, nameof(list));
            ValidateIsList(list);

            return (list as IList<T>).Count;
        }

        public object GetListEntry(object list, int index)
        {
            Validate.ArgumentNotNull(list, nameof(list));
            ValidateIsList(list);

            return (list as IList<T>)[index];
        }

        public void AddListEntry(object list, object value)
        {
            Validate.ArgumentNotNull(list, nameof(list));
            Validate.ArgumentNotNull(value, nameof(value));
            ValidateIsList(list);
            Validate.Argument(
                typeof(T).Equals(value.GetType()),
                $"Value should be of type {typeof(string).Name}, but is of type {value.GetType().Name} instead."
            );

            (list as IList<T>).Add((T)value);
        }

        public object CreateList()
        {
            return new List<T>();
        }

        public int ReadEntryCount(IBinaryReader binaryReader)
        {
            Validate.ArgumentNotNull(binaryReader, nameof(binaryReader));

            int result = binaryReader.ReadInt32();

            if (duplicateEntryCount)
            {
                binaryReader.ReadInt32();
            }

            return result;
        }

        public override uint Write(IBinaryWriter binaryWriter, object value)
        {
            Validate.ArgumentNotNull(binaryWriter, nameof(binaryWriter));
            Validate.ArgumentNotNull(value, nameof(value));
            ValidateIsList(value);

            binaryWriter.WriteInt32((value as IList<T>).Count);
            if (duplicateEntryCount)
            {
                binaryWriter.WriteInt32((value as IList<T>).Count);
            }

            return WriteOffset(binaryWriter);
        }

        private static void ValidateIsList(object list) => Validate.Argument(
            typeof(IList<T>).IsAssignableFrom(list.GetType()),
            $"List argument is of type {list.GetType().Name} which should implement {typeof(IList<T>).Name} but doesn't."
        );
    }
}
