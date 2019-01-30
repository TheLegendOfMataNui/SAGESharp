using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace ShoefitterDX.Editors
{
    public class Editor : DockContent
    {
        private string _title = "";
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                Text = Title + (NeedsSaving ? " *" : "");
            }
        }

        private bool _needsSaving;
        public bool NeedsSaving
        {
            get
            {
                return _needsSaving;
            }
            set
            {
                _needsSaving = value;
                Text = Title + (NeedsSaving ? " *" : "");
            }
        }
    }
}
