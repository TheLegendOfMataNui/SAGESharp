using System.Collections.Generic;

namespace SAGESharp.SLB
{
    /// <summary>
    /// Class with extension methods so is easier to implement the Equals() and GetHashCode() methods.
    /// 
    /// For details on why this hash code implementation see the following StackOverflow answer:
    /// https://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
    /// </summary>
    internal static class EqualityUtils
    {
        /// <summary>
        /// Convenience method to compare a nullable object against other
        /// without the need to do null checks.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the objects.</typeparam>
        /// 
        /// <param name="obj">The object to compare.</param>
        /// <param name="other">The other object to compare.</param>
        /// 
        /// <returns>True if both objects are quals using the <see cref="object.Equals(object)"/> method.</returns>
        public static bool SafeEquals<T>(this T obj, T other) where T : class
        {
            return obj?.Equals(other) ?? other == null;
        }

        /// <summary>
        /// Convenience method to safely compare sequences of objects.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the objects.</typeparam>
        /// 
        /// <param name="values">The sequence to compare.</param>
        /// <param name="otherValues">The other sequence to compare.</param>
        /// 
        /// <returns>True if both sequences have the same elements (using <see cref="object.Equals(object)"/>).</returns>
        public static bool SafeSequenceEquals<T>(this IEnumerable<T> values, IEnumerable<T> otherValues) where T : class
        {
            if (ReferenceEquals(values, otherValues))
            {
                return true;
            }
            else if (values == null || otherValues == null)
            {
                return false;
            }

            using (var enumerator1 = values.GetEnumerator())
            {
                using (var enumerator2 = otherValues.GetEnumerator())
                {
                    while (enumerator1.MoveNext())
                    {
                        // "otherValues" has less objects than "values"
                        if (!enumerator2.MoveNext())
                        {
                            return false;
                        }

                        // If both values are not equals
                        if (!enumerator1.Current.SafeEquals(enumerator2.Current))
                        {
                            return false;
                        }
                    }

                    // "otherValues" has more objects than "values"
                    if (enumerator2.MoveNext())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Convenience method to correctly generate a hash code for a nullable object (reference).
        /// </summary>
        /// 
        /// <example>
        /// The idea of this method is to use it within an implementation of <see cref="object.GetHashCode"/>
        /// 
        /// <code>
        /// public override int GetHashCode()
        /// {
        ///     int hash = 7;
        ///     Property1.AddHashCodeByRef(ref hash, 5);
        ///     Property2.AddHashCodeByRef(ref hash, 5);
        ///     Property3.AddHashCodeByRef(ref hash, 5);
        ///     return hash;
        /// }
        /// </code>
        /// </example>
        /// 
        /// <typeparam name="T">The type of the object.</typeparam>
        /// 
        /// <param name="obj">The object.</param>
        /// <param name="hash">The seed hash.</param>
        /// <param name="prime">A prime number.</param>
        public static void AddHashCodeByRef<T>(this T obj, ref int hash, int prime) where T : class
        {
            hash = hash * prime + (obj?.GetHashCode() ?? 0);
        }

        /// <summary>
        /// Convenience method to correctly generate a has code for a group of nullable values (references).
        /// </summary>
        /// 
        /// <example>
        /// The idea of this method is to use it within an implementation of <see cref="object.GetHashCode"/>
        /// 
        /// <code>
        /// public override int GetHashCode()
        /// {
        ///     int hash = 7;
        ///     ListProperty.AddHashCodesByRef(ref hash, 5, 9);
        ///     // For reference on how to use it with other "AddHashCode..." methods
        ///     Property.AddHashCodeByRef(ref hash, 5);
        ///     return hash;
        /// }
        /// </code>
        /// </example>
        /// 
        /// <typeparam name="T">The type of the values.</typeparam>
        /// 
        /// <param name="values">The values.</param>
        /// <param name="hash">The seed hash.</param>
        /// <param name="emptyPrime">The prime that will be used if the enumerable is empty.</param>
        /// <param name="prime">A prime number.</param>
        public static void AddHashCodesByRef<T>(this IEnumerable<T> values, ref int hash, int prime, int emptyPrime) where T : class
        {
            if (values == null)
            {
                return;
            }

            using (var enumerator = values.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    hash *= emptyPrime;
                    return;
                }

                do
                {
                    enumerator.Current.AddHashCodeByRef(ref hash, prime);
                } while (enumerator.MoveNext());
            }
        }

        /// <summary>
        /// Convenience method to correctly generate a hash code for a non nullable object (value).
        /// </summary>
        /// 
        /// <example>
        /// The idea of this method is to use it within an implementation of <see cref="object.GetHashCode"/>
        /// 
        /// <code>
        /// public override int GetHashCode()
        /// {
        ///     int hash = 7;
        ///     Property1.AddHashCodeByVal(ref hash, 5);
        ///     Property2.AddHashCodeByVal(ref hash, 5);
        ///     Property3.AddHashCodeByVal(ref hash, 5);
        ///     return hash;
        /// }
        /// </code>
        /// </example>
        /// 
        /// <typeparam name="T">The type of the object.</typeparam>
        /// 
        /// <param name="obj">The object.</param>
        /// <param name="hash">The seed hash.</param>
        /// <param name="prime">A prime number.</param>
        public static void AddHashCodeByVal<T>(this T obj, ref int hash, int prime) where T : struct
        {
            hash = hash * prime + obj.GetHashCode();
        }

        /// <summary>
        /// Convenience method to correctly generate a has code for a group of non nullable values (values).
        /// </summary>
        /// 
        /// <example>
        /// The idea of this method is to use it within an implementation of <see cref="object.GetHashCode"/>
        /// 
        /// <code>
        /// public override int GetHashCode()
        /// {
        ///     int hash = 7;
        ///     ListProperty.AddHashCodesByVal(ref hash, 5, 9);
        ///     // For reference on how to use it with other "AddHashCode..." methods
        ///     Property.AddHashCodeByVal(ref hash, 5);
        ///     return hash;
        /// }
        /// </code>
        /// </example>
        /// 
        /// <typeparam name="T">The type of the values.</typeparam>
        /// 
        /// <param name="values">The values.</param>
        /// <param name="hash">The seed hash.</param>
        /// <param name="emptyPrime">The prime that will be used if the enumerable is empty.</param>
        /// <param name="prime">A prime number.</param>
        public static void AddHashCodeByVal<T>(this IEnumerable<T> values, ref int hash, int prime, int emptyPrime) where T : struct
        {
            if (values == null)
            {
                return;
            }

            using (var enumerator = values.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    hash *= emptyPrime;
                    return;
                }

                do
                {
                    enumerator.Current.AddHashCodeByVal(ref hash, prime);
                } while (enumerator.MoveNext());
            }
        }
    }
}
