using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShoefitterDX.Dialogs
{
    public partial class CreateProjectDialog : Form
    {
        public string ProjectName { get; set; }
        public string ContainingDirectory { get; set; }
        public string GameDirectory { get; set; }
        public string GameExecutable { get; set; }
        public ProjectTemplate Template { get; set; }

        public static List<ProjectTemplate> Templates = new List<ProjectTemplate> {
            new ProjectTemplate("LEGO Bionicle (Alpha)", 0, false, "LEGOBionicle.exe"),
            new ProjectTemplate("LEGO Bionicle (Beta)", 0, true, "LEGOBionicle.exe"),
        };

        public CreateProjectDialog()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (ProjectNameTextBox.Text.Length == 0
                || !System.IO.Directory.Exists(ContainingDirectoryTextBox.Text))
            {
                MessageBox.Show("no u dont");
                return;
            }

            DialogResult = DialogResult.OK;
            ProjectName = ProjectNameTextBox.Text;
            ContainingDirectory = ContainingDirectoryTextBox.Text;
            Template = ProjectTypeListView.SelectedItems[0].Tag as ProjectTemplate;
            Close();
        }

        private void CreateProjectDialog_Load(object sender, EventArgs e)
        {
            foreach (ProjectTemplate t in Templates)
            {
                ProjectTypeListView.Items.Add(new ListViewItem(t.Name, t.ImageListIndex) { Tag = t });
            }

            if (this.ProjectName != null)
                this.ProjectNameTextBox.Text = this.ProjectName;
            if (this.ContainingDirectory != null)
                this.ContainingDirectoryTextBox.Text = this.ContainingDirectory;
            this.ProjectTypeListView.SelectedIndices.Clear();
            if (this.Template != null && Templates.Contains(this.Template))
            {
                this.ProjectTypeListView.SelectedIndices.Add(Templates.IndexOf(this.Template));
            }
            else if (Templates.Count > 0)
            {
                this.ProjectTypeListView.SelectedIndices.Add(0);
            }
        }

        // Hack to prevent the user from selecting no template.
        int LastSelectedIndex = -1;
        private void ProjectTypeListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ProjectTypeListView.SelectedIndices.Count == 0 && LastSelectedIndex > -1 && LastSelectedIndex < ProjectTypeListView.Items.Count)
            {
                //ProjectTypeListView.SelectedIndices.Add(LastSelectedIndex); // Cannot be done here because there are zero selected indices for a moment when selecting an item.
            }
            else
            {
                LastSelectedIndex = ProjectTypeListView.SelectedIndices[0];
            }
        }

        private void ProjectTypeListView_MouseUp(object sender, MouseEventArgs e)
        {
            if (ProjectTypeListView.SelectedIndices.Count == 0 && LastSelectedIndex > -1 && LastSelectedIndex < ProjectTypeListView.Items.Count)
            {
                ProjectTypeListView.SelectedIndices.Add(LastSelectedIndex);
            }
        }
    }

    public class ProjectTemplate
    {
        public string Name = "(Invalid Template)";
        public int ImageListIndex = -1; // index into ProjectIconImageList.Images
        public bool CompressOutput = false;
        public string ExecutableName = "";

        public ProjectTemplate(string name, int imageListIndex, bool compressOutput, string executableName)
        {
            this.Name = name;
            this.ImageListIndex = imageListIndex;
            this.CompressOutput = compressOutput;
            this.ExecutableName = executableName;
        }

        public void Apply(Project target)
        {
            target.CompressOutput.Value = this.CompressOutput ? "True" : "False";
            target.ExecutableName.Value = ExecutableName;
        }
    }
}
