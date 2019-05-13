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
        private class TreeEntryMeta
        {
            public string Path { get; }
            public bool IsDirectory { get; }

            public TreeEntryMeta(string path, bool isDirectory)
            {
                this.Path = path;
                this.IsDirectory = isDirectory;
            }
        }

        private TreeNode ProjectNode;
        private Dictionary<string, TreeNode> FileNodes = new Dictionary<string, TreeNode>(StringComparer.OrdinalIgnoreCase);

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

        private void AddFileNode(string filename, TreeNode directoryNode)
        {
            TreeNode newNode = new TreeNode(System.IO.Path.GetFileName(filename));
            newNode.Tag = new TreeEntryMeta(filename, false);
            FileNodes.Add(filename, newNode);
            directoryNode.Nodes.Add(newNode);
        }

        private void AddDirectoryNodes(string directory, TreeNode directoryNode)
        {
            foreach (string childDirectory in System.IO.Directory.EnumerateDirectories(directory))
            {
                if (System.IO.Path.GetFileName(childDirectory).StartsWith("."))
                    continue;
                TreeNode newNode = new TreeNode(System.IO.Path.GetFileName(childDirectory));
                newNode.Tag = new TreeEntryMeta(childDirectory, true);
                AddDirectoryNodes(childDirectory, newNode);
                directoryNode.Nodes.Add(newNode);
            }

            foreach (string filename in System.IO.Directory.EnumerateFiles(directory))
            {
                if (System.IO.Path.GetFileName(filename).StartsWith("."))
                    continue;
                AddFileNode(filename, directoryNode);
            }
        }

        private void RefreshProject()
        {
            FileNodes.Clear();
            ProjectNode.Nodes.Clear();
            if (Program.Project == null)
            {
                ProjectNode.Text = "<ProjectNode>";
                if (treeView1.Nodes.Contains(ProjectNode))
                    treeView1.Nodes.Remove(ProjectNode);
                Text = "Game Explorer - No Project";
                ProjectNode.Tag = new TreeEntryMeta("", true); // A dummy that will be replaced on load
            }
            else
            {
                ProjectNode.Text = System.IO.Path.GetFileNameWithoutExtension(Program.Project.Filename);
                if (!treeView1.Nodes.Contains(ProjectNode))
                    treeView1.Nodes.Add(ProjectNode);
                Text = "Game Explorer - " + ProjectNode.Text;
                treeView1.BeginUpdate();
                AddDirectoryNodes(System.IO.Path.GetDirectoryName(Program.Project.Filename), ProjectNode);
                ProjectNode.Tag = new TreeEntryMeta(System.IO.Path.GetDirectoryName(Program.Project.Filename), true);
                treeView1.EndUpdate();
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
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeView1_NodeMouseDoubleClick);
            // 
            // GameExplorerPane
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.treeView1);
            this.Name = "GameExplorerPane";
            this.Text = "Game Explorer";
            this.ResumeLayout(false);

        }

        private void TreeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeEntryMeta meta = e?.Node?.Tag as TreeEntryMeta;
            if (meta != null)
            {
                if (!meta.IsDirectory)
                {
                    Program.Window.OpenFileEditor(meta.Path);
                }
            }
        }
    }
}
