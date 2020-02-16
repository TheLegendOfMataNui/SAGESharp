/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUtils.Validations;

namespace SAGESharp.IO.Binary.TreeBasedSerialization
{
    internal sealed class OffsetNode : AbstractOffsetNode
    {
        public OffsetNode(IDataNode childNode) : base(childNode)
        {
        }

        public override uint Write(IBinaryWriter binaryWriter, object value)
        {
            Validate.ArgumentNotNull(binaryWriter, nameof(binaryWriter));
            Validate.ArgumentNotNull(value, nameof(value));

            return WriteOffset(binaryWriter);
        }
    }
}
