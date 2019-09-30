/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using System.Collections.Generic;

namespace SAGESharp.IO.Trees
{
    static class TreeWithNodeAtOffset
    {
        public static IDataNode Build() => new BuilderFor.DataNodeSubstitute
        {
            Edges = new List<IEdge>
            {
                new BuilderFor.EdgeSubstitute<Class>
                {
                    ChildNode = new BuilderFor.OffsetNodeSubstitute
                    {
                        ChildNode = new BuilderFor.DataNodeSubstitute().Build()
                    }.Build(),
                    ChildExtractor = value => value.ValueAtOffset
                }.Build(),
                new BuilderFor.EdgeSubstitute<Class>
                {
                    ChildNode = new BuilderFor.DataNodeSubstitute().Build(),
                    ChildExtractor = value => value.ValueInline
                }.Build()
            }
        }.Build();

        public class Class
        {
            public string ValueAtOffset { get; set; }

            public int ValueInline { get; set; }
        }
    }
}
