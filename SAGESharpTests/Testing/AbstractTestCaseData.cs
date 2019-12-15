/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using NUtils.Extensions;

namespace SAGESharp.Testing
{
    /// <summary>
    /// Convenience class to use when writing custom data for <see cref="TestCaseAttribute"/> and <see cref="TestCaseSourceAttribute"/>.
    /// </summary>
    abstract class AbstractTestCaseData
    {
        private readonly string description;

        /// <summary>
        /// Initialize the test case data with the given <paramref name="description"/>.
        /// </summary>
        /// 
        /// <param name="description">The description of the test case data.</param>
        protected AbstractTestCaseData(string description)
          => this.description = description
                .Trim()
                .TakeUnless(s => s.Length == 0)
                ?? base.ToString();

        /// <summary>
        /// Returns a string to name the test case data.
        /// </summary>
        /// 
        /// <returns>A string to name the test case data.</returns>
        public override string ToString() => description;
    }
}
