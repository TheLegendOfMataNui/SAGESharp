namespace ShoefitterDX.Dialogs
{
    partial class CreateProjectDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateProjectDialog));
            this.CancelButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.ProjectTypeListView = new System.Windows.Forms.ListView();
            this.ProjectIconImageList = new System.Windows.Forms.ImageList(this.components);
            this.ProjectNameTextBox = new System.Windows.Forms.TextBox();
            this.ContainingDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.ProjectNameLabel = new System.Windows.Forms.Label();
            this.ContainingDirectoryLabel = new System.Windows.Forms.Label();
            this.GameDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.GameExecutableTextBox = new System.Windows.Forms.TextBox();
            this.GameDirectoryLabel = new System.Windows.Forms.Label();
            this.GameExecutableLabel = new System.Windows.Forms.Label();
            this.BrowseGameButton = new System.Windows.Forms.Button();
            this.BrowseContainingButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButton.Location = new System.Drawing.Point(454, 320);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 0;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.Location = new System.Drawing.Point(373, 320);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 1;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // ProjectTypeListView
            // 
            this.ProjectTypeListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProjectTypeListView.LargeImageList = this.ProjectIconImageList;
            this.ProjectTypeListView.Location = new System.Drawing.Point(12, 12);
            this.ProjectTypeListView.MultiSelect = false;
            this.ProjectTypeListView.Name = "ProjectTypeListView";
            this.ProjectTypeListView.ShowGroups = false;
            this.ProjectTypeListView.Size = new System.Drawing.Size(517, 198);
            this.ProjectTypeListView.TabIndex = 2;
            this.ProjectTypeListView.TileSize = new System.Drawing.Size(96, 128);
            this.ProjectTypeListView.UseCompatibleStateImageBehavior = false;
            // 
            // ProjectIconImageList
            // 
            this.ProjectIconImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ProjectIconImageList.ImageStream")));
            this.ProjectIconImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.ProjectIconImageList.Images.SetKeyName(0, "LEGO Bionicle_101.ico");
            // 
            // ProjectNameTextBox
            // 
            this.ProjectNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProjectNameTextBox.Location = new System.Drawing.Point(119, 216);
            this.ProjectNameTextBox.Name = "ProjectNameTextBox";
            this.ProjectNameTextBox.Size = new System.Drawing.Size(410, 20);
            this.ProjectNameTextBox.TabIndex = 3;
            this.ProjectNameTextBox.Text = "New Project";
            // 
            // ContainingDirectoryTextBox
            // 
            this.ContainingDirectoryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ContainingDirectoryTextBox.Location = new System.Drawing.Point(119, 242);
            this.ContainingDirectoryTextBox.Name = "ContainingDirectoryTextBox";
            this.ContainingDirectoryTextBox.Size = new System.Drawing.Size(377, 20);
            this.ContainingDirectoryTextBox.TabIndex = 4;
            this.ContainingDirectoryTextBox.Text = "D:\\codemastrben\\Documents\\Projects\\Modding\\Bionicle\\SFDX Projects";
            // 
            // ProjectNameLabel
            // 
            this.ProjectNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ProjectNameLabel.AutoSize = true;
            this.ProjectNameLabel.Location = new System.Drawing.Point(12, 219);
            this.ProjectNameLabel.Name = "ProjectNameLabel";
            this.ProjectNameLabel.Size = new System.Drawing.Size(71, 13);
            this.ProjectNameLabel.TabIndex = 5;
            this.ProjectNameLabel.Text = "Project Name";
            // 
            // ContainingDirectoryLabel
            // 
            this.ContainingDirectoryLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ContainingDirectoryLabel.AutoSize = true;
            this.ContainingDirectoryLabel.Location = new System.Drawing.Point(12, 245);
            this.ContainingDirectoryLabel.Name = "ContainingDirectoryLabel";
            this.ContainingDirectoryLabel.Size = new System.Drawing.Size(102, 13);
            this.ContainingDirectoryLabel.TabIndex = 6;
            this.ContainingDirectoryLabel.Text = "Containing Directory";
            // 
            // GameDirectoryTextBox
            // 
            this.GameDirectoryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GameDirectoryTextBox.Location = new System.Drawing.Point(119, 268);
            this.GameDirectoryTextBox.Name = "GameDirectoryTextBox";
            this.GameDirectoryTextBox.Size = new System.Drawing.Size(377, 20);
            this.GameDirectoryTextBox.TabIndex = 7;
            this.GameDirectoryTextBox.Text = "C:\\Program Files (x86)\\LEGO Media\\LEGO Bionicle (Beta)";
            // 
            // GameExecutableTextBox
            // 
            this.GameExecutableTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GameExecutableTextBox.Location = new System.Drawing.Point(119, 294);
            this.GameExecutableTextBox.Name = "GameExecutableTextBox";
            this.GameExecutableTextBox.Size = new System.Drawing.Size(410, 20);
            this.GameExecutableTextBox.TabIndex = 8;
            this.GameExecutableTextBox.Text = "LEGO Bionicle.exe";
            // 
            // GameDirectoryLabel
            // 
            this.GameDirectoryLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.GameDirectoryLabel.AutoSize = true;
            this.GameDirectoryLabel.Location = new System.Drawing.Point(12, 271);
            this.GameDirectoryLabel.Name = "GameDirectoryLabel";
            this.GameDirectoryLabel.Size = new System.Drawing.Size(80, 13);
            this.GameDirectoryLabel.TabIndex = 9;
            this.GameDirectoryLabel.Text = "Game Directory";
            // 
            // GameExecutableLabel
            // 
            this.GameExecutableLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.GameExecutableLabel.AutoSize = true;
            this.GameExecutableLabel.Location = new System.Drawing.Point(12, 297);
            this.GameExecutableLabel.Name = "GameExecutableLabel";
            this.GameExecutableLabel.Size = new System.Drawing.Size(91, 13);
            this.GameExecutableLabel.TabIndex = 10;
            this.GameExecutableLabel.Text = "Game Executable";
            // 
            // BrowseGameButton
            // 
            this.BrowseGameButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseGameButton.Location = new System.Drawing.Point(502, 268);
            this.BrowseGameButton.Name = "BrowseGameButton";
            this.BrowseGameButton.Size = new System.Drawing.Size(27, 20);
            this.BrowseGameButton.TabIndex = 11;
            this.BrowseGameButton.Text = "...";
            this.BrowseGameButton.UseVisualStyleBackColor = true;
            // 
            // BrowseContainingButton
            // 
            this.BrowseContainingButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseContainingButton.Location = new System.Drawing.Point(502, 242);
            this.BrowseContainingButton.Name = "BrowseContainingButton";
            this.BrowseContainingButton.Size = new System.Drawing.Size(27, 20);
            this.BrowseContainingButton.TabIndex = 12;
            this.BrowseContainingButton.Text = "...";
            this.BrowseContainingButton.UseVisualStyleBackColor = true;
            // 
            // CreateProjectDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(541, 355);
            this.ControlBox = false;
            this.Controls.Add(this.BrowseContainingButton);
            this.Controls.Add(this.BrowseGameButton);
            this.Controls.Add(this.GameExecutableLabel);
            this.Controls.Add(this.GameDirectoryLabel);
            this.Controls.Add(this.GameExecutableTextBox);
            this.Controls.Add(this.GameDirectoryTextBox);
            this.Controls.Add(this.ContainingDirectoryLabel);
            this.Controls.Add(this.ProjectNameLabel);
            this.Controls.Add(this.ContainingDirectoryTextBox);
            this.Controls.Add(this.ProjectNameTextBox);
            this.Controls.Add(this.ProjectTypeListView);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.CancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateProjectDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create Project";
            this.Load += new System.EventHandler(this.CreateProjectDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.ListView ProjectTypeListView;
        private System.Windows.Forms.TextBox ProjectNameTextBox;
        private System.Windows.Forms.TextBox ContainingDirectoryTextBox;
        private System.Windows.Forms.Label ProjectNameLabel;
        private System.Windows.Forms.Label ContainingDirectoryLabel;
        private System.Windows.Forms.TextBox GameDirectoryTextBox;
        private System.Windows.Forms.TextBox GameExecutableTextBox;
        private System.Windows.Forms.Label GameDirectoryLabel;
        private System.Windows.Forms.Label GameExecutableLabel;
        private System.Windows.Forms.Button BrowseGameButton;
        private System.Windows.Forms.Button BrowseContainingButton;
        private System.Windows.Forms.ImageList ProjectIconImageList;
    }
}