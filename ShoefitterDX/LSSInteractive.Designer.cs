namespace ShoefitterDX
{
    partial class LSSInteractive
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LSSInteractive));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.SourceTextBox = new FastColoredTextBoxNS.FastColoredTextBox();
            this.ResultTextBox = new FastColoredTextBoxNS.FastColoredTextBox();
            this.Compile = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SourceTextBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ResultTextBox)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(2, 2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.SourceTextBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.ResultTextBox);
            this.splitContainer1.Size = new System.Drawing.Size(796, 407);
            this.splitContainer1.SplitterDistance = 396;
            this.splitContainer1.TabIndex = 0;
            // 
            // SourceTextBox
            // 
            this.SourceTextBox.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.SourceTextBox.AutoScrollMinSize = new System.Drawing.Size(619, 1638);
            this.SourceTextBox.BackBrush = null;
            this.SourceTextBox.CharHeight = 14;
            this.SourceTextBox.CharWidth = 8;
            this.SourceTextBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.SourceTextBox.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.SourceTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SourceTextBox.IsReplaceMode = false;
            this.SourceTextBox.Location = new System.Drawing.Point(0, 0);
            this.SourceTextBox.Name = "SourceTextBox";
            this.SourceTextBox.Paddings = new System.Windows.Forms.Padding(0);
            this.SourceTextBox.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.SourceTextBox.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("SourceTextBox.ServiceColors")));
            this.SourceTextBox.Size = new System.Drawing.Size(396, 407);
            this.SourceTextBox.TabIndex = 0;
            this.SourceTextBox.Text = resources.GetString("SourceTextBox.Text");
            this.SourceTextBox.Zoom = 100;
            // 
            // ResultTextBox
            // 
            this.ResultTextBox.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.ResultTextBox.AutoScrollMinSize = new System.Drawing.Size(27, 14);
            this.ResultTextBox.BackBrush = null;
            this.ResultTextBox.CharHeight = 14;
            this.ResultTextBox.CharWidth = 8;
            this.ResultTextBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.ResultTextBox.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.ResultTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResultTextBox.IsReplaceMode = false;
            this.ResultTextBox.Location = new System.Drawing.Point(0, 0);
            this.ResultTextBox.Name = "ResultTextBox";
            this.ResultTextBox.Paddings = new System.Windows.Forms.Padding(0);
            this.ResultTextBox.ReadOnly = true;
            this.ResultTextBox.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.ResultTextBox.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("ResultTextBox.ServiceColors")));
            this.ResultTextBox.Size = new System.Drawing.Size(396, 407);
            this.ResultTextBox.TabIndex = 0;
            this.ResultTextBox.Zoom = 100;
            // 
            // Compile
            // 
            this.Compile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Compile.Location = new System.Drawing.Point(12, 415);
            this.Compile.Name = "Compile";
            this.Compile.Size = new System.Drawing.Size(75, 23);
            this.Compile.TabIndex = 1;
            this.Compile.Text = "Compile";
            this.Compile.UseVisualStyleBackColor = true;
            this.Compile.Click += new System.EventHandler(this.Compile_Click);
            // 
            // LSSInteractive
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.Compile);
            this.Controls.Add(this.splitContainer1);
            this.Name = "LSSInteractive";
            this.Text = "LSSInteractive";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SourceTextBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ResultTextBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button Compile;
        private FastColoredTextBoxNS.FastColoredTextBox SourceTextBox;
        private FastColoredTextBoxNS.FastColoredTextBox ResultTextBox;
    }
}