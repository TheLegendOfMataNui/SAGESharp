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

        public Jump OutTrueJump
        {
            get
            {
                return OutJumps.FirstOrDefault(j => j.Value.Type == Jump.JumpType.ConditionalTrue).Value;
            }
        }

        public Jump OutFalseJump
        {
            get
            {
                return OutJumps.FirstOrDefault(j => j.Value.Type == Jump.JumpType.ConditionalFalse).Value;
            }
        }

        public Jump OutAlwaysJump
        {
            get
            {
                return OutJumps.FirstOrDefault(j => j.Value.Type == Jump.JumpType.Always).Value;
            }
        }

        public Node()
        {
            OutJumps = new Dictionary<Node, Jump>();
            InJumps = new Dictionary<Node, Jump>();
        }

        public Jump CreateJumpTo(Node destination, Jump.JumpType type)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            Jump result = new Jump(this, destination, type);
            this.OutJumps.Add(destination, result);
            destination.InJumps.Add(this, result);
            return result;
        }
    }

    public class TextNode : Node
    {
        public string Text { get; set; }

        public TextNode(string text)
        {
            this.Text = text;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
