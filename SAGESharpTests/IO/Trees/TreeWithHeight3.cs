/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using System.Collections.Generic;

namespace SAGESharp.IO.Trees
{
    static class TreeWithHeight3
    {
        public static IDataNode Build() => new BuilderFor.DataNodeSubstitute
        {
            Edges = new List<IEdge>
            {
                new BuilderFor.EdgeSubstitute<Class>
                {
                    ChildNode = TreeWithHeight2.Build(),
                    ChildExtractor = value => value.Child1
                }.Build(),
                new BuilderFor.EdgeSubstitute<Class>
                {
                    ChildNode = TreeWithHeight2.Build(),
                    ChildExtractor = value => value.Child2
                }.Build()
            }
        }.Build();

        public class Class
        {
            public TreeWithHeight2.Class Child1 { get; set; }

            public TreeWithHeight2.Class Child2 { get; set; }
        }
    }
}
