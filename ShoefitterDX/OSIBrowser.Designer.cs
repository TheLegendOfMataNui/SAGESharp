namespace ShoefitterDX
{
    partial class OSIBrowser
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
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.goToButton = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.InspectorPanel = new System.Windows.Forms.Panel();
            this.GeneratePseudocodeCheckBox = new System.Windows.Forms.CheckBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.instructionOffsetTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.FullRowSelect = true;
            this.treeView1.HideSelection = false;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(211, 424);
            this.treeView1.TabIndex = 0;
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            this.splitContainer1.Panel1.Controls.Add(this.instructionOffsetTextBox);
            this.splitContainer1.Panel1.Controls.Add(this.goToButton);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(895, 467);
            this.splitContainer1.SplitterDistance = 211;
            this.splitContainer1.TabIndex = 1;
            // 
            // goToButton
            // 
            this.goToButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.goToButton.Location = new System.Drawing.Point(0, 444);
            this.goToButton.Name = "goToButton";
            this.goToButton.Size = new System.Drawing.Size(211, 23);
            this.goToButton.TabIndex = 1;
            this.goToButton.Text = "Go to Instruction (Hex)";
            this.goToButton.UseVisualStyleBackColor = true;
            this.goToButton.Click += new System.EventHandler(this.GoToButton_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.InspectorPanel);
            this.splitContainer2.Panel1.Controls.Add(this.GeneratePseudocodeCheckBox);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.textBox1);
            this.splitContainer2.Size = new System.Drawing.Size(680, 467);
            this.splitContainer2.SplitterDistance = 333;
            this.splitContainer2.TabIndex = 0;
            // 
            // InspectorPanel
            // 
            this.InspectorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InspectorPanel.Location = new System.Drawing.Point(0, 17);
            this.InspectorPanel.Name = "InspectorPanel";
            this.InspectorPanel.Size = new System.Drawing.Size(333, 450);
            this.InspectorPanel.TabIndex = 1;
            // 
            // GeneratePseudocodeCheckBox
            // 
            this.GeneratePseudocodeCheckBox.AutoSize = true;
            this.GeneratePseudocodeCheckBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.GeneratePseudocodeCheckBox.Location = new System.Drawing.Point(0, 0);
            this.GeneratePseudocodeCheckBox.Name = "GeneratePseudocodeCheckBox";
            this.GeneratePseudocodeCheckBox.Size = new System.Drawing.Size(333, 17);
            this.GeneratePseudocodeCheckBox.TabIndex = 0;
            this.GeneratePseudocodeCheckBox.Text = "Generate control flow pseudocode";
            this.GeneratePseudocodeCheckBox.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.White;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(343, 467);
            this.textBox1.TabIndex = 0;
            this.textBox1.WordWrap = false;
            // 
            // instructionOffsetTextBox
            // 
            this.instructionOffsetTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.instructionOffsetTextBox.Location = new System.Drawing.Point(0, 424);
            this.instructionOffsetTextBox.Name = "instructionOffsetTextBox";
            this.instructionOffsetTextBox.Size = new System.Drawing.Size(211, 20);
            this.instructionOffsetTextBox.TabIndex = 2;
            // 
            // OSIBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(895, 467);
            this.Controls.Add(this.splitContainer1);
            this.Name = "OSIBrowser";
            this.Text = "OSIBrowser";
            this.Load += new System.EventHandler(this.OSIBrowser_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox GeneratePseudocodeCheckBox;
        private System.Windows.Forms.Panel InspectorPanel;
        private System.Windows.Forms.Button goToButton;
        private System.Windows.Forms.TextBox instructionOffsetTextBox;
    }
}