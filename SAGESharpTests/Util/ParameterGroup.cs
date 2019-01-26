using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace SAGESharpTests.Util
{
    /// <summary>
    /// Convenience class to use when creating arguments for a <see cref="TestCaseSourceAttribute"/>
    /// </summary>
    /// 
    /// <example>
    /// <code>
    /// static object[] MyTestSource()
    /// {
    ///     return new ParameterGroup()
    ///         .Parameters(1, "string1")
    ///         .Parameters(4, null)
    ///         .Parameters(35, "other string")
    ///         .Build();
    /// }
    /// 
    /// //...
    /// 
    /// [TestCase("MyTestSource")]
    /// public void MyTestCase(int parameter1, string parameter2)
    /// {
    ///     // ...
    /// }
    /// </code>
    /// </example>
    internal class ParameterGroup
    {
        private readonly List<object> group = new List<object>();

        /// <summary>
        /// Add the list of parameters to the parameter group, least one is required.
        /// </summary>
        /// 
        /// <param name="paramater">The first parameter to add.</param>
        /// <param name="parameters">The list of parameters to add.</param>
        /// 
        /// <returns>The same parameter group to chain calls.</returns>
        public ParameterGroup Parameters(object paramater, params object[] parameters)
        {
            // Merge the required parameter and the rest of the parameters.
            object[] allParameters = new object[parameters.Length + 1];
            allParameters[0] = paramater;
            Array.Copy(parameters, 0, allParameters, 1, parameters.Length);

            group.Add(allParameters);
            return this;
        }

        /// <summary>
        /// Build the object array for this parameter group.
        /// </summary>
        /// 
        /// <returns>The object array for this parameter group.</returns>
        public object[] Build()
        {
            return group.ToArray();
        }
    }
}
