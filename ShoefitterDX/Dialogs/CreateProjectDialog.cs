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
            new ProjectTemplate("LEGO Bionicle (Alpha)", 0, false),
            new ProjectTemplate("LEGO Bionicle (Beta)", 0, true),
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
                || !System.IO.Directory.Exists(ContainingDirectoryTextBox.Text)
                || !System.IO.Directory.Exists(GameDirectoryTextBox.Text)
                || !System.IO.File.Exists(System.IO.Path.Combine(GameDirectoryTextBox.Text, GameExecutableTextBox.Text)))
            {
                MessageBox.Show("no u dont");
                return;
            }

            DialogResult = DialogResult.OK;
            ProjectName = ProjectNameTextBox.Text;
            ContainingDirectory = ContainingDirectoryTextBox.Text;
            GameDirectory = GameDirectoryTextBox.Text;
            GameExecutable = GameExecutableTextBox.Text;
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
            if (this.GameDirectory != null)
                this.GameDirectoryTextBox.Text = this.GameDirectory;
            if (this.GameExecutable != null)
                this.GameExecutableTextBox.Text = this.GameExecutable;
            if (this.Template != null && Templates.Contains(this.Template))
            {
                this.ProjectTypeListView.SelectedIndices.Clear();
                this.ProjectTypeListView.SelectedIndices.Add(Templates.IndexOf(this.Template));
            }
        }
    }

    public class ProjectTemplate
    {
        public string Name = "(Invalid Template)";
        public int ImageListIndex = -1; // index into ProjectIconImageList.Images
        public bool CompressOutput = false;

        public ProjectTemplate(string name, int imageListIndex, bool compressOutput)
        {
            this.Name = name;
            this.ImageListIndex = imageListIndex;
            this.CompressOutput = compressOutput;
        }

        public void Apply(Project target)
        {
            target.CompressOutput.Value = this.CompressOutput ? "True" : "False";
        }
    }
}
