using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShoefitterDX.Controls
{
    public partial class AssetChoiceControl : UserControl
    {
        public event EventHandler<string> SelectedFilenameChanged;

        private string _selectedFilename = "";
        public string SelectedFilename
        {
            get
            {
                return _selectedFilename;
            }
            set
            {
                _selectedFilename = value;
                Invalidate();
                SelectedFilenameChanged?.Invoke(this, _selectedFilename);
            }
        }

        public AssetChoiceControl()
        {
            InitializeComponent();
            AllowDrop = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            string label = String.IsNullOrEmpty(SelectedFilename) ? "(No asset selected)" : SelectedFilename;
            SizeF labelSize = e.Graphics.MeasureString(label, Font);

            int startX = 0;
            if (labelSize.Width > AssignButton.Left)
            {
                startX = AssignButton.Left - (int)labelSize.Width;
            }
            SolidBrush brush = new SolidBrush(ForeColor);
            e.Graphics.DrawString(label, Font, brush, startX, Height / 2 - (int)labelSize.Height / 2);
            brush.Dispose();
        }

        private void AssignButton_Click(object sender, EventArgs e)
        {
            SelectedFilename = Program.Window.GameExplorer.SelectedAsset;
        }

        private static string GetValidPaste()
        {
            if (Clipboard.ContainsFileDropList()
                    && Clipboard.GetFileDropList().Count == 1
                    && Program.Window.GameExplorer.AssetExists(Clipboard.GetFileDropList()[0]))
            {
                return Clipboard.GetFileDropList()[0];
            }
            else if (Clipboard.ContainsText()
                && Program.Window.GameExplorer.AssetExists(Clipboard.GetText()))
            {
                return Clipboard.GetText();
            }
            else
            {
                return "";
            }
        }

        private void PathContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            copyPathToolStripMenuItem.Enabled = !String.IsNullOrEmpty(SelectedFilename);
            pastePathToolStripMenuItem.Enabled = !String.IsNullOrEmpty(GetValidPaste());
            clearToolStripMenuItem.Enabled = !String.IsNullOrEmpty(SelectedFilename);
            selectInGameExplorerToolStripMenuItem.Enabled = !String.IsNullOrEmpty(SelectedFilename);
        }

        private void CopyPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(SelectedFilename);
        }

        private void PastePathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string paste = GetValidPaste();
            if (!String.IsNullOrEmpty(paste))
            {
                SelectedFilename = paste;
            }
        }

        private void ClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectedFilename = "";
        }

        private void SelectInGameExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.Window.GameExplorer.SelectedAsset = SelectedFilename;
            Program.Window.GameExplorer.Focus();
        }

        private void AssetChoiceControl_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Link;
        }

        private void AssetChoiceControl_DragDrop(object sender, DragEventArgs e)
        {
            string data = e.Data.GetData(typeof(string)) as string;
            if (!String.IsNullOrEmpty(data) && Program.Window.GameExplorer.AssetExists(data))
            {
                SelectedFilename = data;
            }
        }
    }
}
