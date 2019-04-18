/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp
{
    public class INIConfig
    {
        private Dictionary<string, INISection> Sections = new Dictionary<string, INISection>(StringComparer.InvariantCultureIgnoreCase);

        public Dictionary<string, INISection> TempSections = new Dictionary<string, INISection>(StringComparer.InvariantCultureIgnoreCase);

        public INISection this[string sectionName]
        {
            get
            {
                if (!TempSections.ContainsKey(sectionName) && !Sections.ContainsKey(sectionName))
                    Sections.Add(sectionName, new INISection(sectionName));
                else if(TempSections.ContainsKey(sectionName))
                    return TempSections[sectionName];
                return Sections[sectionName];
            }
        }

        public INIConfig()
        {

        }

        public INIConfig(string filename)
        {
            Read(filename, false);
        }

        public void SetTemporary(string section, string key, string value)
        {
            if (!TempSections.ContainsKey(section))
            {
                INISection tempSection = new INISection(section);

                tempSection.Keys.Add(key, value);

                TempSections.Add(section, tempSection);
            }
            else
            {
                TempSections[section][key] =  value;
            }
        }

        public string GetValueOrDefault(string section, string key, string defaultValue = null)
        {
            if (!Sections.ContainsKey(section) && !TempSections.ContainsKey(section))
                return defaultValue;
            INISection s;
            if (TempSections.ContainsKey(section))
                s = TempSections[section];
            else
                s = Sections[section];
            if (!s.Keys.ContainsKey(key))
                return defaultValue;
            else
                return s.Keys[key];
        }

        public void Read(string filename, bool clearExisting = true)
        {
            if (!System.IO.File.Exists(filename))
                return;

            if (clearExisting)
                Sections.Clear();

            using (System.IO.StreamReader reader = new System.IO.StreamReader(filename))
            {
                INISection currentSection = new INISection(""); // Default unnamed section

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();

                    if (line.Length == 0 || line.StartsWith(";"))
                        continue;

                    if (line.StartsWith("["))
                    {
                        // Read section header
                        Sections.Add(currentSection.Name, currentSection);
                        currentSection = new INISection(line.TrimStart('[').TrimEnd(']'));
                    }
                    else
                    {
                        // Read key
                        string[] parts = line.Split('='); // Assume no equals sign in value
                        currentSection.Keys.Add(parts[0], parts[1]);
                    }
                }
                Sections.Add(currentSection.Name, currentSection);
            }
        }

        public void Write(string filename)
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filename, false))
            {
                foreach (INISection section in Sections.Values)
                {
                    if (!String.IsNullOrEmpty(section.Name))
                        writer.WriteLine("\n[" + section.Name + "]");
                    foreach (KeyValuePair<string, string> pair in section.Keys)
                    {
                        writer.WriteLine(pair.Key + "=" + pair.Value);
                    }
                }
            }
        }
    }

    public class INISection
    {
        public string Name { get; }
        public Dictionary<string, string> Keys = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        public string this[string name]
        {
            get
            {
                return Keys.ContainsKey(name) ? Keys[name] : "";
            }
            set
            {
                if (Keys.ContainsKey(name))
                    Keys[name] = value;
                else
                    Keys.Add(name, value);
            }
        }

        public INISection(string name)
        {
            Name = name;
        }
    }
}
