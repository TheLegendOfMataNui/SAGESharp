using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace ShoefitterDX.Panes
{
    public class GameExplorerPane : DockContent
    {
        private TreeNode ProjectNode;

        private System.Windows.Forms.TreeView treeView1;

        public GameExplorerPane()
        {
            InitializeComponent();

            // Dock content options
            this.DockAreas = DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.DockBottom;

            // Connect to project events
            Program.ProjectOpened += (_, p) => RefreshProject();
            Program.ProjectClosed += (_, p) => RefreshProject();

            // Create tree nodes
            ProjectNode = new TreeNode("<ProjectNode>");

            treeView1.Nodes.Add(ProjectNode);

            RefreshProject();
        }

        private void RefreshProject()
        {
            if (Program.Project == null)
            {
                ProjectNode.Text = "<ProjectNode>";
                if (treeView1.Nodes.Contains(ProjectNode))
                    treeView1.Nodes.Remove(ProjectNode);
                Text = "Game Explorer - No Project";
            }
            else
            {
                ProjectNode.Text = Program.Project.Name;
                if (!treeView1.Nodes.Contains(ProjectNode))
                    treeView1.Nodes.Add(ProjectNode);
                Text = "Game Explorer - " + Program.Project.Name;
            }
        }

        private void InitializeComponent()
        {
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(284, 261);
            this.treeView1.TabIndex = 0;
            // 
            // GameExplorerPane
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.treeView1);
            this.Name = "GameExplorerPane";
            this.Text = "Game Explorer";
            this.ResumeLayout(false);

        }
    }
}
