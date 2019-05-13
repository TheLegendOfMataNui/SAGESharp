using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;

namespace ShoefitterDX.Editors
{
    //[System.ComponentModel.DesignerCategory("Code")]
    public class EditorBase : DockContent
    {
        private string _fileName = "";
        public string FileName
        {
            get => _fileName;
            private set
            {
                _fileName = value;
                IsModified = IsModified; // Refresh tab text.
            }
        }

        private bool _isModified = false;
        public bool IsModified
        {
            get => _isModified;
            protected set
            {
                _isModified = value;
                TabText = System.IO.Path.GetFileName(FileName) + (IsModified ? " *" : "");
            }
        }

        public EditorBase()
        {

        }

        public EditorBase(string fileName)
        {
            this.FileName = fileName;
            this.DockAreas = DockAreas.Document | DockAreas.Float;
        }

        public virtual void Save()
        {

        }
    }
}
