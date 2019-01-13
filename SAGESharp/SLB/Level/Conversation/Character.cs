using System.Collections.Generic;

namespace SAGESharp.SLB.Level.Conversation
{
    public class Character
    {
        public Identifier ToaName { get; set; }

        public Identifier CharName { get; set; }

        public Identifier CharCont { get; set; }

        public IList<Info> Entries { get; } = new List<Info>();
    }
}
