/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NSubstitute;
using System;
using System.Linq;

namespace SAGESharp.Testing
{
    /// <summary>
    /// Class with extensions to the <see cref="Arg"/> class.
    /// </summary>
    static class ArgX
    {
        /// <summary>
        /// Executes <see cref="Arg.Do{T}(Action{T})"/> with each action
        /// in <paramref name="actions"/> in the order they appear.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// 
        /// <param name="actions">
        /// The list of actions to execute, if an action is null is it ignored.
        /// </param>
        /// 
        /// <returns>The value from <see cref="Arg.Do{T}(Action{T})"/>.</returns>
        public static T OrderedDo<T>(params Action<T>[] actions)
        {
            Validate.ArgumentNotNull(nameof(actions), actions);

            int count = 0;
            return Arg.Do<T>(arg =>
            {
                if (count < actions.Length)
                {
                    actions[count++]?.Invoke(arg);
                }
            });
        }
    }

    /// <summary>
    /// Class with matchers using the <see cref="Arg"/> class.
    /// </summary>
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
