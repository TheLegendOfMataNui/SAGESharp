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
            this.CompileButton = new System.Windows.Forms.Button();
            this.ScanButton = new System.Windows.Forms.Button();
            this.ParseButton = new System.Windows.Forms.Button();
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
            this.SourceTextBox.AutoScrollMinSize = new System.Drawing.Size(803, 1190);
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
            // CompileButton
            // 
            this.CompileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CompileButton.Location = new System.Drawing.Point(174, 415);
            this.CompileButton.Name = "CompileButton";
            this.CompileButton.Size = new System.Drawing.Size(75, 23);
            this.CompileButton.TabIndex = 1;
            this.CompileButton.Text = "Compile";
            this.CompileButton.UseVisualStyleBackColor = true;
            this.CompileButton.Click += new System.EventHandler(this.CompileButton_Click);
            // 
            // ScanButton
            // 
            this.ScanButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ScanButton.Location = new System.Drawing.Point(12, 415);
            this.ScanButton.Name = "ScanButton";
            this.ScanButton.Size = new System.Drawing.Size(75, 23);
            this.ScanButton.TabIndex = 2;
            this.ScanButton.Text = "Scan";
            this.ScanButton.UseVisualStyleBackColor = true;
            this.ScanButton.Click += new System.EventHandler(this.ScanButton_Click);
            // 
            // ParseButton
            // 
            this.ParseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ParseButton.Location = new System.Drawing.Point(93, 415);
            this.ParseButton.Name = "ParseButton";
            this.ParseButton.Size = new System.Drawing.Size(75, 23);
            this.ParseButton.TabIndex = 3;
            this.ParseButton.Text = "Parse";
            this.ParseButton.UseVisualStyleBackColor = true;
            this.ParseButton.Click += new System.EventHandler(this.ParseButton_Click);
            // 
            // LSSInteractive
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ParseButton);
            this.Controls.Add(this.ScanButton);
            this.Controls.Add(this.CompileButton);
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
        private System.Windows.Forms.Button CompileButton;
        private FastColoredTextBoxNS.FastColoredTextBox SourceTextBox;
        private FastColoredTextBoxNS.FastColoredTextBox ResultTextBox;
        private System.Windows.Forms.Button ScanButton;
        private System.Windows.Forms.Button ParseButton;
    }
}