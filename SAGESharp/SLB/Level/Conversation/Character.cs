using System.Collections.Generic;
using System.Text;

namespace SAGESharp.SLB.Level.Conversation
{
    public class Character
    {
        public Identifier ToaName { get; set; }

        public Identifier CharName { get; set; }

        public Identifier CharCont { get; set; }

        public IList<Info> Entries { get; set; }

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
    }
}
