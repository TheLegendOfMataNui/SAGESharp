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

        private class SubroutineScope
        {
            public Dictionary<string, int> Locals = new Dictionary<string, int>();
            public SubroutineScope ParentScope { get; }

            public SubroutineScope(SubroutineScope parentScope)
            {
                this.ParentScope = parentScope;
            }
        }

        private class SubroutineContext : ExpressionVisitor<uint, object>, Statements.StatementVisitor<uint>
        {
            public List<Instruction> Instructions { get; } = new List<Instruction>();
            private OSIFile OSI { get; }
            public bool IsFinalized { get; private set; }
            public bool IsInstanceMethod { get; }
            private ushort ParameterCount { get; } // Including 'this'
            private ushort LocalCount = 0;
            private SubroutineScope BaseScope { get; }
            private SubroutineScope CurrentScope { get; set; }

            public SubroutineContext(OSIFile osi, bool isInstanceMethod, IEnumerable<string> parameterNames)
            {
                this.OSI = osi;
                BaseScope = new SubroutineScope(null);
                CurrentScope = BaseScope;
                foreach (string param in parameterNames) {
                    AddLocal(param);
                }
                ParameterCount = LocalCount;
                this.IsInstanceMethod = isInstanceMethod;
            }

            public void FinalizeInstructions()
            {
                if (IsFinalized)
                    throw new InvalidOperationException("Cannot modify a SubroutineContext after it has been finalized.");
                if (LocalCount > ParameterCount)
                {
                    Instructions.Insert(0, new BCLInstruction(BCLOpcode.CreateStackVariables, (sbyte)(LocalCount - ParameterCount)));
                }
                if (this.IsInstanceMethod && ParameterCount > 0)
                {
                    Instructions.Insert(0, new BCLInstruction(BCLOpcode.MemberFunctionArgumentCheck, (sbyte)ParameterCount));
                }
                if ((Instructions[Instructions.Count - 1] as BCLInstruction)?.Opcode != BCLOpcode.Return)
                {
                    Instructions.Add(new BCLInstruction(BCLOpcode.PushNothing));
                    Instructions.Add(new BCLInstruction(BCLOpcode.Return));
                }
            }

            private int AddLocal(string localName)
            {
                if (IsFinalized)
                    throw new InvalidOperationException("Cannot modify a SubroutineContext after it has been finalized.");
                SubroutineScope scope = CurrentScope;
                while (scope != null)
                {
                    if (scope.Locals.ContainsKey(localName))
                        throw new ArgumentException("Variable is already declared, and would conflict.");
                    scope = scope.ParentScope;
                }
                CurrentScope.Locals.Add(localName, LocalCount);
                return LocalCount++;
            }

            // Could be a local in one of the current scopes or a global.
            private ushort? FindVariable(string name)
            {
                ushort? variableID = null;

                // Local?
                SubroutineScope scope = CurrentScope;
                while (scope != null)
                {
                    if (scope.Locals.ContainsKey(name))
                    {
                        variableID = (ushort)scope.Locals[name];
                        break;
                    }
                    scope = scope.ParentScope;
                }

                // Global?
                if (!variableID.HasValue)
                {
                    if (OSI.Globals.Contains(name))
                    {
                        // Highest bit (0x8000) means global
                        variableID = (ushort)(OSI.Globals.IndexOf(name) | (1 << 15));
                    }
                }

                return variableID;
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

            private void EnterScope()
            {
                SubroutineScope scope = new SubroutineScope(CurrentScope);
                CurrentScope = scope;
            }

            private void LeaveScope()
            {
                if (CurrentScope.ParentScope != null)
                {
                    CurrentScope = CurrentScope.ParentScope;
                }
                else
                {
                    throw new InvalidOperationException("Cannot leave the base subroutine scope.");
                }
            }

            private string UnescapeString(string value)
            {
                return value.Replace("\\\"", "\"").Replace("\\n", "\n");
            }

            // TODO: Make a corresponding EscapeString and use it in OSIFile.ToString()

            #region Expressions
            public uint VisitArrayAccessExpression(ArrayAccessExpression expr, object context)
            {
                throw new NotImplementedException();
            }

            public uint VisitArrayExpression(ArrayExpression expr, object context)
            {
                throw new NotImplementedException();
            }

            public uint VisitBinaryExpression(BinaryExpression expr, object context)
            {
                uint size = 0;
                size += expr.Left.AcceptVisitor(this, context);
                size += expr.Right.AcceptVisitor(this, context);
                List<Instruction> ops = new List<Instruction>();
                if (expr.Operation.Type == TokenType.Ampersand)
                {
                    ops.Add(new BCLInstruction(BCLOpcode.BitwiseAnd));
                }
                else if (expr.Operation.Type == TokenType.AmpersandAmpersand)
                {
                    ops.Add(new BCLInstruction(BCLOpcode.And));
                }
                else if (expr.Operation.Type == TokenType.Asterisk)
                {
                    ops.Add(new BCLInstruction(BCLOpcode.Multiply));
                }
                else if (expr.Operation.Type == TokenType.Caret)
                {
                    ops.Add(new BCLInstruction(BCLOpcode.Power));
                }
                else if (expr.Operation.Type == TokenType.ColonColon)
                {
                    throw new NotImplementedException();
                }
                else if (expr.Operation.Type == TokenType.ColonColonDollarSign)
                {
                    throw new NotImplementedException();
                }
                else if (expr.Operation.Type == TokenType.Dash)
                {
                    ops.Add(new BCLInstruction(BCLOpcode.Subtract));
                }
                else if (expr.Operation.Type == TokenType.EqualsEquals)
                {
                    ops.Add(new BCLInstruction(BCLOpcode.EqualTo));
                }
                else if (expr.Operation.Type == TokenType.ExclamationEquals)
                {
                    ops.Add(new BCLInstruction(BCLOpcode.EqualTo));
                    ops.Add(new BCLInstruction(BCLOpcode.Not));
                }
                else if (expr.Operation.Type == TokenType.Greater)
                {
                    ops.Add(new BCLInstruction(BCLOpcode.GreaterThan));
                }
                else if (expr.Operation.Type == TokenType.GreaterEquals)
                {
                    ops.Add(new BCLInstruction(BCLOpcode.GreaterOrEqual));
                }
                else if (expr.Operation.Type == TokenType.GreaterGreater)
                {
                    ops.Add(new BCLInstruction(BCLOpcode.ShiftRight));
                }
                else if (expr.Operation.Type == TokenType.Less)
                {
                    ops.Add(new BCLInstruction(BCLOpcode.LessThan));
                }
                else if (expr.Operation.Type == TokenType.LessEquals)
                {
                    ops.Add(new BCLInstruction(BCLOpcode.LessOrEqual));
                }
                else if (expr.Operation.Type == TokenType.LessLess)
                {
                    ops.Add(new BCLInstruction(BCLOpcode.ShiftLeft));
                }
                else if (expr.Operation.Type == TokenType.Octothorpe)
                {
                    ops.Add(new BCLInstruction(BCLOpcode.BitwiseXor));
                }
                else if (expr.Operation.Type == TokenType.Percent)
                {
                    ops.Add(new BCLInstruction(BCLOpcode.Modulus));
                }
                else if (expr.Operation.Type == TokenType.Period)
                {
                    throw new NotImplementedException();
                }
                else if (expr.Operation.Type == TokenType.PeriodDollarSign)
                {
                    throw new NotImplementedException();
                }
                else if (expr.Operation.Type == TokenType.Pipe)
                {
                    ops.Add(new BCLInstruction(BCLOpcode.BitwiseOr));
                }
                else if (expr.Operation.Type == TokenType.PipePipe)
                {
                    ops.Add(new BCLInstruction(BCLOpcode.Or));
                }
                else if (expr.Operation.Type == TokenType.Plus)
                {
                    ops.Add(new BCLInstruction(BCLOpcode.Add));
                }
                else if (expr.Operation.Type == TokenType.Slash)
                {
                    ops.Add(new BCLInstruction(BCLOpcode.Divide));
                }
                else
                {
                    throw new InvalidOperationException("Invalid binary operator: " + expr.Operation.Type);
                }
                Instructions.AddRange(ops);
                return size + (uint)ops.Sum(instruction => instruction.Size);
            }

            public uint VisitCallExpression(CallExpression expr, object context)
            {
                throw new NotImplementedException();
            }

            public uint VisitConstructorExpression(ConstructorExpression expr, object context)
            {
                throw new NotImplementedException();
            }

            public uint VisitGroupingExpression(GroupingExpression expr, object context)
            {
                return expr.Contents.AcceptVisitor(this, context);
            }

            public uint VisitLiteralExpression(LiteralExpression expr, object context)
            {
                List<Instruction> ops = new List<Instruction>();
                if (expr.Value.Type == TokenType.StringLiteral)
                {
                    ushort index = AddOrGetString(expr.Value.Content.Substring(1, expr.Value.Content.Length - 2));
                    ops.Add(new BCLInstruction(BCLOpcode.PushConstantString, index));
                }
                else if (expr.Value.Type == TokenType.IntegerLiteral)
                {
                    int value = Int32.Parse(expr.Value.Content);
                    if (value == 0)
                    {
                        ops.Add(new BCLInstruction(BCLOpcode.PushConstant0));
                    }
                    else if (value <= SByte.MaxValue && value >= SByte.MinValue)
                    {
                        ops.Add(new BCLInstruction(BCLOpcode.PushConstanti8, (sbyte)value));
                    }
                    else if (value <= Int16.MaxValue && value >= Int16.MinValue)
                    {
                        ops.Add(new BCLInstruction(BCLOpcode.PushConstanti16, (short)value));
                    }
                    else
                    {
                        ops.Add(new BCLInstruction(BCLOpcode.PushConstanti32, value));
                    }
                }
                else if (expr.Value.Type == TokenType.FloatLiteral)
                {
                    ops.Add(new BCLInstruction(BCLOpcode.PushConstantf32, Single.Parse(expr.Value.Content)));
                }
                else if (expr.Value.Type == TokenType.KeywordTrue)
                {
                    ops.Add(new BCLInstruction(BCLOpcode.PushConstanti8, (sbyte)1));
                }
                else if (expr.Value.Type == TokenType.KeywordFalse)
                {
                    ops.Add(new BCLInstruction(BCLOpcode.PushConstant0));
                }
                else if (expr.Value.Type == TokenType.KeywordNull)
                {
                    ops.Add(new BCLInstruction(BCLOpcode.PushNothing));
                }
                else
                {
                    throw new InvalidOperationException("Invalid literal type: " + expr.Value.Type);
                }
                Instructions.AddRange(ops);
                return (uint)ops.Sum(instruction => instruction.Size);
            }

            public uint VisitUnaryExpression(UnaryExpression expr, object context)
            {
                uint size = 0;
                if (expr.IsPrefix && (expr.Operation.Type == TokenType.PlusPlus || expr.Operation.Type == TokenType.DashDash))
                {
                    // TODO: Increment or decrement the assignable before compiling the operand
                    throw new NotImplementedException();
                }
                else
                {
                    size += expr.Contents.AcceptVisitor(this, context);
                    List<Instruction> ops = new List<Instruction>();
                    if (expr.Operation.Type == TokenType.Exclamation)
                    {
                        ops.Add(new BCLInstruction(BCLOpcode.Not));
                    }
                    else if (expr.Operation.Type == TokenType.Tilde)
                    {
                        ops.Add(new BCLInstruction(BCLOpcode.BitwiseNot));
                    }
                    else if (expr.Operation.Type == TokenType.Dash)
                    {
                        ops.Add(new BCLInstruction(BCLOpcode.PushConstanti8, (sbyte)-1));
                        ops.Add(new BCLInstruction(BCLOpcode.Multiply));
                    }
                    else if (expr.Operation.Type == TokenType.PlusPlus)
                    {
                        // TODO: Increment the assignable
                        throw new NotImplementedException();
                    }
                    else if (expr.Operation.Type == TokenType.DashDash)
                    {
                        // TODO: Decrement the assignable
                        throw new NotImplementedException();
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid unary operator " + expr.Operation.Type);
                    }
                    Instructions.AddRange(ops);
                    return size + (uint)ops.Sum(instruction => instruction.Size);
                }
            }

            public uint VisitVariableExpression(VariableExpression expr, object context)
            {
                uint size = 0;
                ushort? variableID = FindVariable(expr.Symbol.Content);

                if (variableID.HasValue)
                {
                    BCLInstruction getVariable = new BCLInstruction(BCLOpcode.GetVariableValue, variableID.Value);
                    Instructions.Add(getVariable);
                    size += getVariable.Size;
                }
                else
                {
                    throw new ArgumentException("Variable '" + expr.Symbol.Content + "' is not a valid local or global.");
                }

                return size;
            }
            #endregion

            #region Statements
            public uint VisitBlockStatement(BlockStatement s)
            {
                uint size = 0;
                EnterScope();
                foreach (InstructionStatement childStatement in s.Instructions)
                {
                    size += childStatement.AcceptVisitor(this);
                }
                LeaveScope();
                return size;
            }

            public uint VisitClassStatement(ClassStatement s)
            {
                throw new InvalidOperationException("Class statements are not allowed in a subroutine body.");
            }

            public uint VisitPropertyStatement(PropertyStatement s)
            {
                throw new InvalidOperationException("Property statements are not allowed in a subroutine body.");
            }

            public uint VisitSubroutineStatement(SubroutineStatement s)
            {
                throw new InvalidOperationException("Subroutine statements are not allowed inside the body of another subroutine.");
            }

            public uint VisitGlobalStatement(GlobalStatement s)
            {
                throw new InvalidOperationException("Global statements are not allowed inside a subroutine body.");
            }

            public uint VisitExpressionStatement(ExpressionStatement s)
            {
                uint size = s.Expression.AcceptVisitor(this, null);
                BCLInstruction popInstruction = new BCLInstruction(BCLOpcode.Pop);
                Instructions.Add(popInstruction);
                size += popInstruction.Size;
                return size;
            }

            public uint VisitReturnStatement(ReturnStatement s)
            {
                List<Instruction> ops = new List<Instruction>();
                uint size = 0;
                if (s.Value != null)
                {
                    size += s.Value.AcceptVisitor(this, null);
                }
                else
                {
                    ops.Add(new BCLInstruction(BCLOpcode.PushNothing));
                }
                ops.Add(new BCLInstruction(BCLOpcode.Return));
                Instructions.AddRange(ops);
                return size + (uint)ops.Sum(instruction => instruction.Size);
            }

            public uint VisitIfStatement(IfStatement s)
            {
                uint size = 0;

                BCLInstruction mainBranchInstruction = null;

                // 'else' statements have no condition
                if (s.Condition != null)
                {
                    size += s.Condition.AcceptVisitor(this, null);
                    mainBranchInstruction = new BCLInstruction(BCLOpcode.CompareAndBranchIfFalse);
                    size += mainBranchInstruction.Size;
                    Instructions.Add(mainBranchInstruction);
                }

                uint bodySize = s.Body.AcceptVisitor(this);
                size += bodySize;

                if (s.ElseStatement != null)
                {
                    BCLInstruction branchToEndInstruction = new BCLInstruction(BCLOpcode.BranchAlways);
                    size += branchToEndInstruction.Size;
                    bodySize += branchToEndInstruction.Size;
                    Instructions.Add(branchToEndInstruction);

                    uint elseSize = s.ElseStatement.AcceptVisitor(this);
                    size += elseSize;
                    branchToEndInstruction.Arguments[0].Value = (short)elseSize;
                }

                if (s.Condition != null)
                {
                    mainBranchInstruction.Arguments[0].Value = (short)bodySize;
                }

                return size;
            }

            public uint VisitWhileStatement(WhileStatement s)
            {
                uint size = 0;

                size += s.Condition.AcceptVisitor(this, null);
                BCLInstruction exitBranch = new BCLInstruction(BCLOpcode.CompareAndBranchIfFalse);
                Instructions.Add(exitBranch);
                size += exitBranch.Size;

                uint bodySize = 0;
                bodySize += s.Body.AcceptVisitor(this);
                BCLInstruction loopBranch = new BCLInstruction(BCLOpcode.BranchAlways);
                Instructions.Add(loopBranch);
                bodySize += loopBranch.Size;

                size += bodySize;
                loopBranch.Arguments[0].Value = (short)(-1 * (short)size);
                exitBranch.Arguments[0].Value = (short)bodySize;

                return size;
            }

            public uint VisitAssignmentStatement(AssignmentStatement s)
            {
                if (s.Target is VariableExpression varExpr)
                {
                    if (varExpr.Symbol.Type == TokenType.KeywordThis)
                    {
                        throw new ArgumentException("'this' is not allowed to be modified.");
                    }

                    ushort? variableID = FindVariable(varExpr.Symbol.Content);

                    if (variableID.HasValue)
                    {
                        uint size = 0;
                        size += s.Value.AcceptVisitor(this, null);
                        BCLInstruction setVariable = new BCLInstruction(BCLOpcode.SetVariableValue, variableID.Value);
                        Instructions.Add(setVariable);
                        size += setVariable.Size;
                        return size;
                    }
                    else
                    {
                        throw new ArgumentException("Variable '" + varExpr.Symbol.Content + "' is not a valid local or global.");
                    }
                }
                else
                {
                    throw new ArgumentException("Cannot assign into ");
                }
            }

            public uint VisitVariableDeclarationStatement(VariableDeclarationStatement s)
            {
                if (s.Name.Type == TokenType.KeywordThis)
                {
                    throw new ArgumentException("Cannot create local variables with reserved name 'this'.");
                }

                uint size = 0;
                ushort localIndex = (ushort)AddLocal(s.Name.Content);
                List<Instruction> ops = new List<Instruction>();
                if (s.Initializer != null)
                {
                    size += s.Initializer.AcceptVisitor(this, null);
                    ops.Add(new BCLInstruction(BCLOpcode.SetVariableValue, (ushort)localIndex));
                }
                Instructions.AddRange(ops);
                return size + (uint)ops.Sum(instruction => instruction.Size);
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
                    OSIFile.ClassInfo clsInfo = new OSIFile.ClassInfo(cls.Name.Content, propertySymbols, methods);
                    classes.Add(cls.Name.Content, clsInfo);
                    result.OSI.Classes.Add(clsInfo);
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
                    SubroutineContext context = new SubroutineContext(result.OSI, false, function.Parameters.Select(token => token.Content));
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
                foreach (ClassStatement cls in parseResult.Classes)
                {
                    foreach (SubroutineStatement method in cls.Methods)
                    {
                        List<string> parameters = new List<string>();
                        parameters.Add("this");
                        parameters.AddRange(method.Parameters.Select(token => token.Content));
                        SubroutineContext context = new SubroutineContext(result.OSI, true, parameters);
                        foreach (InstructionStatement stmt in method.Body.Instructions)
                        {
                            stmt.AcceptVisitor(context);
                        }
                        context.FinalizeInstructions();
                        OSIFile.MethodInfo destination = classes[cls.Name.Content].Methods.Find(m => m.NameSymbol == symbols[method.Name.Content]);
                        destination.Instructions.Clear();
                        destination.Instructions.AddRange(context.Instructions);
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
