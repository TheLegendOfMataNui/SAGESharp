using System.Text;

namespace SAGESharp.SLB.Level.Conversation
{
    public class Frame
    {
        public int ToaAnimation { get; set;  }

        public int CharAnimation { get; set; }

        public int CameraPositionTarget { get; set; }

        public int CameraDistance { get; set; }

        public int StringIndex { get; set; }

        public string ConversationSounds { get; set; }

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
    }
}
