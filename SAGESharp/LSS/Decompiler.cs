using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAGESharp.OSI;
using SAGESharp.LSS.Statements;
using SAGESharp.LSS.Expressions;
using SAGESharp.OSI.ControlFlow;

namespace SAGESharp.LSS
{
    public static class Decompiler
    {
        /// <summary>
        /// Stores the state of decompilation so it can be used across continuous chunks of a subroutine.
        /// </summary>
        // TODO: Make private once the graph analysis is in Decompiler
        public class SubroutineContext
        {
            public OSIFile OSI { get; }
            public string Name { get; }
            public bool IsMemberMethod { get; }
            public List<Token> Parameters { get; }
            //public List<Instruction> Instructions { get; }
            public SourceSpan OutputSpan { get; }

            public Stack<Expression> Stack { get; }
            public List<string> Variables { get; }

            public SubroutineContext(OSIFile osi, string name, bool isMemberMethod, List<Token> parameters, SourceSpan outputSpan)
            {
                this.OSI = osi;
                this.Name = name;
                this.IsMemberMethod = isMemberMethod;
                this.Parameters = parameters;
                this.OutputSpan = outputSpan;
                this.Stack = new Stack<Expression>();
                this.Variables = new List<string>();

                if (this.IsMemberMethod)
                    Variables.Add("this");
                foreach (Token parameter in parameters)
                    Variables.Add(parameter.Content);
            }

            public SubroutineContext(OSIFile osi, string name, bool isMemberMethod, List<Token> parameters, SourceSpan outputSpan, Stack<Expression> stack, List<string> variables)
            {
                this.OSI = osi;
                this.Name = name;
                this.IsMemberMethod = isMemberMethod;
                this.Parameters = parameters;
                this.OutputSpan = outputSpan;
                this.Stack = stack;
                this.Variables = variables;
            }

            /// <summary>
            /// Returns a shallow clone of this SubroutineContext, with the stack cloned as well.
            /// </summary>
            /// <returns></returns>
            public SubroutineContext CloneStack()
            {
                return new SubroutineContext(OSI, Name, IsMemberMethod, Parameters, OutputSpan, new Stack<Expression>(Stack.Reverse()), Variables);
            }
        }

        private static BinaryExpression DecompileBinaryExpression(Stack<Expression> stack, TokenType operatorType, string op, SourceSpan span)
        {
            Expression right = stack.Pop();
            Expression left = stack.Pop();
            return DecompileBinaryExpression(stack, left, right, operatorType, op, span);
        }

        private static BinaryExpression DecompileBinaryExpression(Stack<Expression> stack, Expression left, Expression right, TokenType operatorType, string op, SourceSpan span)
        {
            return new BinaryExpression(left, new Token(operatorType, op, span), right);
        }

        public static SubroutineStatement DecompileFunction(OSIFile osi, OSIFile.FunctionInfo function, SourceSpan outputSpan)
        {
            List<Token> parameters = new List<Token>();

            for (int i = 0; i < function.ParameterCount; i++)
            {
                parameters.Add(new Token(TokenType.Symbol, "param" + (i + 1), outputSpan));
            }

            return DecompileSubroutine(osi, function.Name, false, parameters, function.Instructions, outputSpan, function.BytecodeOffset);
        }

        public static SubroutineStatement DecompileMethod(OSIFile osi, OSIFile.MethodInfo method, SourceSpan outputSpan)
        {
            List<Token> parameters = new List<Token>();
            if (method.Instructions.Count > 0 && method.Instructions[0] is BCLInstruction argCheck && argCheck.Opcode == BCLOpcode.MemberFunctionArgumentCheck)
            {
                for (int i = 0; i < argCheck.Arguments[0].GetValue<sbyte>() - 1; i++)
                {
                    parameters.Add(new Token(TokenType.Symbol, "param" + (i + 1), outputSpan));
                }
            }
            return DecompileSubroutine(osi, osi.Symbols[method.NameSymbol], true, parameters, method.Instructions, outputSpan, method.BytecodeOffset);
        }

