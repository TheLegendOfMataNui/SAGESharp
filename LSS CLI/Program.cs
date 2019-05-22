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
        public enum CompileMode
        {
            Compile,
            Decompile
        }

        public class CLIOptions
        {
            [Option('v', "version", Default = "4.1", HelpText = "The version of OSI file to create.")]
            public string OSIVersion { get; set; }

            [Option('l', "linenumbers", Default = true, HelpText = "Whether to emit `LineNumber` instructions into the output OSI.")]
            public bool EmitLineNumbers { get; set; }

            [Option('o', "output", Default = "", HelpText = "The path of the OSI file to create.")]
            public string Output { get; set; }

            [Option('y', "overwrite", Default = false, HelpText = "If there already exists a file with the same name as the output, overwrite the existing file.")]
            public bool AllowOverwrite { get; set; }

            [Option('r', "recurse", Default = false, HelpText = "Recurse into directories given as input files when adding the .lss files in the directory.")]
            public bool RecurseDirectories { get; set; }

            [Value(0, Required = true)]
            public CompileMode Mode { get; set; }

            [Value(1, Required = true)]
            public IEnumerable<string> Inputs { get; set; }
        }

        private const int EXIT_SUCCESS = 0;
        private const int EXIT_ARGUMENTS = 1;
        private const int EXIT_OUTPUT_EXISTS = 2;
        private const int EXIT_IO_ERROR = 3;
        private const int EXIT_SYNTAX_ERROR = 4;

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

        private static int DoCompile(CLIOptions options)
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

            Compiler.Result result = Compiler.CompileFiles(inputs);

            // TODO: Count the number of messages that are actually errors
            if (result.Errors.Count > 0)
            {
                foreach (SyntaxError error in result.Errors)
                {
                    // TODO: Use message code, filename, and column
                    Console.WriteLine("[ERROR][LSS---][" + error.Span.ToString() + "|?] " + error.Message);
                }
                Console.WriteLine("Finished with " + result.Errors.Count + " errors.");
                return EXIT_SYNTAX_ERROR;
            }
            else
            {
                string outputFilename = System.IO.Path.GetFullPath(options.Output == "" ? "./base.osi" : options.Output);
                if (System.IO.File.Exists(outputFilename) && !options.AllowOverwrite)
                {
                    Console.WriteLine("Aborted - file '" + outputFilename + "' already exists. (Specify -y to overwrite.)");
                    return EXIT_OUTPUT_EXISTS;
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
                        return EXIT_SUCCESS;
                    }
                    catch (System.IO.IOException exception)
                    {
                        Console.WriteLine("Failed to write result to '" + outputFilename + "': " + exception.ToString());
                        return EXIT_IO_ERROR;
                    }
                }
            }

        }

        private static int DoDecompile(CLIOptions options)
        {
            OSIFile osi = null;

            string osiFilename = options.Inputs.ElementAt(0);
            string outputDirectory = options.Output == "" ? System.IO.Path.ChangeExtension(osiFilename, "") : options.Output;

            using (System.IO.FileStream stream = new System.IO.FileStream(osiFilename, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
            using (System.IO.BinaryReader reader = new System.IO.BinaryReader(stream))
            {
                osi = new OSIFile(reader);
            }

            // TEMP TESTING
            OSIFile.MethodInfo method = osi.Classes[1].Methods[0];
            System.Diagnostics.Debug.WriteLine("method " + PrettyPrinter.Print(Decompiler.DecompileMethod(osi, method, new SourceSpan())));

            if (!System.IO.Directory.Exists(outputDirectory))
            {
                System.IO.Directory.CreateDirectory(outputDirectory);
            }

            //Decompiler.DecompileOSIProject(osi, outputDirectory);
            return EXIT_SUCCESS;
        }

        static int Main(string[] args)
        {
            return CommandLine.Parser.Default.ParseArguments<CLIOptions>(args).MapResult(options =>
            {
                if (options.Mode == CompileMode.Compile)
                {
                    return DoCompile(options);
                }
                else if (options.Mode == CompileMode.Decompile)
                {
                    return DoDecompile(options);
                }
                else
                {
                    throw new ArgumentException("Invalid option chosed for CompileMode! Must be Compile or Decompile, but was '" + options.Mode.ToString() + "'!");
                }
            }, (errors) =>
            {
                Console.Error.WriteLine("Invalid command-line arguments:");
                foreach (CommandLine.Error error in errors)
                {
                    Console.Error.WriteLine("    " + error.Tag.ToString());
                }
                return EXIT_ARGUMENTS;
            });
        }
    }
}
