using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.OSI.ControlFlow
{
    public static class Analyzer
    {
        private static TextNode ReconstructNodes(SubroutineGraph newGraph, SubroutineGraph oldGraph, Node startNode, Dictionary<Node, Node> mapping, List<Tuple<Node, Node, Jump.JumpType>> futureJumps)
        {
            List<Node> children = new List<Node>();
            TextNode result = new TextNode(startNode.ToString());

            foreach (Jump outJump in startNode.OutJumps.Values)
            {
                if (outJump.Destination == oldGraph.EndNode)
                {
                    result.CreateJumpTo(newGraph.EndNode, outJump.Type);
                }
                else
                {
                    if (!mapping.ContainsKey(outJump.Destination))
                    {
                        // Reconstruct the destination
                        mapping.Add(outJump.Destination, null); // Placeholder to avoid stack overflow
                        Node newOutJumpTarget = ReconstructNodes(newGraph, oldGraph, outJump.Destination, mapping, futureJumps);
                        newGraph.Nodes.Add(newOutJumpTarget);
                        mapping[outJump.Destination] = newOutJumpTarget;
                        result.CreateJumpTo(mapping[outJump.Destination], outJump.Type);
                    }
                    else if (mapping[outJump.Destination] == null)
                    {
                        // We are in the process of reconstructing the destination, so add to later jumps.
                        futureJumps.Add(new Tuple<Node, Node, Jump.JumpType>(startNode, outJump.Destination, outJump.Type));
                    }
                    else
                    {
                        // The destination is reconstructed and ready to go
                        result.CreateJumpTo(mapping[outJump.Destination], outJump.Type);
                    }
                }
            }

            for (int i = futureJumps.Count - 1; i >= 0; i--)
            {
                Tuple<Node, Node, Jump.JumpType> futureJump = futureJumps[i];
                if (mapping.ContainsKey(futureJump.Item1) && mapping.ContainsKey(futureJump.Item2)
                    && mapping[futureJump.Item1] != null && mapping[futureJump.Item2] != null)
                {
                    mapping[futureJump.Item1].CreateJumpTo(mapping[futureJump.Item2], futureJump.Item3);
                    futureJumps.RemoveAt(i);
                }
            }

            return result;
        }

        private static void AnalyzeIfElse(SubroutineGraph graph, TextNode node, Stack<Node> wipNodes)
        {
            wipNodes.Push(node);
            foreach (Jump outJump in node.OutJumps.Values)
            {
                if (!wipNodes.Contains(outJump.Destination))
                    AnalyzeIfElse(graph, (TextNode)outJump.Destination, wipNodes);
            }

            if (node.OutTrueJump != null && node.OutFalseJump != null && node.OutJumps.Count == 2)
            {
                if (node.OutTrueJump.Destination.OutAlwaysJump != null
                    && node.OutTrueJump.Destination.OutAlwaysJump.Destination == node.OutFalseJump.Destination
                    && node.OutTrueJump.Destination.OutJumps.Count == 1)
                {
                    // If branch
                    // Remove outTrueJump's node and add the text to node, and then switch over all the jumps that came from out of outTrueJump's node
                    Node trueNode = node.OutTrueJump.Destination;
                    node.Text += "if() {\n   " + trueNode.ToString().Replace("\n", "\n   ").TrimEnd(' ') + "}\n";

                    trueNode.OutAlwaysJump.Destination.InJumps.Remove(trueNode);
                    node.OutJumps.Remove(trueNode);
                    Node alwaysNode = node.OutFalseJump.Destination;
                    node.OutJumps.Remove(alwaysNode);
                    alwaysNode.InJumps.Remove(node);
                    graph.Nodes.Remove(trueNode);
                    node.CreateJumpTo(alwaysNode, Jump.JumpType.Always);
                }
                else if (node.OutTrueJump.Destination.OutAlwaysJump != null
                    && node.OutFalseJump.Destination.OutAlwaysJump != null
                    && node.OutTrueJump.Destination.OutAlwaysJump.Destination == node.OutFalseJump.Destination.OutAlwaysJump.Destination)
                {
                    // If & Else branches
                    Node trueNode = node.OutTrueJump.Destination;
                    Node falseNode = node.OutFalseJump.Destination;
                    Node alwaysNode = trueNode.OutAlwaysJump.Destination;
                    node.Text += "if () {\n   " + trueNode.ToString().Replace("\n", "\n   ").TrimEnd(' ') + "}\nelse {\n   " + falseNode.ToString().Replace("\n", "\n   ").TrimEnd(' ') + "}\n";

                    alwaysNode.InJumps.Remove(trueNode);
                    alwaysNode.InJumps.Remove(falseNode);
                    node.OutJumps.Remove(trueNode);
                    node.OutJumps.Remove(falseNode);
                    graph.Nodes.Remove(trueNode);
                    graph.Nodes.Remove(falseNode);
                    node.CreateJumpTo(alwaysNode, Jump.JumpType.Always);
                }
                else if (node.OutTrueJump.Destination.OutAlwaysJump != null
                    && node.OutTrueJump.Destination.OutAlwaysJump.Destination == node
                    && node.OutTrueJump.Destination.OutJumps.Count == 1
                    && node.OutTrueJump.Destination.OutJumps.ContainsKey(node))
                {
                    // Loop
                    Node bodyNode = node.OutTrueJump.Destination;

                    node.Text = "while (" + node.ToString().Replace("\n", "\n       ").TrimEnd(' ') + ") {\n   " + bodyNode.ToString().Replace("\n", "\n   ").TrimEnd(' ') + "}\n";
                    node.OutJumps.Remove(bodyNode);
                    node.InJumps.Remove(bodyNode);
                    graph.Nodes.Remove(bodyNode);

                    Jump oldFalseJump = node.OutFalseJump;
                    oldFalseJump.Destination.InJumps.Remove(node);
                    node.OutJumps.Remove(oldFalseJump.Destination);
                    node.CreateJumpTo(oldFalseJump.Destination, Jump.JumpType.Always);
                }
            }

            if (node.OutJumps.Count == 1 && node.OutAlwaysJump.Destination.InJumps.Count == 1)
            {
                // Merge with following node
                Node follower = node.OutAlwaysJump.Destination;
                graph.Nodes.Remove(follower);

                foreach (Jump followerOutJump in follower.OutJumps.Values)
                {
                    followerOutJump.Destination.InJumps.Remove(follower);
                    node.CreateJumpTo(followerOutJump.Destination, followerOutJump.Type);
                }

                node.OutJumps.Remove(follower);

                node.Text += follower.ToString();
            }

            if (wipNodes.Pop() != node)
                throw new Exception();
        }

        public static SubroutineGraph ReconstructControlFlow(SubroutineGraph graph)
        {
            SubroutineGraph result = new SubroutineGraph();
            TextNode entryNode = ReconstructNodes(result, graph, graph.StartNode.OutAlwaysJump.Destination, new Dictionary<Node, Node>(), new List<Tuple<Node, Node, Jump.JumpType>>());
            result.Nodes.Add(entryNode);
            result.StartNode.CreateJumpTo(entryNode, Jump.JumpType.Always);

            AnalyzeIfElse(result, result.StartNode, new Stack<Node>());

            return result;
        }
    }
}
