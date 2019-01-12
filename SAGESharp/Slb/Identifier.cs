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
            Value = 0;
        }

        /// <summary>
        /// Creates a new instance initializing it with the input value.
        /// </summary>
        /// <param name="value">The input value to initalize the instance.</param>
        public Identifier(uint value)
        {
            Value = value;
        }

        /// <summary>
        /// The value of the identifier.
        /// </summary>
        public uint Value { get; set; }

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
                return Value.GetByte(0);
            }
            set
            {
                Value = Value.SetByte(0, value);
            }
        }

        /// <summary>
        /// Byte 1 of the id in numeric form.
        /// </summary>
        public byte B1
        {
            get
            {
                return Value.GetByte(1);
            }
            set
            {
                Value = Value.SetByte(1, value);
            }
        }

        /// <summary>
        /// Byte 2 of the id in numeric form.
        /// </summary>
        public byte B2
        {
            get
            {
                return Value.GetByte(2);
            }
            set
            {
                Value = Value.SetByte(2, value);
            }
        }

        /// <summary>
        /// Byte 3 of the id in numeric form.
        /// </summary>
        public byte B3
        {
            get
            {
                return Value.GetByte(3);
            }
            set
            {
                Value = Value.SetByte(3, value);
            }
        }
        #endregion

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
            var result = Value.GetByte(b);

            if (!result.IsASCIIDigit() && !result.IsASCIILowercaseLetter() && !result.IsASCIIUppercaseLetter())
            {
                return EMPY_CHAR;
            }

            return result.ToASCIIChar();
        }

        private void SetByteValue(byte b, char value)
        {
            Value = Value.SetByte(b, value.ToASCIIByte());
        }
    }
}
