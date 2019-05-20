using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using SAGESharp;

namespace ShoefitterDX.Editors
{
    public class BKDEditor : EditorBase
    {
        private System.Windows.Forms.ToolStripButton ExportToolStripButton;
        private System.Windows.Forms.ToolStripButton ConfigureToolStripButton;
        private System.Windows.Forms.Panel ConfigurePanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private Controls.AssetChoiceControl MeshChoiceControl;
        private Controls.AssetChoiceControl SkeletonChoiceControl;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.ToolStrip MainToolStrip;

        private BKD bkd { get; }

        public BKDEditor(string fileName) : base(fileName)
        {
            using (System.IO.FileStream stream = new System.IO.FileStream(Program.Project.MakePathAbsolute(fileName), System.IO.FileMode.Open))
            {
                SAGESharp.IO.IBinaryReader reader = SAGESharp.IO.Reader.ForStream(stream);
                bkd = SAGESharp.IO.BinarySerializers.ForBKDFiles.Read(reader);
            }
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.MainToolStrip = new System.Windows.Forms.ToolStrip();
            this.ExportToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.ConfigureToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.ConfigurePanel = new System.Windows.Forms.Panel();
            this.SkeletonChoiceControl = new ShoefitterDX.Controls.AssetChoiceControl();
            this.MeshChoiceControl = new ShoefitterDX.Controls.AssetChoiceControl();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
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
            // ExportToolStripButton
            // 
            this.ExportToolStripButton.Image = global::ShoefitterDX.Properties.Resources.Upload_16x;
            this.ExportToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ExportToolStripButton.Name = "ExportToolStripButton";
            this.ExportToolStripButton.Size = new System.Drawing.Size(60, 22);
            this.ExportToolStripButton.Text = "Export";
            this.ExportToolStripButton.Click += new System.EventHandler(this.ExportToolStripButton_Click);
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
            // ConfigurePanel
            // 
            this.ConfigurePanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ConfigurePanel.Controls.Add(this.SkeletonChoiceControl);
            this.ConfigurePanel.Controls.Add(this.MeshChoiceControl);
            this.ConfigurePanel.Controls.Add(this.label2);
            this.ConfigurePanel.Controls.Add(this.label1);
            this.ConfigurePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ConfigurePanel.Location = new System.Drawing.Point(0, 322);
            this.ConfigurePanel.Name = "ConfigurePanel";
            this.ConfigurePanel.Size = new System.Drawing.Size(389, 60);
            this.ConfigurePanel.TabIndex = 1;
            // 
            // SkeletonChoiceControl
            // 
            this.SkeletonChoiceControl.AllowDrop = true;
            this.SkeletonChoiceControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SkeletonChoiceControl.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SkeletonChoiceControl.Location = new System.Drawing.Point(61, 33);
            this.SkeletonChoiceControl.Name = "SkeletonChoiceControl";
            this.SkeletonChoiceControl.SelectedFilename = "";
            this.SkeletonChoiceControl.Size = new System.Drawing.Size(325, 24);
            this.SkeletonChoiceControl.TabIndex = 3;
            // 
            // MeshChoiceControl
            // 
            this.MeshChoiceControl.AllowDrop = true;
            this.MeshChoiceControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MeshChoiceControl.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MeshChoiceControl.Location = new System.Drawing.Point(45, 3);
            this.MeshChoiceControl.Name = "MeshChoiceControl";
            this.MeshChoiceControl.SelectedFilename = "";
            this.MeshChoiceControl.Size = new System.Drawing.Size(341, 24);
            this.MeshChoiceControl.TabIndex = 2;
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Mesh:";
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

        private void ExportToolStripButton_Click(object sender, EventArgs e)
        {
            XFile model = null;
            BHDFile skeleton = null;

            if (!MeshChoiceControl.HasSelectedAsset)
            {
                MessageBox.Show("Select a model by dragging a .x file from the Game Explorer, or by selecting the .x file in the Game Explorer and clicking the Assign button.");
                return;
            }
            if (!SkeletonChoiceControl.HasSelectedAsset)
            {
                MessageBox.Show("Select a skeleton by dragging a .bhd file from the Game Explorer, or by selecting the .bhd file in the Game Explorer and clicking the Assign button.");
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "COLLADA File (*.dae)|*.dae";
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            using (FileStream modelStream = new FileStream(Program.Project.MakePathAbsolute(MeshChoiceControl.SelectedFilename), FileMode.Open))
            using (BinaryReader modelReader = new BinaryReader(modelStream))
            {
                model = new XFile(modelReader);
            }

            using (FileStream skeletonStream = new FileStream(Program.Project.MakePathAbsolute(SkeletonChoiceControl.SelectedFilename), FileMode.Open))
            using (BinaryReader skeletonReader = new BinaryReader(skeletonStream))
            {
                skeleton = new BHDFile(skeletonReader);
            }

            ColladaUtils.ExportCOLLADA(model, skeleton, dialog.FileName, SharpDX.Matrix.RotationX(-SharpDX.MathUtil.PiOverTwo), true, ".dds", true);
        }
    }
}
