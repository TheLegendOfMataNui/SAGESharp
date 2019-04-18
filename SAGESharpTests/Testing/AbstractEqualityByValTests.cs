/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUnit.Framework;
using System;

namespace SAGESharp.Testing
{
    /// <summary>
    /// Class to facilitate testing the equality methods of a struct.
    /// </summary>
    /// 
    /// This class provides all the logic to test for equality and the user
    /// only needs to provide: how to perform the operator comparisions and
    /// any special cases to compare not equal objects.
    /// 
    /// <remarks>
    /// You need to write the tests to compare different objects yourself,
    /// for details <see cref="TestCompareNonEqualsDefaultValueAndNonDefaultValue(T)"/>
    /// and <see cref="TestCompareNonEqualsNonDefaultValues(T, T)"/>.
    /// </remarks>
    /// 
    /// <typeparam name="T">The class to test.</typeparam>
    abstract class AbstractEqualityByValTests<T> where T : struct, IEquatable<T>
    {
        /// <summary>
        /// Method to call equals operator on the input values.
        /// </summary>
        /// 
        /// Given a generic cannot call the custom equals operator (==)
        /// implementation, the concrete class must provide a way to call
        /// it.
        /// 
        /// <param name="left">The left side of the equals operator.</param>
        /// <param name="right">The right side of the equals operator.</param>
        /// 
        /// <returns>The result of <code>left == right</code>.</returns>
        abstract protected bool EqualsOperator(T left, T right);

        /// <summary>
        /// Method to call not equals operator on the input values.
        /// </summary>
        /// 
        /// Given a generic cannot call the custom not equals operator (!=)
        /// implementation, the concrete class must provide a way to call
        /// it.
        /// 
        /// <param name="left">The left side of the not equals operator.</param>
        /// <param name="right">The right side of the not equals operator.</param>
        /// 
        /// <returns>The result of <code>left != right</code>.</returns>
        abstract protected bool NotEqualsOperator(T left, T right);

        /// <summary>
        /// Tests an instance of <see cref="T"/> is always equals to itself.
        /// </summary>
        [Test]
        public void Test_Compare_Default_Object_With_Itself()
        {
            var obj = default(T);

            AssertSameHashCode(obj, obj);
            AssertAreEqual(obj, obj);
        }

        /// <summary>
        /// Tests two different instances of <see cref="T"/> are always equals.
        /// </summary>
        [Test]
        public void Test_Compare_Two_Default_Objects()
        {
            var a = default(T);
            var b = default(T);

            AssertSameHashCode(a, b);
            AssertAreEqual(a, b);
        }

        /// <summary>
        /// Tests a default value is not equals to the input value.
        /// </summary>
        /// 
        /// <param name="value">The input value different than the default value.</param>
        protected void TestCompareNonEqualsDefaultValueAndNonDefaultValue(T value)
        {
            TestCompareNonEqualsNonDefaultValues(default(T), value);
        }

        /// <summary>
        /// Tests two values are not equals.
        /// </summary>
        /// 
        /// <param name="a">An input value.</param>
        /// <param name="b">Another input value.</param>
        protected void TestCompareNonEqualsNonDefaultValues(T a, T b)
        {
            AssertDifferentHashCode(a, b);
            AssertAreNotEqual(a, b);

            AssertDifferentHashCode(b, a);
            AssertAreNotEqual(b, a);
        }

        private void AssertSameHashCode(T a, T b)
        {
            Assert.That(a.GetHashCode() == b.GetHashCode(), Is.True, "{0} should have the same hash code of {1}", a, b);
        }

        private void AssertAreEqual(T a, T b)
        {
            Assert.That((a as IEquatable<T>).Equals(b), Is.True, "{0} should be equals to {1}", a, b);
            Assert.That((a as object).Equals(b), Is.True, "{0} should be equals to {1}", a, b);
            Assert.That(EqualsOperator(a, b), Is.True, "{0} should be equals to {1}", a, b);
            Assert.That(NotEqualsOperator(a, b), Is.False, "{0} should be equals to {1}", a, b);
        }

        private void AssertDifferentHashCode(T a, T b)
        {
            Assert.That(a.GetHashCode() != b.GetHashCode(), Is.True, "{0} should have different hash code to {1}", a, b);
        }

        private void AssertAreNotEqual(T a, T b)
        {
            Assert.That((a as IEquatable<T>).Equals(b), Is.False, "{0} shouldn't be equals to {1}", a, b);
            Assert.That((a as object).Equals(b), Is.False, "{0} shouldn't be equals to {1}", a, b);
            Assert.That(EqualsOperator(a, b), Is.False, "{0} shouldn't be equals to {1}", a, b);
            Assert.That(NotEqualsOperator(a, b), Is.True, "{0} shouldn't be equals to {1}", a, b);
        }
    }
}
