using Konvenience;
using NUnit.Framework;

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
                .TakeReferenceUnless(s => s.Length == 0)
                ?? base.ToString();

        /// <summary>
        /// Returns a string to name the test case data.
        /// </summary>
        /// 
        /// <returns>A string to name the test case data.</returns>
        public override string ToString() => description;
    }
}
