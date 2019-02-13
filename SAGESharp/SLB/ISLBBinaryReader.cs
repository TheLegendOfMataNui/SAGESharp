namespace SAGESharp.SLB
{
    /// <summary>
    /// Interface that represents an object to read binary data and create SLB objects from it.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of SLB data that is going to be read.</typeparam>
    internal interface ISLBBinaryReader<T>
    {
        /// <summary>
        /// Reads a new SLB object from the binary data in the underlying storage.
        /// </summary>
        /// 
        /// <returns>A new instance SLB object based on the underlying storage.</returns>
        T ReadSLBObject();
    }
}
