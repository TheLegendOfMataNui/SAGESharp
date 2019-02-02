using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.OSI.ControlFlow
{
    public class Node
    {
        public Dictionary<Node, Jump> OutJumps { get; }
        public Dictionary<Node, Jump> InJumps { get; }

        public Node()
        {
            OutJumps = new Dictionary<Node, Jump>();
            InJumps = new Dictionary<Node, Jump>();
        }

        public Jump CreateJumpTo(Node other, Jump.JumpType type)
        {
            Jump result = new Jump(this, other, type);
            this.OutJumps.Add(other, result);
            other.InJumps.Add(this, result);
            return result;
        }
    }
}
