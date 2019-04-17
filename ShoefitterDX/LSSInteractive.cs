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
        private Button LastClicked; // Store the last-clicked button for the Ctrl+Enter command.

        public LSSInteractive()
        {
            InitializeComponent();
            LastClicked = CompileButton;
        }

        private bool TryScan(out List<Token> tokens)
        {
            List<SyntaxError> errors = new List<SyntaxError>();
            tokens = Scanner.Scan(SourceTextBox.Text.Replace("\r\n", "\n"), "<LSSInteractive>", errors, false, false);
            if (errors.Count == 0)
            {
                return true;
            }
            else
            {
                ResultTextBox.Text = "Scan Error! Tokens: \r\n";
                foreach (Token t in tokens)
                {
                    ResultTextBox.Text += t.ToString().Replace("\r\n", "\\n").Replace("\n", "\\n").Replace("\t", "\\t") + "\r\n";
                }
                tokens = null;
                return false;
            }
        }

        private bool TryParse(out Parser.Result result)
        {
            if (TryScan(out List<Token> tokens))
            {
                Parser p = new Parser();
                try
                {
                    result = p.Parse(tokens);
                }
                catch (Exception ex)
                {
                    result = new Parser.Result();
                    result.Errors.Add(new SyntaxError("Parser exception: \n\n" + ex.ToString(),"<LSSInteractive>", 0, 0, 0));
                }
                if (result.Errors.Count == 0)
                {
                    return true;
                }
                else
                {
                    ResultTextBox.Text = result.Errors.Count + " Errors: \n";
                    foreach (SyntaxError error in result.Errors)
                    {
                        ResultTextBox.Text += "    " + error.ToString() + "\n";
                    }
                    result = null;
                    return false;
                }
            }
            else
            {
                result = null;
                return false;
            }
        }

        private bool TryCompile(out Compiler.Result result)
        {
            if (TryParse(out Parser.Result parsed))
            {
                Compiler c = new Compiler();
                try
                {
                    result = c.CompileParsed(parsed);
                }
                catch (Exception ex)
                {
                    result = new Compiler.Result(new SAGESharp.OSI.OSIFile());
                    result.Errors.Add(new SyntaxError("Compiler exception: \n\n" + ex.ToString(), "<LSSInteractive>", 0, 0, 0));
                }
                if (result.Errors.Count == 0)
                {
                    return true;
                }
                else
                {
                    ResultTextBox.Text = result.Errors.Count + " Compile Errors: \n";
                    foreach (SyntaxError error in result.Errors)
                    {
                        ResultTextBox.AppendText("    " + error.ToString() + "\n");
                    }
                    result = null;
                    return false;
                }
            }
            else
            {
                result = null;
                return false;
            }
        }

        private void ScanButton_Click(object sender, EventArgs e)
        {
            LastClicked = ScanButton;

            ResultTextBox.BeginUpdate(); // Don't re-render on text change
            if (TryScan(out List<Token> tokens))
            {
                ResultTextBox.Text = "";
                foreach (Token t in tokens)
                {
                    ResultTextBox.Text += t.ToString().Replace("\r\n", "\\n").Replace("\n", "\\n").Replace("\t", "\\t") + "\r\n";
                }
            }
            ResultTextBox.EndUpdate(); // Ok we're done changing the text rapidly
        }

        private void ParseButton_Click(object sender, EventArgs e)
        {
            LastClicked = ParseButton;

            ResultTextBox.BeginUpdate();
            if (TryParse(out Parser.Result result))
            {
                ResultTextBox.Text = "";
                foreach (var g in result.Globals)
                {
                    ResultTextBox.AppendText(g.ToString() + "\n\n");
                }
                foreach (SAGESharp.LSS.Statements.ClassStatement cls in result.Classes)
                {
                    ResultTextBox.AppendText(cls.ToString() + "\n\n");
                }
                foreach (var f in result.Functions)
                {
                    ResultTextBox.AppendText("function " + f.ToString() + "\n\n");
                }
            }
            ResultTextBox.EndUpdate();
        }

        private void CompileButton_Click(object sender, EventArgs e)
        {
            LastClicked = CompileButton;

            ResultTextBox.BeginUpdate();
            if (TryCompile(out Compiler.Result result))
            {
                ResultTextBox.Text = "";
                ResultTextBox.AppendText(result.OSI.ToString());
            }
            ResultTextBox.EndUpdate();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Enter))
            {
                LastClicked.PerformClick();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
