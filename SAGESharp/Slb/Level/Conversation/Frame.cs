using SAGESharp.Extensions;
using System.IO;
using System.Text;

namespace SAGESharp.Slb.Level.Conversation
{
    public class Frame : ISlbObject
    {
        public int ToaAnimation { get; set;  }

        public int CharAnimation { get; set; }

        public int CameraPositionTarget { get; set; }

        public int CameraDistance { get; set; }

        public int StringIndex { get; set; }

        public string ConversationSounds { get; set; }

        #region ISlbObject
        public void ReadFrom(Stream stream)
        {
            var toaAnimation = stream.ForceReadInt();
            var charAnimation = stream.ForceReadInt();
            var cameraPositionTarget = stream.ForceReadInt();
            var cameraDistance = stream.ForceReadInt();
            var stringIndex = stream.ForceReadInt();

            // read the offset
            stream.ForceReadUInt();
            var conversationSoundsCharCount = stream.ForceReadByte();
            var conversationSounds = new StringBuilder();
            for (int n = 0; n < conversationSoundsCharCount; ++n)
            {
                conversationSounds.Append(stream.ForceReadByte().ToASCIIChar());
            }
            // read the null terminator charcater of the conversationSounds string
            stream.ForceReadByte();

            // First read everything then write everything
            // to prevent modifying the Identifier unless
            // it has been completely read
            ToaAnimation = toaAnimation;
            CharAnimation = charAnimation;
            CameraPositionTarget = cameraPositionTarget;
            CameraDistance = cameraDistance;
            StringIndex = stringIndex;
            ConversationSounds = conversationSounds.ToString();
        }

        public void WriteTo(Stream stream)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
