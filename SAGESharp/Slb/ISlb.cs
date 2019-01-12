using System.IO;

namespace SAGESharp.Slb
{
    /// <summary>
    /// Interface that represents an object in a SLB file.
    /// </summary>
    interface ISlb
    {
        /// <summary>
        /// Reads the data from the given stream into the SLB object.
        /// </summary>
        /// 
        /// <param name="stream">The stream from where the objects will be read.</param>
        void ReadFrom(Stream stream);

        /// <summary>
        /// WRites the SLB object into the given stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream where the objects will be written.</param>
        void WriteTo(Stream stream);
    }
}
