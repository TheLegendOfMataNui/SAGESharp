using System;

namespace SAGESharp.SLB
{
    /// <summary>
    /// Class that represents an identifier within SLB files.
    /// 
    /// The identifier consist of 4 bytes/characters (a 32 bit integer).
    /// </summary>
    public struct Identifier : IEquatable<Identifier>
    {
        /// <summary>
        /// Char that will be shown if any invalid byte is used in the identifier.
        /// </summary>
        public const char EMPY_CHAR = '?';

        /// <summary>
        /// A constant identifier object with value zero.
        /// </summary>
        public static readonly Identifier ZERO = 0;

        private uint value;

        #region Conversions to Identifier
        /// <summary>
        /// Creates an <see cref="Identifier"/> implictly from an integer.
        /// </summary>
        /// 
        /// <param name="value">The integer that will be used to create the identifier.</param>
        public static implicit operator Identifier(int value)
            => new Identifier
            {
                value = (uint)value
            };

        /// <summary>
        /// Creates an <see cref="Identifier"/> from an array of bytes.
        /// </summary>
        /// 
        /// Only the first four elements from the array will be used to
        /// set the bytes of the identifier, if the array is shorter than
        /// four bytes the missing ones are set to zero, if is larger than
        /// four the remaining are ignored.
        /// 
        /// <param name="values">The array of bytes that will be used to create the identifier.</param>
        /// 
        /// <returns>An identifier created from the input values.</returns>
        /// 
        /// <exception cref="ArgumentNullException">If the array of bytes is null.</exception>
        public static Identifier From(byte[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException();
            }

            var result = (Identifier)0;

            result.SetFrom(values);

            return result;
        }

        /// <summary>
        /// Creates an <see cref="Identifier"/> from a string.
        /// </summary>
        /// 
        /// Only the first for characters from the string will be used to
        /// set the bytes of the identifier, if the array is shorter than
        /// four chars the missing bytes are set to zero, if is larger then
        /// four the remaining are ignored.
        /// 
        /// Any character from the string is interpreted as an ASCII value
        /// and that's stored into the identifier.
        /// 
        /// <param name="value">The string that will be used to create the identifier.</param>
        /// 
        /// <returns>An identifier created from the input string.</returns>
        /// 
        /// <exception cref="ArgumentNullException">If the string is null.</exception>
        public static Identifier From(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }

            var result = (Identifier)0;

            result.SetFrom(value);

            return result;
        }
        #endregion

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
        private void SetFrom(byte[] values)
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
        private void SetFrom(string value)
        {
            B0 = (value.Length > 0) ? value[value.Length - 1].ToASCIIByte() : (byte)0;
            B1 = (value.Length > 1) ? value[value.Length - 2].ToASCIIByte() : (byte)0;
            B2 = (value.Length > 2) ? value[value.Length - 3].ToASCIIByte() : (byte)0;
            B3 = (value.Length > 3) ? value[value.Length - 4].ToASCIIByte() : (byte)0;
        }

        #region Copy modifying a byte
        /// <summary>
        /// Creates a copy with the byte 0 set to the input value.
        /// </summary>
        /// 
        /// <param name="value">The new value of the byte 0.</param>
        /// 
        /// <returns>A copy with the byte 0 set to the input value.</returns>
        public Identifier WithB0(byte value)
        {
            var result = new Identifier() { value = this.value };

            result.SetByteValue(0, value);

            return result;
        }

        /// <summary>
        /// Creates a copy with the byte 1 set to the input value.
        /// </summary>
        /// 
        /// <param name="value">The new value of the byte 1.</param>
        /// 
        /// <returns>A copy with the byte 1 set to the input value.</returns>
        public Identifier WithB1(byte value)
        {
            var result = new Identifier() { value = this.value };

            result.SetByteValue(1, value);

            return result;
        }

        /// <summary>
        /// Creates a copy with the byte 2 set to the input value.
        /// </summary>
        /// 
        /// <param name="value">The new value of the byte 2.</param>
        /// 
        /// <returns>A copy with the byte 2 set to the input value.</returns>
        public Identifier WithB2(byte value)
        {
            var result = new Identifier() { value = this.value };

            result.SetByteValue(2, value);

            return result;
        }

        /// <summary>
        /// Creates a copy with the byte 3 set to the input value.
        /// </summary>
        /// 
        /// <param name="value">The new value of the byte 3.</param>
        /// 
        /// <returns>A copy with the byte 3 set to the input value.</returns>
        public Identifier WithB3(byte value)
        {
            var result = new Identifier() { value = this.value };

            result.SetByteValue(3, value);

            return result;
        }
        #endregion

        #region Conversion from Identifier
        /// <summary>
        /// Implicitly converts the <see cref="Identifier"/> to an integer.
        /// </summary>
        /// 
        /// <param name="identifier">The integer value of the identifier.</param>
        public static implicit operator int(Identifier identifier) => (int)identifier.value;

        /// <summary>
        /// Gets the identifier as a (4 character) string.
        /// </summary>
        /// 
        /// <remarks>Byte 3 is the first element of the string, and so on.</remarks>
        /// 
        /// <returns>The identifier as a (4 character) string.</returns>
        public override string ToString()
        {
            return new string(new[] { C3, C2, C1, C0 });
        }
        #endregion

        /// <inheritdoc/>
        public override bool Equals(object other)
        {
            if (!(other is Identifier identifier))
            {
                return false;
            }

            return Equals(identifier);
        }

        /// <inheritdoc/>
        public bool Equals(Identifier other)
        {
            return value == other.value;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        /// <summary>
        /// Returns true if both identifiers are equals, false otherwise.
        /// </summary>
        /// 
        /// <param name="left">The left side of the comparision.</param>
        /// <param name="right">The right side of the comparision.</param>
        /// 
        /// <returns>True if both are equal, false otherwise.</returns>
        public static bool operator ==(Identifier left, Identifier right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns true if both identifiers are not equals, false otherwise.
        /// </summary>
        /// 
        /// <param name="left">The left side of the comparision.</param>
        /// <param name="right">The right side of the comparision.</param>
        /// 
        /// <returns>True if both are not equal, false otherwise.</returns>
        public static bool operator !=(Identifier left, Identifier right)
        {
            return !(left == right);
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
