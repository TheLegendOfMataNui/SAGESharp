/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using System.Collections.Generic;

namespace SAGESharp.IO.Trees
{
    static class TreeWithHeight1
    {
        public static IDataNode Build() => new BuilderFor.DataNodeSubstitute
        {
            Edges = new List<IEdge>
            {
                new BuilderFor.EdgeSubstitute<Class>
                {
                    ChildNode = new BuilderFor.DataNodeSubstitute().Build(),
                    ChildExtractor = value => value.Int
                }.Build(),
                new BuilderFor.EdgeSubstitute<Class>
                {
                    ChildNode = new BuilderFor.DataNodeSubstitute().Build(),
                    ChildExtractor = value => value.Float,
                }.Build(),
                new BuilderFor.EdgeSubstitute<Class>
                {
                    ChildNode = new BuilderFor.DataNodeSubstitute().Build(),
                    ChildExtractor = value => value.Byte
                }.Build()
            }
        }.Build();

        public class Class
        {
            public int Int { get; set; }

            public float Float { get; set; }

            public byte Byte { get; set; }
        }
    }
}
