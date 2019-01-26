using NUnit.Framework;
using SAGESharp.SLB.Internal;
using SAGESharpTests.Util;
using System.Collections.Generic;

namespace SAGESharpTests.SLB.Internal
{
    class EqualityUtilsTests
    {
        private const string STRING1 = "string1";
        private const string STRING2 = "string2";

        private static readonly int HASH_CODE1 = STRING1.GetHashCode();
        private static readonly int HASH_CODE2 = STRING2.GetHashCode();

        [TestCase(STRING1, STRING1, ExpectedResult = true)]
        [TestCase(STRING1, STRING2, ExpectedResult = false)]
        [TestCase(STRING2, STRING1, ExpectedResult = false)]
        [TestCase(STRING2, STRING2, ExpectedResult = true)]
        [TestCase(STRING1, STRING1, ExpectedResult = true)]
        [TestCase(STRING1, null, ExpectedResult = false)]
        [TestCase(null, STRING1, ExpectedResult = false)]
        [TestCase(null, null, ExpectedResult = true)]
        public bool Test_Is_Equals(string a, string b)
        {
            return a.SafeEquals(b);
        }

        [TestCaseSource(nameof(SingleReferences))]
        public void Test_AddHashCodeByRef_For_A_Single_Reference(string value, int hash, int prime, int expectedHash)
        {
            value.AddHashCodeByRef(ref hash, prime);

            Assert.That(hash, Is.EqualTo(expectedHash));
        }

        static object[] SingleReferences() => new ParameterGroup()
            .Parameters(STRING1, 5, 7, 35 + STRING1.GetHashCode())
            .Parameters(null, 5, 7, 35)
            .Build();

        [TestCaseSource(nameof(MultipleReferences))]
        public void Test_AddHashCodesByRef_For_Multiple_References(IEnumerable<string> values, int hash, int prime, int emptyPrime, int expectedHash)
        {
            values.AddHashCodesByRef(ref hash, prime, emptyPrime);

            Assert.That(hash, Is.EqualTo(expectedHash));
        }

        static object[] MultipleReferences() => new ParameterGroup()
            // values, hash, prime, emptyPrime, expectedHash
            .Parameters(null, 5, 7, 9, 5)
            .Parameters(new List<string>(), 5, 7, 9, 45)
            .Parameters(new List<string> { null }, 5, 7, 9, 35)
            .Parameters(new List<string> { STRING1 }, 5, 7, 9, 35 + HASH_CODE1)
            .Parameters(new List<string> { STRING1, null }, 5, 7, 9, (35 + HASH_CODE1) * 7)
            .Parameters(new List<string> { STRING1, STRING2 }, 5, 7, 9, (35 + HASH_CODE1) * 7 + HASH_CODE2)
            .Parameters(new List<string> { null, STRING1 }, 5, 7, 9, (35 * 7) + HASH_CODE1)
            .Build();
        
        [TestCase(1, 5, 7, 36)]
        public void Test_AddHashCodeByVal_For_A_Single_Value(int value, int hash, int prime, int expectedHash)
        {
            value.AddHashCodeByVal(ref hash, prime);

            Assert.That(hash, Is.EqualTo(expectedHash));
        }

        [TestCaseSource(nameof(MultipleValues))]
        public void Test_AddHashCodesByVal_For_Multiple_Values(IEnumerable<int> values, int hash, int prime, int emptyPrime, int expectedHash)
        {
            values.AddHashCodeByVal(ref hash, prime, emptyPrime);

            Assert.That(hash, Is.EqualTo(expectedHash));
        }

        static object[] MultipleValues() => new ParameterGroup()
            // values, hash, prime, emptyPrime, expectedHash
            .Parameters(null, 5, 7, 9, 5)
            .Parameters(new List<int>(), 5, 7, 9, 45)
            .Parameters(new List<int> { 0 }, 5, 7, 9, 35)
            .Parameters(new List<int> { 1 }, 5, 7, 9, 35 + 1)
            .Parameters(new List<int> { 1, 0 }, 5, 7, 9, (35 + 1) * 7)
            .Parameters(new List<int> { 1, 2 }, 5, 7, 9, (35 + 1) * 7 + 2)
            .Parameters(new List<int> { 0, 1 }, 5, 7, 9, (35 * 7) + 1)
            .Build();
    }
}
