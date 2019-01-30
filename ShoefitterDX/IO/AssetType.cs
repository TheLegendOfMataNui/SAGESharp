using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoefitterDX.IO
{
    public class AssetType
    {
        public static readonly AssetType UnknownAsset = new AssetType("Unknown Asset", 1);

        public static readonly AssetType[] AssetTypes = {
            UnknownAsset,
        };

        public string Name { get; }
        public int IconIndex { get; }
        private List<Type> EditorTypes;

        public AssetType(string name, int iconIndex, params Type[] editorTypes)
        {
            this.Name = name;
            this.IconIndex = iconIndex;
            this.EditorTypes = new List<Type>(editorTypes);
        }

        public IEnumerable<Type> GetEditorTypes()
        {
            return EditorTypes;
        }

        public Type PickDefaultEditorType()
        {
            if (EditorTypes.Count > 0)
                return EditorTypes[0];
            else
                return null;
        }
    }
}
