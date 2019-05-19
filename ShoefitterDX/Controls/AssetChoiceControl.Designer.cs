namespace ShoefitterDX.Controls
{
    partial class AssetChoiceControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.PathContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pastePathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectInGameExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AssignButton = new System.Windows.Forms.Button();
            this.PathContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // PathContextMenuStrip
            // 
            this.PathContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyPathToolStripMenuItem,
            this.pastePathToolStripMenuItem,
            this.toolStripMenuItem1,
            this.clearToolStripMenuItem,
            this.selectInGameExplorerToolStripMenuItem});
            this.PathContextMenuStrip.Name = "PathContextMenuStrip";
            this.PathContextMenuStrip.Size = new System.Drawing.Size(198, 120);
            this.PathContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.PathContextMenuStrip_Opening);
            // 
            // copyPathToolStripMenuItem
            // 
            this.copyPathToolStripMenuItem.Name = "copyPathToolStripMenuItem";
            this.copyPathToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.copyPathToolStripMenuItem.Text = "Copy Path";
            this.copyPathToolStripMenuItem.Click += new System.EventHandler(this.CopyPathToolStripMenuItem_Click);
            // 
            // pastePathToolStripMenuItem
            // 
            this.pastePathToolStripMenuItem.Name = "pastePathToolStripMenuItem";
            this.pastePathToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.pastePathToolStripMenuItem.Text = "Paste Path";
            this.pastePathToolStripMenuItem.Click += new System.EventHandler(this.PastePathToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(194, 6);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.ClearToolStripMenuItem_Click);
            // 
            // selectInGameExplorerToolStripMenuItem
            // 
            this.selectInGameExplorerToolStripMenuItem.Name = "selectInGameExplorerToolStripMenuItem";
            this.selectInGameExplorerToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.selectInGameExplorerToolStripMenuItem.Text = "Select in Game Explorer";
            this.selectInGameExplorerToolStripMenuItem.Click += new System.EventHandler(this.SelectInGameExplorerToolStripMenuItem_Click);
            // 
            // AssignButton
            // 
            this.AssignButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.AssignButton.Image = global::ShoefitterDX.Properties.Resources.ShiftToLeft_16x;
            this.AssignButton.Location = new System.Drawing.Point(118, 0);
            this.AssignButton.Name = "AssignButton";
            this.AssignButton.Size = new System.Drawing.Size(24, 24);
            this.AssignButton.TabIndex = 1;
            this.AssignButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.AssignButton.UseVisualStyleBackColor = true;
            this.AssignButton.Click += new System.EventHandler(this.AssignButton_Click);
            // 
            // AssetChoiceControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.PathContextMenuStrip;
            this.Controls.Add(this.AssignButton);
            this.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "AssetChoiceControl";
            this.Size = new System.Drawing.Size(142, 24);
            this.PathContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button AssignButton;
        private System.Windows.Forms.ContextMenuStrip PathContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem copyPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pastePathToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectInGameExplorerToolStripMenuItem;
    }
}
