/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO;
using SAGESharp.IO.Binary.TreeBasedSerialization;
using System.Collections.Generic;

namespace SAGESharp.Tests.IO.Binary.TreeBasedSerialization.Trees
{
    static class TreeWithUnknownNodeType
    {
        public static IDataNode Build() => new BuilderFor.DataNodeSubstitute
        {
            Edges = new List<IEdge>
            {
                new BuilderFor.EdgeSubstitute<object>
                {
                    ChildNode = new UnknownNodeType(),
                    ChildExtractor = value => value,
                    ChildSetter = (value, childValue) => { }
                }.Build()
            }
        }.Build();

        internal class UnknownNodeType
        {
        }
    }
}
