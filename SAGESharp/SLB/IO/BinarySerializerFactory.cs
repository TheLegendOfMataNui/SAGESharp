using System;

namespace SAGESharp.SLB.IO
{
    /// <summary>
    /// Interface to create instances of <see cref="IBinarySerializer"/>.
    /// </summary>
    public interface IBinarySerializerFactory
    {
        /// <summary>
        /// Creates the corresponding <see cref="IBinarySerializer"/> instance for the given <paramref name="type"/>.
        /// </summary>
        /// 
        /// <param name="type">The type to read.</param>
        /// 
        /// <returns>An instance of <see cref="IBinarySerializer" /> for <paramref name="type"/>.</returns>
        IBinarySerializer GetSerializerForType(Type type);
    }
}
