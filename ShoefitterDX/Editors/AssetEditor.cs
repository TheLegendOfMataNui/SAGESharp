using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace ShoefitterDX.Editors
{
    public class AssetEditor : Editor
    {
        public IO.GameAsset Asset { get; }

        public AssetEditor(IO.GameAsset asset)
        {
            this.Asset = asset;
        }
    }
}
