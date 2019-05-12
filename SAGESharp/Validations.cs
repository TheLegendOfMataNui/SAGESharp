/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using System;

namespace SAGESharp
{
    internal static class Validate
    {
        /// <summary>
        /// Validates that <paramref name="argument"/> is not null,
        /// or else throws an <see cref="ArgumentNullException"/> with the given <paramref name="argumentName"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the argument to check.</typeparam>
        /// 
        /// <param name="argumentName">The name of the argument to check.</param>
        /// <param name="argument">The argument to check.</param>
        public static void ArgumentNotNull<T>(string argumentName, T argument) where T : class
        {
            if (argument is null)
            {
                throw new ArgumentNullException($"\"{argumentName}\" cannot be null.");
            }
        }
    }
}
