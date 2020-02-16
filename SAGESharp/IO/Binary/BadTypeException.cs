/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using System;

namespace SAGESharp.IO.Binary
{
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
}
