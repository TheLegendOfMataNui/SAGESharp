/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUtils.Extensions;
using NUtils.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SAGESharp.IO.Binary.TreeBasedSerialization
{
    internal static class TreeBuilder
    {
        public static IDataNode BuildTreeForType(Type type)
        {
            Validate.ArgumentNotNull(type, nameof(type));

            return BuildUserTypeDataNode(type);
        }

        private static object BuildPaddedNodeForProperty(PropertyInfo propertyInfo)
        {
            object node = BuildNodeForProperty(propertyInfo);

            RightPaddingAttribute rightPaddingAttribute = propertyInfo
                .GetCustomAttribute<RightPaddingAttribute>();

            if (rightPaddingAttribute is null)
            {
                return node;
            }
            else if (node is IDataNode dataNode)
            {
                return new PaddingNode(rightPaddingAttribute.Size, dataNode);
            }
            else
            {
                throw new BadTypeException(propertyInfo.DeclaringType,
                    $"Property {propertyInfo.Name} cannot be padded.");
            }
        }

        private static object BuildNodeForProperty(PropertyInfo propertyInfo)
        {
            if (IsPrimitiveType(propertyInfo.PropertyType))
            {
                return PrimitiveTypeDataNode(propertyInfo.PropertyType);
            }
            else if (typeof(string) == propertyInfo.PropertyType)
            {
                return StringDataNode(propertyInfo);
            }
            else if (IsListType(propertyInfo.PropertyType))
            {
                return BuildListNode(propertyInfo);
            }
            else
            {
                return BuildUserTypeDataNode(propertyInfo.PropertyType);
            }
        }

        #region PrimitiveTypeDataNode
        private static IDataNode PrimitiveTypeDataNode(Type type)
        {
            return (IDataNode)typeof(PrimitiveTypeDataNode<>)
                .MakeGenericType(type)
                .GetConstructor(Array.Empty<Type>())
                .Invoke(Array.Empty<object>());
        }

        private static bool IsPrimitiveType(Type type)
        {
            return type == typeof(byte) || type == typeof(short) || type == typeof(ushort)
                 || type == typeof(int) || type == typeof(uint) || type == typeof(long)
                 || type == typeof(ulong) || type == typeof(SLB.Identifier)
                 || type == typeof(float) || type == typeof(double)
                 || (type.IsEnum && IsPrimitiveType(Enum.GetUnderlyingType(type)));
        }
        #endregion

        #region StringDataNode
        private static object StringDataNode(PropertyInfo propertyInfo)
        {
            OffsetStringAttribute offsetStringAttribute = propertyInfo
                .GetCustomAttribute<OffsetStringAttribute>();

            InlineStringAttribute inlineStringAttribute = propertyInfo
                .GetCustomAttribute<InlineStringAttribute>();

            if (offsetStringAttribute != null && inlineStringAttribute != null)
            {
                throw new BadTypeException(propertyInfo.DeclaringType,
                    $"Property {propertyInfo.Name} has duplicate string location attributes.");
            }
            else if (offsetStringAttribute != null)
            {
                return new OffsetNode(new StringDataNode());
            }
            else if (inlineStringAttribute != null)
            {
                return new StringDataNode(inlineStringAttribute.Length);
            }
            else
            {
                throw new BadTypeException(propertyInfo.DeclaringType,
                    $"Property {propertyInfo.Name} is missing a string location attribute.");
            }
        }
        #endregion

        #region UserTypeDataNode
        private static IDataNode BuildUserTypeDataNode(Type type)
        {
            ValidateUserTypeHasConstructor(type);

            IReadOnlyList<IEdge> edges = type.GetProperties()
                .Select(p => SerializableProperty.For(p))
                .Where(p => p != null)
                .OrderBy(p => p.Order)
                .Also(ps => ValidateSerializableProperties(type, ps))
                .Select(p => BuildEdge(p.PropertyInfo))
                .ToList();

            return (IDataNode)typeof(UserTypeDataNode<>)
                .MakeGenericType(type)
                .GetConstructor(new Type[] { typeof(IReadOnlyList<IEdge>) }).Invoke(new object[] { edges });
        }

        private static void ValidateUserTypeHasConstructor(Type type)
        {
            if (type.GetConstructor(Array.Empty<Type>()) is null)
            {
                throw new BadTypeException(type, "Type doesn't have a public parameterless constructor.");
            }
        }

        private static void ValidateSerializableProperties(Type type, IOrderedEnumerable<SerializableProperty> properties)
        {
            int count = properties.Count();
            if (count == 0)
            {
                throw new BadTypeException(type, "Type doesn't have serializable properties.");
            }
            else if (count != properties.Select(p => p.Order).Distinct().Count())
            {
                throw new BadTypeException(type, "Type has two or more properties with the same serialization order.");
            }
        }

        private class SerializableProperty
        {
            private SerializableProperty(PropertyInfo propertyInfo, byte order)
            {
                PropertyInfo = propertyInfo;
                Order = order;
            }

            public PropertyInfo PropertyInfo { get; }

            public byte Order { get; }

            public static SerializableProperty For(PropertyInfo propertyInfo) => propertyInfo
                .GetCustomAttribute<SerializablePropertyAttribute>()
                ?.Let(attribute => new SerializableProperty(propertyInfo, attribute.BinaryOrder));
        }
        #endregion

        #region Edge
        private static IEdge BuildEdge(PropertyInfo propertyInfo)
        {
            Type edgeType = typeof(Edge<>).MakeGenericType(propertyInfo.DeclaringType);
            Type extractorType = typeof(Func<,>).MakeGenericType(propertyInfo.DeclaringType, typeof(object));
            Type setterType = typeof(Action<,>).MakeGenericType(propertyInfo.DeclaringType, typeof(object));

            object extractor = GetExtractorForEdge(propertyInfo);
            object setter = GetSetterForEdge(propertyInfo);
            object childNode = BuildPaddedNodeForProperty(propertyInfo);

            return (IEdge)edgeType.GetConstructor(new Type[] { extractorType, setterType, typeof(object) })
                .Invoke(new object[] { extractor, setter, childNode });
        }

        private static object GetExtractorForEdge(PropertyInfo propertyInfo)
        {
            ParameterExpression valueParameter = Expression.Parameter(propertyInfo.DeclaringType);

            MethodCallExpression methodCall = Expression.Call(valueParameter, propertyInfo.GetMethod);

            UnaryExpression castedMethodCall = Expression.Convert(methodCall, typeof(object));

            LambdaExpression lambdaExpression = Expression.Lambda(castedMethodCall, valueParameter);

            return lambdaExpression.Compile();
        }

        private static object GetSetterForEdge(PropertyInfo propertyInfo)
        {
            ParameterExpression valueParameter = Expression.Parameter(propertyInfo.DeclaringType);
            ParameterExpression childValueParameter = Expression.Parameter(typeof(object));
            UnaryExpression castedChildValueParameter = Expression.Convert(childValueParameter, propertyInfo.PropertyType);

            MethodCallExpression methodCall = Expression.Call(valueParameter, propertyInfo.SetMethod, castedChildValueParameter);

            LambdaExpression lambdaExpression = Expression.Lambda(methodCall, valueParameter, childValueParameter);

            return lambdaExpression.Compile();
        }
        #endregion

        #region ListNode
        private static object BuildListNode(PropertyInfo propertyInfo)
        {
            Type typeOfList = propertyInfo.PropertyType.GenericTypeArguments[0];

            object childNode = BuildUserTypeDataNode(typeOfList);

            bool duplicateEntryCount = propertyInfo
                .GetCustomAttribute<DuplicateEntryCountAttribute>() != null;

            return typeof(ListNode<>)
                .MakeGenericType(typeOfList)
                .GetConstructor(new Type[] { typeof(IDataNode), typeof(bool) })
                .Invoke(new object[] { childNode, duplicateEntryCount });
        }

        private static bool IsListType(Type type)
        {
            return type.IsGenericType && typeof(IList<>) == type.GetGenericTypeDefinition();
        }
        #endregion
    }
}
