/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NUnit.Framework;
using SAGESharp.Utils;
using System.Collections.Generic;

namespace SAGESharp.Tests.Utils
{
    class ToStringUtilsTests
    {
        [TestCaseSource(nameof(ListToStringTestCases))]
        public void Test_ListToString(ListToStringTestCase testCase)
        {
            string result = ToStringUtils.ListToString(testCase.Value);

            result.Should().Be(testCase.Expected);
        }

        static ListToStringTestCase[] ListToStringTestCases() => new ListToStringTestCase[]
        {
            new ListToStringTestCase(
                value: null,
                expected: string.Empty,
                description: "Test with a null list"
            ),
            new ListToStringTestCase(
                value: new List<int>(),
                expected: "{}",
                description: "Test with an empty list"
            ),
            new ListToStringTestCase(
                value: new List<int> { 44 },
                expected: "{44}",
                description: "Test with a list with one value"
            ),
            new ListToStringTestCase(
                value: new List<int> { 11, 22, 33, 44, 55 },
                expected: "{11, 22, 33, 44, 55}",
                description: "Test with a list woth several values"
            )
        };

        public class ListToStringTestCase : AbstractTestCase
        {
            public ListToStringTestCase(IList<int> value, string expected, string description) : base(description)
            {
                Value = value;
                Expected = expected;
            }

            public IList<int> Value { get; }

            public string Expected { get; }
        }
    }
}
