/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO;
using System;

namespace SAGESharp.Tests
{
    /// <summary>
    /// Class to be used when testing instances of <see cref="IBinarySerializer{T}"/>.
    /// </summary>
    /// 
    /// <typeparam name="T">The type to read.</typeparam>
    class SerializationTestCaseData<T> : AbstractTestCaseData
    {
        private readonly Func<T> expectedProvider;

        /// <summary>
        /// Creates a new instance of the test case data.
        /// </summary>
        /// 
        /// <param name="description">The description of the test case.</param>
        /// <param name="testFilePath">The full path where the file to use in the test is located.</param>
        /// <param name="expectedProvider">A function to provide the expected instance of <typeparamref name="T"/>.</param>
        public SerializationTestCaseData(string description, string testFilePath, Func<T> expectedProvider)
            : base(description)
        {
            TestFilePath = testFilePath;
            this.expectedProvider = expectedProvider;
        }

        /// <summary>
        /// The full path to the file to use in the test.
        /// </summary>
        public string TestFilePath { get; }

        /// <summary>
        /// The <see cref="TestFilePath"/> with the SLB extension appended at the end.
        /// </summary>
        public string SLBFilePath { get => $"{TestFilePath}.slb"; }

        /// <summary>
        /// The <see cref="TestFilePath"/> with the Yaml extension appended at the end.
        /// </summary>
        public string YamlFilePath { get => $"{TestFilePath}.yaml"; }

        /// <summary>
        /// The object that is expected to be read or write in the test.
        /// </summary>
        public T Expected { get => expectedProvider(); }
    }
}
