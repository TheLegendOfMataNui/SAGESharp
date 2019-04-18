/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NSubstitute;
using System.Linq;

namespace SAGESharp.Testing
{
    static class Matcher
    {
        /// <summary>
        /// Method to match a byte array.
        /// </summary>
        /// 
        /// <param name="expected">The expected match array.</param>
        /// 
        /// <example>
        /// <code>
        /// var substitute = Substitute.For<MyClass>();
        /// 
        /// myClass.SomeMethod(Matcher.ForEquivalentArray(new byte[] { 1, 2, 3 }));
        /// </code>
        /// </example>
        /// 
        /// <returns>The matched array.</returns>
        public static byte[] ForEquivalentArray(byte[] expected)
            => Arg.Is<byte[]>(actual => CompareByteArrays(expected, actual));

        private static bool CompareByteArrays(byte[] expected, byte[] actual)
            => expected?.SequenceEqual(actual) ?? actual == null;
    }
}
