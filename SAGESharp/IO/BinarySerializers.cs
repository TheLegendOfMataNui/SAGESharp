/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUtils.Extensions;
using NUtils.Validations;
using System;
using System.Collections.Generic;
using System.IO;

namespace SAGESharp.IO
{
    #region Interface
    /// <summary>
    /// Represents an object that serializes objects of type <typeparamref name="T"/>.
    /// </summary>
    /// 
    /// <typeparam name="T">Type of the objects to serialize.</typeparam>
    public interface IBinarySerializer<T>
    {
        /// <summary>
        /// Reads an object of type <typeparamref name="T"/> from the input <paramref name="binaryReader"/>.
        /// </summary>
        /// 
        /// <param name="binaryReader">The binary reader used to read the object.</param>
        /// 
        /// <returns>The object read from the <paramref name="binaryReader"/>.</returns>
        /// 
        /// <exception cref="ArgumentNullException">If <paramref name="binaryReader"/> is null.</exception>
        T Read(IBinaryReader binaryReader);

        /// <summary>
        /// Writes <paramref name="value"/> to the output <paramref name="binaryWriter"/>.
        /// </summary>
        /// 
        /// <param name="binaryWriter">The output binary writer were the object will be written.</param>
        /// <param name="value">The object to write.</param>
        /// 
        /// <exception cref="ArgumentNullException">If either <paramref name="binaryWriter"/> or <paramref name="value"/> are null.</exception>
        void Write(IBinaryWriter binaryWriter, T value);
    }

    /// <summary>
    /// Represents an object that can serialize itself.
    /// </summary>
    public interface IBinarySerializable
    {
        /// <summary>
        /// Reads data from the <paramref name="binaryReader"/> into the object itself.
        /// </summary>
        /// 
        /// <param name="binaryReader">The input reader.</param>
        void Read(IBinaryReader binaryReader);

        /// <summary>
        /// Writes data to the <paramref name="binaryWriter"/> from the object itself.
        /// </summary>
        /// 
        /// <param name="binaryWriter">The output writer.</param>
        void Write(IBinaryWriter binaryWriter);
    }

    /// <summary>
    /// The exception that is thrown when a type cannot be serialized.
    /// </summary>
    public class BadTypeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BadTypeException"/>
        /// class with the given <paramref name="type"/>.
        /// </summary>
        /// 
        /// <param name="type">The type that cannot be serialized.</param>
        public BadTypeException(Type type) : base()
            => Type = type;

        /// <summary>
        /// Initializes a new instance of the <see cref="BadTypeException"/>
        /// class with the given <paramref name="type"/> and the given
        /// <paramref name="message"/>.
        /// </summary>
        /// 
        /// <param name="type">The type that cannot be serialized.</param>
        /// <param name="message">The message that describes the error.</param>
        public BadTypeException(Type type, string message) : base(message)
            => Type = type;

        /// <summary>
        /// Initializes a new instance of the <see cref="BadTypeException"/>
        /// class with the given <paramref name="type"/>, the given
        /// <paramref name="message"/> and the given <paramref name="innerException"/>.
        /// </summary>
        /// 
        /// <param name="type">The type that cannot be serialized.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">he exception that is the cause of the current exception.</param>
        public BadTypeException(Type type, string message, Exception innerException)
            : base(message, innerException) => Type = type;

        /// <summary>
        /// The type that cannot be serialized.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Creates a "type safe" <see cref="BadTypeException"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">The type that cannot be serialized.</typeparam>
        /// 
        /// <returns>An instance of <see cref="BadTypeException"/> with type <typeparamref name="T"/>.</returns>
        public static BadTypeException For<T>()
            => new BadTypeException(typeof(T));

        /// <summary>
        /// Creates a "type safe" <see cref="BadTypeException"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">The type that cannot be serialized.</typeparam>
        /// 
        /// <param name="message">The message that describes the error.</param>
        /// 
        /// <returns>
        /// An instance of <see cref="BadTypeException"/> with type <typeparamref name="T"/> and <paramref name="message"/>.
        /// </returns>
        public static BadTypeException For<T>(string message)
            => new BadTypeException(typeof(T), message);

        /// <summary>
        /// Creates a "type safe" <see cref="BadTypeException"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">The type that cannot be serialized.</typeparam>
        /// 
        /// <param name="message">The message that describes the error.</param>
        /// 
        /// <returns>
        /// An instance of <see cref="BadTypeException"/> with type <typeparamref name="T"/>, <paramref name="message"/> and <paramref name="innerException"/>.
        /// </returns>
        public static BadTypeException For<T>(string message, Exception innerException)
            => new BadTypeException(typeof(T), message, innerException);
    }

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
    #endregion

    #region Implementation
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
    #endregion
}
