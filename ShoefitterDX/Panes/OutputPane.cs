using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace ShoefitterDX.Panes
{
    public class OutputPane : DockContent
    {
        private ToolStrip toolStrip1;
        private ToolStripButton ClearOutputButton;
        private TextBox OutputTextBox;

        public OutputPane()
        {
            InitializeComponent();

            // Dock content options
            this.DockAreas = DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.DockBottom;

            // Connect to output event
            Program.OutputWritten += Program_OutputWritten;
        }

        private void Program_OutputWritten(object sender, string e)
        {
            OutputTextBox.AppendText(e);
        }

        private void InitializeComponent()
        {
            this.OutputTextBox = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.ClearOutputButton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // OutputTextBox
            // 
            this.OutputTextBox.BackColor = System.Drawing.Color.White;
            this.OutputTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OutputTextBox.Location = new System.Drawing.Point(0, 25);
            this.OutputTextBox.Multiline = true;
            this.OutputTextBox.Name = "OutputTextBox";
            this.OutputTextBox.ReadOnly = true;
            this.OutputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.OutputTextBox.Size = new System.Drawing.Size(284, 236);
            this.OutputTextBox.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ClearOutputButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(284, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // ClearOutputButton
            // 
            this.ClearOutputButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ClearOutputButton.Image = global::ShoefitterDX.Properties.Resources.CleanData_16x;
            this.ClearOutputButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ClearOutputButton.Name = "ClearOutputButton";
            this.ClearOutputButton.Size = new System.Drawing.Size(23, 22);
            this.ClearOutputButton.Text = "toolStripButton1";
            this.ClearOutputButton.Click += new System.EventHandler(this.ClearOutputButton_Click);
            // 
            // OutputPane
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.OutputTextBox);
            this.Controls.Add(this.toolStrip1);
            this.Name = "OutputPane";
            this.Text = "Output";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void ClearOutputButton_Click(object sender, EventArgs e)
        {
            OutputTextBox.Text = "";
        }
    }
}
