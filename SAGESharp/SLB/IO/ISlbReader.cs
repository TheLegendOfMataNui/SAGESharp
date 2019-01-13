namespace SAGESharp.SLB.IO
{
    /// <summary>
    /// Interface representing an object that reads SLB objects.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of SLB objects to read.</typeparam>
    public interface ISlbReader<T>
    {
        /// <summary>
        /// Reads the SLB object.
        /// </summary>
        /// 
        /// <returns>The SLB object that was read.</returns>
        T ReadSlbObject();
    }
}
