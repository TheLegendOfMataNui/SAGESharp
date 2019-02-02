using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.OSI.ControlFlow
{
    public class SubroutineGraph
    {
        public Node StartNode { get; }
        public Node EndNode { get; }
        public List<Node> Nodes { get; }

        public SubroutineGraph()
        {
            StartNode = new Node();
            EndNode = new Node();
            Nodes = new List<Node>();
        }

        public SubroutineGraph(IEnumerable<Instruction> instructions, uint bytecodeBaseOffset) : this()
        {
            Dictionary<long, Instruction> instructionLocations = new Dictionary<long, Instruction>();
            Dictionary<long, Node> nodeLocations = new Dictionary<long, Node>();

            // For making jumps to nodes that might now exist yet
            List<Tuple<Node, long, Jump.JumpType>> delayedJumps = new List<Tuple<Node, long, Jump.JumpType>>();

            OSINode currentNode = new OSINode(new List<Instruction>());
            StartNode.CreateJumpTo(currentNode, Jump.JumpType.Always);

            Nodes.Add(StartNode);

            uint offset = bytecodeBaseOffset;

            nodeLocations.Add(offset, currentNode);
            Nodes.Add(currentNode);

            foreach (Instruction ins in instructions)
            {
                if (ins is BCLInstruction bcl)
                {
                    // Check whether we need to start a new block as a jump target
                    if (delayedJumps.Any((dj) => dj.Item2 == offset) && !nodeLocations.ContainsKey(offset)) {
                        OSINode nextNode = new OSINode(new List<Instruction>());
                        Nodes.Add(nextNode);
                        nodeLocations.Add(offset, nextNode);
                        currentNode.CreateJumpTo(nextNode, Jump.JumpType.Always);

                        currentNode = nextNode;
                    }

                    // Check whether we need to start a new block after this instruction as a jump source
                    if (bcl.Opcode == BCLOpcode.Return)
                    {
                        currentNode.Instructions.Add(bcl);

                        currentNode.CreateJumpTo(EndNode, Jump.JumpType.Always);

                        currentNode = new OSINode(new List<Instruction>());
                        Nodes.Add(currentNode);
                        nodeLocations.Add(offset + ins.Size, currentNode);
                    }
                    else if (bcl.Opcode == BCLOpcode.CompareAndBranchIfFalse)
                    {
                        currentNode.Instructions.Add(bcl);

                        short delta = bcl.Arguments[0].GetValue<short>();

                        if (delta == 0)
                        {
                            // An empty node...
                            OSINode trueNode = new OSINode(new List<Instruction>());
                            currentNode.CreateJumpTo(trueNode, Jump.JumpType.ConditionalTrue);
                            Nodes.Add(trueNode);
                            // HACK: Don't add trueNode's location - it's empty, so nobody should want it. Let the false node own the location.

                            OSINode falseNode = new OSINode(new List<Instruction>());
                            currentNode.CreateJumpTo(falseNode, Jump.JumpType.ConditionalFalse);
                            Nodes.Add(falseNode);
                            nodeLocations.Add(offset + ins.Size, falseNode);

                            trueNode.CreateJumpTo(falseNode, Jump.JumpType.Always);

                            currentNode = falseNode;
                        }
                        else
                        {
                            delayedJumps.Add(new Tuple<Node, long, Jump.JumpType>(currentNode, offset + delta + ins.Size, Jump.JumpType.ConditionalFalse));

                            OSINode trueNode = new OSINode(new List<Instruction>());
                            Nodes.Add(trueNode);
                            nodeLocations.Add(offset + ins.Size, trueNode);
                            currentNode.CreateJumpTo(trueNode, Jump.JumpType.ConditionalTrue);

                            currentNode = trueNode;
                        }
                    }
#if true
                    else if (bcl.Opcode == BCLOpcode.BranchAlways)
                    {
                        currentNode.Instructions.Add(bcl);

                        short delta = bcl.Arguments[0].GetValue<short>();

                        if (delta == 0)
                        {
                            throw new NotImplementedException();
                        }
                        else
                        {
                            delayedJumps.Add(new Tuple<Node, long, Jump.JumpType>(currentNode, offset + delta + ins.Size, Jump.JumpType.Always));

                            currentNode = new OSINode(new List<Instruction>());
                            Nodes.Add(currentNode);
                            nodeLocations.Add(offset + ins.Size, currentNode);
                        }

                    }
#endif
                    else 
                    {
                        currentNode.Instructions.Add(bcl);
                    }
                }
                else
                {
                    currentNode.Instructions.Add(ins);
                }

                offset += ins.Size;
            }

            foreach (Tuple<Node, long, Jump.JumpType> j in delayedJumps)
            {
                Node dest = nodeLocations[j.Item2];
                if (j.Item1.OutJumps.ContainsKey(dest))
                {
                    if (j.Item1.OutJumps[dest].Type != j.Item3)
                    {
                        throw new Exception("Cannot have two jumps of different types for the same jump pair");
                    }
                }
                else
                {
                    j.Item1.CreateJumpTo(dest, j.Item3);
                }
            }

            if (currentNode.InJumps.Count == 0 && currentNode.OutJumps.Count == 0)
            {
                Nodes.Remove(currentNode);
            }

            Nodes.Add(EndNode);
        }
    }
}
