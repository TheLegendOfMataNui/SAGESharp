using System.Collections.Generic;

namespace SAGESharp.Slb.Level.Conversation
{
    public class Info
    {
        public uint LineSide { get; set; }

        public uint ConditionStart { get; set;  }

        public uint ConditionEnd { get; set; }

        public Identifier StringLabel { get; } = new Identifier();

        public int StringIndex { get; set;  }

        public IList<Frame> Frames { get; } = new List<Frame>();
    }
}
