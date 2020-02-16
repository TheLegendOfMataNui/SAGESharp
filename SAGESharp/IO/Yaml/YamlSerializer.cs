﻿/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using YamlDotNet.Serialization;

namespace SAGESharp.IO.Yaml
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
}
