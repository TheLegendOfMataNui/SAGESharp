/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using YamlDotNet.Serialization;

namespace SAGESharp.IO.Yaml
{
    /// <summary>
    /// Provides serializers to convert from Yaml.
    /// </summary>
    public static class YamlDeserializer
    {
        public static IDeserializer BuildSLBDeserializer() => new DeserializerBuilder()
                .WithSLBOverrides()
                .Build();
    }
}
