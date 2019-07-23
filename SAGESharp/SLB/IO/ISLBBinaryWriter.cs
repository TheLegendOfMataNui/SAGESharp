/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
namespace SAGESharp.SLB.IO
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
