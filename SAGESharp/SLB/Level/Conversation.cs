/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using Equ;
using SAGESharp.IO;
using SAGESharp.IO.Binary;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAGESharp.SLB.Level
{
    public sealed class Conversation : IEquatable<Conversation>
    {
        [SerializableProperty(1, name: nameof(Conversation))]
        public IList<ConversationCharacter> Entries { get; set; }

        public bool Equals(Conversation other)
            => MemberwiseEqualityComparer<Conversation>.ByProperties.Equals(this, other);

        public override string ToString()
        {
            if (Entries == null)
            {
                return $"{nameof(Entries)}=null";
            }
            else if (Entries.Count != 0)
            {
                return $"{nameof(Entries)}=[({string.Join("),(", Entries)})]";
            }
            else
            {
                return $"{nameof(Entries)}=[]";
            }
        }

        public override bool Equals(object obj) => Equals(obj as Conversation);

        public override int GetHashCode()
            => MemberwiseEqualityComparer<Conversation>.ByProperties.GetHashCode(this);

        public static bool operator ==(Conversation left, Conversation right)
            => left?.Equals(right) ?? right?.Equals(left) ?? true;

        public static bool operator !=(Conversation left, Conversation right)
            => !(left == right);
    }

    public sealed class ConversationCharacter : IEquatable<ConversationCharacter>
    {
        [SerializableProperty(1)]
        public Identifier ToaName { get; set; }

        [SerializableProperty(2)]
        public Identifier CharName { get; set; }

        [SerializableProperty(3)]
        public Identifier CharCont { get; set; }

        [SerializableProperty(4)]
        public IList<Info> Entries { get; set; }

        public bool Equals(ConversationCharacter other)
            => MemberwiseEqualityComparer<ConversationCharacter>.ByProperties.Equals(this, other);

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

        public override bool Equals(object other) => Equals(other as ConversationCharacter);

        public override int GetHashCode()
            => MemberwiseEqualityComparer<ConversationCharacter>.ByProperties.GetHashCode(this);

        public static bool operator ==(ConversationCharacter left, ConversationCharacter right)
            => left?.Equals(right) ?? right?.Equals(left) ?? true;

        public static bool operator !=(ConversationCharacter left, ConversationCharacter right)
            => !(left == right);
    }

    public sealed class Info : IEquatable<Info>
    {
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
            => MemberwiseEqualityComparer<Info>.ByProperties.Equals(this, other);

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

        public override bool Equals(object other) => Equals(other as Info);

        public override int GetHashCode()
            => MemberwiseEqualityComparer<Info>.ByProperties.GetHashCode(this);

        public static bool operator ==(Info left, Info right)
            => left?.Equals(right) ?? right?.Equals(left) ?? true;

        public static bool operator !=(Info left, Info right) => !(left == right);
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
        [OffsetString]
        public string ConversationSounds { get; set; }

        public bool Equals(Frame other)
            => MemberwiseEqualityComparer<Frame>.ByProperties.Equals(this, other);

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

        public override bool Equals(object obj) => Equals(obj as Frame);

        public override int GetHashCode()
            => MemberwiseEqualityComparer<Frame>.ByProperties.GetHashCode(this);

        public static bool operator ==(Frame left, Frame right)
            => left?.Equals(right) ?? right?.Equals(left) ?? true;

        public static bool operator !=(Frame left, Frame right) => !(left == right);
    }
}
