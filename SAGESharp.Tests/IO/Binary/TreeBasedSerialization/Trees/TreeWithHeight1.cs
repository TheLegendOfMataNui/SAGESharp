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
    static class TreeWithHeight1
    {
        public static IDataNode Build() => new BuilderFor.DataNodeSubstitute
        {
            Edges = new List<IEdge>
            {
                new BuilderFor.EdgeSubstitute<Class>
                {
                    ChildNode = new BuilderFor.DataNodeSubstitute().Build(),
                    ChildExtractor = value => value.Int,
                    ChildSetter = (value, childValue) => value.Int = (int)childValue
                }.Build(),
                new BuilderFor.EdgeSubstitute<Class>
                {
                    ChildNode = new BuilderFor.DataNodeSubstitute().Build(),
                    ChildExtractor = value => value.Float,
                    ChildSetter = (value, childValue) => value.Float = (float)childValue
                }.Build(),
                new BuilderFor.EdgeSubstitute<Class>
                {
                    ChildNode = new BuilderFor.DataNodeSubstitute().Build(),
                    ChildExtractor = value => value.Byte,
                    ChildSetter = (value, childValue) => value.Byte = (byte)childValue
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
