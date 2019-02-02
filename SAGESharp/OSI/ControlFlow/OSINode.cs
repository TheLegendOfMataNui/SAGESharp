using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.OSI.ControlFlow
{
    public class OSINode : Node
    {
        public List<Instruction> Instructions { get; }

        public OSINode(List<Instruction> instructions) : base()
        {
            this.Instructions = instructions;
        }
    }
}
