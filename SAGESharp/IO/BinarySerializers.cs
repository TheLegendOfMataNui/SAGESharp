/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using Konvenience;
using System;
using System.Collections.Generic;

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
                treeReader: new TreeReader(Reader.DoAtPosition),
                treeWriter: new TreeWriter(OffsetWriter),
                rootNode: TreeBuilder.BuildTreeForType(typeof(T)),
                footerAligner: FooterAligner
            );
        }

        /// <summary>
        /// The singleton instance to serialize <see cref="BKD"/> files.
        /// </summary>
        public static IBinarySerializer<BKD> ForBKDFiles { get => bkdBinarySerializer.Value; }

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
    #endregion

    #region Implementation
    internal sealed class TreeBinarySerializer<T> : IBinarySerializer<T>
    {
        internal const uint FOOTER_MAGIC_NUMBER = 0x00C0FFEE;

        private readonly ITreeReader treeReader;

        private readonly ITreeWriter treeWriter;

        private readonly IDataNode rootNode;

        private readonly Action<IBinaryWriter> alignFooter;

        public TreeBinarySerializer(
            ITreeReader treeReader,
            ITreeWriter treeWriter,
            IDataNode rootNode,
            Action<IBinaryWriter> footerAligner
        ) {
            Validate.ArgumentNotNull(nameof(treeReader), treeReader);
            Validate.ArgumentNotNull(nameof(treeWriter), treeWriter);
            Validate.ArgumentNotNull(nameof(rootNode), rootNode);
            Validate.ArgumentNotNull(nameof(footerAligner), footerAligner);

            this.treeReader = treeReader;
            this.treeWriter = treeWriter;
            this.rootNode = rootNode;
            alignFooter = footerAligner;
        }

        public T Read(IBinaryReader binaryReader)
        {
            Validate.ArgumentNotNull(nameof(binaryReader), binaryReader);

            return (T)treeReader.Read(binaryReader, rootNode);
        }

        public void Write(IBinaryWriter binaryWriter, T value)
        {
            Validate.ArgumentNotNull(nameof(binaryWriter), binaryWriter);
            Validate.ArgumentNotNull<object>(nameof(value), value);

            IReadOnlyList<uint> offsets = treeWriter.Write(binaryWriter, value, rootNode);

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
            Validate.ArgumentNotNull(nameof(binaryReader), binaryReader);

            T result = constructor();

            result.Read(binaryReader);

            return result;
        }

        public void Write(IBinaryWriter binaryWriter, T value)
        {
            Validate.ArgumentNotNull(nameof(binaryWriter), binaryWriter);
            Validate.ArgumentNotNull<object>(nameof(value), value);

            value.Write(binaryWriter);
        }
    }
    #endregion
}
