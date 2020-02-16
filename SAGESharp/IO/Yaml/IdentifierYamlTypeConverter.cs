/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUtils.Validations;
using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

using Identifier = SAGESharp.SLB.Identifier;

namespace SAGESharp.IO.Yaml
{
    internal sealed class IdentifierYamlTypeConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            Validate.ArgumentNotNull(type, nameof(type));

            return typeof(Identifier) == type;
        }

        public object ReadYaml(IParser parser, Type type)
        {
            Validate.ArgumentNotNull(parser, nameof(parser));
            Validate.ArgumentNotNull(type, nameof(type));
            ValidateType(type);

            string value = parser.Consume<Scalar>().Value;

            return Identifier.From(value);
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            Validate.ArgumentNotNull(emitter, nameof(emitter));
            Validate.ArgumentNotNull(value, nameof(value));
            Validate.ArgumentNotNull(type, nameof(type));
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
