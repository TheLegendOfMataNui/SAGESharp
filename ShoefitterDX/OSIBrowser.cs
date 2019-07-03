using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAGESharp.LSS;
using SAGESharp.OSI;
using SAGESharp.OSI.ControlFlow;

namespace ShoefitterDX
{
    public partial class OSIBrowser : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public OSIFile OSI { get; private set; } = null;

        public OSISubroutineInspector CurrentInspector { get; private set; }

        private TreeNode ClassesNode = null;
        private TreeNode FunctionsNode = null;

        public OSIBrowser()
        {
            InitializeComponent();
            TabText = "OSI Browser";
            DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document | WeifenLuo.WinFormsUI.Docking.DockAreas.Float;
        }

        public void LoadOSI(OSIFile osi)
        {
            this.OSI = osi;
            treeView1.Nodes.Clear();

            FunctionsNode = new TreeNode("Functions");
            foreach (OSIFile.FunctionInfo info in osi.Functions)
            {
                TreeNode node = new TreeNode(info.Name);
                node.Tag = info;
                FunctionsNode.Nodes.Add(node);
            }
            treeView1.Nodes.Add(FunctionsNode);

            ClassesNode = new TreeNode("Classes");
            foreach (OSIFile.ClassInfo classInfo in osi.Classes)
            {
                TreeNode classNode = new TreeNode(classInfo.Name);

                foreach (OSIFile.MethodInfo methodInfo in classInfo.Methods)
                {
                    TreeNode methodNode = new TreeNode(osi.Symbols[methodInfo.NameSymbol]);
                    methodNode.Tag = methodInfo;
                    classNode.Nodes.Add(methodNode);
                }

                ClassesNode.Nodes.Add(classNode);
            }
            treeView1.Nodes.Add(ClassesNode);
        }

