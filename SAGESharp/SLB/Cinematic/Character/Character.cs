using Konvenience;
using SAGESharp.SLB.IO;
using System;
using System.Collections.Generic;

namespace SAGESharp.SLB.Cinematic.Character
{
    public sealed class Character : IEquatable<Character>
    {
        [SLBElement(1)]
        public Identifier AnimHierarchy { get; set; }

        [SLBElement(2)]
        public Identifier Character { get; set; }

        [SLBElement(3)]
        public Identifier AnimBaked { get; set; }

        [SLBElement(4)]
        public float SwitchMaskTimes1 { get; set; }

        [SLBElement(5)]
        public float SwitchMaskTimes2 { get; set; }

        [SLBElement(6)]
        public IList<Location> Locations { get; set; }



        public bool Equals(Character other)
        {
            if (other == null)
            {
                return false;
            }

            return AnimHierarchy == other.AnimHierarchy &&
                Character == other.Character &&
                AnimBaked == other.AnimBaked &&
                SwitchMaskTimes1 == other.SwitchMaskTimes1 &&
                SwitchMaskTimes2 == other.SwitchMaskTimes2 &&
                Locations.SafeSequenceEquals(other.Locations);
        }

        public override string ToString() => $"AnimHierarchy={AnimHierarchy}," +
            $"Character={Character}," +
            $"AnimBaked={AnimBaked}," +
            $"SwitchMaskTimes1={SwitchMaskTimes1}," +
            $"SwitchMaskTimes2={SwitchMaskTimes2}," +
            $"Locations={Locations?.Let(Locations => "[(" + string.Join("), (", Locations) + ")]") ?? "null"}";

        public override bool Equals(object other)
            => Equals(other as Character);

        public override int GetHashCode()
        {
            int hash = 7639;
            AnimHierarchy.AddHashCodeByVal(ref hash, 4539);
            Character.AddHashCodeByVal(ref hash, 4539);
            AnimBaked.AddHashCodeByVal(ref hash, 4539);
            SwitchMaskTimes1.AddHashCodeByVal(ref hash, 4539);
            SwitchMaskTimes2.AddHashCodeByVal(ref hash, 4539);
            Locations.AddHashCodesByRef(ref hash, 4539, 8219);

            return hash;
        }

        public static bool operator ==(Character left, Character right)
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

        public static bool operator !=(Character left, Character right)
            => !(left == right);
    }
}
