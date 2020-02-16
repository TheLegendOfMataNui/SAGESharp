/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUtils.Validations;
using System;

namespace SAGESharp.IO.Binary
{
    internal sealed class BinarySerializableSerializer<T> : IBinarySerializer<T> where T : IBinarySerializable, new()
    {
        private readonly Func<T> constructor;

        public BinarySerializableSerializer()
        {
            constructor = () => (T)Activator.CreateInstance(typeof(T), null);
        }

        public T Read(IBinaryReader binaryReader)
        {
            Validate.ArgumentNotNull(binaryReader, nameof(binaryReader));

            T result = constructor();

            result.Read(binaryReader);

            return result;
        }

        public void Write(IBinaryWriter binaryWriter, T value)
        {
            Validate.ArgumentNotNull(binaryWriter, nameof(binaryWriter));
            Validate.ArgumentNotNull<object>(value, nameof(value));

            value.Write(binaryWriter);
        }
    }
}
