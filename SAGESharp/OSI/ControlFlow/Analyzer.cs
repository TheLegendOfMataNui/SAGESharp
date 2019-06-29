using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAGESharp.LSS;
using SAGESharp.LSS.Statements;
using SAGESharp.LSS.Expressions;

namespace SAGESharp.OSI.ControlFlow
{
    public class LSSNode : Node
    {
        public List<InstructionStatement> Statements { get; }
        //public Stack<Expression> StackLeftOver { get; }
        public Expression EndConditional { get; }

        public LSSNode(List<InstructionStatement> statements/*, Stack<Expression> stackLeftOver*/, Expression endConditional)
        {
            this.Statements = statements;
            //this.StackLeftOver = stackLeftOver;
            this.EndConditional = endConditional;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Statement s in Statements)
            {
                sb.AppendLine(PrettyPrinter.Print(s));
            }
            return sb.ToString();
        }
    }

    public static class Analyzer
    {
        private static Node ReconstructNodes(SubroutineGraph newGraph, SubroutineGraph oldGraph, Node startNode, Decompiler.SubroutineContext decompileContext, Dictionary<Node, Node> mapping, List<Tuple<Node, Node, Jump.JumpType>> futureJumps)
        {
            List<Node> children = new List<Node>();
            Node result; // new TextNode(startNode.ToString());

            //Stack<Expression> stackAfterThisNode = decompileContext.Stack;
            //Expression conditional = null; // Pop the condition of an if / while before decompiling the following chunks
            if (startNode is TextNode textStart)
            {
                result = new TextNode(textStart.Text);
            }
            else if (startNode is SAGESharp.OSI.ControlFlow.OSINode osiStart)
            {
                //result = new OSINode(osiStart.Instructions);
                List<InstructionStatement> statements = Decompiler.DecompileSubroutineChunk(decompileContext, osiStart.Instructions);
                result = new LSSNode(statements, startNode.OutAlwaysJump == null ? decompileContext.Stack.Pop() : null);
                /*if (startNode.OutAlwaysJump == null)
                {
                    conditional = decompileContext.Stack.Pop();
                }*/
            }
            else if (startNode is LSSNode lssStart)
            {
                result = new LSSNode(lssStart.Statements/*, lssStart.StackLeftOver*/, lssStart.EndConditional);
            }
            else
            {
                throw new Exception("o no");
            }

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
                        Node newOutJumpTarget = ReconstructNodes(newGraph, oldGraph, outJump.Destination, decompileContext.CloneStack(), mapping, futureJumps);
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

        private static void AnalyzeIfElse(SubroutineGraph graph, LSSNode node, Stack<Node> wipNodes)
        {
            wipNodes.Push(node);
            foreach (Jump outJump in node.OutJumps.Values)
            {
                if (outJump.Destination is LSSNode lssDestination)
                {
                    if (!wipNodes.Contains(outJump.Destination))
                        AnalyzeIfElse(graph, lssDestination, wipNodes);
                }
                else if (outJump.Destination == graph.EndNode)
                {

                }
                else
                {
                    throw new Exception("o no!");
                }
            }

            if (node.OutTrueJump != null && node.OutFalseJump != null && node.OutJumps.Count == 2)
            {
                if (node.OutTrueJump.Destination.OutAlwaysJump != null
                    && node.OutTrueJump.Destination.OutAlwaysJump.Destination == node.OutFalseJump.Destination
                    && node.OutTrueJump.Destination.OutJumps.Count == 1)
                {
                    // If branch
                    // Remove outTrueJump's node and add the text to node, and then switch over all the jumps that came from out of outTrueJump's node
                    LSSNode trueNode = node.OutTrueJump.Destination as LSSNode;
                    node.Statements.Add(new IfStatement(new SourceSpan(), node.EndConditional, new BlockStatement(new SourceSpan(), trueNode.Statements), null)); ;

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
                    LSSNode trueNode = node.OutTrueJump.Destination as LSSNode;
                    LSSNode falseNode = node.OutFalseJump.Destination as LSSNode;
                    Node alwaysNode = trueNode.OutAlwaysJump.Destination;
                    IfStatement elseIf = null;
                    if (falseNode?.Statements.Count == 1 && falseNode.Statements[0] is IfStatement innerIf)
                    {
                        elseIf = innerIf;
                    }
                    else
                    {
                        elseIf = new IfStatement(new SourceSpan(), null, new BlockStatement(new SourceSpan(), falseNode.Statements), null);
                    }
                    node.Statements.Add(new IfStatement(new SourceSpan(), node.EndConditional, new BlockStatement(new SourceSpan(), trueNode.Statements), elseIf));

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
                    LSSNode bodyNode = node.OutTrueJump.Destination as LSSNode;

                    node.Statements.Add(new WhileStatement(new SourceSpan(), node.EndConditional, new BlockStatement(new SourceSpan(), bodyNode.Statements)));

                    node.OutJumps.Remove(bodyNode);
                    node.InJumps.Remove(bodyNode);
                    graph.Nodes.Remove(bodyNode);

                    Jump oldFalseJump = node.OutFalseJump;
                    oldFalseJump.Destination.InJumps.Remove(node);
                    node.OutJumps.Remove(oldFalseJump.Destination);
                    node.CreateJumpTo(oldFalseJump.Destination, Jump.JumpType.Always);
                }
            }

            if (node.OutJumps.Count == 1 && node.OutAlwaysJump.Destination.InJumps.Count == 1 /*&& node != graph.StartNode*/ && node.OutAlwaysJump.Destination != graph.EndNode)
            {
                // Merge with following node
                LSSNode follower = node.OutAlwaysJump.Destination as LSSNode;
                graph.Nodes.Remove(follower);

                foreach (Jump followerOutJump in follower.OutJumps.Values)
                {
                    followerOutJump.Destination.InJumps.Remove(follower);
                    node.CreateJumpTo(followerOutJump.Destination, followerOutJump.Type);
                }

                node.OutJumps.Remove(follower);

                node.Statements.AddRange(follower.Statements);
            }

            if (wipNodes.Pop() != node)
                throw new Exception();
        }

        public static SubroutineGraph ReconstructControlFlow(SubroutineGraph graph, Decompiler.SubroutineContext decompileContext)
        {
            SubroutineGraph result = new SubroutineGraph();
            Node entryNode = ReconstructNodes(result, graph, graph.StartNode.OutAlwaysJump.Destination, decompileContext, new Dictionary<Node, Node>(), new List<Tuple<Node, Node, Jump.JumpType>>());
            result.Nodes.Add(entryNode);
            result.StartNode.CreateJumpTo(entryNode, Jump.JumpType.Always);

            AnalyzeIfElse(result, result.StartNode.OutAlwaysJump.Destination as LSSNode, new Stack<Node>());

            return result;
        }
    }
}
