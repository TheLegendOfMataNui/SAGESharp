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

namespace ShoefitterDX
{
    public partial class LSSInteractive : Form
    {
        public LSSInteractive()
        {
            InitializeComponent();
        }

        private void Compile_Click(object sender, EventArgs e)
        {
            List<SyntaxError> errors = new List<SyntaxError>();
            List<Token> tokens = Scanner.Scan(SourceTextBox.Text, errors, false, true);
            if (errors.Count == 0)
            {
                if (true)
                {
                    Parser p = new Parser();
                    Parser.Result parseResult = p.Parse(tokens);

                    if (parseResult.Errors.Count > 0)
                    {
                        ResultTextBox.Text = parseResult.Errors.Count + " Errors: \n";
                        foreach (SyntaxError error in parseResult.Errors)
                        {
                            ResultTextBox.Text += "    " + error.ToString() + "\n";
                        }
                    }
                    else
                    {
                        ResultTextBox.Text = "Parsed with 0 errors.\n\n";
                        foreach (var g in parseResult.Globals)
                        {
                            ResultTextBox.AppendText(g.ToString() + "\n\n");
                        }
                        foreach (SAGESharp.LSS.Statements.ClassStatement cls in parseResult.Classes)
                        {
                            ResultTextBox.AppendText(cls.ToString() + "\n\n");
                        }
                        foreach (var f in parseResult.Functions)
                        {
                            ResultTextBox.AppendText("function " + f.ToString() + "\n\n");
                        }
                    }

                }
                else
                {
                    ResultTextBox.Text = "Scan Success! Tokens: \r\n";
                    foreach (Token t in tokens)
                    {
                        ResultTextBox.Text += t.ToString().Replace("\r\n", "\\n").Replace("\n", "\\n").Replace("\t", "\\t") + "\r\n";
                    }
                }
            }
            else
            {
                ResultTextBox.Text = "Scan Error! Tokens: \r\n";
                foreach (Token t in tokens)
                {
                    ResultTextBox.Text += t.ToString().Replace("\r\n", "\\n").Replace("\n", "\\n").Replace("\t", "\\t") + "\r\n";
                }

            }

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Enter))
            {
                Compile.PerformClick();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
