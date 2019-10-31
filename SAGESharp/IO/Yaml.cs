/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.TypeInspectors;

using Identifier = SAGESharp.SLB.Identifier;

namespace SAGESharp.IO
{
    /// <summary>
    /// Provides serializers to convert to Yaml.
    /// </summary>
    public static class YamlSerializer
    {
        public static ISerializer BuildSLBSerializer() => new SerializerBuilder()
                .WithSLBOverrides()
                .Build();
    }

    /// <summary>
    /// Provides serializers to convert from Yaml.
    /// </summary>
    public static class YamlDeserializer
    {
        public static IDeserializer BuildSLBDeserializer() => new DeserializerBuilder()
                .WithSLBOverrides()
                .Build();
    }

    internal static class BuilderSkeletonExtensions
    {
        public static TBuilder WithSLBOverrides<TBuilder>(this TBuilder builderSkeleton) where TBuilder : BuilderSkeleton<TBuilder>
        {
            return builderSkeleton
                .WithTypeConverter(new IdentifierYamlTypeConverter())
                .WithTypeInspector(inner => new SLBTypeInspector(inner), s => s.InsteadOf<YamlAttributesTypeInspector>());
        }
    }

    internal sealed class SLBTypeInspector : TypeInspectorSkeleton
    {
        private readonly ITypeInspector typeInspector;

        public SLBTypeInspector(ITypeInspector typeInspector)
        {
            this.typeInspector = typeInspector;
        }

        public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
        {
            return typeInspector.GetProperties(type, container)
                .Select(ConvertPropertyDescriptor)
                .Where(p => !(p is null));
        }

        private static IPropertyDescriptor ConvertPropertyDescriptor(IPropertyDescriptor propertyDescriptor)
        {
            SerializablePropertyAttribute attribute = propertyDescriptor.GetCustomAttribute<SerializablePropertyAttribute>();

            if (attribute is null)
            {
                return null;
            }

            PropertyDescriptor result = new PropertyDescriptor(propertyDescriptor);

            if (attribute.Name != null)
            {
                result.Name = attribute.Name;
            }

            result.Order = attribute.BinaryOrder;

            return result;
        }
    }

    internal sealed class IdentifierYamlTypeConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            Validate.ArgumentNotNull(nameof(type), type);

            return typeof(Identifier) == type;
        }

        public object ReadYaml(IParser parser, Type type)
        {
            Validate.ArgumentNotNull(nameof(parser), parser);
            Validate.ArgumentNotNull(nameof(type), type);
            ValidateType(type);

            string value = parser.Consume<Scalar>().Value;

            return Identifier.From(value);
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            Validate.ArgumentNotNull(nameof(emitter), emitter);
            Validate.ArgumentNotNull(nameof(value), value);
            Validate.ArgumentNotNull(nameof(type), type);
            Validate.Argument(value.GetType() == typeof(Identifier),
                $"The input value is type {value.GetType().Name}, was expecting {typeof(Identifier).Name} instead.");
            ValidateType(type);

            emitter.Emit(new Scalar(null, value.ToString()));
        }

        private static void ValidateType(Type type) => Validate.Argument(
            type == typeof(Identifier),
            $"Was expecting type {typeof(Identifier).Name} but found {type.Name}"
        );
    }
}
