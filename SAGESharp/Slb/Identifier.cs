using SAGESharp.Extensions;

namespace SAGESharp.Slb
{
    /// <summary>
    /// Class that represents an identifier within SLB files.
    /// 
    /// The identifier consist of 4 bytes/characters (a 32 bit integer).
    /// </summary>
    public class Identifier
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
        public static explicit operator Identifier(uint value)
        {
            return new Identifier
            {
                value = value
            };
        }

        /// <summary>
        /// Creates a new instance initialize it with the input values.
        /// </summary>
        /// 
        /// <param name="values">An array of bytes that will be used to initialize the identifier.</param>
        public static explicit operator Identifier(byte[] values)
        {
            return new Identifier
            {
                B0 = (values.Length > 0) ? values[0] : (byte)0,
                B1 = (values.Length > 1) ? values[1] : (byte)0,
                B2 = (values.Length > 2) ? values[2] : (byte)0,
                B3 = (values.Length > 3) ? values[3] : (byte)0,
            };
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
            return new string(new[] { C3, C2, C1, C0 });
        }

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
