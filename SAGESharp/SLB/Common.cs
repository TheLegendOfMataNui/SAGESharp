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
using System.Text;
using SharpDX;
using System.ComponentModel;

namespace SAGESharp.SLB
{
    /// <summary>
    /// Immutable struct that represents an identifier within SLB files.
    /// 
    /// The identifier consist of 4 bytes/characters (a 32 bit integer).
    /// </summary>
    public struct Identifier : IEquatable<Identifier>
    {
        private const char EscapeCharacter = '|';

        /// <summary>
        /// A constant identifier object with value zero.
        /// </summary>
        public static readonly Identifier Zero = 0;

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
        /// Creates an <see cref="Identifier"/> from an array of bytes,
        /// which needs to be exactly four bytes long.
        /// </summary>
        /// 
        /// <param name="values">
        /// The array of bytes that will be used to create the identifier.
        /// </param>
        /// 
        /// <returns>An identifier created from the input values.</returns>
        /// 
        /// <exception cref="ArgumentNullException">If the array of bytes is null.</exception>
        /// <exception cref="ArgumentException">If the array is not four bytes long.</exception>
        public static Identifier From(byte[] values)
        {
            Validate.ArgumentNotNull(values, nameof(values));
            Validate.Argument(values.Length == 4, "Input is not 4 bytes long.");

            Identifier result = new Identifier
            {
                value = values[0]
            };

            result.SetByteValue(1, values[1]);
            result.SetByteValue(2, values[2]);
            result.SetByteValue(3, values[3]);

            return result;
        }

        /// <summary>
        /// Creates an <see cref="Identifier"/> from a string.
        /// </summary>
        /// 
        /// <param name="value">The string that will be used to create the identifier.</param>
        /// 
        /// <returns>An identifier created from the input string.</returns>
        /// 
        /// <exception cref="ArgumentNullException">If the string is null.</exception>
        /// <exception cref="ArgumentException">If the input <paramref name="value"/> is ill formed.</exception>
        public static Identifier From(string value)
        {
            Validate.ArgumentNotNull(value, nameof(value));

            byte[] bytes = new byte[sizeof(uint)];
            int n = bytes.Length;
            bool escaping = false;
            string escapedValue = string.Empty;
            foreach (char @char in value)
            {
                if (!escaping)
                {
                    if (@char != EscapeCharacter)
                    {
                        try
                        {
                            bytes[--n] = @char.ToASCIIByte();
                        }
                        catch (IndexOutOfRangeException exception)
                        {
                            throw new ArgumentException($"\"{value}\" is not a valid Identifier.", exception);
                        }
                    }
                    else
                    {
                        escaping = true;
                        escapedValue = string.Empty;
                    }
                }
                else
                {
                    if (@char != EscapeCharacter)
                    {
                        escapedValue += @char;
                    }
                    else
                    {
                        bool isHex = escapedValue.StartsWith("0x");
                        if (isHex)
                        {
                            escapedValue = escapedValue.Replace("0x", string.Empty);
                        }

                        try
                        {
                            bytes[--n] = Convert.ToByte(escapedValue, isHex ? 16 : 10);
                        }
                        catch (Exception exception)
                        {
                            throw new ArgumentException($"\"{value}\" is not a valid Identifier.", exception);
                        }

                        escaping = false;
                    }
                }
            }

            if (escaping || n != 0)
            {
                throw new ArgumentException($"\"{value}\" is not a valid Identifier.");
            }

            return From(bytes);
        }
        #endregion

        #region Access individual bytes as chars
        /// <summary>
        /// Byte 0 of the id in ASCII character form.
        /// </summary>
        public char C0
        {
            get => B0.ToASCIIChar();
        }

        /// <summary>
        /// Byte 1 of the id in ASCII character form.
        /// </summary>
        public char C1
        {
            get => B1.ToASCIIChar();
        }

        /// <summary>
        /// Byte 2 of the id in ASCII character form.
        /// </summary>
        public char C2
        {
            get => B2.ToASCIIChar();
        }

        /// <summary>
        /// Byte 3 of the id in ASCII character form.
        /// </summary>
        public char C3
        {
            get => B3.ToASCIIChar();
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
                return value.GetByte(3);
            }
        }

        /// <summary>
        /// Byte 1 of the id in numeric form.
        /// </summary>
        public byte B1
        {
            get
            {
                return value.GetByte(2);
            }
        }

        /// <summary>
        /// Byte 2 of the id in numeric form.
        /// </summary>
        public byte B2
        {
            get
            {
                return value.GetByte(1);
            }
        }

        /// <summary>
        /// Byte 3 of the id in numeric form.
        /// </summary>
        public byte B3
        {
            get
            {
                return value.GetByte(0);
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

            result.SetByteValue(3, value);

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

            result.SetByteValue(2, value);

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

            result.SetByteValue(1, value);

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

            result.SetByteValue(0, value);

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

            appendByte(B0);
            appendByte(B1);
            appendByte(B2);
            appendByte(B3);

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

        private void SetByteValue(byte bytePosition, byte value)
        {
            this.value = this.value.SetByte(bytePosition, value);
        }
    }

    public sealed class Point3D : IEquatable<Point3D>, INotifyPropertyChanged
    {
        private float x;
        [SerializableProperty(1)]
        public float X
        {
            get => x;
            set
            {
                x = value;
                RaisePropertyChanged(nameof(X));
            }
        }

        private float y;
        [SerializableProperty(2)]
        public float Y
        {
            get => y;
            set
            {
                y = value;
                RaisePropertyChanged(nameof(Y));
            }
        }

        private float z;
        [SerializableProperty(3)]
        public float Z
        {
            get => z;
            set
            {
                z = value;
                RaisePropertyChanged(nameof(Z));
            }
        }

        public Point3D() : this(0.0f, 0.0f, 0.0f)
        {
            
        }

        public Point3D(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public override string ToString()
            => $"X={X}, Y={Y}, Z={Z}";

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Conversion to & from Vector3
        public static implicit operator Vector3(Point3D point) => new Vector3(point.X, point.Y, point.Z);

        public static implicit operator Point3D(Vector3 vector3) => new Point3D(vector3.X, vector3.Y, vector3.Z);
        #endregion

        #region Equality
        public bool Equals(Point3D other)
            => MemberwiseEqualityComparer<Point3D>.ByProperties.Equals(this, other);

        public override bool Equals(object other)
            => Equals(other as Point3D);

        public override int GetHashCode()
            => MemberwiseEqualityComparer<Point3D>.ByProperties.GetHashCode(this);

        public static bool operator ==(Point3D left, Point3D right)
            => left?.Equals(right) ?? right?.Equals(left) ?? true;

        public static bool operator !=(Point3D left, Point3D right)
            => !(left == right);
        #endregion
    }
}
