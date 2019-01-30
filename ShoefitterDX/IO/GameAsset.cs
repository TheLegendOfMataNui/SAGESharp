using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoefitterDX.IO
{
    public class GameAsset
    {
        public AssetType Type { get; }
        public string Path { get; }
        public int IconIndex { get; }
        private List<GameFile> Files;
        private List<Editors.AssetEditor> OpenEditors;

        public GameAsset(AssetType type, string path, int iconIndex, IEnumerable<GameFile> files)
        {
            this.Type = type;
            this.Path = path;
            this.IconIndex = iconIndex;
            this.Files = new List<GameFile>(files);
        }

        public IEnumerable<GameFile> GetFiles()
        {
            return Files;
        }

        public IEnumerable<Editors.AssetEditor> GetOpenEditors()
        {
            return OpenEditors;
        }

        public Editors.AssetEditor OpenEditor(Type editorType = null)
        {
            Editors.AssetEditor editor = null;

            if (editorType == null)
            {
                editorType = Type.PickDefaultEditorType();
            }

            foreach (Editors.AssetEditor existing in OpenEditors)
            {
                if (existing.GetType() == editorType)
                {
                    editor = existing;
                    break;
                }
            }

            if (editor == null)
            {
                if (editorType == null)
                {
                    Program.WriteOutput("No editors for type '" + this.Type.Name + "'.");
                    return null;
                }
                else
                {
                    editor = Activator.CreateInstance(editorType, this) as Editors.AssetEditor;
                }
            }

            Program.Window.ShowContent(editor);
            return editor;
        }
    }
}
