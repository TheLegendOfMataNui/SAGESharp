using ShoefitterDX.Dialogs;
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

        private Dictionary<string, Editors.EditorBase> OpenEditors = new Dictionary<string, Editors.EditorBase>();

        private Dictionary<string, Func<string, Editors.EditorBase>> ExtensionMappings = new Dictionary<string, Func<string, Editors.EditorBase>>()
        {
            { ".txt", (filename) => new Editors.TextEditor(filename, FastColoredTextBoxNS.Language.Custom) },
            { ".json", (filename) => new Editors.TextEditor(filename, FastColoredTextBoxNS.Language.Custom) },
            { "", (filename) => new Editors.TextEditor(filename, FastColoredTextBoxNS.Language.Custom) },
            { ".md", (filename) => new Editors.TextEditor(filename, FastColoredTextBoxNS.Language.Custom) },
            { ".osas", (filename) => new Editors.TextEditor(filename, FastColoredTextBoxNS.Language.Custom) },
            { ".bkd", (filename) => new Editors.BKDEditor(filename) }
        };

        public Window()
        {
            InitializeComponent();

            dockPanel1.Theme = new WeifenLuo.WinFormsUI.Docking.VS2015DarkTheme();
            dockPanel1.Theme.ApplyTo(menuStrip1);
            dockPanel1.Theme.ApplyTo(statusStrip1);

            Program.ProjectOpened += Program_ProjectOpened;
            Program.ProjectClosed += Program_ProjectClosed;

            GameExplorer = new Panes.GameExplorerPane();
            GameExplorer.Show(dockPanel1, WeifenLuo.WinFormsUI.Docking.DockState.DockRight);

            Output = new Panes.OutputPane();
            Output.Show(dockPanel1, WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);

            Output.WriteText("Shoefitter-DX v" + System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString());
        }

        /// <summary>
        /// Whether any of the project assets are modified from on disk.
        /// </summary>
        public bool AnythingModified
        {
            get
            {
                return OpenEditors.Values.Any((editor) => editor.IsModified);
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
            foreach (Editors.EditorBase editor in OpenEditors.Values)
            {
                if (editor.IsModified)
                    editor.Save();
            }
            // HACK: Save the project file directly (In the future, it will be saved or open in an edtior which will save it)
            Program.Project.Save();
        }

        public void ShowNewProject()
        {
            if (!EnsureCloseAllowed())
                return;

            CreateProjectDialog dialog = new CreateProjectDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ProjectTemplate template = dialog.Template;
                if (template == null)
                {
                    MessageBox.Show("No template selected.");
                    return;
                }

                string projectDirectory = System.IO.Path.Combine(dialog.ContainingDirectory, dialog.ProjectName);
                string projectFilename = System.IO.Path.Combine(projectDirectory, dialog.ProjectName + "." + Project.PROJECT_EXTENSION);
                Project newProject = new Project(projectFilename);

                template.Apply(newProject);

                System.IO.Directory.CreateDirectory(projectDirectory);
                foreach (string subdirectory in Project.PROJECT_REQUIRED_SUBDIRECTORIES)
                    System.IO.Directory.CreateDirectory(System.IO.Path.Combine(projectDirectory, subdirectory));

                newProject.Save(projectFilename);
                Program.Project = newProject;
                Program.Config["Recents"]["LastProject"] = projectFilename;
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
                    Project project = Project.Load(dialog.FileName);
                    Program.Project = project;
                    Program.Config["Recents"]["LastProject"] = dialog.FileName;
                    Program.SaveConfig();
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

        public void OpenFileEditor(string filename)
        {
            if (OpenEditors.ContainsKey(filename))
            {
                OpenEditors[filename].Show(dockPanel1);
            }
            else
            {
                string extension = System.IO.Path.GetExtension(filename);
                if (ExtensionMappings.ContainsKey(extension))
                {
                    Editors.EditorBase newEditor = ExtensionMappings[extension](filename);
                    OpenEditors.Add(filename, newEditor);
                    newEditor.FormClosed += (sender, e) => OpenEditors.Remove(filename);
                    newEditor.Show(dockPanel1);
                }
                else
                {
                    MessageBox.Show("No editor for file type '" + extension + "'.");
                }
            }
        }

        //
        // UI Event Handlers
        //

        private void Window_Load(object sender, EventArgs e)
        {
            string lastProject = Program.Config.GetValueOrDefault("Recents", "LastProject", "");
            if (!String.IsNullOrEmpty(lastProject))
            {
                if (System.IO.File.Exists(lastProject))
                {
                    Project project = Project.Load(lastProject);
                    Program.Project = project;
                }
                else
                {
                    Output.WriteText("[INFO]: Last opened project '" + lastProject + "' doesn't exist anymore.");
                    Program.Config["Recents"]["LastProject"] = "";
                }
            }
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

        private void Program_ProjectClosed(object sender, Project e)
        {
            Text = "Shoefitter-DX";
        }

        private void Program_ProjectOpened(object sender, Project e)
        {
            Text = "Shoefitter-DX - " + System.IO.Path.GetFileNameWithoutExtension(Program.Project.Filename);
        }
    }
}
