/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NUnit.Framework;
using SAGESharp.Utils;

namespace SAGESharp.Tests.Utils
{
    class EqualityUtilsTests
    {
        [TestCaseSource(nameof(EqualsOperatorTestCases))]
        public void Test_EqualsOperator(EqualsOperatorTestCase testCase)
        {
            bool result = EqualityUtils.EqualsOperator(left: testCase.Left, right: testCase.Right);

            result.Should().Be(testCase.Expected);
        }

        static EqualsOperatorTestCase[] EqualsOperatorTestCases() => new EqualsOperatorTestCase[]
        {
            new EqualsOperatorTestCase(
                left: "a string",
                right: "a string",
                expected: true,
                description: "Test with two equal objects"
            ),
            new EqualsOperatorTestCase(
                left: "a string",
                right: string.Empty,
                expected: false,
                description: "Test with two inequal objects"
            ),
            new EqualsOperatorTestCase(
                left: "a string",
                right: null,
                expected: false,
                description: "Test with a non null object and a null object"
            ),
            new EqualsOperatorTestCase(
                left: null,
                right: "a string",
                expected: false,
                description: "Test with a null object and a non null object"
            ),
            new EqualsOperatorTestCase(
                left: null,
                right: null,
                expected: true,
                description: "Test with two null objects"
            )
        };

        public class EqualsOperatorTestCase : AbstractTestCase
        {
            public EqualsOperatorTestCase(
                string left,
                string right,
                bool expected,
                string description
            ) : base(description) {
                Left = left;
                Right = right;
                Expected = expected;
            }

            public string Left { get; }

            public string Right { get; }

            public bool Expected { get; }
        }
    }
}
