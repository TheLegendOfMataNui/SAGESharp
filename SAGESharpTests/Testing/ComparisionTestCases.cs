/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NUtils.Extensions;
using NUtils.Validations;
using System;
using System.Linq.Expressions;

namespace SAGESharp.Testing
{
    /// <summary>
    /// Interface that represents a test to compare an object of type <typeparamref name="T"/>.
    /// </summary>
    /// 
    /// <typeparam name="T">The type with which comparison tests are made.</typeparam>
    interface IComparisionTestCase<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Executes the comparision tests.
        /// </summary>
        void Execute();
    }

    static class ComparisionTestCase
    {
        #region Test cases
        /// <summary>
        /// Creates an instance of <see cref="IComparisionTestCase{T}"/> to test comparing <paramref name="value"/> against itself.
        /// </summary>
        /// 
        /// <typeparam name="T">The type with which comparision tests are made.</typeparam>
        /// 
        /// <param name="value">The value to compare.</param>
        /// 
        /// <returns>
        /// A new instance of <see cref="IComparisionTestCase{T}"/>.
        /// </returns>
        public static IComparisionTestCase<T> CompareObjectAgainstItself<T>(T value) where T : IEquatable<T>
            => new CompareSameObjectTestCase<T>(value);

        private class CompareSameObjectTestCase<T> : IComparisionTestCase<T> where T : IEquatable<T>
        {
            private readonly T value;

            public CompareSameObjectTestCase(T value)
            {
                Validate.ArgumentNotNull<object>(value, nameof(value));

                this.value = value;
            }

            public void Execute()
            {
                value.Equals(value).Should().BeTrue(because: "Equals(T) was called with the object itself");
                value.Equals((object)value).Should().BeTrue(because: "Equals(object) was called with the object itself");
                ComparisionOperators<T>.EqualOperator(value, value).Should().BeTrue(because: "== was called with the same object at both sides");
                ComparisionOperators<T>.NotEqualOperator(value, value).Should().BeFalse(because: "!= was called with the same object at both sides");
            }

            public override string ToString() => $"Compare an object of type {typeof(T).FullName} against itself";
        }

        /// <summary>
        /// Creates an instance of <see cref="IComparisionTestCase{T}"/> to test comparing two values of type <typeparamref name="T"/>
        /// between each other.
        /// </summary>
        /// 
        /// <typeparam name="T">The type with which comparision tests are made.</typeparam>
        /// 
        /// <param name="supplier">
        /// A function that builds the instances of <typeparamref name="T"/> to compare.
        /// </param>
        /// 
        /// <returns>
        /// A new instance of <see cref="IComparisionTestCase{T}"/>.
        /// </returns>
        public static IComparisionTestCase<T> CompareTwoEqualObjects<T>(Func<T> supplier) where T : IEquatable<T>
        {
            Validate.ArgumentNotNull(supplier, nameof(supplier));

            return new CompareTwoEqualObjectsTestCase<T>(a: supplier(), b: supplier());
        }

        private class CompareTwoEqualObjectsTestCase<T> : IComparisionTestCase<T> where T : IEquatable<T>
        {
            private readonly T a;

            private readonly T b;

            public CompareTwoEqualObjectsTestCase(T a, T b)
            {
                Validate.ArgumentNotNull<object>(a, nameof(a));
                Validate.ArgumentNotNull<object>(b, nameof(b));

                this.a = a;
                this.b = b;
            }

            public void Execute()
            {
                a.Equals(b).Should().BeTrue(because: "Equals(T) was called with an equivalent object");
                a.Equals((object)b).Should().BeTrue(because: "Equals(object) was called with an equivalent object");
                ComparisionOperators<T>.EqualOperator(a, b).Should().BeTrue(because: "== was called with equivalent objects");
                ComparisionOperators<T>.NotEqualOperator(a, b).Should().BeFalse(because: "!= was called with equivalent objects");

                b.Equals(a).Should().BeTrue(because: "Equals(T) was called with an equivalent object");
                b.Equals((object)a).Should().BeTrue(because: "Equals(object) was called with an equivalent object");
                ComparisionOperators<T>.EqualOperator(b, a).Should().BeTrue(because: "== was called with equivalent objects");
                ComparisionOperators<T>.NotEqualOperator(b, a).Should().BeFalse(because: "!= was called with equivalent objects");

                a.GetHashCode().Should().Be(b.GetHashCode(), because: "Two equivalent objects should have the same hash code");
            }

            public override string ToString() => $"Compare two equivalent objects of type {typeof(T).FullName}";
        }

        /// <summary>
        /// Creates an instance of <see cref="IComparisionTestCase{T}"/> to test comparing two nulls of type
        /// <typeparamref name="T"/> with the equals and not equals operator.
        /// </summary>
        /// 
        /// <typeparam name="T">The type with which comparision tests are made.</typeparam>
        /// 
        /// <returns>
        /// A new instance of <see cref="IComparisionTestCase{T}"/>.
        /// </returns>
        public static IComparisionTestCase<T> CompareNullWithOperators<T>() where T : class, IEquatable<T>
            => new CompareNullWithOperatorsTestCase<T>();

        private class CompareNullWithOperatorsTestCase<T> : IComparisionTestCase<T> where T : class, IEquatable<T>
        {
            public void Execute()
            {
                ComparisionOperators<T>.EqualOperator(null, null).Should().BeTrue(because: "== was used to compare null and null");
                ComparisionOperators<T>.NotEqualOperator(null, null).Should().BeFalse(because: "!= was used to compare null and null");
            }

            public override string ToString() => $"Compare two null values with == and != operators";
        }

        /// <summary>
        /// Creates an instance of <see cref="IComparisionTestCase{T}"/> to test comparing two objects of type
        /// <typeparamref name="T"/> which are not equal.
        /// </summary>
        /// 
        /// <typeparam name="T">The type with which comparision tests are made.</typeparam>
        /// 
        /// <param name="supplier">Creates the objects to compare.</param>
        /// <param name="updater">Function to update one of the objects and make it different.</param>
        /// 
        /// <returns>
        /// A new instance of <see cref="IComparisionTestCase{T}"/>.
        /// </returns>
        public static IComparisionTestCase<T> CompareTwoNotEqualObjects<T>(Func<T> supplier, Action<T> updater) where T : class, IEquatable<T>
        {
            Validate.ArgumentNotNull(supplier, nameof(supplier));
            Validate.ArgumentNotNull(updater, nameof(updater));

            return new CompareTwoNotEqualObjectsTestCase<T>(a: supplier(), b: supplier().Also(updater));
        }

        private class CompareTwoNotEqualObjectsTestCase<T> : IComparisionTestCase<T> where T : IEquatable<T>
        {
            private readonly T a;

            private readonly T b;

            public CompareTwoNotEqualObjectsTestCase(T a, T b)
            {
                Validate.ArgumentNotNull<object>(a, nameof(a));
                Validate.ArgumentNotNull<object>(b, nameof(b));

                this.a = a;
                this.b = b;
            }

            public void Execute()
            {
                a.Equals(b).Should().BeFalse(because: "Equals(T) was called with a not equivalent object");
                a.Equals((object)b).Should().BeFalse(because: "Equals(object) was called with a not equivalent object");
                ComparisionOperators<T>.EqualOperator(a, b).Should().BeFalse(because: "== was called with not equivalent objects");
                ComparisionOperators<T>.NotEqualOperator(a, b).Should().BeTrue(because: "!= was called with not equivalent objects");

                b.Equals(a).Should().BeFalse(because: "Equals(T) was called with a not equivalent object");
                b.Equals((object)a).Should().BeFalse(because: "Equals(object) was called with a not equivalent object");
                ComparisionOperators<T>.EqualOperator(b, a).Should().BeFalse(because: "== was called with not equivalent objects");
                ComparisionOperators<T>.NotEqualOperator(b, a).Should().BeTrue(because: "!= was called with not equivalent objects");

                a.GetHashCode().Should().NotBe(b.GetHashCode(), because: "Two not equivalent objects should not have the same hash code");
            }

            public override string ToString() => $"Compare two not equivalent objects of type {typeof(T).FullName}";
        }

        /// <summary>
        /// Creates an instance of <see cref="IComparisionTestCase{T}"/> to test comparing an object of type
        /// <typeparamref name="T"/> against null.
        /// </summary>
        /// 
        /// <typeparam name="T">The type with which comparision tests are made.</typeparam>
        /// 
        /// <param name="value">The value to use in the tests.</param>
        /// 
        /// <returns>
        /// A new instance of <see cref="IComparisionTestCase{T}"/>.
        /// </returns>
        public static IComparisionTestCase<T> CompareNotNullObjectAgainstNull<T>(T value) where T : class, IEquatable<T>
            => new CompareNotNullObjectAgainstNullTestCase<T>(value);

        private class CompareNotNullObjectAgainstNullTestCase<T> : IComparisionTestCase<T> where T : class, IEquatable<T>
        {
            private readonly T value;

            public CompareNotNullObjectAgainstNullTestCase(T value)
            {
                Validate.ArgumentNotNull<object>(value, nameof(value));

                this.value = value;
            }
            public void Execute()
            {
                value.Equals((T)null).Should().BeFalse(because: "Equals(T) was called with a null value");
                value.Equals((object)null).Should().BeFalse(because: "Equals(object) was called with a null value");
                ComparisionOperators<T>.EqualOperator(value, null).Should().BeFalse(because: "== was called with a value and a null value");
                ComparisionOperators<T>.NotEqualOperator(value, null).Should().BeTrue(because: "!= was called with a value and a null value");

                ComparisionOperators<T>.EqualOperator(null, value).Should().BeFalse(because: "== was called with a value and a null value");
                ComparisionOperators<T>.NotEqualOperator(null, value).Should().BeTrue(because: "!= was called with a value and a null value");
            }

            public override string ToString() => $"Compare a value of type {typeof(T).FullName} with null";
        }
        #endregion

        #region Comparision operator logic
        private delegate bool ComparisionOperator<T>(T left, T right);

        private static class ComparisionOperators<T>
        {
            public static ComparisionOperator<T> EqualOperator { get; } = BuildEqualOperator();

            public static ComparisionOperator<T> NotEqualOperator { get; } = BuildNotEqualOperator();

            private static ComparisionOperator<T> BuildEqualOperator()
            {
                ParameterExpression leftParameter = Expression.Parameter(typeof(T), nameof(leftParameter));
                ParameterExpression rightParameter = Expression.Parameter(typeof(T), nameof(rightParameter));
                Expression equalExpression = Expression.Equal(leftParameter, rightParameter);
                Expression<ComparisionOperator<T>> lambda = Expression.Lambda<ComparisionOperator<T>>(equalExpression, leftParameter, rightParameter);

                return lambda.Compile();
            }

            private static ComparisionOperator<T> BuildNotEqualOperator()
            {
                ParameterExpression leftParameter = Expression.Parameter(typeof(T), nameof(leftParameter));
                ParameterExpression rightParameter = Expression.Parameter(typeof(T), nameof(rightParameter));
                Expression equalExpression = Expression.NotEqual(leftParameter, rightParameter);
                Expression<ComparisionOperator<T>> lambda = Expression.Lambda<ComparisionOperator<T>>(equalExpression, leftParameter, rightParameter);

                return lambda.Compile();
            }
        }
        #endregion
    }
}
