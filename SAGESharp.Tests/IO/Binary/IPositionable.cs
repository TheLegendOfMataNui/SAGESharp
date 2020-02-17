/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
namespace SAGESharp.Tests.IO.Binary
{
    /*
     * This is a very janky solution for a limitation of the substitution
     * framework: you cannot mock/setup several calls over the same property.
     * 
     * This is particularly problematic for the `DoAtPosition` extension
     * methods that are considerably used for (de)serializing objects in binary
     * format.
     * 
     * The way to workaround is to "alias" the `Position` property for
     * `IBinaryReader` and `IBinaryWriter` in abstract classes and that way
     * you can setup the consecutive calls via `GetPosition` and `SetPosition`.
     */
    interface IPositionable
    {
        long GetPosition();

        void SetPosition(long value);
    }
}
