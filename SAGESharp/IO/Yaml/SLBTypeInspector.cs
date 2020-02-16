/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.TypeInspectors;

namespace SAGESharp.IO.Yaml
{
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
}
