using System.Collections.Generic;

namespace SAGESharp.Slb.Level.Conversation
{
    public class Character
    {
        public Identifier ToaName { get; } = new Identifier();

        public Identifier CharName { get; } = new Identifier();

        public Identifier CharCont { get; } = new Identifier();

        public IList<Info> Entries { get; } = new List<Info>();
    }
}
