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
            this.BrowseContainingButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButton.Location = new System.Drawing.Point(460, 275);
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
            this.OKButton.Location = new System.Drawing.Point(379, 275);
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
            this.ProjectTypeListView.HideSelection = false;
            this.ProjectTypeListView.LargeImageList = this.ProjectIconImageList;
            this.ProjectTypeListView.Location = new System.Drawing.Point(12, 12);
            this.ProjectTypeListView.MultiSelect = false;
            this.ProjectTypeListView.Name = "ProjectTypeListView";
            this.ProjectTypeListView.ShowGroups = false;
            this.ProjectTypeListView.Size = new System.Drawing.Size(523, 205);
            this.ProjectTypeListView.TabIndex = 2;
            this.ProjectTypeListView.TileSize = new System.Drawing.Size(96, 128);
            this.ProjectTypeListView.UseCompatibleStateImageBehavior = false;
            this.ProjectTypeListView.SelectedIndexChanged += new System.EventHandler(this.ProjectTypeListView_SelectedIndexChanged);
            this.ProjectTypeListView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ProjectTypeListView_MouseUp);
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
            this.ProjectNameTextBox.Location = new System.Drawing.Point(119, 223);
            this.ProjectNameTextBox.Name = "ProjectNameTextBox";
            this.ProjectNameTextBox.Size = new System.Drawing.Size(416, 20);
            this.ProjectNameTextBox.TabIndex = 3;
            this.ProjectNameTextBox.Text = "New Project";
            // 
            // ContainingDirectoryTextBox
            // 
            this.ContainingDirectoryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ContainingDirectoryTextBox.Location = new System.Drawing.Point(119, 249);
            this.ContainingDirectoryTextBox.Name = "ContainingDirectoryTextBox";
            this.ContainingDirectoryTextBox.Size = new System.Drawing.Size(383, 20);
            this.ContainingDirectoryTextBox.TabIndex = 4;
            this.ContainingDirectoryTextBox.Text = "D:\\codemastrben\\Documents\\Projects\\Modding\\Bionicle\\SFDX Projects";
            // 
            // ProjectNameLabel
            // 
            this.ProjectNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ProjectNameLabel.AutoSize = true;
            this.ProjectNameLabel.Location = new System.Drawing.Point(12, 226);
            this.ProjectNameLabel.Name = "ProjectNameLabel";
            this.ProjectNameLabel.Size = new System.Drawing.Size(71, 13);
            this.ProjectNameLabel.TabIndex = 5;
            this.ProjectNameLabel.Text = "Project Name";
            // 
            // ContainingDirectoryLabel
            // 
            this.ContainingDirectoryLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ContainingDirectoryLabel.AutoSize = true;
            this.ContainingDirectoryLabel.Location = new System.Drawing.Point(12, 252);
            this.ContainingDirectoryLabel.Name = "ContainingDirectoryLabel";
            this.ContainingDirectoryLabel.Size = new System.Drawing.Size(102, 13);
            this.ContainingDirectoryLabel.TabIndex = 6;
            this.ContainingDirectoryLabel.Text = "Containing Directory";
            // 
            // BrowseContainingButton
            // 
            this.BrowseContainingButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseContainingButton.Location = new System.Drawing.Point(508, 249);
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
            this.ClientSize = new System.Drawing.Size(547, 310);
            this.ControlBox = false;
            this.Controls.Add(this.BrowseContainingButton);
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
        private System.Windows.Forms.Button BrowseContainingButton;
        private System.Windows.Forms.ImageList ProjectIconImageList;
    }
}