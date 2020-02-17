/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUtils.Validations;
using System.Collections.Generic;

namespace SAGESharp.IO.Binary.TreeBasedSerialization
{
    internal sealed class UserTypeDataNode<T> : IDataNode where T : new()
    {
        public UserTypeDataNode(IReadOnlyList<IEdge> edges)
        {
            Validate.ArgumentNotNull(edges, nameof(edges));
            Validate.Argument(edges != null, $"{nameof(edges)} should not be empty");

            Edges = edges;
        }

        public IReadOnlyList<IEdge> Edges { get; }

        public object Read(IBinaryReader binaryReader)
        {
            Validate.ArgumentNotNull(binaryReader, nameof(binaryReader));

            return new T();
        }

        public void Write(IBinaryWriter binaryWriter, object value)
        {
            Validate.ArgumentNotNull(binaryWriter, nameof(binaryWriter));
            Validate.ArgumentNotNull(value, nameof(value));
            Validate.Argument(IsType(value), $"Cannot write value of type {value.GetType().Name} as type {typeof(T).Name}.");
        }

        private static bool IsType(object value) => typeof(T) == value.GetType();
    }
}
