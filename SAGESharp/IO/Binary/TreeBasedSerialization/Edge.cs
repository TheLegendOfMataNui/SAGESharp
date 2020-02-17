/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUtils.Validations;
using System;

namespace SAGESharp.IO.Binary.TreeBasedSerialization
{
    internal sealed class Edge<T> : IEdge
    {
        private readonly Func<T, object> extractor;

        private readonly Action<T, object> setter;

        public Edge(Func<T, object> extractor, Action<T, object> setter, object childNode)
        {
            Validate.ArgumentNotNull(extractor, nameof(extractor));
            Validate.ArgumentNotNull(setter, nameof(setter));
            Validate.ArgumentNotNull(childNode, nameof(childNode));

            this.extractor = extractor;
            this.setter = setter;
            ChildNode = childNode;
        }

        public object ChildNode { get; }

        public object ExtractChildValue(object value)
        {
            Validate.ArgumentNotNull(value, nameof(value));
            Validate.Argument(IsType(value), $"Expected {nameof(value)} to be of type {typeof(T).Name} but was of type {value.GetType().Name} instead");

            return extractor((T)value);
        }

        public void SetChildValue(object value, object childValue)
        {
            Validate.ArgumentNotNull(value, nameof(value));
            Validate.ArgumentNotNull(childValue, nameof(childValue));
            Validate.Argument(IsType(value), $"Expected {nameof(value)} to be of type {typeof(T).Name} but was of type {value.GetType().Name} instead");

            setter((T)value, childValue);
        }

        private static bool IsType(object value) => typeof(T) == value.GetType();
    }
}
