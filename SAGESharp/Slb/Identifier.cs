using System;

namespace SAGESharp.Slb
{
    /// <summary>
    /// Class that represents an identifier within SLB files.
    /// 
    /// The identifier consist of 4 bytes/characters (a 32 bit integer).
    /// </summary>
    public struct Identifier
    {
        /// <summary>
        /// Char that will be shown if any invalid byte is used in the identifier.
        /// </summary>
        public const char EMPY_CHAR = '?';

        /// <summary>
        /// Creates a new instance initializing it with the input value.
        /// </summary>
        /// <param name="value">The input value to initalize the instance.</param>
        public Identifier(uint value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The value of the identifier.
        /// </summary>
        public uint Value
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #region Byte level access
        /// <summary>
        /// Byte 0 of the id in character form.
        /// </summary>
        public char C0
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Byte 1 of the id in chracter form.
        /// </summary>
        public char C1
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Byte 2 of the id in chracter form.
        /// </summary>
        public char C2
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Byte 3 of the id in chracter form.
        /// </summary>
        public char C3
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Byte 0 of the id in numeric form.
        /// </summary>
        public byte B0
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Byte 1 of the id in numeric form.
        /// </summary>
        public byte B1
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Byte 2 of the id in numeric form.
        /// </summary>
        public byte B2
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Byte 3 of the id in numeric form.
        /// </summary>
        public byte B3
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
