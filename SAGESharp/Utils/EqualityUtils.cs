/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using System;

namespace SAGESharp.Utils
{
    internal static class EqualityUtils
    {
        public static bool EqualsOperator<T>(T left, T right) where T : class, IEquatable<T>
            => left?.Equals(right) ?? right?.Equals(left) ?? true;
    }
}
