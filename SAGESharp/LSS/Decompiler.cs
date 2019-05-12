using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAGESharp.OSI;
using SAGESharp.LSS.Statements;
using SAGESharp.LSS.Expressions;

namespace SAGESharp.LSS
{
    public static class Decompiler
    {
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

        public static SubroutineStatement DecompileSubroutine(OSIFile osi, string name, bool isMemberMethod, List<Token> parameters, List<Instruction> instructions, SourceSpan outputSpan)
        {
            List<InstructionStatement> statements = new List<InstructionStatement>();

            Stack<Expression> stack = new Stack<Expression>();
            List<string> variables = new List<string>();

            if (isMemberMethod)
                variables.Add("this");
            foreach (Token parameter in parameters)
                variables.Add(parameter.Content);

            for (int i = 0; i < instructions.Count; i++)
            {
                if (instructions[i] is BCLInstruction bcl)
                {
                    switch (bcl.Opcode)
                    {
                        case BCLOpcode.Add:
                            stack.Push(DecompileBinaryExpression(stack, TokenType.Plus, "+", outputSpan));
                            break;
                        case BCLOpcode.And:
                            stack.Push(DecompileBinaryExpression(stack, TokenType.AmpersandAmpersand, "&&", outputSpan));
                            break;
                        case BCLOpcode.AppendToArray:
                            {
                                Expression value = stack.Pop();
                                Expression array = stack.Pop();
                                Expression arrayAlso = stack.Pop();
                                // TODO: if array is ArrayExpression, collapse into it
                                stack.Push(new CallExpression(outputSpan, DecompileBinaryExpression(stack, array, new VariableExpression(new Token(TokenType.KeywordAppend, "__append", outputSpan)), TokenType.Period, ".", outputSpan), new Expression[] { value }));
                                break;
                            }
                        case BCLOpcode.BitwiseAnd:
                            stack.Push(DecompileBinaryExpression(stack, TokenType.Ampersand, "&", outputSpan));
                            break;
                        case BCLOpcode.BitwiseNot:
                            stack.Push(new UnaryExpression(stack.Pop(), new Token(TokenType.Tilde, "~", outputSpan), false));
                            break;
                        case BCLOpcode.BitwiseOr:
                            stack.Push(DecompileBinaryExpression(stack, TokenType.PipePipe, "||", outputSpan));
                            break;
                        case BCLOpcode.BitwiseXor:
                            stack.Push(DecompileBinaryExpression(stack, TokenType.Octothorpe, "#", outputSpan));
                            break;
                        case BCLOpcode.CallGameFunction:
                            throw new NotImplementedException();
                        case BCLOpcode.CallGameFunctionDirect:
                            throw new NotImplementedException();
                        case BCLOpcode.CallGameFunctionFromString:
                            throw new NotImplementedException();
                        case BCLOpcode.ConvertToFloat:
                            stack.Push(new CallExpression(outputSpan, new VariableExpression(new Token(TokenType.KeywordToFloat, "__tofloat", outputSpan)), new Expression[] { stack.Pop() }));
                            break;
                        case BCLOpcode.ConvertToInteger:
                            stack.Push(new CallExpression(outputSpan, new VariableExpression(new Token(TokenType.KeywordToInt, "__toint", outputSpan)), new Expression[] { stack.Pop() }));
                            break;
                        case BCLOpcode.ConvertToString:
                            stack.Push(new CallExpression(outputSpan, new VariableExpression(new Token(TokenType.KeywordToString, "__tostring", outputSpan)), new Expression[] { stack.Pop() }));
                            break;
                        case BCLOpcode.CreateArray:
                            stack.Push(new ArrayExpression(outputSpan, new Expression[] { }));
                            break;
                        case BCLOpcode.PushConstant0:
                            stack.Push(new LiteralExpression(new Token(TokenType.IntegerLiteral, "0", outputSpan)));
                            break;
                        case BCLOpcode.PushConstanti8:
                            stack.Push(new LiteralExpression(new Token(TokenType.IntegerLiteral, instructions[i].Arguments[0].GetValue<sbyte>().ToString(), outputSpan)));
                            break;
                        case BCLOpcode.PushConstanti16:
                            stack.Push(new LiteralExpression(new Token(TokenType.IntegerLiteral, instructions[i].Arguments[0].GetValue<short>().ToString(), outputSpan)));
                            break;
                        case BCLOpcode.PushConstanti24:
                            throw new NotImplementedException();
                        case BCLOpcode.PushConstanti32:
                            stack.Push(new LiteralExpression(new Token(TokenType.IntegerLiteral, instructions[i].Arguments[0].GetValue<int>().ToString(), outputSpan)));
                            break;
                        case BCLOpcode.PushConstantf32:
                            stack.Push(new LiteralExpression(new Token(TokenType.FloatLiteral, instructions[i].Arguments[0].GetValue<float>().ToString(), outputSpan)));
                            break;
                        case BCLOpcode.PushConstantColor8888:
                            stack.Push(new CallExpression(outputSpan, new VariableExpression(new Token(TokenType.KeywordRGBA, "rgba", outputSpan)), new Expression[]
                            {
                                new LiteralExpression(new Token(TokenType.IntegerLiteral, ((instructions[i].Arguments[0].GetValue<uint>() & 0xFF000000) >> 24).ToString(), outputSpan)),
                                new LiteralExpression(new Token(TokenType.IntegerLiteral, ((instructions[i].Arguments[0].GetValue<uint>() & 0x00FF0000) >> 16).ToString(), outputSpan)),
                                new LiteralExpression(new Token(TokenType.IntegerLiteral, ((instructions[i].Arguments[0].GetValue<uint>() & 0x0000FF00) >> 8).ToString(), outputSpan)),
                                new LiteralExpression(new Token(TokenType.IntegerLiteral, ((instructions[i].Arguments[0].GetValue<uint>() & 0x000000FF) >> 0).ToString(), outputSpan))
                            }));
                            break;
                        case BCLOpcode.PushNothing:
                            stack.Push(new LiteralExpression(new Token(TokenType.KeywordNull, "null", outputSpan)));
                            break;
                        case BCLOpcode.Return:
                            statements.Add(new ReturnStatement(new Token(TokenType.KeywordReturn, "return", outputSpan), stack.Pop()));
                            break;
                        case BCLOpcode.PushConstantString:
                            stack.Push(new LiteralExpression(new Token(TokenType.StringLiteral, "\"" + Compiler.EscapeString(osi.Strings[instructions[i].Arguments[0].GetValue<ushort>()]) + "\"", outputSpan)));
                            break;
                        case BCLOpcode.PushConstantColor5551:
                            throw new NotImplementedException();
                        case BCLOpcode.GetVariableValue:
                            stack.Push(new VariableExpression(new Token(TokenType.Symbol, variables[instructions[i].Arguments[0].GetValue<ushort>()], outputSpan)));
                            break;
                        case BCLOpcode.SetThisMemberValue:
                            statements.Add(new AssignmentStatement(new BinaryExpression(new VariableExpression(new Token(TokenType.KeywordThis, "this", outputSpan)), new Token(TokenType.Period, ".", outputSpan), new VariableExpression(new Token(TokenType.Symbol, osi.Symbols[instructions[i].Arguments[0].GetValue<ushort>()], outputSpan))), stack.Pop()));
                            break;
                        case BCLOpcode.GetThisMemberFunction:
                            stack.Push(new BinaryExpression(new VariableExpression(new Token(TokenType.KeywordThis, "this", outputSpan)), new Token(TokenType.Period, ".", outputSpan), new VariableExpression(new Token(TokenType.Symbol, osi.Symbols[instructions[i].Arguments[0].GetValue<ushort>()], outputSpan))));
                            break;
                        case BCLOpcode.JumpAbsolute:
                            {
                                Expression function = stack.Pop(); // Pop function
                                int argCount = instructions[i].Arguments[0].GetValue<sbyte>() - 1; // 'this' doesn't count as an argument
                                List<Expression> arguments = new List<Expression>();
                                for (int j = 0; j < argCount; j++)
                                {
                                    arguments.Insert(0, stack.Pop()); // Pop arguments right to left
                                }
                                stack.Pop(); // 'this' was the first argument
                                // TODO: Assert that this was really 'this'
                                stack.Push(new CallExpression(outputSpan, function, arguments));
                                break;
                            }
                        case BCLOpcode.Pop:
                            statements.Add(new ExpressionStatement(stack.Pop()));
                            break;
                    }
                }
                else
                {
                    throw new FormatException("Cannot decompile abstract instruction " + i.GetType().ToString());
                }
            }

            BlockStatement body = new BlockStatement(outputSpan, statements);
            return new SubroutineStatement(outputSpan, new Token(TokenType.Symbol, name, outputSpan), parameters, body);
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
                List<Token> parameters = new List<Token>();

                for (int i = 0; i < function.ParameterCount; i++)
                {
                    parameters.Add(new Token(TokenType.Symbol, "param" + (i + 1), span));
                }

                result.Functions.Add(DecompileSubroutine(osi, function.Name, false, parameters, function.Instructions, span));
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
                    List<Token> parameters = new List<Token>();
                    if (method.Instructions.Count > 0 && method.Instructions[0] is BCLInstruction argCheck && argCheck.Opcode == BCLOpcode.MemberFunctionArgumentCheck)
                    {
                        for (int i = 0; i < argCheck.Arguments[0].GetValue<sbyte>() - 1; i++)
                        {
                            parameters.Add(new Token(TokenType.Symbol, "param" + (i + 1), span));
                        }
                    }
                    methods.Add(DecompileSubroutine(osi, osi.Symbols[method.NameSymbol], true, parameters, method.Instructions, span));
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
