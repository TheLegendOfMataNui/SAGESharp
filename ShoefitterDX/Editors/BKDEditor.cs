using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoefitterDX.Editors
{
    public class BKDEditor : EditorBase
    {
        private System.Windows.Forms.ToolStripButton ExportToolStripButton;
        private System.Windows.Forms.ToolStripButton ConfigureToolStripButton;
        private System.Windows.Forms.Panel ConfigurePanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private Controls.AssetChoiceControl assetChoiceControl1;
        private Controls.AssetChoiceControl assetChoiceControl2;
        private System.Windows.Forms.ToolStrip MainToolStrip;

        public BKDEditor(string fileName) : base(fileName)
        {

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.MainToolStrip = new System.Windows.Forms.ToolStrip();
            this.ConfigurePanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ExportToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.ConfigureToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.assetChoiceControl2 = new ShoefitterDX.Controls.AssetChoiceControl();
            this.assetChoiceControl1 = new ShoefitterDX.Controls.AssetChoiceControl();
            this.MainToolStrip.SuspendLayout();
            this.ConfigurePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainToolStrip
            // 
            this.MainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ExportToolStripButton,
            this.ConfigureToolStripButton});
            this.MainToolStrip.Location = new System.Drawing.Point(0, 0);
            this.MainToolStrip.Name = "MainToolStrip";
            this.MainToolStrip.Size = new System.Drawing.Size(389, 25);
            this.MainToolStrip.TabIndex = 0;
            this.MainToolStrip.Text = "toolStrip1";
            // 
            // ConfigurePanel
            // 
            this.ConfigurePanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ConfigurePanel.Controls.Add(this.assetChoiceControl2);
            this.ConfigurePanel.Controls.Add(this.assetChoiceControl1);
            this.ConfigurePanel.Controls.Add(this.label2);
            this.ConfigurePanel.Controls.Add(this.label1);
            this.ConfigurePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ConfigurePanel.Location = new System.Drawing.Point(0, 322);
            this.ConfigurePanel.Name = "ConfigurePanel";
            this.ConfigurePanel.Size = new System.Drawing.Size(389, 60);
            this.ConfigurePanel.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Mesh:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Skeleton:";
            // 
            // ExportToolStripButton
            // 
            this.ExportToolStripButton.Image = global::ShoefitterDX.Properties.Resources.Upload_16x;
            this.ExportToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ExportToolStripButton.Name = "ExportToolStripButton";
            this.ExportToolStripButton.Size = new System.Drawing.Size(60, 22);
            this.ExportToolStripButton.Text = "Export";
            // 
            // ConfigureToolStripButton
            // 
            this.ConfigureToolStripButton.Checked = true;
            this.ConfigureToolStripButton.CheckOnClick = true;
            this.ConfigureToolStripButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ConfigureToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ConfigureToolStripButton.Image = global::ShoefitterDX.Properties.Resources.Settings_Inverse_16x;
            this.ConfigureToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ConfigureToolStripButton.Name = "ConfigureToolStripButton";
            this.ConfigureToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.ConfigureToolStripButton.Text = "toolStripButton1";
            this.ConfigureToolStripButton.Click += new System.EventHandler(this.ConfigureToolStripButton_Click);
            // 
            // assetChoiceControl2
            // 
            this.assetChoiceControl2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.assetChoiceControl2.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.assetChoiceControl2.Location = new System.Drawing.Point(61, 33);
            this.assetChoiceControl2.Name = "assetChoiceControl2";
            this.assetChoiceControl2.SelectedFilename = "";
            this.assetChoiceControl2.Size = new System.Drawing.Size(325, 24);
            this.assetChoiceControl2.TabIndex = 3;
            // 
            // assetChoiceControl1
            // 
            this.assetChoiceControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.assetChoiceControl1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.assetChoiceControl1.Location = new System.Drawing.Point(45, 3);
            this.assetChoiceControl1.Name = "assetChoiceControl1";
            this.assetChoiceControl1.SelectedFilename = "";
            this.assetChoiceControl1.Size = new System.Drawing.Size(341, 24);
            this.assetChoiceControl1.TabIndex = 2;
            // 
            // BKDEditor
            // 
            this.ClientSize = new System.Drawing.Size(389, 382);
            this.Controls.Add(this.ConfigurePanel);
            this.Controls.Add(this.MainToolStrip);
            this.Name = "BKDEditor";
            this.MainToolStrip.ResumeLayout(false);
            this.MainToolStrip.PerformLayout();
            this.ConfigurePanel.ResumeLayout(false);
            this.ConfigurePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void ConfigureToolStripButton_Click(object sender, EventArgs e)
        {
            ConfigurePanel.Visible = ConfigureToolStripButton.Checked;
        }
    }
}
