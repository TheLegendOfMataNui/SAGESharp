using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoefitterDX
{
    public class Project : SAGESharp.INIConfig
    {
        public const string PROJECT_EXTENSION = "sfdx";
        public const string SUBDIRECTORY_SCRIPT = "src";
        public const string SUBDIRECTORY_DATA = "data";
        public const string SUBDIRECTORY_OUTPUT = "build";

        /// <summary>
        /// A string property of a <see cref="Project"/>, with an event which is fired when the value is set via the <see cref="Value"/> property.
        /// </summary>
        public class ProjectProperty
        {
            /// <summary>
            /// Fired after the value of this property has been changed.
            /// The event contains the Project that was changed and the new value of the property.
            /// </summary>
            public event EventHandler<string> Changed;

            protected Project Project { get; }
            protected string SectionName { get; } = "";
            protected string KeyName { get; } = "";
            protected string DefaultValue { get; } = null;

            /// <summary>
            /// The value of the project property.
            /// Changing this to a different value than it is already set to will fire the <see cref="Changed"/> event.
            /// </summary>
            /// <remarks>
            /// Setting this to itself could create the INI entry in the Project if it does not already exist, and assign it the <see cref="DefaultValue"/>.
            /// </remarks>
            public string Value
            {
                get => Project.GetValueOrDefault(SectionName, KeyName, DefaultValue);
                set
                {
                    if (value != Project.GetValueOrDefault(SectionName, KeyName, DefaultValue))
                    {
                        Project[SectionName][KeyName] = value;
                        Changed?.Invoke(Project, value);
                    }
                }
            }

            /// <summary>
            /// Creates a new ProjectProperty that wraps the INI entry with the given key in the given section.
            /// </summary>
            /// <param name="project">The <see cref="Project"/> whose value is accessed and modified by this ProjectProperty.</param>
            /// <param name="sectionName">The name of the INI section of the key to access and modify.</param>
            /// <param name="keyName">The name of the INI key to access and modify.</param>
            /// <param name="defaultValue">The value that will be returned from the <see cref="Value"/> accessor if no entry with the given section and key exists.</param>
            public ProjectProperty(Project project, string sectionName, string keyName, string defaultValue = null)
            {
                this.Project = project;
                this.SectionName = sectionName;
                this.KeyName = keyName;
                this.DefaultValue = defaultValue;
            }

            public static implicit operator string(ProjectProperty property)
            {
                return property.Value;
            }
        }

        public ProjectProperty Name;
        public ProjectProperty GameDirectory;
        public ProjectProperty GameExecutable;
        public ProjectProperty CompressOutput;

        public string Filename { get; private set; } = "";

        public Project(string name, string gameDirectory, string gameExecutable) : this()
        {
            this.Name.Value = name;
            this.GameDirectory.Value = gameDirectory;
            this.GameExecutable.Value = gameExecutable;
        }

        public Project(string filename) : this()
        {
            this.Read(filename, true);
            this.Filename = filename;
        }

        private Project()
        {
            this.Name = new ProjectProperty(this, "Project", "Name", "");
            this.GameDirectory = new ProjectProperty(this, "Project", "GameDirectory", "");
            this.GameExecutable = new ProjectProperty(this, "Project", "GameExecutable", "");
            this.CompressOutput = new ProjectProperty(this, "Project", "CompressOutput", "False");
        }

        public void Save(string filename = null)
        {
            if (filename != null)
            {
                this.Filename = filename;
            }

            if (this.Filename == "")
            {
                throw new ArgumentException("A filename must be provided if this Project has no Filename set.");
            }

            this.Write(this.Filename);
        }
    }
}
