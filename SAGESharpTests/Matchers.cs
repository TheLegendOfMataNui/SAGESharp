using NSubstitute;
using System.Linq;

namespace SAGESharp
{
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
