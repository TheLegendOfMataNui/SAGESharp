/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using YamlDotNet.Serialization;

namespace SAGESharp.IO.Yaml
{
    internal static class BuilderSkeletonExtensions
    {
        public static TBuilder WithSLBOverrides<TBuilder>(this TBuilder builderSkeleton) where TBuilder : BuilderSkeleton<TBuilder>
        {
            return builderSkeleton
                .WithTypeConverter(new IdentifierYamlTypeConverter())
                .WithTypeInspector(inner => new SLBTypeInspector(inner), s => s.InsteadOf<YamlAttributesTypeInspector>());
        }
    }
}
