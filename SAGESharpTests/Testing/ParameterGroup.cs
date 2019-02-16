using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace SAGESharp.Testing
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
    class ParameterGroup : AbstractParameterGroup
    {
        /// <summary>
        /// Add the list of parameters to the parameter group, at least one is required.
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

            AddParameters(allParameters);
            
            return this;
        }
    }

    /// <summary>
    /// Type safe version of <see cref="ParameterGroup"/>.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// 
    /// <seealso cref="ParameterGroup"/>
    class ParameterGroup<T> : AbstractParameterGroup
    {
        /// <summary>
        /// Adds a new entry to the parameter group.
        /// </summary>
        /// 
        /// <param name="parameter">The parameter to add.</param>
        /// 
        /// <returns>The same parameter group to chain calls.</returns>
        /// 
        /// <seealso cref="ParameterGroup.Parameters(object, object[])"/>
        public ParameterGroup<T> Parameters(T parameter)
        {
            AddParameters(new object[] { parameter });
            return this;
        }
    }

    /// <summary>
    /// Type safe version of <see cref="ParameterGroup"/>.
    /// </summary>
    /// 
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// 
    /// <seealso cref="ParameterGroup"/>
    class ParameterGroup<T1, T2> : AbstractParameterGroup
    {
        /// <summary>
        /// Adds a new entry to the parameter group.
        /// </summary>
        /// 
        /// <param name="parameter1">The first parameter to add.</param>
        /// <param name="parameter2">The second parameter to add.</param>
        /// 
        /// <returns>The same parameter group to chain calls.</returns>
        /// 
        /// <seealso cref="ParameterGroup.Parameters(object, object[])"/>
        public ParameterGroup<T1,T2> Parameters(T1 parameter1, T2 parameter2)
        {
            AddParameters(new object[] { parameter1, parameter2 });
            return this;
        }
    }

    /// <summary>
    /// Type safe version of <see cref="ParameterGroup"/>.
    /// </summary>
    /// 
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// 
    /// <seealso cref="ParameterGroup"/>
    class ParameterGroup<T1, T2, T3> : AbstractParameterGroup
    {
        /// <summary>
        /// Adds a new entry to the parameter group.
        /// </summary>
        /// 
        /// <param name="parameter1">The first parameter to add.</param>
        /// <param name="parameter2">The second parameter to add.</param>
        /// <param name="parameter3">The third parameter to add.</param>
        /// 
        /// <returns>The same parameter group to chain calls.</returns>
        /// 
        /// <seealso cref="ParameterGroup.Parameters(object, object[])"/>
        public ParameterGroup<T1, T2, T3> Parameters(T1 parameter1, T2 parameter2, T3 parameter3)
        {
            AddParameters(new object[] { parameter1, parameter2, parameter3 });
            return this;
        }
    }

    /// <summary>
    /// Type safe version of <see cref="ParameterGroup"/>.
    /// </summary>
    /// 
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// 
    /// <seealso cref="ParameterGroup"/>
    class ParameterGroup<T1, T2, T3, T4> : AbstractParameterGroup
    {
        /// <summary>
        /// Adds a new entry to the parameter group.
        /// </summary>
        /// 
        /// <param name="parameter1">The first parameter to add.</param>
        /// <param name="parameter2">The second parameter to add.</param>
        /// <param name="parameter3">The third parameter to add.</param>
        /// <param name="parameter4">The fourth parameter to add.</typeparam>
        /// 
        /// <returns>The same parameter group to chain calls.</returns>
        /// 
        /// <seealso cref="ParameterGroup.Parameters(object, object[])"/>
        public ParameterGroup<T1, T2, T3, T4> Parameters(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4)
        {
            AddParameters(new object[] { parameter1, parameter2, parameter3, parameter4 });
            return this;
        }
    }

    /// <summary>
    /// Type safe version of <see cref="ParameterGroup"/>.
    /// </summary>
    /// 
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter.</typeparam>
    /// 
    /// <seealso cref="ParameterGroup"/>
    class ParameterGroup<T1, T2, T3, T4, T5> : AbstractParameterGroup
    {
        /// <summary>
        /// Adds a new entry to the parameter group.
        /// </summary>
        /// 
        /// <param name="parameter1">The first parameter to add.</param>
        /// <param name="parameter2">The second parameter to add.</param>
        /// <param name="parameter3">The third parameter to add.</param>
        /// <param name="parameter4">The fourth parameter to add.</typeparam>
        /// <param name="parameter5">The fifth parameter to add.</typeparam>
        /// 
        /// <returns>The same parameter group to chain calls.</returns>
        /// 
        /// <seealso cref="ParameterGroup.Parameters(object, object[])"/>
        public ParameterGroup<T1, T2, T3, T4, T5> Parameters(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5)
        {
            AddParameters(new object[] { parameter1, parameter2, parameter3, parameter4, parameter5 });
            return this;
        }
    }

    /// <summary>
    /// Class To facilitate some methods when writing <see cref="ParameterGroup"/> classes.
    /// </summary>
    abstract class AbstractParameterGroup
    {
        private readonly List<object> group = new List<object>();

        protected void AddParameters(object[] parameters)
        {
            group.Add(parameters);
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
