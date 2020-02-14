/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using Equ;
using NUtils.Validations;
using SAGESharp.IO;
using SAGESharp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAGESharp.SLB
{
    /// <summary>
    /// Class that represents an identifier within SLB files.
    /// 
    /// The identifier consist of 4 bytes/characters (a 32 bit integer).
    /// </summary>
    public struct Identifier : IEquatable<Identifier>
    {
        private const char EscapeCharacter = '|';

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
            => new Identifier { value = (uint)value };

        /// <summary>
        /// Creates an <see cref="Identifier"/> implictly from an unsigned integer.
        /// </summary>
        /// 
        /// <param name="value">The unsigned integer that will be used to create the identifier.</param>
        public static implicit operator Identifier(uint value)
            => new Identifier { value = value };

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
            Validate.ArgumentNotNull(values, nameof(values));

            Identifier result = new Identifier
            {
                value = values.ElementAtOrDefault(0)
            };

            result.SetByteValue(1, values.ElementAtOrDefault(1));
            result.SetByteValue(2, values.ElementAtOrDefault(2));
            result.SetByteValue(3, values.ElementAtOrDefault(3));

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
            Validate.ArgumentNotNull(value, nameof(value));

            IEnumerable<char> chars = value.Reverse();
            Identifier result = new Identifier
            {
                value = chars.ElementAtOrDefault(0).ToASCIIByte()
            };

            result.SetByteValue(1, chars.ElementAtOrDefault(1).ToASCIIByte());
            result.SetByteValue(2, chars.ElementAtOrDefault(2).ToASCIIByte());
            result.SetByteValue(3, chars.ElementAtOrDefault(3).ToASCIIByte());

            return result;
        }
        #endregion

        #region Access individual bytes as chars
        /// <summary>
        /// Byte 0 of the id in character form.
        /// </summary>
        public char C0
        {
            get
            {
                return GetReadableByte(0);
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
        }
        #endregion

        #region Access individual bytes
        /// <summary>
        /// Byte 0 of the id in numeric form.
        /// </summary>
        public byte B0
        {
            get
            {
                return value.GetByte(0);
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
        }
        #endregion

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

        #region Copy modifying a byte as a char
        /// <summary>
        /// Creates a copy with the byte 0 set to the ASCII value of the input character.
        /// </summary>
        /// 
        /// <param name="value">The new value of the byte 0 as an ASCII character.</param>
        /// 
        /// <returns>A copy with the byte 0 set to the input ASCII character.</returns>
        public Identifier WithC0(char value) => WithB0(value.ToASCIIByte());

        /// <summary>
        /// Creates a copy with the byte 1 set to the ASCII value of the input character.
        /// </summary>
        /// 
        /// <param name="value">The new value of the byte 1 as an ASCII character.</param>
        /// 
        /// <returns>A copy with the byte 1 set to the input ASCII character.</returns>
        public Identifier WithC1(char value) => WithB1(value.ToASCIIByte());

        /// <summary>
        /// Creates a copy with the byte 2 set to the ASCII value of the input character.
        /// </summary>
        /// 
        /// <param name="value">The new value of the byte 2 as an ASCII character.</param>
        /// 
        /// <returns>A copy with the byte 0 set to the input ASCII character.</returns>
        public Identifier WithC2(char value) => WithB2(value.ToASCIIByte());

        /// <summary>
        /// Creates a copy with the byte 3 set to the ASCII value of the input character.
        /// </summary>
        /// 
        /// <param name="value">The new value of the byte 3 as an ASCII character.</param>
        /// 
        /// <returns>A copy with the byte 3 set to the input ASCII character.</returns>
        public Identifier WithC3(char value) => WithB3(value.ToASCIIByte());
        #endregion

        #region Conversion from Identifier
        /// <summary>
        /// Implicitly converts the <see cref="Identifier"/> to an integer.
        /// </summary>
        /// 
        /// <param name="identifier">The integer value of the identifier.</param>
        public static implicit operator int(Identifier identifier) => (int)identifier.value;

        /// <summary>
        /// Implicitly converts the <see cref="Identifier"/> to an unsigned integer.
        /// </summary>
        /// 
        /// <param name="identifier">The unsigned integer value of the identifier.</param>
        public static implicit operator uint(Identifier identifier) => identifier.value;

        /// <summary>
        /// Gets the identifier as a string. Each byte in the identifier is converted
        /// to an ASCII character (if possible), if not it is scaped between horizontal
        /// bars.
        /// </summary>
        /// 
        /// <remarks>Byte 3 is the first element of the string, and so on.</remarks>
        /// 
        /// <returns>The identifier as a string.</returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            void appendByte(byte @byte)
            {
                // Any ASCII character before 0x20 (space)
                // and after 0x7A (z) are going to be printed
                // escaped (as hexadecimal between EscapeCharacter
                if (@byte < 0x20 || @byte > 0x7A)
                {
                    stringBuilder.Append(EscapeCharacter);
                    stringBuilder.AppendFormat("0x{0:X2}", @byte);
                    stringBuilder.Append(EscapeCharacter);
                }
                else
                {
                    stringBuilder.Append(@byte.ToASCIIChar());
                }
            }

            appendByte(B3);
            appendByte(B2);
            appendByte(B1);
            appendByte(B0);

            return stringBuilder.ToString();
        }
        #endregion

        #region Equality
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

        private void SetByteValue(byte bytePosition, byte value)
        {
            this.value = this.value.SetByte(bytePosition, value);
        }
    }

    public sealed class Point3D : IEquatable<Point3D>
    {
        [SerializableProperty(1)]
        public float X { get; set; }

        [SerializableProperty(2)]
        public float Y { get; set; }

        [SerializableProperty(3)]
        public float Z { get; set; }

        public bool Equals(Point3D other)
            => MemberwiseEqualityComparer<Point3D>.ByProperties.Equals(this, other);

        public override string ToString()
            => $"X={X}, Y={Y}, Z={Z}";

        public override bool Equals(object other)
            => Equals(other as Point3D);

        public override int GetHashCode()
            => MemberwiseEqualityComparer<Point3D>.ByProperties.GetHashCode(this);

        public static bool operator ==(Point3D left, Point3D right)
            => left?.Equals(right) ?? right?.Equals(left) ?? true;

        public static bool operator !=(Point3D left, Point3D right)
            => !(left == right);
    }
}
