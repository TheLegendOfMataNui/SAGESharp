using SAGESharp.Extensions;
using System.IO;

namespace SAGESharp.Slb
{
    /// <summary>
    /// Class that represents an identifier within SLB files.
    /// 
    /// The identifier consist of 4 bytes/characters (a 32 bit integer).
    /// </summary>
    public class Identifier : ISlbObject
    {
        /// <summary>
        /// Char that will be shown if any invalid byte is used in the identifier.
        /// </summary>
        public const char EMPY_CHAR = '?';

        /// <summary>
        /// Creates a new instance with the value initialized to zero.
        /// </summary>
        public Identifier()
        {
        }

        /// <summary>
        /// Creates a new instance initializing it with the input value.
        /// </summary>
        /// 
        /// <param name="value">The input value to initalize the instance.</param>
        public Identifier(uint value)
        {
            SetFrom(value);
        }

        /// <summary>
        /// Creates a new instance initialize it with the input values.
        /// 
        /// If the byte array is shorter than 4, the rest of values will be set to zero.
        /// If the byte array is bigger than 4, the leftover bytes will be ignored.
        /// </summary>
        /// 
        /// <param name="values">An array of bytes that will be used to initialize the identifier.</param>
        public Identifier(byte[] values)
        {
            SetFrom(values);
        }

        /// <summary>
        /// Creates a new instance initialize it with the input value.
        /// 
        /// If the string is shorter than 4, the rest of values will be set to zero.
        /// If the string is bigger than 4, the leftover characters will be ignored.
        /// </summary>
        /// 
        /// <param name="value">A string that will be used to initialize the identifier.</param>
        public Identifier(string value)
        {
            SetFrom(value);
        }

        private uint value = 0;

        #region Byte level access
        /// <summary>
        /// Byte 0 of the id in character form.
        /// </summary>
        public char C0
        {
            get
            {
                return GetReadableByte(0);
            }
            set
            {
                SetByteValue(0, value);
            }
        }

        /// <summary>
        /// Byte 1 of the id in chracter form.
        /// </summary>
        public char C1
        {
            get
            {
                return GetReadableByte(1);
            }
            set
            {
                SetByteValue(1, value);
            }
        }

        /// <summary>
        /// Byte 2 of the id in chracter form.
        /// </summary>
        public char C2
        {
            get
            {
                return GetReadableByte(2);
            }
            set
            {
                SetByteValue(2, value);
            }
        }

        /// <summary>
        /// Byte 3 of the id in chracter form.
        /// </summary>
        public char C3
        {
            get
            {
                return GetReadableByte(3);
            }
            set
            {
                SetByteValue(3, value);
            }
        }

        /// <summary>
        /// Byte 0 of the id in numeric form.
        /// </summary>
        public byte B0
        {
            get
            {
                return value.GetByte(0);
            }
            set
            {
                SetByteValue(0, value);
            }
        }

        /// <summary>
        /// Byte 1 of the id in numeric form.
        /// </summary>
        public byte B1
        {
            get
            {
                return value.GetByte(1);
            }
            set
            {
                SetByteValue(1, value);
            }
        }

        /// <summary>
        /// Byte 2 of the id in numeric form.
        /// </summary>
        public byte B2
        {
            get
            {
                return value.GetByte(2);
            }
            set
            {
                SetByteValue(2, value);
            }
        }

        /// <summary>
        /// Byte 3 of the id in numeric form.
        /// </summary>
        public byte B3
        {
            get
            {
                return value.GetByte(3);
            }
            set
            {
                SetByteValue(3, value);
            }
        }
        #endregion

        /// <summary>
        /// Set the value of the identifier to the input integer.
        /// </summary>
        /// 
        /// <param name="value">The input integer that will be used to set the value of the identifier.</param>
        public void SetFrom(uint value)
        {
            this.value = value;
        }

        /// <summary>
        /// Sets the value of the identifier to the input array of bytes.
        /// 
        /// If the byte array is shorter than 4, the rest of values will be set to zero.
        /// If the byte array is bigger than 4, the leftover bytes will be ignored.
        /// </summary>
        /// 
        /// <param name="values">An array of bytes that will be used to set the value of the identifier.</param>
        public void SetFrom(byte[] values)
        {
            B0 = (values.Length > 0) ? values[0] : (byte)0;
            B1 = (values.Length > 1) ? values[1] : (byte)0;
            B2 = (values.Length > 2) ? values[2] : (byte)0;
            B3 = (values.Length > 3) ? values[3] : (byte)0;
        }

        /// <summary>
        /// Sets the value of the identifier to the input string.
        /// 
        /// If the string is shorter than 4, the rest of values will be set to zero.
        /// If the string is bigger than 4, the leftover characters will be ignored.
        /// </summary>
        /// 
        /// <param name="value">A string that will be used to set the value of the identifier.</param>
        public void SetFrom(string value)
        {
            B0 = (value.Length > 0) ? value[0].ToASCIIByte() : (byte)0;
            B1 = (value.Length > 1) ? value[1].ToASCIIByte() : (byte)0;
            B2 = (value.Length > 2) ? value[2].ToASCIIByte() : (byte)0;
            B3 = (value.Length > 3) ? value[3].ToASCIIByte() : (byte)0;
        }

        /// <summary>
        /// Gets the identifier as an unsigned (32 bit) integer.
        /// </summary>
        /// 
        /// <returns>The identifer as an unsigned (32 bit) integer.</returns>
        public uint ToInteger()
        {
            return value;
        }

        /// <summary>
        /// Gets the identifier as a (4 character) string.
        /// </summary>
        /// 
        /// <returns>The identifier as a (4 character) string.</returns>
        public override string ToString()
        {
            return new string(new[] { C0, C1, C2, C3 });
        }

        #region ISlb
        /// <inheritdoc/>
        public void ReadFrom(Stream stream)
        {
            byte readByteFromStream()
            {
                int value = stream.ReadByte();
                if (value == -1)
                {
                    throw new EndOfStreamException();
                }

                return (byte)value;
            }

            var b0 = readByteFromStream();
            var b1 = readByteFromStream();
            var b2 = readByteFromStream();
            var b3 = readByteFromStream();

            // First read everything then write everything
            // to prevent modifying the Identifier unless
            // it has been completely read
            B0 = b0;
            B1 = b1;
            B2 = b2;
            B3 = b3;
        }

        /// <inheritdoc/>
        public void WriteTo(Stream stream)
        {
            stream.WriteByte(B0);
            stream.WriteByte(B1);
            stream.WriteByte(B2);
            stream.WriteByte(B3);
        }
        #endregion

        private char GetReadableByte(byte b)
        {
            var result = value.GetByte(b);

            if (!result.IsASCIIDigit() && !result.IsASCIILowercaseLetter() && !result.IsASCIIUppercaseLetter())
            {
                return EMPY_CHAR;
            }

            return result.ToASCIIChar();
        }

        private void SetByteValue(byte bytePosition, char value)
        {
            SetByteValue(bytePosition, value.ToASCIIByte());
        }

        private void SetByteValue(byte bytePosition, byte value)
        {
            this.value = this.value.SetByte(bytePosition, value);
        }
    }
}
