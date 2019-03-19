using System;
using System.IO;

namespace SAGESharp.SLB.IO
{
    /// <summary>
    /// Interface to read chunks of binary data as numbers.
    /// </summary>
    public interface IBinaryReader
    {
        /// <summary>
        /// Pointer position of the underlying binary data.
        /// </summary>
        long Position { get; set; }

        /// <summary>
        /// Reads a single byte.
        /// </summary>
        /// 
        /// <returns>A single byte read.</returns>
        byte ReadByte();

        /// <summary>
        /// Reads "<paramref name="count"/>" bytes and returns them as an array.
        /// </summary>
        /// 
        /// <param name="count">The count of bytes to read.</param>
        /// 
        /// <returns>A byte array with the "<paramref name="count"/>" of bytes.</returns>
        byte[] ReadBytes(int count);

        /// <summary>
        /// Reads a <see cref="short"/> number.
        /// </summary>
        /// 
        /// <returns>A <see cref="short"/> number.</returns>
        short ReadInt16();

        /// <summary>
        /// Reads an <see cref="ushort"/> number.
        /// </summary>
        /// 
        /// <returns>An <see cref="ushort"/> number.</returns>
        ushort ReadUInt16();

        /// <summary>
        /// Reads an <see cref="int"/> number.
        /// </summary>
        /// 
        /// <returns>An <see cref="int"/> number.</returns>
        int ReadInt32();

        /// <summary>
        /// Reads an <see cref="uint"/> number.
        /// </summary>
        /// 
        /// <returns>An <see cref="uint"/> number.</returns>
        uint ReadUInt32();

        /// <summary>
        /// Reads a <see cref="float"/> number.
        /// </summary>
        /// 
        /// <returns>A <see cref="float"/> number.</returns>
        float ReadFloat();

        /// <summary>
        /// Reads a <see cref="double"/> number.
        /// </summary>
        /// 
        /// <returns>A <see cref="double"/> number.</returns>
        double ReadDouble();
    }

    /// <summary>
    /// Static class to function as a simple factory for <see cref="IBinaryReader"/> instances.
    /// </summary>
    public static class Reader
    {
        /// <summary>
        /// Gets a <see cref="IBinaryReader"/> for the intpu <paramref name="stream"/>.
        /// </summary>
        /// 
        /// <param name="stream">The input stream to use in the reader.</param>
        /// 
        /// <returns>A <see cref="IBinaryReader"/> to read the input <paramref name="stream"/>.</returns>
        public static IBinaryReader ForStream(Stream stream)
            => new StreamBinaryReader(stream);
    }

    internal sealed class StreamBinaryReader : IBinaryReader
    {
        private readonly Stream stream;

        public StreamBinaryReader(Stream stream)
            => this.stream = stream ?? throw new ArgumentNullException();

        public long Position
        {
            get => stream.Position;
            set => stream.Position = value;
        }

        public byte ReadByte() => ReadBytes(1)[0];

        public byte[] ReadBytes(int count)
        {
            if (count == 0)
            {
                return Array.Empty<byte>();
            }

            var buffer = new byte[count];
            if (stream.Read(buffer, 0, count) == count)
            {
                return buffer;
            }
            else
            {
                throw new EndOfStreamException();
            }
        }

        public short ReadInt16()
            => BitConverter.ToInt16(ReadBytes(2), 0);

        public ushort ReadUInt16()
            => BitConverter.ToUInt16(ReadBytes(2), 0);

        public int ReadInt32()
            => BitConverter.ToInt32(ReadBytes(4), 0);

        public uint ReadUInt32()
            => BitConverter.ToUInt32(ReadBytes(4), 0);

        public float ReadFloat()
            => BitConverter.ToSingle(ReadBytes(4), 0);

        public double ReadDouble()
            => BitConverter.ToDouble(ReadBytes(8), 0);
    }

    internal static class IBinaryReaderExtensions
    {
        /// <summary>
        /// Executes <paramref name="action"/> while temporarily moving the reader to <paramref name= "position" />.
        /// </summary>
        /// 
        /// <param name="reader">The reader that w</param>
        /// <param name="position">The position where the reader will be moved temporarily.</param>
        /// <param name="action">The action to execute.</param>
        public static void OnPositionDo(this IBinaryReader reader, uint position, Action action)
        {
            var originalPosition = reader.Position;
            reader.Position = position;

            action();
            reader.Position = originalPosition;
        }

        /// <summary>
        /// Returns the result of <paramref name="function"/> while temporarily moving the reader to <paramref name= "position" />.
        /// </summary>
        /// 
        /// <typeparam name="TResult">The type that will be returned.</typeparam>
        /// 
        /// <param name="reader">The reader that will be changed temproarily.</param>
        /// <param name="position">The position where the reader will be moved temporarily.</param>
        /// <param name="function">The action to execute.</param>
        /// 
        /// <returns>The result of <paramref name="function"/>.</returns>
        public static TResult OnPositionDo<TResult>(this IBinaryReader reader, uint position, Func<TResult> function)
        {
            var originalPosition = reader.Position;
            reader.Position = position;

            var result = function();
            reader.Position = originalPosition;

            return result;
        }
    }
}
