namespace SAGESharp.SLB
{
    /// <summary>
    /// Interface that represents an object to write SLB objects as binary data.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of SLB data that is going to be written.</typeparam>
    internal interface ISLBBinaryWriter<T>
    {
        /// <summary>
        /// Writes a SLB object into the underlying storage as binary data.
        /// </summary>
        /// 
        /// <param name="slbObject">The SLB object that is going to be written as binary data.</param>
        void WriteSLBObject(T slbObject);
    }
}
