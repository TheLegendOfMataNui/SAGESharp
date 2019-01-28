using NUnit.Framework;
using System;

namespace SAGESharpTests.Util
{
    /// <summary>
    /// Class to facilitate testing the equality methods of another class.
    /// </summary>
    /// 
    /// This class provides all the logic to test for equality and the user
    /// only needs to provide: a default object to use in comparisions, how
    /// to perform the comparisions and any logic to compare different objects.
    /// 
    /// <remarks>
    /// You need to write the tests to compare different objects yourself,
    /// for details <see cref="TestCompareDefaultObjectWithModifiedObject"/>
    /// and <see cref="TestCompareModifiedObjects(Action{T}, Action{T})"/>
    /// </remarks>
    /// 
    /// <typeparam name="T">The class to test.</typeparam>
    internal abstract class AbstractEqualityByRefTests<T> where T : class, IEquatable<T>
    {
        /// <summary>
        /// Method to generate a default instance of <see cref="T"/>.
        /// </summary>
        /// 
        /// This is used for all the other test methods to ensure
        /// 
        /// <returns>A default instance of <see cref="T"/>.</returns>
        abstract protected T GetDefault();

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
            var obj = GetDefault();

            AssertSameHashCode(obj, obj);
            AssertAreEqual(obj, obj);
        }

        /// <summary>
        /// Tests two different instances of <see cref="T"/> are always equals.
        /// </summary>
        [Test]
        public void Test_Compare_Two_Default_Objects()
        {
            var a = GetDefault();
            var b = GetDefault();

            AssertSameHashCode(a, b);
            AssertAreEqual(a, b);

            AssertSameHashCode(b, a);
            AssertAreEqual(b, a);
        }

        /// <summary>
        /// Tests an instance of <see cref="T"/> is never equals to null.
        /// </summary>
        [Test]
        public void Test_Compare_Default_Object_With_Null()
        {
            var obj = GetDefault();

            AssertAreNotEqual(obj, null);

            // Test null in the left side
            EqualsOperator(null, obj);
            NotEqualsOperator(null, obj);
        }

        /// <summary>
        /// Tests two different instances of <see cref="T"/> are always different.
        /// </summary>
        /// 
        /// This method generates two different instances of <see cref="T"/> using
        /// <see cref="GetDefault"/> and modifies one of them with the provided
        /// <see cref="Action{T}"/>, then both instances are compared.
        /// 
        /// <example>
        /// In order to use this method it needs to be called from the test class.
        /// 
        /// Things become easier leveraging NUnit's <see cref="TestCaseSourceAttribute"/>.
        /// 
        /// <code>
        /// class MyClassEqualityTests : AbstractEqualityByRefTests<MyClass>
        /// {
        ///     // ...
        ///     
        ///     [TestCaseSource(nameof(Modifiers))]
        ///     public void Test_Compare_Default_Object_With_Modified_Object(Action<T> modifier) =>
        ///         TestCompareDefaultObjectWithModifiedObject(modifier);
        ///     
        ///     static object[] Modifiers() => new object[] {
        ///         new Action<MyClass>[] { c => c.Property++ },
        ///         new Action<MyClass>[] { c => c.Property-- }
        ///     };
        /// }
        /// </code>
        /// </example>
        /// 
        /// <param name="modifier">An action to modify the default instance of <see cref="T"/>.</param>
        protected void TestCompareDefaultObjectWithModifiedObject(Action<T> modifier)
        {
            TestCompareModifiedObjects(a => { }, modifier);
        }

        /// <summary>
        /// Tests two different instances of <see cref="T"/> are always different.
        /// </summary>
        /// 
        /// This method generates two different instances of <see cref="T"/> using
        /// <see cref="GetDefault"/> and modifies each one with the provided
        /// <see cref="Action{T}"/> objects, then both instances are compared.
        /// 
        /// <example>
        /// In order to use this method it needs to be called from the test class.
        /// 
        /// Things become easier leveraging NUnit's <see cref="TestCaseSourceAttribute"/>.
        /// 
        /// <code>
        /// class MyClassEqualityTests : AbstractEqualityByRefTests<MyClass>
        /// {
        ///     // ...
        ///     
        ///     [TestCaseSource(nameof(Modifiers))]
        ///     public void Test_Compare_Modified_Objects(Action<T> modifier) =>
        ///         TestCompareModifiedObjects(modifier);
        ///     
        ///     static object[] Modifiers() => new object[] {
        ///         new Action<MyClass>[] { c => c.Property++, c => c.Property-- },
        ///         new Action<MyClass>[] { c => c.Property--, c => c.Property++ }
        ///     };
        /// }
        /// </code>
        /// </example>
        /// 
        /// <param name="modifierA">An action to modify the first instance of <see cref="T"/>.</param>
        /// <param name="modifierA">An action to modify the second instance of <see cref="T"/>.</param>
        protected void TestCompareModifiedObjects(Action<T> modifierA, Action<T> modifierB)
        {
            var a = GetDefault();
            var b = GetDefault();

            modifierA(a);
            modifierB(b);

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
