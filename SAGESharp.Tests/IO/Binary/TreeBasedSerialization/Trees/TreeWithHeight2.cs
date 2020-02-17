/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO.Binary.TreeBasedSerialization;
using System.Collections.Generic;

namespace SAGESharp.Tests.IO.Binary.TreeBasedSerialization.Trees
{
    static class TreeWithHeight2
    {
        public static IDataNode Build() => new BuilderFor.DataNodeSubstitute
        {
            Edges = new List<IEdge>
            {
                new BuilderFor.EdgeSubstitute<Class>
                {
                    ChildNode = new BuilderFor.DataNodeSubstitute().Build(),
                    ChildExtractor = value => value.Long,
                    ChildSetter = (value, childValue) => value.Long = (long)childValue
                }.Build(),
                new BuilderFor.EdgeSubstitute<Class>
                {
                    ChildNode = TreeWithHeight1.Build(),
                    ChildExtractor = value => value.Child,
                    ChildSetter = (value, childValue) => value.Child = (TreeWithHeight1.Class)childValue
                }.Build()
            }
        }.Build();

        public class Class
        {
            public long Long { get; set; }

            public TreeWithHeight1.Class Child { get; set; }
        }
    }
}
