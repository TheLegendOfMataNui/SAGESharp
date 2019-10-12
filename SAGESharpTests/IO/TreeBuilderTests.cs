/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using FluentAssertions;
using NUnit.Framework;
using SAGESharp.Testing;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SAGESharp.IO
{
    class TreeBuilderTests
    {
        #region Building UserTypeDataNode
        [Test]
        public void Test_Building_A_Tree_For_A_Class_With_Primitive_Properties()
        {
            IDataNode rootNode = TreeBuilder.BuildTreeForType(typeof(ClassWithPrimitiveProperties));

            ClassWithPrimitiveProperties.BuildSample().VerifyTree(rootNode);
        }

        private sealed class ClassWithPrimitiveProperties
        {
            [SerializableProperty(0)]
            public int Int { get; set; }

            [SerializableProperty(1)]
            public float Float { get; set; }

            public void VerifyTree(IDataNode dataNode)
            {
                dataNode.Should().BeOfType<UserTypeDataNode<ClassWithPrimitiveProperties>>();

                dataNode.Edges.Should().HaveCount(2);

                VerifyEdge(dataNode.Edges[0], this, v => v.Int);

                dataNode.Edges[0].ChildNode.Should().BeOfType<PrimitiveTypeDataNode<int>>();

                VerifyEdge(dataNode.Edges[1], this, v => v.Float);

                dataNode.Edges[1].ChildNode.Should().BeOfType<PrimitiveTypeDataNode<float>>();
            }

            public static ClassWithPrimitiveProperties BuildSample() => new ClassWithPrimitiveProperties
            {
                Int = 32,
                Float = 9.2f
            };
        }

        [Test]
        public void Test_Building_A_Tree_For_A_Class_With_Nested_Class_With_Primitive_Properties()
        {
            IDataNode rootNode = TreeBuilder.BuildTreeForType(typeof(ClassWithNestedClassWithPrimitiveProperties));

            ClassWithNestedClassWithPrimitiveProperties.BuildSample().VerifyTree(rootNode);
        }

        private sealed class ClassWithNestedClassWithPrimitiveProperties
        {
            [SerializableProperty(0)]
            public ClassWithPrimitiveProperties FlatClass { get; set; }

            [SerializableProperty(1)]
            public byte Byte { get; set; }

            [SerializableProperty(2)]
            public SLB.Identifier Identifier { get; set; }

            public void VerifyTree(IDataNode dataNode)
            {
                dataNode.Should().BeOfType<UserTypeDataNode<ClassWithNestedClassWithPrimitiveProperties>>();

                dataNode.Edges.Should().HaveCount(3);

                VerifyEdge(dataNode.Edges[0], this, v => v.FlatClass);

                FlatClass.VerifyTree(dataNode.Edges[0].ChildNode as IDataNode);

                VerifyEdge(dataNode.Edges[1], this, v => v.Byte);

                dataNode.Edges[1].ChildNode.Should().BeOfType<PrimitiveTypeDataNode<byte>>();

                VerifyEdge(dataNode.Edges[2], this, v => v.Identifier);

                dataNode.Edges[2].ChildNode.Should().BeOfType<PrimitiveTypeDataNode<SLB.Identifier>>();
            }

            public static ClassWithNestedClassWithPrimitiveProperties BuildSample() => new ClassWithNestedClassWithPrimitiveProperties
            {
                FlatClass = ClassWithPrimitiveProperties.BuildSample(),
                Byte = 12
            };
        }
        #endregion

        #region Building ListNode
        [Test]
        public void Test_Building_A_Tree_For_A_Class_With_A_List()
        {
            IDataNode rootNode = TreeBuilder.BuildTreeForType(typeof(ClassWithAList));

            VerifyTreeForClassWithAList(
                dataNode: rootNode,
                expected: new ClassWithAList
                {
                    Values = new List<ClassWithPrimitiveProperties>()
                },
                extractor: v => v.Values,
                duplicateEntryCount: false
            );
        }

        private sealed class ClassWithAList
        {
            [SerializableProperty(0)]
            public IList<ClassWithPrimitiveProperties> Values { get; set; }
        }

        [Test]
        public void Test_Building_A_Tree_For_A_Class_With_A_List_With_Duplicated_Entry_Count()
        {
            IDataNode rootNode = TreeBuilder.BuildTreeForType(typeof(ClassWithAListWithDuplicatedEntryCount));

            VerifyTreeForClassWithAList(
                dataNode: rootNode,
                expected: new ClassWithAListWithDuplicatedEntryCount
                {
                    Values = new List<ClassWithPrimitiveProperties>()
                },
                extractor: v => v.Values,
                duplicateEntryCount: true
            );
        }

        private sealed class ClassWithAListWithDuplicatedEntryCount
        {
            [SerializableProperty(0)]
            [DuplicateEntryCount]
            public IList<ClassWithPrimitiveProperties> Values { get; set; }
        }

        private static void VerifyTreeForClassWithAList<T>(IDataNode dataNode, T expected, Func<T, object> extractor, bool duplicateEntryCount) where T : new()
        {
            dataNode.Should().BeOfType<UserTypeDataNode<T>>();

            dataNode.Edges.Should().HaveCount(1);

            VerifyEdge(dataNode.Edges[0], expected, extractor);

            ListNode<ClassWithPrimitiveProperties> listNode = dataNode.Edges[0].ChildNode.Should()
                .BeOfType<ListNode<ClassWithPrimitiveProperties>>()
                .Subject;

            FieldInfo duplicateEntryCountField = typeof(ListNode<ClassWithPrimitiveProperties>)
                .GetField("duplicateEntryCount", BindingFlags.NonPublic | BindingFlags.Instance);

            duplicateEntryCountField.GetValue(listNode).Should().Be(duplicateEntryCount);

            ClassWithPrimitiveProperties.BuildSample().VerifyTree(listNode.ChildNode);
        }
        #endregion

        #region Building StringDataNode
        [Test]
        public void Test_Building_A_Tree_For_A_Class_With_A_String_At_Offset()
        {
            IDataNode rootNode = TreeBuilder.BuildTreeForType(typeof(ClassWithStringAtOffset));

            ClassWithStringAtOffset.BuildSample().VerifyTree(rootNode);
        }

        private sealed class ClassWithStringAtOffset
        {
            [SerializableProperty(0)]
            [OffsetString]
            public string Value { get; set; }

            public void VerifyTree(IDataNode dataNode)
            {
                dataNode.Should().BeOfType<UserTypeDataNode<ClassWithStringAtOffset>>();

                dataNode.Edges.Should().HaveCount(1);

                VerifyEdge(dataNode.Edges[0], this, v => v.Value);

                StringDataNode stringDataNode = dataNode.Edges[0].ChildNode.Should().BeOfType<OffsetNode>()
                    .Subject.ChildNode.Should().BeOfType<StringDataNode>()
                    .Subject;

                GetLengthForStringDataNode(stringDataNode).Should().Be(StringDataNode.OFFSET_STRING_MAX_LENGTH);
            }

            public static ClassWithStringAtOffset BuildSample() => new ClassWithStringAtOffset
            {
                Value = "This string is at offset"
            };
        }

        [Test]
        public void Test_Building_A_Tree_For_A_Class_With_A_String_Inline()
        {
            IDataNode rootNode = TreeBuilder.BuildTreeForType(typeof(ClassWithStringInline));

            ClassWithStringInline.BuildSample().VerifyTree(rootNode);
        }

        private sealed class ClassWithStringInline
        {
            private const int LENGTH = 27;

            [SerializableProperty(0)]
            [InlineString(LENGTH)]
            public string Value { get; set; }

            public void VerifyTree(IDataNode dataNode)
            {
                dataNode.Should().BeOfType<UserTypeDataNode<ClassWithStringInline>>();

                dataNode.Edges.Should().HaveCount(1);

                VerifyEdge(dataNode.Edges[0], this, v => v.Value);

                StringDataNode stringDataNode = dataNode.Edges[0].ChildNode.Should().BeOfType<StringDataNode>()
                    .Subject;

                GetLengthForStringDataNode(stringDataNode).Should().Be(LENGTH);
            }

            public static ClassWithStringInline BuildSample() => new ClassWithStringInline
            {
                Value = "This string is inline"
            };
        }

        private static int GetLengthForStringDataNode(StringDataNode stringDataNode)
        {
            FieldInfo field = typeof(StringDataNode)
                .GetField("length", BindingFlags.NonPublic | BindingFlags.Instance);

            return (byte)field.GetValue(stringDataNode);
        }
        #endregion

        #region Error checking
        [Test]
        public void Test_Building_A_Tree_For_A_Null_Type()
        {
            Action action = () => TreeBuilder.BuildTreeForType(null);

            action.Should()
                .ThrowArgumentNullException("type");
        }

        [Test]
        public void Test_Building_A_Tree_For_A_Class_With_Private_Parameterless_Constructor()
        {
            BadTypeExceptionShouldBeThrownForType<ClassWithPrivateParameterlessConstructor>
            (
                message: $"Type doesn't have a public parameterless constructor."
            );
        }

        private class ClassWithPrivateParameterlessConstructor
        {
            private ClassWithPrivateParameterlessConstructor()
            {
            }
        }

        [Test]
        public void Test_Building_A_Tree_For_A_Class_Without_Parameterless_Constructor()
        {
            BadTypeExceptionShouldBeThrownForType<ClassWithoutParameterlessConstructor>
            (
                message: $"Type doesn't have a public parameterless constructor."
            );
        }

        private class ClassWithoutParameterlessConstructor
        {
            public ClassWithoutParameterlessConstructor(int value)
            {
            }
        }

        [Test]
        public void Test_Building_A_Tree_For_A_Class_Without_Serializable_Properties()
        {
            BadTypeExceptionShouldBeThrownForType<ClassWithoutSerializableProperties>
            (
                message: $"Type doesn't have serializable properties."
            );
        }

        private class ClassWithoutSerializableProperties
        {
            public int Int { get; set; }
        }

        [Test]
        public void Test_Building_A_Tree_For_A_Class_With_Properties_With_Duplicate_Serialization_Order()
        {
            BadTypeExceptionShouldBeThrownForType<ClassWithPropertiesWithDuplicateSerializationOrder>
            (
                message: $"Type has two or more properties with the same serialization order."
            );
        }

        private class ClassWithPropertiesWithDuplicateSerializationOrder
        {
            [SerializableProperty(0)]
            public int Int { get; set; }

            [SerializableProperty(0)]
            public float Float { get; set; }
        }

        [Test]
        public void Test_Building_A_Tree_For_A_Class_With_String_Property_Without_String_Attribute()
        {
            string propertyName = nameof(ClassWithStringPropertyWithoutStringAttribute.Value);

            BadTypeExceptionShouldBeThrownForType<ClassWithStringPropertyWithoutStringAttribute>
            (
                message: $"Property {propertyName} is missing a string location attribute."
            );
        }

        private class ClassWithStringPropertyWithoutStringAttribute
        {
            [SerializableProperty(0)]
            public string Value { get; set; }
        }

        [Test]
        public void Test_Building_A_Tree_For_A_Class_With_String_Property_With_Both_String_Attributes()
        {
            string propertyName = nameof(ClassWithStringPropertyWithBothStringAttributes.Value);
            
            BadTypeExceptionShouldBeThrownForType<ClassWithStringPropertyWithBothStringAttributes>
            (
                message: $"Property {propertyName} has duplicate string location attributes."
            );
        }

        private class ClassWithStringPropertyWithBothStringAttributes
        {
            [SerializableProperty(0)]
            [OffsetString]
            [InlineString(39)]
            public string Value { get; set; }
        }
        #endregion

        private static void BadTypeExceptionShouldBeThrownForType<T>(string message)
        {
            Action action = () => TreeBuilder.BuildTreeForType(typeof(T));

            action.Should()
                .ThrowExactly<BadTypeException>()
                .WithMessage(message)
                .Where(e => typeof(T) == e.Type);
        }

        private static void VerifyEdge<T>(IEdge edge, T expected, Func<T, object> extractor) where T : new()
        {
            Edge<T> actualEdge = edge.Should().BeOfType<Edge<T>>().Subject;
            T value = new T();

            actualEdge.SetChildValue(value, extractor(expected));

            extractor(value).Should().BeEquivalentTo(extractor(expected));

            object childValue = actualEdge.ExtractChildValue(value);
            if (childValue.GetType().IsValueType)
            {
                childValue.Should().BeEquivalentTo(extractor(expected));
            }
            else
            {
                childValue.Should().BeSameAs(extractor(expected));
            }
        }
    }
}