        public static SubroutineStatement DecompileSubroutine(OSIFile osi, string name, bool isMemberMethod, List<Token> parameters, List<Instruction> instructions, SourceSpan outputSpan, uint bytecodeOffset)
        {
            //List<InstructionStatement> statements = new List<InstructionStatement>();

            //Stack<Expression> stack = new Stack<Expression>();
            //List<string> variables = new List<string>();

            SubroutineGraph graph = Analyzer.ReconstructControlFlow(new SubroutineGraph(instructions, bytecodeOffset), new SubroutineContext(osi, name, isMemberMethod, parameters, outputSpan));

            List<InstructionStatement> statements = (graph.StartNode.OutAlwaysJump.Destination as LSSNode).Statements;
            BlockStatement body = new BlockStatement(outputSpan, statements);
            return new SubroutineStatement(outputSpan, new Token(TokenType.Symbol, name, outputSpan), parameters, body);
        }

        public static List<InstructionStatement> DecompileSubroutineChunk(SubroutineContext context, List<Instruction> instructions)
        {
            List<InstructionStatement> statements = new List<InstructionStatement>();

            for (int i = 0; i < instructions.Count; i++)
            {
                if (instructions[i] is BCLInstruction bcl)
                {
                    switch (bcl.Opcode)
                    {
                        case BCLOpcode.Add:
                            context.Stack.Push(DecompileBinaryExpression(context.Stack, TokenType.Plus, "+", context.OutputSpan));
                            break;
                        case BCLOpcode.And:
                            context.Stack.Push(DecompileBinaryExpression(context.Stack, TokenType.AmpersandAmpersand, "&&", context.OutputSpan));
                            break;
                        case BCLOpcode.EqualTo:
                            context.Stack.Push(DecompileBinaryExpression(context.Stack, TokenType.EqualsEquals, "==", context.OutputSpan));
                            break;
                        case BCLOpcode.LessThan:
                            context.Stack.Push(DecompileBinaryExpression(context.Stack, TokenType.Less, "<", context.OutputSpan));
                            break;
                        case BCLOpcode.GreaterThan:
                            context.Stack.Push(DecompileBinaryExpression(context.Stack, TokenType.Greater, ">", context.OutputSpan));
                            break;
                        case BCLOpcode.LessOrEqual:
                            context.Stack.Push(DecompileBinaryExpression(context.Stack, TokenType.LessEquals, "<=", context.OutputSpan));
                            break;
                        case BCLOpcode.GreaterOrEqual:
                            context.Stack.Push(DecompileBinaryExpression(context.Stack, TokenType.GreaterEquals, ">=", context.OutputSpan));
                            break;
                        case BCLOpcode.Or:
                            context.Stack.Push(DecompileBinaryExpression(context.Stack, TokenType.PipePipe, "||", context.OutputSpan));
                            break;
                        case BCLOpcode.Not:
                            {
                                Expression popped = context.Stack.Pop();
                                if (popped is BinaryExpression binaryOperand && binaryOperand.Operation.Type == TokenType.EqualsEquals)
                                {
                                    context.Stack.Push(new BinaryExpression(binaryOperand.Left, new Token(TokenType.ExclamationEquals, "!=", context.OutputSpan), binaryOperand.Right));
                                }
                                else
                                {
                                    context.Stack.Push(new UnaryExpression(popped, new Token(TokenType.Exclamation, "!", context.OutputSpan), true));
                                }
                                break;
                            }
                        case BCLOpcode.Subtract:
                            context.Stack.Push(DecompileBinaryExpression(context.Stack, TokenType.Dash, "-", context.OutputSpan));
                            break;
                        case BCLOpcode.Multiply:
                            context.Stack.Push(DecompileBinaryExpression(context.Stack, TokenType.Asterisk, "*", context.OutputSpan));
                            break;
                        case BCLOpcode.Divide:
                            context.Stack.Push(DecompileBinaryExpression(context.Stack, TokenType.Slash, "/", context.OutputSpan));
                            break;
                        case BCLOpcode.Power:
                            context.Stack.Push(DecompileBinaryExpression(context.Stack, TokenType.Caret, "^", context.OutputSpan));
                            break;
                        case BCLOpcode.Modulus:
                            context.Stack.Push(DecompileBinaryExpression(context.Stack, TokenType.Percent, "%", context.OutputSpan));
                            break;
                        case BCLOpcode.ShiftLeft:
                            context.Stack.Push(DecompileBinaryExpression(context.Stack, TokenType.LessLess, "<<", context.OutputSpan));
                            break;
                        case BCLOpcode.ShiftRight:
                            context.Stack.Push(DecompileBinaryExpression(context.Stack, TokenType.GreaterGreater, ">>", context.OutputSpan));
                            break;
                        case BCLOpcode.AppendToArray:
                            {
                                Expression value = context.Stack.Pop();
                                Expression array = context.Stack.Pop();
                                // If array is ArrayExpression, collapse into it
                                if (array is ArrayExpression arrExpr && context.Stack.Peek() == arrExpr)
                                {
                                    context.Stack.Pop();
                                    List<Expression> elements = new List<Expression>(arrExpr.Elements);
                                    elements.Add(value);
                                    context.Stack.Push(new ArrayExpression(arrExpr.Span, elements));
                                }
                                else
                                {
                                    statements.Add(new ExpressionStatement(new CallExpression(context.OutputSpan, DecompileBinaryExpression(context.Stack, array, new VariableExpression(new Token(TokenType.KeywordAppend, "__append", context.OutputSpan)), TokenType.Period, ".", context.OutputSpan), new Expression[] { value })));
                                }
                                break;
                            }
                        case BCLOpcode.BitwiseAnd:
                            context.Stack.Push(DecompileBinaryExpression(context.Stack, TokenType.Ampersand, "&", context.OutputSpan));
                            break;
                        case BCLOpcode.BitwiseNot:
                            context.Stack.Push(new UnaryExpression(context.Stack.Pop(), new Token(TokenType.Tilde, "~", context.OutputSpan), false));
                            break;
                        case BCLOpcode.BitwiseOr:
                            context.Stack.Push(DecompileBinaryExpression(context.Stack, TokenType.PipePipe, "||", context.OutputSpan));
                            break;
                        case BCLOpcode.BitwiseXor:
                            context.Stack.Push(DecompileBinaryExpression(context.Stack, TokenType.Octothorpe, "#", context.OutputSpan));
                            break;
                        case BCLOpcode.ConvertToFloat:
                            context.Stack.Push(new CallExpression(context.OutputSpan, new VariableExpression(new Token(TokenType.KeywordToFloat, "__tofloat", context.OutputSpan)), new Expression[] { context.Stack.Pop() }));
                            break;
                        case BCLOpcode.ConvertToInteger:
                            context.Stack.Push(new CallExpression(context.OutputSpan, new VariableExpression(new Token(TokenType.KeywordToInt, "__toint", context.OutputSpan)), new Expression[] { context.Stack.Pop() }));
                            break;
                        case BCLOpcode.ConvertToString:
                            context.Stack.Push(new CallExpression(context.OutputSpan, new VariableExpression(new Token(TokenType.KeywordToString, "__tostring", context.OutputSpan)), new Expression[] { context.Stack.Pop() }));
                            break;
                        case BCLOpcode.CreateArray:
                            context.Stack.Push(new ArrayExpression(context.OutputSpan, new Expression[] { }));
                            break;
                        case BCLOpcode.PushConstant0:
                            context.Stack.Push(new LiteralExpression(new Token(TokenType.IntegerLiteral, "0", context.OutputSpan)));
                            break;
                        case BCLOpcode.PushConstanti8:
                            context.Stack.Push(new LiteralExpression(new Token(TokenType.IntegerLiteral, instructions[i].Arguments[0].GetValue<sbyte>().ToString(), context.OutputSpan)));
                            break;
                        case BCLOpcode.PushConstanti16:
                            context.Stack.Push(new LiteralExpression(new Token(TokenType.IntegerLiteral, instructions[i].Arguments[0].GetValue<short>().ToString(), context.OutputSpan)));
                            break;
                        case BCLOpcode.PushConstanti24:
                            throw new NotImplementedException();
                        case BCLOpcode.PushConstanti32:
                            context.Stack.Push(new LiteralExpression(new Token(TokenType.IntegerLiteral, instructions[i].Arguments[0].GetValue<int>().ToString(), context.OutputSpan)));
                            break;
                        case BCLOpcode.PushConstantf32:
                            context.Stack.Push(new LiteralExpression(new Token(TokenType.FloatLiteral, instructions[i].Arguments[0].GetValue<float>().ToString("0.0##############"), context.OutputSpan)));
                            break;
                        case BCLOpcode.PushConstantColor8888:
                            context.Stack.Push(new CallExpression(context.OutputSpan, new VariableExpression(new Token(TokenType.KeywordRGBA, "rgba", context.OutputSpan)), new Expression[]
                            {
                                new LiteralExpression(new Token(TokenType.IntegerLiteral, ((instructions[i].Arguments[0].GetValue<uint>() & 0xFF000000) >> 24).ToString(), context.OutputSpan)),
                                new LiteralExpression(new Token(TokenType.IntegerLiteral, ((instructions[i].Arguments[0].GetValue<uint>() & 0x00FF0000) >> 16).ToString(), context.OutputSpan)),
                                new LiteralExpression(new Token(TokenType.IntegerLiteral, ((instructions[i].Arguments[0].GetValue<uint>() & 0x0000FF00) >> 8).ToString(), context.OutputSpan)),
                                new LiteralExpression(new Token(TokenType.IntegerLiteral, ((instructions[i].Arguments[0].GetValue<uint>() & 0x000000FF) >> 0).ToString(), context.OutputSpan))
                            }));
                            break;
                        case BCLOpcode.PushNothing:
                            context.Stack.Push(new LiteralExpression(new Token(TokenType.KeywordNull, "null", context.OutputSpan)));
                            break;
                        case BCLOpcode.Return:
                            statements.Add(new ReturnStatement(new Token(TokenType.KeywordReturn, "return", context.OutputSpan), context.Stack.Pop()));
                            break;
                        case BCLOpcode.PushConstantString:
                            context.Stack.Push(new LiteralExpression(new Token(TokenType.StringLiteral, "\"" + Compiler.EscapeString(context.OSI.Strings[instructions[i].Arguments[0].GetValue<ushort>()]) + "\"", context.OutputSpan)));
                            break;
                        case BCLOpcode.PushConstantColor5551:
                            throw new NotImplementedException();
                        case BCLOpcode.GetVariableValue:
                            {
                                ushort index = instructions[i].Arguments[0].GetValue<ushort>();
                                string variableName = (index & (1 << 15)) > 0 ? context.OSI.Globals[index & ~(1 << 15)] : context.Variables[index];
                                context.Stack.Push(new VariableExpression(new Token(TokenType.Symbol, variableName, context.OutputSpan)));
                                break;
                            }
                        case BCLOpcode.SetThisMemberValue:
                            statements.Add(new AssignmentStatement(new BinaryExpression(new VariableExpression(new Token(TokenType.KeywordThis, "this", context.OutputSpan)), new Token(TokenType.Period, ".", context.OutputSpan), new VariableExpression(new Token(TokenType.Symbol, context.OSI.Symbols[instructions[i].Arguments[0].GetValue<ushort>()], context.OutputSpan))), context.Stack.Pop()));
                            break;
                        case BCLOpcode.GetThisMemberFunction:
                            context.Stack.Push(new BinaryExpression(new VariableExpression(new Token(TokenType.KeywordThis, "this", context.OutputSpan)), new Token(TokenType.Period, ".", context.OutputSpan), new VariableExpression(new Token(TokenType.Symbol, context.OSI.Symbols[instructions[i].Arguments[0].GetValue<ushort>()], context.OutputSpan))));
                            break;
                        case BCLOpcode.JumpAbsolute:
                            {
                                Expression function = context.Stack.Pop(); // Pop function
                                int argCount = instructions[i].Arguments[0].GetValue<sbyte>() - 1; // 'this' doesn't count as an argument
                                List<Expression> arguments = new List<Expression>();
                                for (int j = 0; j < argCount; j++)
                                {
                                    arguments.Insert(0, context.Stack.Pop()); // Pop arguments right to left
                                }
                                context.Stack.Pop(); // 'this' was the first argument
                                // TODO: Assert that this was really 'this'
                                context.Stack.Push(new CallExpression(context.OutputSpan, function, arguments));
                                break;
                            }
                        case BCLOpcode.Pop:
                            {
                                Expression value = context.Stack.Pop();
                                // Recognise the constructor call after instantiation
                                if (value is CallExpression ctorCall
                                    && ctorCall.Target is BinaryExpression binExp
                                    && binExp.Left is ConstructorExpression ctor
                                    && binExp.Right is VariableExpression ctorName
                                    && ctor.TypeName.Content == ctorName.Symbol.Content
                                    && context.Stack.Count > 0
                                    && context.Stack.Peek() is ConstructorExpression ctorTop
                                    && ctorTop.Arguments == null
                                    && ctorTop.TypeName.Content == ctor.TypeName.Content)
                                {
                                    // We are popping off the result of the constructor call,
                                    // so just silently remove it, and then use it to complete the
                                    // half-baked ConstructorExpression.
                                    context.Stack.Pop(); // Pop the half-baked ConstructorExpression aka ctorTop
                                    context.Stack.Push(new ConstructorExpression(context.OutputSpan, ctorTop.TypeName, ctorCall.Arguments));
                                }
                                else
                                {
                                    // Some if chains have an extra Pop at the end (probably emitted by compiling a switch statement).
                                    // Consider the HACK: If we aren't popping a call, just discard.
                                    // Instead, we do a more precise detectionn in Analyzer.AnalyzeIfElse
                                    //if (value is CallExpression)
                                    //{
                                        statements.Add(new ExpressionStatement(value));
                                    //}
                                    //else
                                    //{
                                    //    // Comment out to discard non-call expression statements
                                    //    statements.Add(new ExpressionStatement(value));
                                    //}
                                }
                                break;
                            }
                        case BCLOpcode.SetMemberValue:
                            {
                                string memberName = context.OSI.Symbols[instructions[i].Arguments[0].GetValue<ushort>()];
                                Expression target = context.Stack.Pop();
                                Expression value = context.Stack.Pop();
                                statements.Add(new AssignmentStatement(new BinaryExpression(target, new Token(TokenType.Period, ".", context.OutputSpan), new VariableExpression(new Token(TokenType.Symbol, memberName, context.OutputSpan))), value));
                                break;
                            }
                        case BCLOpcode.GetMemberValue:
                            context.Stack.Push(new BinaryExpression(context.Stack.Pop(), new Token(TokenType.Period, ".", context.OutputSpan), new VariableExpression(new Token(TokenType.Symbol, context.OSI.Symbols[instructions[i].Arguments[0].GetValue<ushort>()], context.OutputSpan))));
                            break;
                        case BCLOpcode.CreateStackVariables:
                            {
                                for (int j = 0; j < instructions[i].Arguments[0].GetValue<sbyte>(); j++)
                                {
                                    string localName = "var" + (j + 1);
                                    context.Variables.Add(localName);
                                    statements.Add(new VariableDeclarationStatement(context.OutputSpan, new Token(TokenType.Symbol, localName, context.OutputSpan), null));
                                    context.Stack.Push(new VariableExpression(new Token(TokenType.Symbol, localName, context.OutputSpan)));
                                }
                                break;
                            }
                        case BCLOpcode.GetMemberFunction:
                            {
                                string functionName = context.OSI.Symbols[instructions[i].Arguments[0].GetValue<ushort>()];
                                Expression target = context.Stack.Pop();
                                context.Stack.Push(new BinaryExpression(target, new Token(TokenType.Period, ".", context.OutputSpan), new VariableExpression(new Token(TokenType.Symbol, functionName, context.OutputSpan))));
                                break;
                            }
                        case BCLOpcode.GetThisMemberValue:
                            context.Stack.Push(new BinaryExpression(new VariableExpression(new Token(TokenType.KeywordThis, "this", context.OutputSpan)), new Token(TokenType.Period, ".", context.OutputSpan), new VariableExpression(new Token(TokenType.Symbol, context.OSI.Symbols[instructions[i].Arguments[0].GetValue<ushort>()], context.OutputSpan))));
                            break;
                        case BCLOpcode.Dup:
                            //context.Stack.Push(context.Stack.Peek().Duplicate());
                            context.Stack.Push(context.Stack.Peek());
                            break;
                        case BCLOpcode.Pull:
                            //context.Stack.Push(context.Stack.ToArray()[instructions[i].Arguments[0].GetValue<sbyte>() - 1].Duplicate());
                            context.Stack.Push(context.Stack.ToArray()[instructions[i].Arguments[0].GetValue<sbyte>() - 1]);
                            break;
                        case BCLOpcode.CallGameFunction:
                            {
                                int argCount = instructions[i].Arguments[2].GetValue<sbyte>();
                                string functionNamespace = context.OSI.Strings[instructions[i].Arguments[0].GetValue<ushort>()];
                                string functionName = context.OSI.Strings[instructions[i].Arguments[1].GetValue<ushort>()];
                                Expression[] arguments = new Expression[argCount];
                                // Arguments are popped right to left.
                                for (int j = argCount - 1; j >= 0; j--)
                                {
                                    arguments[j] = context.Stack.Pop();
                                }
                                context.Stack.Push(new CallExpression(context.OutputSpan, new BinaryExpression(new VariableExpression(new Token(TokenType.Symbol, functionNamespace, context.OutputSpan)), new Token(TokenType.ColonColon, "::", context.OutputSpan), new VariableExpression(new Token(TokenType.Symbol, functionName, context.OutputSpan))), arguments));
                                break;
                            }
                        case BCLOpcode.SetVariableValue:
                            {
                                ushort index = instructions[i].Arguments[0].GetValue<ushort>();
                                string variableName = (index & (1 << 15)) > 0 ? context.OSI.Globals[index & ~(1 << 15)] : context.Variables[index];
                                statements.Add(new AssignmentStatement(new VariableExpression(new Token(TokenType.Symbol, variableName, context.OutputSpan)), context.Stack.Pop()));
                                break;
                            }
                        case BCLOpcode.CreateObject:
                            context.Stack.Push(new ConstructorExpression(context.OutputSpan, new Token(TokenType.Symbol, context.OSI.Classes[instructions[i].Arguments[0].GetValue<ushort>()].Name, context.OutputSpan), null));
                            break;
                        case BCLOpcode.GetArrayValue:
                            {
                                Expression index = context.Stack.Pop();
                                Expression array = context.Stack.Pop();
                                context.Stack.Push(new ArrayAccessExpression(context.OutputSpan, array, index));
                                break;
                            }
                        case BCLOpcode.ElementsInArray:
                            {
                                context.Stack.Push(new BinaryExpression(context.Stack.Pop(), new Token(TokenType.Period, ".", context.OutputSpan), new VariableExpression(new Token(TokenType.KeywordLength, "__length", context.OutputSpan))));
                                break;
                            }
                        case BCLOpcode.SetArrayValue:
                            {
                                Expression value = context.Stack.Pop();
                                Expression index = context.Stack.Pop();
                                Expression array = context.Stack.Pop();
                                statements.Add(new AssignmentStatement(new ArrayAccessExpression(context.OutputSpan, array, index), value));
                                break;
                            }
                        case BCLOpcode.IncrementVariable:
                            {
                                ushort index = instructions[i].Arguments[0].GetValue<ushort>();
                                string variableName = (index & (1 << 15)) > 0 ? context.OSI.Globals[index & ~(1 << 15)] : context.Variables[index];
                                statements.Add(new AssignmentStatement(new VariableExpression(new Token(TokenType.Symbol, variableName, context.OutputSpan)), new BinaryExpression(new VariableExpression(new Token(TokenType.Symbol, variableName, context.OutputSpan)), new Token(TokenType.Plus, "+", context.OutputSpan), new LiteralExpression(new Token(TokenType.IntegerLiteral, "1", context.OutputSpan)))));
                                break;
                            }
                        case BCLOpcode.DecrementVariable:
                            {
                                ushort index = instructions[i].Arguments[0].GetValue<ushort>();
                                string variableName = (index & (1 << 15)) > 0 ? context.OSI.Globals[index & ~(1 << 15)] : context.Variables[index];
                                statements.Add(new AssignmentStatement(new VariableExpression(new Token(TokenType.Symbol, variableName, context.OutputSpan)), new BinaryExpression(new VariableExpression(new Token(TokenType.Symbol, variableName, context.OutputSpan)), new Token(TokenType.Dash, "-", context.OutputSpan), new LiteralExpression(new Token(TokenType.IntegerLiteral, "1", context.OutputSpan)))));
                                break;
                            }
                        case BCLOpcode.SetRedValue:
                            {
                                Expression value = context.Stack.Pop();
                                Expression target = context.Stack.Pop();
                                if (target is CallExpression colorCtor && colorCtor.Target is VariableExpression colorCtorName && colorCtorName.Symbol.Type == TokenType.KeywordRGBA)
                                {
                                    colorCtor.Arguments[0] = value;
                                    context.Stack.Push(colorCtor);
                                }
                                else
                                {
                                    statements.Add(new AssignmentStatement(new BinaryExpression(target, new Token(TokenType.Period, ".", context.OutputSpan), new VariableExpression(new Token(TokenType.KeywordRed, "__red", context.OutputSpan))), value));
                                    if ((instructions[i + 1] as BCLInstruction)?.Opcode == BCLOpcode.Pop)
                                    {
                                        i++;
                                    }
                                    else
                                    {
                                        throw new FormatException("SetRedValue not on a RGBA constructor must be immediately followed by a Pop opcode!");
                                    }
                                }
                                break;
                            }
                        case BCLOpcode.SetGreenValue:
                            {
                                Expression value = context.Stack.Pop();
                                Expression target = context.Stack.Pop();
                                if (target is CallExpression colorCtor && colorCtor.Target is VariableExpression colorCtorName && colorCtorName.Symbol.Type == TokenType.KeywordRGBA)
                                {
                                    colorCtor.Arguments[1] = value;
                                    context.Stack.Push(colorCtor);
                                }
                                else
                                {
                                    statements.Add(new AssignmentStatement(new BinaryExpression(target, new Token(TokenType.Period, ".", context.OutputSpan), new VariableExpression(new Token(TokenType.KeywordGreen, "__green", context.OutputSpan))), value));
                                    if ((instructions[i + 1] as BCLInstruction)?.Opcode == BCLOpcode.Pop)
                                    {
                                        i++;
                                    }
                                    else
                                    {
                                        throw new FormatException("SetGreenValue not on a RGBA constructor must be immediately followed by a Pop opcode!");
                                    }
                                }
                                break;
                            }
                        case BCLOpcode.SetBlueValue:
                            {
                                Expression value = context.Stack.Pop();
                                Expression target = context.Stack.Pop();
                                if (target is CallExpression colorCtor && colorCtor.Target is VariableExpression colorCtorName && colorCtorName.Symbol.Type == TokenType.KeywordRGBA)
                                {
                                    colorCtor.Arguments[2] = value;
                                    context.Stack.Push(colorCtor);
                                }
                                else
                                {
                                    statements.Add(new AssignmentStatement(new BinaryExpression(target, new Token(TokenType.Period, ".", context.OutputSpan), new VariableExpression(new Token(TokenType.KeywordBlue, "__blue", context.OutputSpan))), value));
                                    if ((instructions[i + 1] as BCLInstruction)?.Opcode == BCLOpcode.Pop)
                                    {
                                        i++;
                                    }
                                    else
                                    {
                                        throw new FormatException("SetBlueValue not on a RGBA constructor must be immediately followed by a Pop opcode!");
                                    }
                                }
                                break;
                            }
                        case BCLOpcode.SetAlphaValue:
                            {
                                Expression value = context.Stack.Pop();
                                Expression target = context.Stack.Pop();
                                if (target is CallExpression colorCtor && colorCtor.Target is VariableExpression colorCtorName && colorCtorName.Symbol.Type == TokenType.KeywordRGBA)
                                {
                                    colorCtor.Arguments[3] = value;
                                    context.Stack.Push(colorCtor);
                                }
                                else
                                {
                                    statements.Add(new AssignmentStatement(new BinaryExpression(target, new Token(TokenType.Period, ".", context.OutputSpan), new VariableExpression(new Token(TokenType.KeywordAlpha, "__alpha", context.OutputSpan))), value));
                                    if ((instructions[i + 1] as BCLInstruction)?.Opcode == BCLOpcode.Pop)
                                    {
                                        i++;
                                    }
                                    else
                                    {
                                        throw new FormatException("SetAlphaValue not on a RGBA constructor must be immediately followed by a Pop opcode!");
                                    }
                                }
                                break;
                            }
                        case BCLOpcode.MemberFunctionArgumentCheck:
                        case BCLOpcode.Nop:
                        case BCLOpcode.DebugOn:
                        case BCLOpcode.DebugOff:
                        case BCLOpcode.LineNumber:
                        case BCLOpcode.LineNumberAlt1:
                        case BCLOpcode.LineNumberAlt2:
                        case BCLOpcode.Halt:
                            break;
                        default:
                            throw new NotImplementedException("Decompiling opcode '" + bcl.Opcode.ToString() + "' (0x" + ((byte)bcl.Opcode).ToString("X2") + ") is not implemented!");
                    }
                }
                else
                {
                    throw new FormatException("Cannot decompile abstract instruction " + instructions[i].GetType().ToString());
                }
            }

            return statements;
        }

