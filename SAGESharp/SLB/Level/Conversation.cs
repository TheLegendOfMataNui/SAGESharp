/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using SAGESharp.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAGESharp.SLB.Level
{
    public sealed class Character : IEquatable<Character>
    {
        /// <summary>
        /// The size of a <see cref="Character"/> entry in a binary SLB file.
        /// </summary>
        /// 
        /// This includes only <see cref="ToaName"/>, <see cref="CharName"/>,
        /// <see cref="CharCont"/>, the size of <see cref="Entries"/> and the
        /// position (offset) for all the entries (not represented in <see cref="Character"/>)
        /// in that order.
        internal const int BINARY_SIZE = 20;

        [SerializableProperty(1)]
        public Identifier ToaName { get; set; }

        [SerializableProperty(2)]
        public Identifier CharName { get; set; }

        [SerializableProperty(3)]
        public Identifier CharCont { get; set; }

        [SerializableProperty(4)]
        public IList<Info> Entries { get; set; }

        public bool Equals(Character other)
        {
            if (other is null)
            {
                return false;
            }

            return ToaName == other.ToaName &&
                CharName == other.CharName &&
                CharCont == other.CharCont &&
                Entries.SafeSequenceEquals(other.Entries);
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat("ToaName={0}", ToaName).Append(", ");
            result.AppendFormat("CharName={0}", ToaName).Append(", ");
            result.AppendFormat("CharCont={0}", ToaName).Append(", ");
            if (Entries == null)
            {
                result.Append("Entries=null");
            }
            else if (Entries.Count != 0)
            {
                result.AppendFormat("Entries=[({0})]", string.Join("), (", Entries));
            }
            else
            {
                result.Append("Entries=[]");
            }

            return result.ToString();
        }

        public override bool Equals(object other)
        {
            return Equals(other as Character);
        }

        public override int GetHashCode()
        {
            int hash = 19;
            ToaName.AddHashCodeByVal(ref hash, 89);
            CharName.AddHashCodeByVal(ref hash, 89);
            CharCont.AddHashCodeByVal(ref hash, 89);
            Entries.AddHashCodesByRef(ref hash, 89, 53);

            return hash;
        }

        public static bool operator ==(Character left, Character right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            else if(left is null)
            {
                return right.Equals(left);
            }
            else
            {
                return left.Equals(right);
            }
        }

        public static bool operator !=(Character left, Character right)
        {
            return !(left == right);
        }
    }

    public sealed class Info : IEquatable<Info>
    {
        /// <summary>
        /// The size in bytes of an <see cref="Info"/> object in a binary SLB file.
        /// </summary>
        /// 
        /// This include all the properties (as 32 bit numbers) except by
        /// <see cref="Frames"/>, for those the size and an offset for the
        /// actual entries are saved instead.
        internal const int BINARY_SIZE = 28;

        [SerializableProperty(1)]
        public LineSide LineSide { get; set; }

        [SerializableProperty(2)]
        public uint ConditionStart { get; set; }

        [SerializableProperty(3)]
        public uint ConditionEnd { get; set; }

        [SerializableProperty(4)]
        public Identifier StringLabel { get; set; }

        [SerializableProperty(5)]
        public int StringIndex { get; set; }

        [SerializableProperty(6)]
        public IList<Frame> Frames { get; set; }

        public bool Equals(Info other)
        {
            if (other == null)
            {
                return false;
            }

            return LineSide == other.LineSide &&
                ConditionStart == other.ConditionStart &&
                ConditionEnd == other.ConditionEnd &&
                StringLabel == other.StringLabel &&
                StringIndex == other.StringIndex &&
                Frames.SafeSequenceEquals(other.Frames);
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat("LineSide={0}", LineSide).Append(", ");
            result.AppendFormat("ConditionStart={0}", ConditionStart).Append(", ");
            result.AppendFormat("ConditionEnd={0}", ConditionEnd).Append(", ");
            result.AppendFormat("StringLabel={0}", StringLabel).Append(", ");
            result.AppendFormat("StringIndex={0}", StringIndex).Append(", ");
            if (Frames == null)
            {
                result.Append("Frames=null");
            }
            else if (Frames.Count != 0)
            {
                result.AppendFormat("Frames=[({0})]", string.Join("), (", Frames));
            }
            else
            {
                result.Append("Frames=[]");
            }

            return result.ToString();
        }

        public override bool Equals(object other)
        {
            return Equals(other as Info);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            LineSide.AddHashCodeByVal(ref hash, 73);
            ConditionStart.AddHashCodeByVal(ref hash, 73);
            ConditionEnd.AddHashCodeByVal(ref hash, 73);
            StringLabel.AddHashCodeByVal(ref hash, 73);
            StringIndex.AddHashCodeByVal(ref hash, 73);
            Frames.AddHashCodesByRef(ref hash, 73, 37);

            return hash;
        }

        public static bool operator ==(Info left, Info right)
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

        public static bool operator !=(Info left, Info right)
        {
            return !(left == right);
        }
    }

    [Flags]
    public enum LineSide
    {
        None = 0x00,
        Right = 0x01,
        Left = 0x02
    }

    public sealed class Frame : IEquatable<Frame>
    {
        /// <summary>
        /// The size in bytes of an <see cref="Frame"/> in a binary SLB file.
        /// </summary>
        /// 
        /// This includes all the properties (as 32 bit numbers) except by
        /// <see cref="ConversationSounds"/>, for that property the only the
        /// offset where the actual string is located is stored instead.
        internal const int BINARY_SIZE = 24;

        [SerializableProperty(1)]
        public int ToaAnimation { get; set; }

        [SerializableProperty(2)]
        public int CharAnimation { get; set; }

        [SerializableProperty(3)]
        public int CameraPositionTarget { get; set; }

        [SerializableProperty(4)]
        public int CameraDistance { get; set; }

        [SerializableProperty(5)]
        public int StringIndex { get; set; }

        [SerializableProperty(6)]
        public string ConversationSounds { get; set; }

        public bool Equals(Frame other)
        {
            if (other == null)
            {
                return false;
            }

            return ToaAnimation == other.ToaAnimation &&
                CharAnimation == other.CharAnimation &&
                CameraPositionTarget == other.CameraPositionTarget &&
                CameraDistance == other.CameraDistance &&
                StringIndex == other.StringIndex &&
                ConversationSounds.SafeEquals(other.ConversationSounds);
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat("ToaAnimation={0}", ToaAnimation).Append(", ");
            result.AppendFormat("CharAnimation={0}", CharAnimation).Append(", ");
            result.AppendFormat("CameraPositionTarget={0}", CameraPositionTarget).Append(", ");
            result.AppendFormat("CameraDistance={0}", CameraDistance).Append(", ");
            result.AppendFormat("StringIndex={0}", StringIndex).Append(", ");
            result.AppendFormat("ConversationSounds={0}", ConversationSounds);

            return result.ToString();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Frame);
        }

        public override int GetHashCode()
        {
            var hash = 31;
            ToaAnimation.AddHashCodeByVal(ref hash, 89);
            CharAnimation.AddHashCodeByVal(ref hash, 89);
            CameraPositionTarget.AddHashCodeByVal(ref hash, 89);
            CameraDistance.AddHashCodeByVal(ref hash, 89);
            StringIndex.AddHashCodeByVal(ref hash, 89);
            ConversationSounds.AddHashCodeByRef(ref hash, 89);

            return hash;
        }

        public static bool operator ==(Frame left, Frame right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            else if (right is null)
            {
                return left.Equals(right);
            }
            else
            {
                return right.Equals(left);
            }
        }

        public static bool operator !=(Frame left, Frame right)
        {
            return !(left == right);
        }
    }
}
