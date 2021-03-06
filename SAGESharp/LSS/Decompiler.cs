﻿using System;
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

        private static Expression DecompileBinaryExpression(Stack<Expression> stack, TokenType operatorType, string op, SourceSpan span)
        {
            Expression right = stack.Pop();
            Expression left = stack.Pop();
            return DecompileBinaryExpression(stack, left, right, operatorType, op, span);
        }

        private static Expression DecompileBinaryExpression(Stack<Expression> stack, Expression left, Expression right, TokenType operatorType, string op, SourceSpan span)
        {
            return new GroupingExpression(span, new BinaryExpression(left, new Token(operatorType, op, span), right));
        }

        private static Expression Ungroup(Expression expr)
        {
            if (expr is GroupingExpression group)
            {
                return group.Contents;
            }
            else
            {
                return expr;
            }
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
                                Expression value = Ungroup(context.Stack.Pop());
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
                                    statements.Add(new ExpressionStatement(new CallExpression(context.OutputSpan, new BinaryExpression(array, new Token(TokenType.Period, ".", context.OutputSpan), new VariableExpression(new Token(TokenType.KeywordAppend, "__append", context.OutputSpan))), new Expression[] { value })));
                                    if (i < instructions.Count - 1 && instructions[i + 1] is BCLInstruction popBCL && popBCL.Opcode == BCLOpcode.Pop && context.Stack.Peek() == array)
                                    {
                                        context.Stack.Pop();
                                        i++;
                                    }
                                    else
                                    {
                                        //throw new Exception("o no!");
                                    }
                                }
                                break;
                            }
                        case BCLOpcode.BitwiseAnd:
                            context.Stack.Push(DecompileBinaryExpression(context.Stack, TokenType.Ampersand, "&", context.OutputSpan));
                            break;
                        case BCLOpcode.BitwiseNot:
                            context.Stack.Push(new UnaryExpression(context.Stack.Pop(), new Token(TokenType.Tilde, "~", context.OutputSpan), true));
                            break;
                        case BCLOpcode.BitwiseOr:
                            context.Stack.Push(DecompileBinaryExpression(context.Stack, TokenType.Pipe, "|", context.OutputSpan));
                            break;
                        case BCLOpcode.BitwiseXor:
                            context.Stack.Push(DecompileBinaryExpression(context.Stack, TokenType.Octothorpe, "#", context.OutputSpan));
                            break;
                        case BCLOpcode.ConvertToFloat:
                            context.Stack.Push(new CallExpression(context.OutputSpan, new VariableExpression(new Token(TokenType.KeywordToFloat, "__tofloat", context.OutputSpan)), new Expression[] { Ungroup(context.Stack.Pop()) }));
                            break;
                        case BCLOpcode.ConvertToInteger:
                            context.Stack.Push(new CallExpression(context.OutputSpan, new VariableExpression(new Token(TokenType.KeywordToInt, "__toint", context.OutputSpan)), new Expression[] { Ungroup(context.Stack.Pop()) }));
                            break;
                        case BCLOpcode.ConvertToString:
                            context.Stack.Push(new CallExpression(context.OutputSpan, new VariableExpression(new Token(TokenType.KeywordToString, "__tostring", context.OutputSpan)), new Expression[] { Ungroup(context.Stack.Pop()) }));
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
                            statements.Add(new ReturnStatement(new Token(TokenType.KeywordReturn, "return", context.OutputSpan), Ungroup(context.Stack.Pop())));
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
                            statements.Add(new AssignmentStatement(new BinaryExpression(new VariableExpression(new Token(TokenType.KeywordThis, "this", context.OutputSpan)), new Token(TokenType.Period, ".", context.OutputSpan), new VariableExpression(new Token(TokenType.Symbol, context.OSI.Symbols[instructions[i].Arguments[0].GetValue<ushort>()], context.OutputSpan))), Ungroup(context.Stack.Pop())));
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
                                    arguments.Insert(0, Ungroup(context.Stack.Pop())); // Pop arguments right to left
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
                                    // Instead, we do a more precise detection in Analyzer.AnalyzeIfElse (for both switch -> if chains, and foreach -> while chains)
                                    //if (value is CallExpression)
                                    //{
                                        statements.Add(new ExpressionStatement(Ungroup(value)));
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
                                Expression value = Ungroup(context.Stack.Pop());
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
                                    arguments[j] = Ungroup(context.Stack.Pop());
                                }
                                context.Stack.Push(new CallExpression(context.OutputSpan, new BinaryExpression(new VariableExpression(new Token(TokenType.Symbol, functionNamespace, context.OutputSpan)), new Token(TokenType.ColonColon, "::", context.OutputSpan), new VariableExpression(new Token(TokenType.Symbol, functionName, context.OutputSpan))), arguments));
                                break;
                            }
                        case BCLOpcode.SetVariableValue:
                            {
                                ushort index = instructions[i].Arguments[0].GetValue<ushort>();
                                string variableName = (index & (1 << 15)) > 0 ? context.OSI.Globals[index & ~(1 << 15)] : context.Variables[index];
                                statements.Add(new AssignmentStatement(new VariableExpression(new Token(TokenType.Symbol, variableName, context.OutputSpan)), Ungroup(context.Stack.Pop())));
                                break;
                            }
                        case BCLOpcode.CreateObject:
                            context.Stack.Push(new ConstructorExpression(context.OutputSpan, new Token(TokenType.Symbol, context.OSI.Classes[instructions[i].Arguments[0].GetValue<ushort>()].Name, context.OutputSpan), null));
                            break;
                        case BCLOpcode.GetArrayValue:
                            {
                                Expression index = Ungroup(context.Stack.Pop());
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
                                Expression value = Ungroup(context.Stack.Pop());
                                Expression index = Ungroup(context.Stack.Pop());
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
                                Expression value = Ungroup(context.Stack.Pop());
                                Expression target = context.Stack.Pop();
                                if (target is CallExpression colorCtor && colorCtor.Target is VariableExpression colorCtorName && colorCtorName.Symbol.Type == TokenType.KeywordRGBA)
                                {
                                    colorCtor.Arguments[0] = value;
                                    context.Stack.Push(colorCtor);
                                }
                                else
                                {
                                    context.Stack.Push(new CallExpression(context.OutputSpan, new BinaryExpression(target, new Token(TokenType.Period, ".", context.OutputSpan), new VariableExpression(new Token(TokenType.KeywordWithRed, "__withred", context.OutputSpan))), new List<Expression> { value }));
                                }
                                break;
                            }
                        case BCLOpcode.SetGreenValue:
                            {
                                Expression value = Ungroup(context.Stack.Pop());
                                Expression target = context.Stack.Pop();
                                if (target is CallExpression colorCtor && colorCtor.Target is VariableExpression colorCtorName && colorCtorName.Symbol.Type == TokenType.KeywordRGBA)
                                {
                                    colorCtor.Arguments[1] = value;
                                    context.Stack.Push(colorCtor);
                                }
                                else
                                {
                                    context.Stack.Push(new CallExpression(context.OutputSpan, new BinaryExpression(target, new Token(TokenType.Period, ".", context.OutputSpan), new VariableExpression(new Token(TokenType.KeywordWithGreen, "__withgreen", context.OutputSpan))), new List<Expression> { value }));
                                }
                                break;
                            }
                        case BCLOpcode.SetBlueValue:
                            {
                                Expression value = Ungroup(context.Stack.Pop());
                                Expression target = context.Stack.Pop();
                                if (target is CallExpression colorCtor && colorCtor.Target is VariableExpression colorCtorName && colorCtorName.Symbol.Type == TokenType.KeywordRGBA)
                                {
                                    colorCtor.Arguments[2] = value;
                                    context.Stack.Push(colorCtor);
                                }
                                else
                                {
                                    context.Stack.Push(new CallExpression(context.OutputSpan, new BinaryExpression(target, new Token(TokenType.Period, ".", context.OutputSpan), new VariableExpression(new Token(TokenType.KeywordWithBlue, "__withblue", context.OutputSpan))), new List<Expression> { value }));
                                }
                                break;
                            }
                        case BCLOpcode.SetAlphaValue:
                            {
                                Expression value = Ungroup(context.Stack.Pop());
                                Expression target = context.Stack.Pop();
                                if (target is CallExpression colorCtor && colorCtor.Target is VariableExpression colorCtorName && colorCtorName.Symbol.Type == TokenType.KeywordRGBA)
                                {
                                    colorCtor.Arguments[3] = value;
                                    context.Stack.Push(colorCtor);
                                }
                                else
                                {
                                    context.Stack.Push(new CallExpression(context.OutputSpan, new BinaryExpression(target, new Token(TokenType.Period, ".", context.OutputSpan), new VariableExpression(new Token(TokenType.KeywordWithAlpha, "__withalpha", context.OutputSpan))), new List<Expression> { value }));
                                }
                                break;
                            }
                        case BCLOpcode.GetMemberFunctionFromString:
                            {
                                Expression functionName = context.Stack.Pop();
                                Expression target = context.Stack.Pop();
                                context.Stack.Push(new BinaryExpression(target, new Token(TokenType.PeriodDollarSign, ".$", context.OutputSpan), new GroupingExpression(context.OutputSpan, functionName)));
                                break;
                            }
                        case BCLOpcode.RemoveFromArray:
                            {
                                Expression index = Ungroup(context.Stack.Pop());
                                Expression array = context.Stack.Pop();
                                //Expression alsoArray = context.Stack.Pop();
                                //context.Stack.Push(new CallExpression(context.OutputSpan, new BinaryExpression(array, new Token(TokenType.Period, ".", context.OutputSpan), new VariableExpression(new Token(TokenType.KeywordRemoveAt, "__removeat", context.OutputSpan))), new List<Expression> { index }));
                                statements.Add(new ExpressionStatement(new CallExpression(context.OutputSpan, new BinaryExpression(array, new Token(TokenType.Period, ".", context.OutputSpan), new VariableExpression(new Token(TokenType.KeywordRemoveAt, "__removeat", context.OutputSpan))), new List<Expression> { index })));
                                if (i < instructions.Count - 1 && instructions[i + 1] is BCLInstruction popBCL && popBCL.Opcode == BCLOpcode.Pop && context.Stack.Peek() == array)
                                {
                                    context.Stack.Pop();
                                    i++;
                                }
                                else
                                {
                                    //throw new Exception("o no!");
                                }

                                break;
                            }
                        case BCLOpcode.InsertIntoArray:
                            {
                                Expression value = Ungroup(context.Stack.Pop());
                                Expression index = Ungroup(context.Stack.Pop());
                                Expression array = context.Stack.Pop();
                                statements.Add(new ExpressionStatement(new CallExpression(context.OutputSpan, new BinaryExpression(array, new Token(TokenType.Period, ".", context.OutputSpan), new VariableExpression(new Token(TokenType.KeywordInsertAt, "__insertat", context.OutputSpan))), new List<Expression> { index, value })));
                                if (i < instructions.Count - 1 && instructions[i + 1] is BCLInstruction popBCL && popBCL.Opcode == BCLOpcode.Pop && context.Stack.Peek() == array)
                                {
                                    context.Stack.Pop();
                                    i++;
                                }
                                else
                                {
                                    //throw new Exception("o no!");
                                }

                                break;
                            }
                        case BCLOpcode.Increment:
                            context.Stack.Push(new GroupingExpression(context.OutputSpan, new BinaryExpression(context.Stack.Pop(), new Token(TokenType.Plus, "+", context.OutputSpan), new LiteralExpression(new Token(TokenType.IntegerLiteral, "1", context.OutputSpan)))));
                            break;
                        case BCLOpcode.Decrement:
                            context.Stack.Push(new GroupingExpression(context.OutputSpan, new BinaryExpression(context.Stack.Pop(), new Token(TokenType.Dash, "-", context.OutputSpan), new LiteralExpression(new Token(TokenType.IntegerLiteral, "1", context.OutputSpan)))));
                            break;
                        case BCLOpcode.GetRedValue:
                            context.Stack.Push(new BinaryExpression(context.Stack.Pop(), new Token(TokenType.Period, ".", context.OutputSpan), new VariableExpression(new Token(TokenType.KeywordRed, "__red", context.OutputSpan))));
                            break;
                        case BCLOpcode.GetGreenValue:
                            context.Stack.Push(new BinaryExpression(context.Stack.Pop(), new Token(TokenType.Period, ".", context.OutputSpan), new VariableExpression(new Token(TokenType.KeywordGreen, "__green", context.OutputSpan))));
                            break;
                        case BCLOpcode.GetBlueValue:
                            context.Stack.Push(new BinaryExpression(context.Stack.Pop(), new Token(TokenType.Period, ".", context.OutputSpan), new VariableExpression(new Token(TokenType.KeywordBlue, "__blue", context.OutputSpan))));
                            break;
                        case BCLOpcode.GetAlphaValue:
                            context.Stack.Push(new BinaryExpression(context.Stack.Pop(), new Token(TokenType.Period, ".", context.OutputSpan), new VariableExpression(new Token(TokenType.KeywordAlpha, "__alpha", context.OutputSpan))));
                            break;
                        case BCLOpcode.IsInteger:
                            context.Stack.Push(new CallExpression(context.OutputSpan, new VariableExpression(new Token(TokenType.KeywordIsInt, "__isint", context.OutputSpan)), new List<Expression> { Ungroup(context.Stack.Pop()) }));
                            break;
                        case BCLOpcode.IsFloat:
                            context.Stack.Push(new CallExpression(context.OutputSpan, new VariableExpression(new Token(TokenType.KeywordIsFloat, "__isfloat", context.OutputSpan)), new List<Expression> { Ungroup(context.Stack.Pop()) }));
                            break;
                        case BCLOpcode.IsString:
                            context.Stack.Push(new CallExpression(context.OutputSpan, new VariableExpression(new Token(TokenType.KeywordIsString, "__isstring", context.OutputSpan)), new List<Expression> { Ungroup(context.Stack.Pop()) }));
                            break;
                        case BCLOpcode.IsAnObject:
                            context.Stack.Push(new CallExpression(context.OutputSpan, new VariableExpression(new Token(TokenType.KeywordIsInstance, "__isinstance", context.OutputSpan)), new List<Expression> { Ungroup(context.Stack.Pop()) }));
                            break;
                        case BCLOpcode.IsGameObject:
                            context.Stack.Push(new CallExpression(context.OutputSpan, new VariableExpression(new Token(TokenType.KeywordIsObject, "__isobject", context.OutputSpan)), new List<Expression> { Ungroup(context.Stack.Pop()) }));
                            break;
                        case BCLOpcode.IsArray:
                            context.Stack.Push(new CallExpression(context.OutputSpan, new VariableExpression(new Token(TokenType.KeywordIsArray, "__isarray", context.OutputSpan)), new List<Expression> { Ungroup(context.Stack.Pop()) }));
                            break;
                        case BCLOpcode.GetObjectClassID:
                            context.Stack.Push(new CallExpression(context.OutputSpan, new VariableExpression(new Token(TokenType.KeywordClassID, "__classid", context.OutputSpan)), new List<Expression> { Ungroup(context.Stack.Pop()) }));
                            break;
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
                else if (instructions[i] is JumpStaticInstruction jumpStatic)
                {
                    // Pop call arguments right to left
                    List<Expression> arguments = new List<Expression>();
                    for (int j = 0; j < jumpStatic.ArgumentCount; j++)
                    {
                        arguments.Insert(0, context.Stack.Pop());
                    }
                    context.Stack.Push(new CallExpression(context.OutputSpan, new VariableExpression(new Token(TokenType.Symbol, jumpStatic.FunctionName, context.OutputSpan)), arguments));
                }
                else
                {
                    throw new FormatException("Cannot decompile abstract instruction " + instructions[i].GetType().ToString());
                }
            }
            if (context.Stack.Count > 0)
                context.Stack.Push(Ungroup(context.Stack.Pop())); // HACK: All control-flow conditions can be ungrouped.

            return statements;
        }

        public static Parser.Result DecompileOSI(OSIFile osi)
        {
            osi.TransformToJumpStatic();
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
                // Best-effort attempt to find the superclass
                OSIFile.ClassInfo superclass = null;
                int sharedMemberCount = 0;
                foreach (OSIFile.ClassInfo otherCls in osi.Classes)
                {
                    if (cls != otherCls)
                    {
                        bool couldMatch = true;
                        int membersInCommon = 0;
                        foreach (ushort propSymbol in otherCls.PropertySymbols)
                        {
                            if (!cls.PropertySymbols.Contains(propSymbol))
                            {
                                couldMatch = false;
                                break;
                            }
                            membersInCommon++;
                        }
                        if (couldMatch)
                        {
                            foreach (OSIFile.MethodInfo method in otherCls.Methods)
                            {
                                if (!cls.Methods.Any(m => m.NameSymbol == method.NameSymbol)) // TODO: Make sure the methods are actually the same
                                {
                                    couldMatch = false;
                                    break;
                                }
                                membersInCommon++;
                            }
                        }
                        if (couldMatch && membersInCommon > sharedMemberCount)
                        {
                            superclass = otherCls;
                            sharedMemberCount = membersInCommon;
                        }
                    }
                }

                List<PropertyStatement> properties = new List<PropertyStatement>();
                foreach (ushort propSymbolIndex in cls.PropertySymbols)
                {
                    if (superclass != null && superclass.PropertySymbols.Contains(propSymbolIndex))
                        continue;

                    properties.Add(new PropertyStatement(span, new Token(TokenType.Symbol, osi.Symbols[propSymbolIndex], span)));
                }
                List<SubroutineStatement> methods = new List<SubroutineStatement>();
                foreach (OSIFile.MethodInfo method in cls.Methods)
                {
                    if (superclass != null && superclass.Methods.Any(m => m.NameSymbol == method.NameSymbol && m.BytecodeOffset == method.BytecodeOffset))
                        continue;

                    try
                    {
                        methods.Add(DecompileMethod(osi, method, span));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[ERROR]: " + cls.Name + "." + osi.Symbols[method.NameSymbol] + ": \n\n" + ex.ToString());
                    }
                }
                result.Classes.Add(new ClassStatement(span, new Token(TokenType.Symbol, cls.Name, span), superclass == null ? null : new Token(TokenType.Symbol, superclass.Name, span), properties, methods));
            }

            // Ugly class tree printout
            System.Diagnostics.Debug.WriteLine("\n\nCLASS HIERARCHY:\n\n");
            foreach (ClassStatement cls in result.Classes)
            {
                if (cls.SuperclassName == null)
                {
                    // Pre-order traversal
                    Stack<Tuple<ClassStatement, int>> stack = new Stack<Tuple<ClassStatement, int>>();
                    stack.Push(new Tuple<ClassStatement, int>(cls, 0));

                    while (stack.Count > 0)
                    {
                        Tuple<ClassStatement, int> stmt = stack.Pop();
                        System.Diagnostics.Debug.WriteLine("".PadLeft(3 * stmt.Item2, ' ') + stmt.Item1.Name.Content);
                        foreach (ClassStatement child in result.Classes.Where(c => c.SuperclassName?.Content == stmt.Item1.Name.Content))
                        {
                            stack.Push(new Tuple<ClassStatement, int>(child, stmt.Item2 + 1));
                        }
                    }
                }
            }

            return result;
        }

        public static void DecompileOSIProject(OSIFile osi, string outputDirectory, string classesDirectoryName = "classes", string functionsDirectoryName = "functions")
        {
            Parser.Result decompiled = DecompileOSI(osi);

            // Create directories
            if (System.IO.Directory.Exists(outputDirectory))
                System.IO.Directory.Delete(outputDirectory, true);
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
                string classDirectory = System.IO.Path.Combine(outputDirectory, classesDirectoryName, PathForClass(cls, decompiled.Classes));
                System.IO.Directory.CreateDirectory(classDirectory);
                System.IO.File.WriteAllText(System.IO.Path.Combine(classDirectory, cls.Name.Content + ".lss"), PrettyPrinter.Print(cls));
            }
        }

        private static string PathForClass(ClassStatement cls, List<ClassStatement> allClasses)
        {
            List<string> segments = new List<string>();
            while (cls != null)
            {
                segments.Insert(0, cls.Name.Content);
                cls = allClasses.FirstOrDefault(c => c.Name.Content == cls.SuperclassName?.Content);
            }
            segments.RemoveAt(segments.Count - 1);
            return System.IO.Path.Combine(segments.ToArray());
        }
    }
}
