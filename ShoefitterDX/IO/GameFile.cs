using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoefitterDX.IO
{
    /// <summary>
    /// One logical file in the game's data, either uncompressed and in the native filesystem, or compressed in a blockfile.
    /// </summary>
    public class GameFile
    {
        /// <summary>
        /// An object that handles reading and writing to the data in a <see cref="GameFile"/>.
        /// </summary>
        public class GameFileHandle : IDisposable
        {
            /// <summary>
            /// The <see cref="GameFile"/> whose data this <see cref="GameFileHandle"/> provides access to.
            /// </summary>
            private GameFile File;

            public bool IsDisposed { get; private set; } = false;

            public GameFileHandle(GameFile file)
            {
                if (file.IsOpen)
                    throw new InvalidOperationException("Cannot create a handle for a file which is already open.");

                this.File = file;
                File.CurrentHandle = this;
            }

            public byte[] GetData()
            {
                // TODO
                throw new NotImplementedException();
            }

            public void SetData(byte[] data)
            {
                // TODO
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                if (IsDisposed)
                    throw new ObjectDisposedException("GameFileHandle for GameFile '" + File.Path + "'");

                IsDisposed = true;
                File.CurrentHandle = null;
            }
        }

        private GameFileHandle CurrentHandle { get; set; } = null;
        public bool IsOpen => CurrentHandle != null;
        public int IconIndex { get; }
        public string Path { get; }

        public GameFile(string path, int iconIndex)
        {
            this.Path = path;
            this.IconIndex = iconIndex;
        }

        public GameFileHandle Open()
        {
            if (IsOpen)
                throw new InvalidOperationException("This GameFile is already open.");

            return new GameFileHandle(this);
        }

        public void Close(GameFileHandle handle)
        {
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));

            if (handle != CurrentHandle)
                throw new ArgumentException("This GameFile can only close the handle that is currently open for it.", nameof(handle));

            handle.Dispose();
        }
    }
}
