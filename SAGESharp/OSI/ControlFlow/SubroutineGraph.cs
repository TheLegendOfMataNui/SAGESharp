using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.OSI.ControlFlow
{
    public class SubroutineGraph
    {
        public TextNode StartNode { get; }
        public TextNode EndNode { get; }
        public List<Node> Nodes { get; }

        public SubroutineGraph()
        {
            Nodes = new List<Node>();
            StartNode = new TextNode("<Start>");
            Nodes.Add(StartNode);
            EndNode = new TextNode("<End>");
            Nodes.Add(EndNode);
        }

        public SubroutineGraph(List<Instruction> instructions, uint bytecodeBaseOffset) : this()
        {
            Dictionary<long, Node> nodeLocations = new Dictionary<long, Node>();
            Dictionary<Node, long> nodeStarts = new Dictionary<Node, long>();
            Dictionary<Node, long> nodeLengths = new Dictionary<Node, long>();

            // For making jumps to nodes that might now exist yet
            List<Tuple<Node, long, Jump.JumpType>> delayedJumps = new List<Tuple<Node, long, Jump.JumpType>>();

            OSINode currentNode = new OSINode(new List<Instruction>());
            StartNode.CreateJumpTo(currentNode, Jump.JumpType.Always);


            uint offset = bytecodeBaseOffset;

            nodeLocations.Add(offset, currentNode);
            nodeStarts.Add(currentNode, offset);
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
                        nodeStarts.Add(nextNode, offset);
                        currentNode.CreateJumpTo(nextNode, Jump.JumpType.Always);
                        nodeLengths.Add(currentNode, offset + ins.Size - nodeStarts[currentNode]);

                        currentNode = nextNode;
                    }

                    // Check whether we need to start a new block after this instruction as a jump source
                    // HUUUUGE HACK: Don't make jumps to <End> for early returns, as this completely breaks control flow analysis.
                    if (bcl.Opcode == BCLOpcode.Return && ins == instructions[instructions.Count - 1])
                    {
                        currentNode.Instructions.Add(bcl);
                        nodeLengths.Add(currentNode, offset + ins.Size - nodeStarts[currentNode]);

                        currentNode.CreateJumpTo(EndNode, Jump.JumpType.Always);

                        currentNode = new OSINode(new List<Instruction>());
                        Nodes.Add(currentNode);
                        nodeLocations.Add(offset + ins.Size, currentNode);
                        nodeStarts.Add(currentNode, offset + ins.Size);
                    }
                    else if (bcl.Opcode == BCLOpcode.CompareAndBranchIfFalse)
                    {
                        //currentNode.Instructions.Add(bcl);

                        short delta = bcl.Arguments[0].GetValue<short>();

                        /*if (delta < 0)
                        {
                            throw new NotImplementedException();
                        }
                        else */if (delta == 0)
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
                            nodeStarts.Add(falseNode, offset + ins.Size);
                            nodeLengths.Add(currentNode, offset + ins.Size - nodeStarts[currentNode]);

                            trueNode.CreateJumpTo(falseNode, Jump.JumpType.Always);

                            currentNode = falseNode;
                        }
                        else
                        {
                            delayedJumps.Add(new Tuple<Node, long, Jump.JumpType>(currentNode, offset + delta + ins.Size, Jump.JumpType.ConditionalFalse));

                            OSINode trueNode = new OSINode(new List<Instruction>());
                            nodeLengths.Add(currentNode, offset + ins.Size - nodeStarts[currentNode]);
                            Nodes.Add(trueNode);
                            nodeLocations.Add(offset + ins.Size, trueNode);
                            nodeStarts.Add(trueNode, offset + ins.Size);
                            currentNode.CreateJumpTo(trueNode, Jump.JumpType.ConditionalTrue);

                            currentNode = trueNode;
                        }
                    }
                    else if (bcl.Opcode == BCLOpcode.BranchAlways)
                    {
                        //currentNode.Instructions.Add(bcl);

                        short delta = bcl.Arguments[0].GetValue<short>();

                        /*if (delta < 0)
                        {
                            //throw new NotImplementedException();
                            nodeLengths.Add(currentNode, offset + ins.Size - nodeStarts[currentNode]);

                            // Continue reading instructions into a new node
                            currentNode = new OSINode(new List<Instruction>());
                            Nodes.Add(currentNode);
                            nodeLocations.Add(offset + ins.Size, currentNode);
                            nodeStarts.Add(currentNode, offset + ins.Size);
                        }
                        else */if (delta == 0)
                        {
                            //throw new NotImplementedException();
                            // Simply ignore the jump!
                        }
                        else
                        {
                            delayedJumps.Add(new Tuple<Node, long, Jump.JumpType>(currentNode, offset + delta + ins.Size, Jump.JumpType.Always));
                            nodeLengths.Add(currentNode, offset + ins.Size - nodeStarts[currentNode]);

                            // Continue reading instructions into a new node
                            currentNode = new OSINode(new List<Instruction>());
                            Nodes.Add(currentNode);
                            nodeLocations.Add(offset + ins.Size, currentNode);
                            nodeStarts.Add(currentNode, offset + ins.Size);
                        }

                    }
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
            nodeLengths.Add(currentNode, offset - nodeStarts[currentNode]);

            foreach (Tuple<Node, long, Jump.JumpType> j in delayedJumps)
            {
                Node dest = null;
                if (nodeLocations.ContainsKey(j.Item2))
                {
                    // Jump to the beginning of the node! EZ!
                    dest = nodeLocations[j.Item2];
                }
                else
                {
                    // Find the containing node and split it
                    foreach (Node n in Nodes)
                    {
                        if (n is OSINode node && nodeStarts.ContainsKey(n))
                        {
                            if (j.Item2 >= nodeStarts[n] && j.Item2 < nodeStarts[n] + nodeLengths[n])
                            {
                                // Time to split j
                                long targetLength = j.Item2 - nodeStarts[n];
                                OSINode secondPart = new OSINode(new List<Instruction>());
                                Nodes.Add(secondPart);

                                // Transfer instructions to secondPart
                                long l = nodeLengths[n];
                                while (l > targetLength)
                                {
                                    Instruction ins = node.Instructions[node.Instructions.Count - 1];
                                    node.Instructions.RemoveAt(node.Instructions.Count - 1);
                                    secondPart.Instructions.Insert(0, ins);
                                    l -= ins.Size;
                                }

                                // Transfer outjumps to secondPart
                                foreach (Jump outJump in n.OutJumps.Values)
                                {
                                    secondPart.CreateJumpTo(outJump.Destination, outJump.Type);
                                    outJump.Destination.InJumps.Remove(n);
                                }
                                n.OutJumps.Clear();
                                n.CreateJumpTo(secondPart, Jump.JumpType.Always);

                                nodeLengths.Add(secondPart, nodeLengths[n] - l);
                                nodeLengths[n] = l;
                                nodeStarts.Add(secondPart, nodeStarts[n] + l);

                                dest = secondPart;

                                break;
                            }
                        }
                    }
                }
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
        }
    }
}
