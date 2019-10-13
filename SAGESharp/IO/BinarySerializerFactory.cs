/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using System;

namespace SAGESharp.IO
{
    /// <summary>
    /// Interface to create instances of <see cref="IBinarySerializer"/>.
    /// </summary>
    public interface IBinarySerializerFactory
    {
        /// <summary>
        /// Creates the corresponding <see cref="IBinarySerializer"/> instance for the given <typeparamref name="T"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// 
        /// <returns>An instance of <see cref="IBinarySerializer" /> for <typeparamref name="T"/>.</returns>
        IBinarySerializer<T> GetSerializerForType<T>();
    }

    /// <summary>
    /// Static class to provide a singleton of the <see cref="IBinarySerializerFactory"/>.
    /// </summary>
    public static class BinarySerializers
    {
        private static readonly Lazy<IBinarySerializerFactory> instance
            = new Lazy<IBinarySerializerFactory>(
                () => new TreeBasedBinarySerializerFactory()
            );

        /// <summary>
        /// The singleton instance of the <see cref="IBinarySerializerFactory"/> interface.
        /// </summary>
        public static IBinarySerializerFactory Factory { get => instance.Value; }

        private static readonly Lazy<IBinarySerializer<BKD>> bkdBinarySerializer
            = new Lazy<IBinarySerializer<BKD>>(() => new BinarySerializableSerializer<BKD>());

        /// <summary>
        /// The singleton instance to serialize <see cref="BKD"/> files.
        /// </summary>
        public static IBinarySerializer<BKD> ForBKDFiles { get => bkdBinarySerializer.Value; }
    }

    internal sealed class TreeBasedBinarySerializerFactory : IBinarySerializerFactory
    {
        public IBinarySerializer<T> GetSerializerForType<T>()
        {
            return new TreeBinarySerializer<T>(
                treeReader: new TreeReader(Reader.DoAtPosition),
                treeWriter: new TreeWriter(OffsetWriter),
                rootNode: TreeBuilder.BuildTreeForType(typeof(T)),
                footerAligner: FooterAligner
            );
        }

        internal static void OffsetWriter(IBinaryWriter binaryWriter, uint offset)
        {
            binaryWriter.DoAtPosition(offset, originalPosition =>
            {
                Validate.Argument(originalPosition <= uint.MaxValue,
                    $"Offset 0x{originalPosition:X} is larger than {sizeof(uint)} bytes.");

                binaryWriter.WriteUInt32((uint)originalPosition);
            });
        }

        internal static void FooterAligner(IBinaryWriter binaryWriter)
        {
            // We get the amount of bytes that are unaligned
            // by getting bytes after the last aligned integer (end % 4)
            // and counting the amount of additional bytes needed
            // to be aligned (4 - bytes after last aligned integer)
            var bytesToFill = 4 - (binaryWriter.Position % 4);
            if (bytesToFill != 4)
            {
                binaryWriter.WriteBytes(new byte[bytesToFill]);
            }
        }
    }
}
