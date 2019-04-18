/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using Konvenience;
using SAGESharp.SLB.IO;
using System;
using System.Collections.Generic;

namespace SAGESharp.SLB.Cinematic.Object
{
    public sealed class Object : IEquatable<Object>
    {
        [SLBElement(1)]
        public Identifier Instance { get; set; }

        [SLBElement(2)]
        public Identifier Type { get; set; }

        [SLBElement(3)]
        public IList<Location> Locations { get; set; }



        public bool Equals(Object other)
        {
            if (other == null)
            {
                return false;
            }

            return Instance == other.Instance &&
                Type == other.Type &&
                Locations.SafeSequenceEquals(other.Locations);
        }

        public override string ToString() => $"Instance={Instance}," +
            $"Type={Type}," +
            $"Locations={Locations?.Let(Locations => "[(" + string.Join("), (", Locations) + ")]") ?? "null"}";

        public override bool Equals(object other)
            => Equals(other as Object);

        public override int GetHashCode()
        {
            int hash = 9787;
            Instance.AddHashCodeByVal(ref hash, 9323);
            Type.AddHashCodeByVal(ref hash, 9323);
            Locations.AddHashCodesByRef(ref hash, 9323, 7699);

            return hash;
        }

        public static bool operator ==(Object left, Object right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            else if (left is null)
            {
                return right.Equals(left);
            }
            else
            {
                return left.Equals(right);
            }
        }

        public static bool operator !=(Object left, Object right)
            => !(left == right);
    }
}
