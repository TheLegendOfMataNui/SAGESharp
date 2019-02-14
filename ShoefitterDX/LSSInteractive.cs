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
            List<Token> tokens = Scanner.Scan(SourceTextBox.Text, errors, true, true);

            ResultTextBox.Text = "";
            foreach (Token t in tokens)
            {
                ResultTextBox.Text += t.ToString() + "\r\n";
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
