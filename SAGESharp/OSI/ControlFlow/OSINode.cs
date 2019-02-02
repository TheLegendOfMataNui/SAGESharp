using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.OSI.ControlFlow
{
    public class OSINode : Node
    {
        public List<OSIInstruction> Instructions { get; }

        public OSINode(List<OSIInstruction> instructions) : base()
        {
            this.Instructions = instructions;
        }
    }
}
