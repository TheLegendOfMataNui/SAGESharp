/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NSubstitute;
using SAGESharp.IO;
using System;
using System.Collections.Generic;

namespace SAGESharp.Tests.IO.Binary.TreeBasedSerialization.Trees
{
    static class BuilderFor
    {
        public sealed class DataNodeSubstitute
        {
            public IReadOnlyList<IEdge> Edges { get; set; } = new List<IEdge>();

            public IDataNode Build()
            {
                IDataNode node = Substitute.For<IDataNode>();

                node.Edges.Returns(Edges);

                return node;
            }
        }

        public sealed class OffsetNodeSubstitute
        {
            public IDataNode ChildNode { get; set; }

            public IOffsetNode Build()
            {
                IOffsetNode node = Substitute.For<IOffsetNode>();

                node.ChildNode.Returns(ChildNode);

                return node;
            }
        }

        public sealed class ListNodeSubstitute<T>
        {
            public IDataNode ChildNode { get; set; }

            public IListNode Build()
            {
                IListNode node = Substitute.For<IListNode>();

                node.ChildNode.Returns(ChildNode);

                node.CreateList().Returns(new List<T>());

                node.GetListCount(Arg.Any<IList<T>>()).Returns(args =>
                {
                    IList<T> list = (IList<T>)args[0];

                    return list.Count;
                });

                node.GetListEntry(Arg.Any<IList<T>>(), Arg.Any<int>()).Returns(args =>
                {
                    IList<T> list = (IList<T>)args[0];
                    int index = (int)args[1];

                    return list[index];
                });

                SetupAddEntry(node);

                return node;
            }

            private void SetupAddEntry(IListNode node)
            {
                // Setup in this convoluted way so the "list" variable
                // is different everytime "AddListEntry" is called
                // otherwise "list" will be the same for different calls
                void setup()
                {
                    IList<T> list = default(IList<T>);
                    node.AddListEntry(
                        Arg.Do<IList<T>>(val => list = val),
                        Arg.Do<T>(entry => list.Add(entry))
                    );
                }

                setup();
            }
        }

        public sealed class EdgeSubstitute<T>
        {
            public object ChildNode { get; set; }

            public Func<T, object> ChildExtractor { get; set; }

            public Action<T, object> ChildSetter { get; set; }

            public IEdge Build()
            {
                IEdge edge = Substitute.For<IEdge>();

                edge.ChildNode.Returns(ChildNode);

                if (ChildExtractor != null)
                {
                    edge.ExtractChildValue(Arg.Any<T>())
                        .Returns(args => ChildExtractor((T)args[0]));
                }

                SetupSetChildValue(edge);

                return edge;
            }

            private void SetupSetChildValue(IEdge edge)
            {
                // Setup in this convoluted way so the "value" variable
                // is different everytime "SetChildValue" is called
                // otherwise "value" will be the same for different calls
                void setup()
                {
                    T value = default(T);
                    edge.SetChildValue(
                        Arg.Do<T>(val => value = val),
                        Arg.Do<object>(childVal => ChildSetter(value, childVal))
                    );
                }

                setup();
            }
        }
    }
}
