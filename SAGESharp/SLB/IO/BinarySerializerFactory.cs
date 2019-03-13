using System;

namespace SAGESharp.SLB.IO
{
    /// <summary>
    /// Interface to create instances of <see cref="IBinarySerializer"/>.
    /// </summary>
    public interface IBinarySerializerFactory
    {
        /// <summary>
        /// Creates the corresponding <see cref="IBinarySerializer"/> instance for the given <typeparamref name="T"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// 
        /// <returns>An instance of <see cref="IBinarySerializer" /> for <typeparamref name="T"/>.</returns>
        IBinarySerializer<T> GetSerializerForType<T>();
    }
}