        public static Parser.Result DecompileOSI(OSIFile osi)
        {
            Parser.Result result = new Parser.Result();

            SourceSpan span = new SourceSpan("<Decompiled>", 0, 0, 0);
            foreach (string global in osi.Globals)
            {
                result.Globals.Add(new GlobalStatement(span, new Token(TokenType.Symbol, global, span.Start.Filename, span.Start.Offset, span.Start.Line, span.Length)));
            }

            foreach (OSIFile.FunctionInfo function in osi.Functions)
            {
                result.Functions.Add(DecompileFunction(osi, function, span));
            }

            foreach (OSIFile.ClassInfo cls in osi.Classes)
            {
                List<PropertyStatement> properties = new List<PropertyStatement>();
                foreach (ushort propSymbolIndex in cls.PropertySymbols)
                {
                    properties.Add(new PropertyStatement(span, new Token(TokenType.Symbol, osi.Symbols[propSymbolIndex], span)));
                }
                List<SubroutineStatement> methods = new List<SubroutineStatement>();
                foreach (OSIFile.MethodInfo method in cls.Methods)
                {
                    methods.Add(DecompileMethod(osi, method, span));
                }
                // TODO: Find SuperclassName
                result.Classes.Add(new ClassStatement(span, new Token(TokenType.Symbol, cls.Name, span), null, properties, methods));
            }

            return result;
        }

