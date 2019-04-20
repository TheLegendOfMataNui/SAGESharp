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
        public Panes.OutputPane Output;

        public Window()
        {
            InitializeComponent();

            dockPanel1.Theme = new WeifenLuo.WinFormsUI.Docking.VS2015DarkTheme();
            dockPanel1.Theme.ApplyTo(menuStrip1);
            dockPanel1.Theme.ApplyTo(statusStrip1);

            GameExplorer = new Panes.GameExplorerPane();
            GameExplorer.Show(dockPanel1, WeifenLuo.WinFormsUI.Docking.DockState.DockRight);

            Output = new Panes.OutputPane();
            Output.Show(dockPanel1, WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);

            Program.WriteOutput("Shoefitter-DX v" + System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString());
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

            ShoefitterDX.Dialogs.CreateProjectDialog dialog = new Dialogs.CreateProjectDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Dialogs.ProjectTemplate template = dialog.Template;
                if (template == null)
                {
                    MessageBox.Show("No template selected.");
                    return;
                }

                Project newProject = new Project(dialog.ProjectName, dialog.GameDirectory, dialog.GameExecutable);
                string projectDirectory = System.IO.Path.Combine(dialog.ContainingDirectory, dialog.ProjectName);
                string projectFilename = System.IO.Path.Combine(projectDirectory, dialog.ProjectName + "." + Project.PROJECT_EXTENSION);

                template.Apply(newProject);

                System.IO.Directory.CreateDirectory(projectDirectory);
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(projectDirectory, Project.SUBDIRECTORY_DATA));
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(projectDirectory, Project.SUBDIRECTORY_SCRIPT));
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(projectDirectory, Project.SUBDIRECTORY_OUTPUT));

                newProject.Save(projectFilename);
                Program.Project = newProject;
            }
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

        public void ShowContent(WeifenLuo.WinFormsUI.Docking.DockContent content)
        {
            content.Show(dockPanel1, WeifenLuo.WinFormsUI.Docking.DockState.Document);
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

        private void LSSInteractiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LSSInteractive lssInteractive = new LSSInteractive();
            lssInteractive.Show(dockPanel1, WeifenLuo.WinFormsUI.Docking.DockState.Document);
        }

        private void OSIBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "OSI File (*.osi)|*.osi";
            // TODO: Remember last used directory
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                using (System.IO.FileStream stream = new System.IO.FileStream(dialog.FileName, System.IO.FileMode.Open))
                using (System.IO.BinaryReader reader = new System.IO.BinaryReader(stream))
                {
                    SAGESharp.OSI.OSIFile osi = new SAGESharp.OSI.OSIFile(reader);
                    OSIBrowser osiBrowser = new OSIBrowser();
                    osiBrowser.LoadOSI(osi);
                    osiBrowser.TabText += " - " + System.IO.Path.GetFileName(dialog.FileName);
                    osiBrowser.Show(dockPanel1, WeifenLuo.WinFormsUI.Docking.DockState.Document);
                }
            }
        }
    }
}
