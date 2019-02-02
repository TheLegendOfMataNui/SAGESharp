using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.OSI.ControlFlow
{
    public class Jump
    {
        public enum JumpType
        {
            Always,
            ConditionalTrue,
            ConditionalFalse,
        }

        public Node Source { get; }
        public Node Destination { get; }
        public JumpType Type { get; }

        public Jump(Node source, Node destination, JumpType type)
        {
            this.Source = source;
            this.Destination = destination;
            this.Type = type;
        }
    }
}
