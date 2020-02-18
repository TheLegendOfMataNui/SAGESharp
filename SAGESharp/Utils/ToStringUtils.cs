/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using NUtils.Extensions;
using System.Collections.Generic;

namespace SAGESharp.Utils
{
    internal static class ToStringUtils
    {
        public static string ListToString<T>(this IList<T> values)
            => values?.Let(v => "{" + string.Join(", ", values) + "}") ?? string.Empty;
    }
}
