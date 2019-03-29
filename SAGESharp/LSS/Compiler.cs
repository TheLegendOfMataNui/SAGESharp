using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAGESharp.LSS.Expressions;
using SAGESharp.LSS.Statements;
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

        private class SubroutineContext : ExpressionVisitor<object, object>, Statements.StatementVisitor<object>
        {
            public List<Instruction> Instructions { get; } = new List<Instruction>();
            private Dictionary<string, int> LocalLocations = new Dictionary<string, int>();
            private OSIFile OSI { get; }
            public bool IsFinalized { get; private set; }

            public SubroutineContext(OSIFile osi, IEnumerable<string> parameterNames)
            {
                this.OSI = osi;
                foreach (string param in parameterNames) {
                    AddLocal(param);
                }
            }

            public void FinalizeInstructions()
            {
                if (IsFinalized)
                    throw new InvalidOperationException("Cannot modify a SubroutineContext after it has been finalized.");
            }

            private int AddLocal(string localName)
            {
                if (IsFinalized)
                    throw new InvalidOperationException("Cannot modify a SubroutineContext after it has been finalized.");
                int localIndex = LocalLocations.Count;
                LocalLocations.Add(localName, localIndex);
                return localIndex;
            }

            private ushort AddOrGetString(string value)
            {
                if (IsFinalized)
                    throw new InvalidOperationException("Cannot modify a SubroutineContext after it has been finalized.");
                int index = OSI.Strings.IndexOf(value);
                if (index == -1)
                {
                    index = OSI.Strings.Count;
                    OSI.Strings.Add(value);
                }
                return (ushort)index;
            }

            private string UnescapeString(string value)
            {
                return value.Replace("\\\"", "\"").Replace("\\n", "\n");
            }

            // TODO: Make a corresponding EscapeString and use it in OSIFile.ToString()

            #region Expressions
            public object VisitArrayAccessExpression(ArrayAccessExpression expr, object context)
            {
                throw new NotImplementedException();
            }

            public object VisitArrayExpression(ArrayExpression expr, object context)
            {
                throw new NotImplementedException();
            }

            public object VisitBinaryExpression(BinaryExpression expr, object context)
            {
                throw new NotImplementedException();
            }

            public object VisitCallExpression(CallExpression expr, object context)
            {
                throw new NotImplementedException();
            }

            public object VisitConstructorExpression(ConstructorExpression expr, object context)
            {
                throw new NotImplementedException();
            }

            public object VisitGroupingExpression(GroupingExpression expr, object context)
            {
                return expr.Contents.AcceptVisitor(this, context);
            }

            public object VisitLiteralExpression(LiteralExpression expr, object context)
            {
                if (expr.Value.Type == TokenType.StringLiteral)
                {
                    ushort index = AddOrGetString(expr.Value.Content.Substring(1, expr.Value.Content.Length - 2));
                    Instructions.Add(new BCLInstruction(BCLOpcode.PushConstantString, index));
                }
                else if (expr.Value.Type == TokenType.IntegerLiteral)
                {
                    int value = Int32.Parse(expr.Value.Content);
                    if (value == 0)
                    {
                        Instructions.Add(new BCLInstruction(BCLOpcode.PushConstant0));
                    }
                    else if (value <= SByte.MaxValue && value >= SByte.MinValue)
                    {
                        Instructions.Add(new BCLInstruction(BCLOpcode.PushConstanti8, (sbyte)value));
                    }
                    else if (value <= Int16.MaxValue && value >= Int16.MinValue)
                    {
                        Instructions.Add(new BCLInstruction(BCLOpcode.PushConstanti16, (short)value));
                    }
                    else
                    {
                        Instructions.Add(new BCLInstruction(BCLOpcode.PushConstanti32, value));
                    }
                }
                else if (expr.Value.Type == TokenType.FloatLiteral)
                {
                    Instructions.Add(new BCLInstruction(BCLOpcode.PushConstantf32, Single.Parse(expr.Value.Content)));
                }
                else
                {
                    throw new InvalidOperationException("Invalid literal type: " + expr.Value.Type);
                }
                return null;
            }

            public object VisitUnaryExpression(UnaryExpression expr, object context)
            {
                throw new NotImplementedException();
            }

            public object VisitVariableExpression(VariableExpression expr, object context)
            {
                throw new NotImplementedException();
            }
            #endregion

            #region Statements
            public object VisitBlockStatement(BlockStatement s)
            {
                throw new NotImplementedException();
            }

            public object VisitClassStatement(ClassStatement s)
            {
                throw new NotImplementedException();
            }

            public object VisitPropertyStatement(PropertyStatement s)
            {
                throw new NotImplementedException();
            }

            public object VisitSubroutineStatement(SubroutineStatement s)
            {
                throw new NotImplementedException();
            }

            public object VisitGlobalStatement(GlobalStatement s)
            {
                throw new NotImplementedException();
            }

            public object VisitExpressionStatement(ExpressionStatement s)
            {
                return s.Expression.AcceptVisitor(this, null);
            }

            public object VisitReturnStatement(ReturnStatement s)
            {
                throw new NotImplementedException();
            }

            public object VisitIfStatement(IfStatement s)
            {
                throw new NotImplementedException();
            }

            public object VisitWhileStatement(WhileStatement s)
            {
                throw new NotImplementedException();
            }

            public object VisitAssignmentStatement(AssignmentStatement s)
            {
                throw new NotImplementedException();
            }

            public object VisitVariableDeclarationStatement(VariableDeclarationStatement s)
            {
                throw new NotImplementedException();
            }
            #endregion
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
            Dictionary<string, ushort> strings = new Dictionary<string, ushort>();
            Dictionary<string, ushort> symbols = new Dictionary<string, ushort>();
            Dictionary<string, ushort> globals = new Dictionary<string, ushort>();

            //List<string> functions = new List<string>();
            //List<string> classes = new List<string>();
            Dictionary<string, OSIFile.FunctionInfo> functions = new Dictionary<string, OSIFile.FunctionInfo>();
            Dictionary<string, OSIFile.ClassInfo> classes = new Dictionary<string, OSIFile.ClassInfo>();

            // Collect existing names
            for (int i = 0; i < result.OSI.Strings.Count; i++)
                strings.Add(result.OSI.Strings[i], (ushort)i);
            for (int i = 0; i < result.OSI.Symbols.Count; i++)
                symbols.Add(result.OSI.Symbols[i], (ushort)i);
            for (int i = 0; i < result.OSI.Globals.Count; i++)
                globals.Add(result.OSI.Globals[i], (ushort)i);
            for (int i = 0; i < result.OSI.Functions.Count; i++)
                functions.Add(result.OSI.Functions[i].Name, result.OSI.Functions[i]);
            for (int i = 0; i < result.OSI.Classes.Count; i++)
                classes.Add(result.OSI.Classes[i].Name, result.OSI.Classes[i]);

            // Add new symbols, globals, class info, and method info
            foreach (Parser.Result parseResult in parseResults)
            {
                foreach (Statements.ClassStatement cls in parseResult.Classes)
                {
                    List<ushort> propertySymbols = new List<ushort>();
                    List<OSIFile.MethodInfo> methods = new List<OSIFile.MethodInfo>();
                    if (classes.ContainsKey(cls.Name.Content))
                    {
                        result.Errors.Add(new SyntaxError("Class already exists with same name.", cls.Name.SourceLocation.Offset, cls.Name.SourceLength, 0));
                    }
                    else
                    {
                        foreach (var prop in cls.Properties)
                        {
                            if (!symbols.ContainsKey(prop.Name.Content))
                            {
                                ushort propIndex = (ushort)result.OSI.Symbols.Count;
                                result.OSI.Symbols.Add(prop.Name.Content);
                                symbols.Add(prop.Name.Content, propIndex);
                                propertySymbols.Add(propIndex);
                            }
                            else
                            {
                                propertySymbols.Add(symbols[prop.Name.Content]);
                            }
                        }
                        foreach (var method in cls.Methods)
                        {
                            if (!symbols.ContainsKey(method.Name.Content))
                            {
                                ushort methodIndex = (ushort)result.OSI.Symbols.Count;
                                result.OSI.Symbols.Add(method.Name.Content);
                                symbols.Add(method.Name.Content, methodIndex);
                                methods.Add(new OSIFile.MethodInfo(methodIndex));
                            }
                            else
                            {
                                methods.Add(new OSIFile.MethodInfo(symbols[method.Name.Content]));
                            }
                        }
                    }
                    result.OSI.Classes.Add(new OSIFile.ClassInfo(cls.Name.Content, propertySymbols, methods));
                }

                foreach (Statements.SubroutineStatement func in parseResult.Functions)
                {
                    if (functions.ContainsKey(func.Name.Content))
                    {
                        result.Errors.Add(new SyntaxError("Function already exists with same name.", func.Name.SourceLocation.Offset, func.Name.SourceLength, 0));
                    }
                    else
                    {
                        OSIFile.FunctionInfo newFunc = new OSIFile.FunctionInfo(func.Name.Content, (ushort)func.Parameters.Count);
                        functions.Add(func.Name.Content, newFunc);
                        result.OSI.Functions.Add(newFunc);
                    }
                }

                foreach (Statements.GlobalStatement global in parseResult.Globals)
                {
                    if (globals.ContainsKey(global.Name.Content))
                    {
                        // Duplicate globals are technically allowed
                        // TODO: Warn about duplicate globals
                        //result.Errors.Add(new SyntaxError("Global already exists with same name.", global.Name.SourceLocation.Offset, global.Name.SourceLength, 0));
                    }
                    else
                    {
                        ushort globalIndex = (ushort)result.OSI.Globals.Count;
                        result.OSI.Globals.Add(global.Name.Content);
                        globals.Add(global.Name.Content, globalIndex);
                    }
                }

                // Compile the functions
                foreach (SubroutineStatement function in parseResult.Functions)
                {
                    SubroutineContext context = new SubroutineContext(result.OSI, function.Parameters.Select(token => token.Content));
                    foreach (InstructionStatement stmt in function.Body.Instructions)
                    {
                        stmt.AcceptVisitor(context);
                    }
                    context.FinalizeInstructions();
                    OSIFile.FunctionInfo destination = functions[function.Name.Content];
                    destination.Instructions.Clear();
                    destination.Instructions.AddRange(context.Instructions);
                }

                // Compile the methods
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
