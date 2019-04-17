using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAGESharp.OSI;
using SAGESharp.LSS;
using CommandLine;

namespace LSS_CLI
{
    class Program
    {
        public class CLIOptions
        {
            [Option('v', "version", Default = "4.1", HelpText = "The version of OSI file to create.")]
            public string OSIVersion { get; set; }

            [Option('l', "linenumbers", Default = true, HelpText = "Whether to emit `LineNumber` instructions into the output OSI.")]
            public bool EmitLineNumbers { get; set; }

            [Option('o', "output", Default = "./base.osi", HelpText = "The path of the OSI file to create.")]
            public string Output { get; set; }

            [Option('y', "overwrite", Default = false, HelpText = "If there already exists a file with the same name as the output, overwrite the existing file.")]
            public bool AllowOverwrite { get; set; }

            [Option('r', "recurse", Default = false, HelpText = "Recurse into directories given as input files when adding the .lss files in the directory.")]
            public bool RecurseDirectories { get; set; }

            [Value(0, Required = true)]
            public IEnumerable<string> Inputs { get; set; }
        }

        private static void EnumerateDirectory(string directory, List<string> files, bool recurse)
        {
            foreach (string filename in System.IO.Directory.EnumerateFiles(directory))
            {
                if (System.IO.Path.GetExtension(filename).ToLowerInvariant() == ".lss")
                {
                    files.Add(filename);
                }
            }
            if (recurse)
            {
                foreach (string child in System.IO.Directory.EnumerateDirectories(directory))
                {
                    EnumerateDirectory(child, files, recurse);
                }
            }
        }

        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<CLIOptions>(args).WithParsed(options =>
            {
                // Translate CLIOptions into compiler options
                Compiler.Settings settings = new Compiler.Settings();
                settings.EmitLineNumbers = options.EmitLineNumbers;
                string[] versionNumberParts = options.OSIVersion.Split('.');
                if (versionNumberParts.Length == 2)
                {
                    settings.VersionMajor = Int32.Parse(versionNumberParts[0]);
                    settings.VersionMinor = Int32.Parse(versionNumberParts[1]);
                }
                else
                {
                    Console.WriteLine("[WARNING][LSSC01][:|] Invalid OSI version. Version number should be specified like '4.1'.");
                }

                // Check options.Inputs for directory names, and replace them with the files they contain, considering the option to be recursive
                List<string> inputs = new List<string>();
                foreach (string input in options.Inputs)
                {
                    if (System.IO.Directory.Exists(input))
                    {
                        EnumerateDirectory(input, inputs, options.RecurseDirectories);
                    }
                    else if (System.IO.File.Exists(input))
                    {
                        inputs.Add(input);
                    }
                }

                Compiler compiler = new Compiler();
                Compiler.Result result = compiler.CompileFiles(inputs);

                // TODO: Count the number of messages that are actually errors
                if (result.Errors.Count > 0)
                {
                    foreach (SyntaxError error in result.Errors)
                    {
                        // TODO: Use message code, filename, and column
                        Console.WriteLine("[ERROR][LSS---][" + error.Span.ToString() + "|?] " + error.Message);
                    }
                    Console.WriteLine("Finished with " + result.Errors.Count + " errors.");
                }
                else
                {
                    string outputFilename = System.IO.Path.GetFullPath(options.Output);
                    if (System.IO.File.Exists(outputFilename) && !options.AllowOverwrite)
                    {
                        Console.WriteLine("Aborted - file '" + outputFilename + "' already exists. (Specify -y to overwrite.)");
                    }
                    else
                    {
                        try
                        {
                            using (System.IO.FileStream stream = new System.IO.FileStream(outputFilename, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Read))
                            using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(stream))
                            {
                                result.OSI.Write(writer);
                            }
                            Console.WriteLine("Compiled into '" + outputFilename + "'.");
                        }
                        catch (System.IO.IOException exception)
                        {
                            Console.WriteLine("Failed to write result to '" + outputFilename + "': " + exception.ToString());
                        }
                    }
                }
            });
        }
    }
}
