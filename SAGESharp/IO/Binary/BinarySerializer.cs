/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUtils.Validations;
using SAGESharp.Animations;
using SAGESharp.IO.Binary.TreeBasedSerialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace SAGESharp.IO.Binary
{
    public static class BinarySerializer
    {
        private static readonly Lazy<IBinarySerializer<BKD>> bkdBinarySerializer
            = new Lazy<IBinarySerializer<BKD>>(() => new BinarySerializableSerializer<BKD>());

        private static readonly TreeReader treeReader = new TreeReader();

        /// <summary>
        /// Builds a binary serializer for type <typeparamref name="T"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">The type for the binary serializer.</typeparam>
        /// 
        /// <returns>A binary serializer for type <typeparamref name="T"/>.</returns>
        public static IBinarySerializer<T> ForType<T>()
        {
            return new TreeBinarySerializer<T>(
                treeReader: treeReader.Read,
                treeWriter: WriteTree,
                rootNode: TreeBuilder.BuildTreeForType(typeof(T)),
                footerAligner: AlignFooter
            );
        }

        /// <summary>
        /// The singleton instance to serialize <see cref="BKD"/> files.
        /// </summary>
        public static IBinarySerializer<BKD> ForBKDFiles { get => bkdBinarySerializer.Value; }

        internal static void AlignFooter(IBinaryWriter binaryWriter)
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

        // TreeWriter class is not stateless so every write needs a new instance of the class
        private static IReadOnlyList<uint> WriteTree(IBinaryWriter binaryWriter, object value, IDataNode root)
            => new TreeWriter().Write(binaryWriter, value, root);

        /// <summary>
        /// Reads an object of type <typeparamref name="T"/> from the input <paramref name="stream"/>
        /// using the given <paramref name="binarySerializer"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">Type of the objects to read.</typeparam>
        /// 
        /// <param name="binarySerializer">The binary serializer to use for reading an object.</param>
        /// <param name="stream">The input stream that will be used to read the object.</param>
        /// 
        /// <returns>The object read from <paramref name="stream"/> with <paramref name="binarySerializer"/>.</returns>
        public static T Read<T>(this IBinarySerializer<T> binarySerializer, Stream stream)
        {
            Validate.ArgumentNotNull(binarySerializer, nameof(binarySerializer));
            Validate.ArgumentNotNull(stream, nameof(stream));

            return binarySerializer.Read(Reader.ForStream(stream));
        }

        /// <summary>
        /// Writes <paramref name="value"/> to the output <paramref name="stream"/>
        /// using the given <paramref name="binarySerializer"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">Type of the objects to write.</typeparam>
        /// 
        /// <param name="binarySerializer">THe binary serializer to use for writing the object.</param>
        /// <param name="stream">The output stream that will be used to write the object.</param>
        /// <param name="value">The object to write.</param>
        public static void Write<T>(this IBinarySerializer<T> binarySerializer, Stream stream, T value)
        {
            Validate.ArgumentNotNull(binarySerializer, nameof(binarySerializer));
            Validate.ArgumentNotNull(stream, nameof(stream));
            Validate.ArgumentNotNull<object>(value, nameof(value));

            binarySerializer.Write(Writer.ForStream(stream), value);
        }
    }
}
