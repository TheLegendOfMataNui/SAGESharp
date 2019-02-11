using System.Collections.Generic;

namespace SAGESharp.SLB
{
    /// <summary>
    /// Interface that writes the footer for a given SLB object.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the SLB object.</typeparam>
    internal interface ISLBFooterWriter<T>
    {
        /// <summary>
        /// Writes the footer for the given SLB object.
        /// </summary>
        /// 
        /// <param name="slbObject">The SLB object to write.</param>
        void WriteFooter(T slbObject);
    }

    /// <summary>
    /// Interface that generates the footer for the given SLB object.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the SLB object </typeparam>
    internal interface ISLBFooterGenerator<T>
    {
        /// <summary>
        /// Generates the footer.
        /// </summary>
        /// <returns>The footer.</returns>
        /// <param name="slbObject">Slb object.</param>
        IDictionary<uint, uint> GenerateFooter(T slbObject);
    }
}
