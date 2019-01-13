using System.IO;

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
            throw new System.NotImplementedException();
        }

        public void WriteTo(Stream stream)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
