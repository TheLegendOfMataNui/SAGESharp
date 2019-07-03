using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.OSI.ControlFlow
{
    public class OSINode : Node
    {
        public uint StartOffset { get; set; }
        public List<Instruction> Instructions { get; }

        public OSINode(uint startOffset, List<Instruction> instructions) : base()
        {
            this.StartOffset = startOffset;
            this.Instructions = instructions;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Instruction i in Instructions)
            {
                sb.AppendLine(i.ToString());
            }
            return sb.ToString();
        }
    }
}
