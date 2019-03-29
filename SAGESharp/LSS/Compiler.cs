using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAGESharp.OSI;

namespace SAGESharp.LSS
{
    public class Compiler
    {
        public class Result
        {
            public OSIFile OSI;
            public List<SyntaxError> Errors;

            public Result(OSIFile osi)
            {
                this.OSI = osi;
                this.Errors = new List<SyntaxError>();
            }
        }

        // Compiles a single source string into an OSI.
        public Result Compile(string source, byte versionMajor = 4, byte versionMinor = 1)
        {
            OSIFile osi = new OSIFile(versionMajor, versionMinor);
            Result result = new Result(osi);
            Parser.Result parseResults;
            Parser p = new Parser();

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(Encoding.ASCII.GetBytes(source)))
            using (System.IO.StreamReader reader = new System.IO.StreamReader(ms))
            {
                List<SyntaxError> scanErrors = new List<SyntaxError>();
                List<Token> tokens = Scanner.Scan(source, scanErrors, true, true);
                if (scanErrors.Count == 0)
                {
                    parseResults = p.Parse(tokens);
                    if (parseResults.Errors.Count == 0)
                    {
                        CompileInto(result, parseResults);
                    }
                    else
                    {
                        result.Errors = parseResults.Errors;
                    }
                }
                else
                {
                    result.Errors = scanErrors;
                }
            }

            return result;
        }

        // Compiles the given files into an OSI.
        public Result CompileFiles(IEnumerable<string> filenames, byte versionMajor = 4, byte versionMinor = 1)
        {
            OSIFile osi = new OSIFile(versionMajor, versionMinor);
            Result result = new Result(osi);
            Parser p = new Parser();
            List<Parser.Result> parseResults = new List<Parser.Result>();

            foreach (string filename in filenames)
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(filename))
                {
                    List<SyntaxError> scanErrors = new List<SyntaxError>();
                    List<Token> tokens = Scanner.Scan(reader.ReadToEnd(), scanErrors, true, true);
                    if (scanErrors.Count == 0)
                    {
                        Parser.Result parseResult = p.Parse(tokens);
                        parseResults.Add(parseResult);
                        if (parseResult.Errors.Count == 0)
                        {
                            
                        }
                        else
                        {
                            result.Errors.AddRange(parseResult.Errors);
                        }
                    }
                    else
                    {
                        result.Errors.AddRange(scanErrors);
                    }
                }
            }
            CompileInto(result, parseResults.ToArray());
            return result;
        }

        public Result CompileParsed(Parser.Result parseResult)
        {
            Result result = new Result(new OSIFile());
            CompileInto(result, parseResult);
            return result;
        }

        private void CompileInto(Result result, params Parser.Result[] parseResults)
        {
            Dictionary<string, int> strings = new Dictionary<string, int>();
            Dictionary<string, int> symbols = new Dictionary<string, int>();
            Dictionary<string, int> globals = new Dictionary<string, int>();

            // Collect existing names
            for (int i = 0; i < result.OSI.Strings.Count; i++)
                strings.Add(result.OSI.Strings[i], i);
            for (int i = 0; i < result.OSI.Symbols.Count; i++)
                symbols.Add(result.OSI.Symbols[i], i);
            for (int i = 0; i < result.OSI.Globals.Count; i++)
                globals.Add(result.OSI.Globals[i], i);

            // Collect new names
            foreach (Parser.Result parseResult in parseResults)
            {
                foreach (Statements.ClassStatement cls in parseResult.Classes)
                {
                    if (symbols.ContainsKey(cls.Name.Content))
                    {
                        result.Errors.Add(new SyntaxError("Class already exists with same name.", cls.Name.SourceLocation.Offset, cls.Name.SourceLength, 0));
                    }
                    else
                    {
                        /*int clsIndex = result.OSI.Symbols.Count;
                        result.OSI.Symbols.Add(cls.Name.Content);
                        symbols.Add(cls.Name.Content, clsIndex);*/
                        foreach (var prop in cls.Properties)
                        {
                            if (!symbols.ContainsKey(prop.Name.Content))
                            {
                                int propIndex = result.OSI.Symbols.Count;
                                result.OSI.Symbols.Add(prop.Name.Content);
                                symbols.Add(prop.Name.Content, propIndex);
                            }
                        }
                        foreach (var method in cls.Methods)
                        {
                            if (!symbols.ContainsKey(method.Name.Content))
                            {
                                int methodIndex = result.OSI.Symbols.Count;
                                result.OSI.Symbols.Add(method.Name.Content);
                                symbols.Add(method.Name.Content, methodIndex);
                            }
                        }
                    }
                }

                foreach (Statements.SubroutineStatement func in parseResult.Functions)
                {
                    if (symbols.ContainsKey(func.Name.Content))
                    {
                        result.Errors.Add(new SyntaxError("Function already exists with same name.", func.Name.SourceLocation.Offset, func.Name.SourceLength, 0));
                    }
                    else
                    {
                        int funcIndex = result.OSI.Symbols.Count;
                        result.OSI.Symbols.Add(func.Name.Content);
                        symbols.Add(func.Name.Content, funcIndex);
                    }
                }

                foreach (Statements.GlobalStatement global in parseResult.Globals)
                {
                    if (globals.ContainsKey(global.Name.Content))
                    {
                        result.Errors.Add(new SyntaxError("Global already exists with same name.", global.Name.SourceLocation.Offset, global.Name.SourceLength, 0));
                    }
                    else
                    {
                        int globalIndex = result.OSI.Globals.Count;
                        result.OSI.Globals.Add(global.Name.Content);
                        globals.Add(global.Name.Content, globalIndex);
                    }
                }
            }
        }

        public string DecompileOSI(OSIFile osi)
        {
            throw new NotImplementedException();
        }

        public void DecompileOSIProject(OSIFile osi, string outputDirectory)
        {
            throw new NotImplementedException();
        }

        public string DecompileInstructions(OSIFile osi, List<Instruction> instructions, uint bytecodeOffset)
        {
            SAGESharp.OSI.ControlFlow.SubroutineGraph graph = new OSI.ControlFlow.SubroutineGraph(instructions, bytecodeOffset);
            // TODO
            return "(Not Implemented)";
        }
    }
}
