using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoefitterDX.IO
{
    public class AssetManager
    {
        private Dictionary<string, GameAsset> Assets { get; } = new Dictionary<string, GameAsset>();
        private Dictionary<string, GameFile> Files { get; } = new Dictionary<string, GameFile>();

        private string GameDataDirectory { get; }
        private string ProjectDataDirectory { get; }

        public AssetManager(string gameDataDirectory, string projectDataDirectory)
        {
            this.GameDataDirectory = gameDataDirectory;
            this.ProjectDataDirectory = projectDataDirectory;
            Refresh();
        }

        public void Refresh()
        {
            Assets.Clear();
            Files.Clear();

            // Get the modified files from the project
            foreach (string filename in System.IO.Directory.EnumerateFiles(this.ProjectDataDirectory, "*", System.IO.SearchOption.AllDirectories))
            {
                string name = filename.Substring(this.ProjectDataDirectory.Length);
                if (name.StartsWith("\\"))
                    name = name.Substring(1);
                Files.Add(name, new GameFile(name, -1));
            }

            // Get the base files from the game data
            foreach (string filename in System.IO.Directory.EnumerateFiles(this.GameDataDirectory, "*", System.IO.SearchOption.AllDirectories))
            {
                string name = filename.Substring(this.GameDataDirectory.Length);
                if (name.StartsWith("\\"))
                    name = name.Substring(1);
                if (!Files.ContainsKey(name))
                {
                    Files.Add(name, new GameFile(name, -1));
                }
            }

            // Filter them into assets

        }

        public IEnumerable<GameAsset> EnumerateAssets()
        {
            return Assets.Values;
        }

        public IEnumerable<GameFile> EnumerateFiles()
        {
            return Files.Values;
        }

        public GameAsset GetAsset(string filename)
        {
            if (Assets.ContainsKey(filename))
            {
                return Assets[filename];
            }
            else
            {
                return null;
            }
        }

        public GameFile GetFile(string filename)
        {
            if (Files.ContainsKey(filename))
            {
                return Files[filename];
            }
            else
            {
                return null;
            }
        }
    }
}