        public static void DecompileOSIProject(OSIFile osi, string outputDirectory, string classesDirectoryName = "classes", string functionsDirectoryName = "functions")
        {
            Parser.Result decompiled = DecompileOSI(osi);

            // Create directories
            System.IO.Directory.CreateDirectory(outputDirectory);
            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(outputDirectory, classesDirectoryName));
            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(outputDirectory, functionsDirectoryName));

            // Write globals
            StringBuilder globalText = new StringBuilder();
            foreach (GlobalStatement global in decompiled.Globals)
            {
                globalText.AppendLine(PrettyPrinter.Print(global));
            }
            System.IO.File.WriteAllText(System.IO.Path.Combine(outputDirectory, "globals.lss"), globalText.ToString());

            // Write functions
            foreach (SubroutineStatement function in decompiled.Functions)
            {
                System.IO.File.WriteAllText(System.IO.Path.Combine(outputDirectory, functionsDirectoryName, function.Name.Content + ".lss"), "function " + PrettyPrinter.Print(function));
            }

            // Write classes
            foreach (ClassStatement cls in decompiled.Classes)
            {
                System.IO.File.WriteAllText(System.IO.Path.Combine(outputDirectory, classesDirectoryName, cls.Name.Content + ".lss"), PrettyPrinter.Print(cls));
            }
        }
    }
}
