/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using FluentAssertions.Specialized;
using System;

namespace SAGESharp.Tests
{
    /// <summary>
    /// Class which contains extensions to the FluentAssertions library.
    /// </summary>
    static class Assertions
    {
        /// <summary>
        /// Asserts that a validation for a null argument happened.
        /// </summary>
        /// 
        /// <param name="actionAssertions">A reference to the method or property.</param>
        /// <param name="argumentName">The name of the argument to be verified.</param>
        public static ExceptionAssertions<ArgumentNullException> ThrowArgumentNullException(this ActionAssertions actionAssertions, string argumentName)
            => actionAssertions
                .ThrowExactly<ArgumentNullException>()
                .WithMessage($"*{argumentName}*");
    }
}
