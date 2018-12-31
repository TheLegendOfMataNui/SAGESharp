using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShoefitterDX
{
    public partial class Window : Form
    {
        public Panes.GameExplorerPane GameExplorer;

        public Window()
        {
            InitializeComponent();

            GameExplorer = new Panes.GameExplorerPane();
            GameExplorer.Show(dockPanel1, WeifenLuo.WinFormsUI.Docking.DockState.DockRight);
        }

        /// <summary>
        /// Whether any of the project assets are modified from on disk.
        /// </summary>
        public bool AnythingModified
        {
            get
            {
                // TODO: Enumerate open editors and return true if any are modified
                return Program.Project != null;
            }
        }

        /// <summary>
        /// Determines whether the user will allow the project to be closed, by assuming true if the project is saved, and prompting otherwise.
        /// </summary>
        /// <returns>Whether the project is ready to be closed.</returns>
        public bool EnsureCloseAllowed()
        {
            if (AnythingModified)
            {
                DialogResult result = MessageBox.Show("Save changes before closing?", "Shoefitter-DX", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    SaveAll();
                    return true;
                }
                else if (result == DialogResult.No)
                {
                    return true;
                }
                else if (result == DialogResult.Cancel)
                {
                    return false;
                }
                else
                {
                    throw new Exception("Um... that's not a valid dialog result...");
                }
            }
            else
            {
                return true;
            }
        }

        public void SaveAll()
        {
            // TODO: Enumerate all the open editors and save each
            // HACK: Save the project file directly (In the future, it will be saved or open in an edtior which will save it)
            Program.Project.Save();
        }

        public void ShowNewProject()
        {
            if (!EnsureCloseAllowed())
                return;
        }

        public void ShowOpenProject()
        {
            if (!EnsureCloseAllowed())
                return;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Shoefitter-DX Projects (*." + Project.PROJECT_EXTENSION + ")|*." + Project.PROJECT_EXTENSION;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Project project = new Project(dialog.FileName);
                    Program.Project = project;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load project.\n\nDetails:\n\n" + ex.ToString());
                }
            }
        }

        //
        // UI Event Handlers
        //

        private void Window_Load(object sender, EventArgs e)
        {
            // TODO: Load last opened project if possible, otherwise show start page
        }

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowNewProject();
        }

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowOpenProject();
        }
    }
}