        private void OSIBrowser_Load(object sender, EventArgs e)
        {

        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag is OSIFile.FunctionInfo func)
            {
                DisplayFunction(func);
            }
            else if (e.Node.Tag is OSIFile.MethodInfo meth)
            {
                DisplayMethod(meth);
            }
        }

        private void LoadGraph(ref SubroutineGraph g, string name, bool isMemberMethod, List<Token> parameters)
        {
            if (CurrentInspector != null)
            {
                InspectorPanel.Controls.Remove(CurrentInspector);
            }
            if (GeneratePseudocodeCheckBox.Checked)
            {
                g = Analyzer.ReconstructControlFlow(g, new Decompiler.SubroutineContext(OSI, name, isMemberMethod, parameters, new SourceSpan()));
            }
            CurrentInspector = new OSISubroutineInspector(g);
            if (!GeneratePseudocodeCheckBox.Checked)
            {
                g = Analyzer.ReconstructControlFlow(g, new Decompiler.SubroutineContext(OSI, name, isMemberMethod, parameters, new SourceSpan()));
            }
        }

        public void DisplayMethod(OSIFile.MethodInfo method)
        {
            SubroutineGraph g = new SubroutineGraph(method.Instructions, method.BytecodeOffset);
            List<Token> parameters = new List<Token>();
            if (method.Instructions.Count > 0 && method.Instructions[0] is BCLInstruction argCheck && argCheck.Opcode == BCLOpcode.MemberFunctionArgumentCheck)
            {
                for (int i = 0; i < argCheck.Arguments[0].GetValue<sbyte>() - 1; i++)
                {
                    parameters.Add(new Token(TokenType.Symbol, "param" + (i + 1), new SourceSpan()));
                }
            }
            LoadGraph(ref g, OSI.Symbols[method.NameSymbol], true, parameters);

            InspectorPanel.Controls.Add(CurrentInspector);
            CurrentInspector.Dock = DockStyle.Fill;
            if (g.Nodes.Count == 3)
            {
                try
                {
                    textBox1.Text = "";
                    textBox1.AppendText(PrettyPrinter.Print(Decompiler.DecompileMethod(OSI, method, new SourceSpan())));
                }
                catch (NotImplementedException ex)
                {
                    textBox1.AppendText("<Exception decompiling!>\r\n");
                    textBox1.AppendText(ex.ToString() + "\r\n");
                }
            }
            else
            {
                textBox1.Text = "<Not fully decompilable, contact benji>";
            }
        }

        public void DisplayFunction(OSIFile.FunctionInfo function)
        {
            SubroutineGraph g = new SubroutineGraph(function.Instructions, function.BytecodeOffset);
            List<Token> parameters = new List<Token>();
            for (int i = 0; i < function.ParameterCount; i++)
            {
                parameters.Add(new Token(TokenType.Symbol, "param" + (i + 1), new SourceSpan()));
            }
            LoadGraph(ref g, function.Name, false, parameters);

            InspectorPanel.Controls.Add(CurrentInspector);
            CurrentInspector.Dock = DockStyle.Fill;
            if (g.Nodes.Count == 3)
            {
                try
                {
                    textBox1.Text = "";
                    textBox1.AppendText(PrettyPrinter.Print(Decompiler.DecompileFunction(OSI, function, new SourceSpan())));
                }
                catch (NotImplementedException ex)
                {
                    textBox1.AppendText("<Exception decompiling!>\r\n");
                    textBox1.AppendText(ex.ToString() + "\r\n");
                }
            }
            else
            {
                textBox1.Text = "<Not fully decompilable, contact benji>";
            }
        }

        private void GoToButton_Click(object sender, EventArgs e)
        {
            if (UInt32.TryParse(instructionOffsetTextBox.Text, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out uint address))
            {
                OSIFile.FunctionInfo nearestFunction = null;
                OSIFile.MethodInfo nearestMethod = null;
                uint nearestOffset = address;

                foreach (OSIFile.FunctionInfo function in OSI.Functions)
                {
                    if (function.BytecodeOffset <= address)
                    {
                        uint offset = address - function.BytecodeOffset;
                        if (offset < nearestOffset)
                        {
                            nearestOffset = offset;
                            nearestMethod = null;
                            nearestFunction = function;
                        }
                    }
                }

                foreach (OSIFile.ClassInfo cls in OSI.Classes)
                {
                    foreach (OSIFile.MethodInfo method in cls.Methods)
                    {
                        if (method.BytecodeOffset <= address)
                        {
                            uint offset = address - method.BytecodeOffset;
                            if (offset < nearestOffset)
                            {
                                nearestOffset = offset;
                                nearestFunction = null;
                                nearestMethod = method;
                            }
                        }
                    }
                }

                if (nearestFunction != null)
                {
                    foreach (TreeNode child in FunctionsNode.Nodes)
                    {
                        if (child.Tag == nearestFunction)
                        {
                            treeView1.SelectedNode = child;
                            child.EnsureVisible();
                        }
                    }
                    DisplayFunction(nearestFunction);
                    CurrentInspector.HilightedOffset = address;
                }
                else if (nearestMethod != null)
                {
                    foreach (TreeNode classNode in ClassesNode.Nodes)
                    {
                        foreach (TreeNode child in classNode.Nodes)
                        {
                            if (child.Tag == nearestMethod)
                            {
                                treeView1.SelectedNode = child;
                                child.EnsureVisible();
                            }
                        }
                    }
                    DisplayMethod(nearestMethod);
                    CurrentInspector.HilightedOffset = address;
                }
                else
                {
                    MessageBox.Show("Couldn't find closest subroutine!");
                }
            }
        }
    }

    public class OSISubroutineInspector : Control
    {
        public SubroutineGraph Graph { get; }
        public uint HilightedOffset { get; set; }

        private Dictionary<Node, Rectangle> NodeLocations = new Dictionary<Node, Rectangle>();
        private Dictionary<Node, string> NodeContents = new Dictionary<Node, string>();
        private int CameraX = 0;
        private int CameraY = 0;

        private const int NodePadding = 30;

        public OSISubroutineInspector(SubroutineGraph graph)
        {
            this.Graph = graph;

            this.Font = new Font("Consolas", 12);

            LayoutNode(Graph.StartNode, 0, 0);

            foreach (Node n in Graph.Nodes)
            {
                if (!NodeLocations.ContainsKey(n))
                {
                    NodeLocations.Add(n, new Rectangle(0, 0, 100, 10));
                }
                if (!NodeContents.ContainsKey(n))
                {
                    NodeContents.Add(n, "<Orphaned Node>");
                }
            }

            this.Resize += OSISubroutineInspector_Resize;

            DoubleBuffered = true;
        }

        private void OSISubroutineInspector_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        private int LayoutNode(Node n, int x, int y)
        {
            //bool hasOldY = false;
            //int oldY = 0;
            if (NodeContents.ContainsKey(n))
                NodeContents.Remove(n);
            if (NodeLocations.ContainsKey(n))
            {
                //hasOldY = true;
                //oldY = NodeLocations[n].Y;
                if (NodeLocations[n].Y > y)
                    y = NodeLocations[n].Y;
                NodeLocations.Remove(n);
            }

            string content = "<unknown node>";


            /*if (n is OSINode osiNode)
            {
                StringBuilder builder = new StringBuilder();
                foreach (Instruction ins in osiNode.Instructions)
                    builder.AppendLine(ins.ToString());
                content = builder.ToString();
                content = content.TrimEnd();
            }
            else if (n == Graph.StartNode)
            {
                content = "<Start>";
            }
            else if (n == Graph.EndNode)
            {
                content = "<End>";
            }*/
            content = n.ToString();

            NodeContents.Add(n, content);

            Size textSize = TextRenderer.MeasureText(content, Font);
            int contentWidth = textSize.Width;
            NodeLocations.Add(n, new Rectangle(x, y, textSize.Width, textSize.Height));

            //if (n is OSINode alsoOsiNode)
            //{
                int childrenWidth = 0;
                // TODO: Block infinite loops
                foreach (Jump j in n.OutJumps.Values)
                {
                    if (!NodeLocations.ContainsKey(j.Destination))
                        childrenWidth += LayoutNode(j.Destination, x + childrenWidth, y + textSize.Height + NodePadding);
                    //else
                    //    childrenWidth += NodeLocations[j.Destination].Width;
                    if (j != n.OutJumps.Values.ElementAt(n.OutJumps.Values.Count - 1))
                        childrenWidth += NodePadding;
                }
                if (childrenWidth > contentWidth)
                    contentWidth = childrenWidth;
            //}

            NodeLocations.Remove(n);
            NodeLocations.Add(n, new Rectangle(x + contentWidth / 2 - textSize.Width / 2, y, textSize.Width < 100 ? 100 : textSize.Width, textSize.Height < 20 ? 20 : textSize.Height));
            //NodeLocations.Add(n, new Rectangle(x, y, contentWidth, textSize.Height));

            return contentWidth;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Pen alwaysJumpPen = new Pen(Color.Blue, 3);
            alwaysJumpPen.StartCap = System.Drawing.Drawing2D.LineCap.RoundAnchor;
            alwaysJumpPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            Pen trueJumpPen = new Pen(Color.Green, 3);
            trueJumpPen.StartCap = System.Drawing.Drawing2D.LineCap.RoundAnchor;
            trueJumpPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            Pen falseJumpPen = new Pen(Color.Red, 3);
            falseJumpPen.StartCap = System.Drawing.Drawing2D.LineCap.RoundAnchor;
            falseJumpPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;

            foreach (Node n in Graph.Nodes)
            {

                Rectangle r = NodeLocations[n];

                e.Graphics.FillRectangle(Brushes.White, r.X - CameraX, r.Y - CameraY, r.Width, r.Height);
                if (n is OSINode osiNode)
                {
                    uint offset = osiNode.StartOffset;
                    int lineHeight = r.Height / (osiNode.Instructions.Count + 1);
                    int line = 0;
                    foreach (Instruction i in osiNode.Instructions)
                    {
                        if (offset >= this.HilightedOffset && offset < this.HilightedOffset + i.Size)
                        {
                            e.Graphics.FillRectangle(Brushes.Yellow, r.X - CameraX, r.Y - CameraY + line * lineHeight, r.Width, lineHeight);
                        }
                        line++;
                        offset += i.Size;
                    }
                }
                e.Graphics.DrawRectangle(Pens.Black, r.X - CameraX, r.Y - CameraY, r.Width, r.Height);
                TextRenderer.DrawText(e.Graphics, NodeContents[n], Font, new Rectangle(r.X - CameraX, r.Y - CameraY, r.Width, r.Height), Color.Black, TextFormatFlags.Left);

                foreach (Jump j in n.OutJumps.Values)
                {
                    Rectangle r2 = NodeLocations[j.Destination];

                    Pen p = alwaysJumpPen;
                    if (j.Type == Jump.JumpType.ConditionalTrue)
                        p = trueJumpPen;
                    else if (j.Type == Jump.JumpType.ConditionalFalse)
                        p = falseJumpPen;

                    e.Graphics.DrawLine(p, r.X + r.Width / 2 - CameraX, r.Y + r.Height - CameraY, r2.X + r2.Width / 2 - CameraX, r2.Y - CameraY); // Diagonal
                    //DrawZigZag(e.Graphics, p, r.X + r.Width / 2 - CameraX, r.Y + r.Height - CameraY, r2.X + r2.Width / 2 - CameraX, r2.Y - CameraY, (r.Y + r.Height - CameraY + r2.Y - CameraY) * 0.5f); // Mid-Y
                    //DrawZigZag(e.Graphics, p, r.X + r.Width / 2 - CameraX, r.Y + r.Height - CameraY, r2.X + r2.Width / 2 - CameraX, r2.Y - CameraY, (int)(r.Y + r.Height + NodePadding * 0.5f - CameraY)); // Top-Y
                }
            }
            alwaysJumpPen.Dispose();
        }

        private static void DrawZigZag(Graphics g, Pen p, int x1, int y1, int x2, int y2, int midY)
        {
            g.DrawLine(p, x1, y1, x1, midY);
            g.DrawLine(p, x1, midY, x2, midY);
            g.DrawLine(p, x2, midY, x2, y2);
        }

        private int MouseButtonCount = 0;
        private int LastMouseX = 0;
        private int LastMouseY;
        private Node DragNode = null;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            MouseButtonCount++;
            LastMouseX = e.X;
            LastMouseY = e.Y;

            foreach (KeyValuePair<Node, Rectangle> p in NodeLocations)
            {
                if (p.Value.Contains(new Point(e.X + CameraX, e.Y + CameraY)))
                {
                    DragNode = p.Key;
                    
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (MouseButtonCount > 0)
            {
                if (DragNode == null)
                {
                    CameraX -= (e.X - LastMouseX);
                    CameraY -= (e.Y - LastMouseY);
                }
                else
                {
                    Rectangle r = NodeLocations[DragNode];
                    NodeLocations[DragNode] = new Rectangle(r.X + (e.X - LastMouseX), r.Y + (e.Y - LastMouseY), r.Width, r.Height);
                }
            }
            Invalidate();
            LastMouseX = e.X;
            LastMouseY = e.Y;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            MouseButtonCount--;
            if (MouseButtonCount < 0)
                MouseButtonCount = 0;
            DragNode = null;
        }
    }
}
