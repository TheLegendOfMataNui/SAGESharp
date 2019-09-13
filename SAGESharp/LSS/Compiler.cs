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
    public static class Compiler
    {
        public class Settings
        {
            public int VersionMajor { get; set; } = 4;
            public int VersionMinor { get; set; } = 1;
            public bool EmitLineNumbers { get; set; } = false;
        }

        public class Result
        {
            public OSIFile OSI;
            public List<CompileMessage> Messages;

            public Result(OSIFile osi)
            {
                this.OSI = osi;
                this.Messages = new List<CompileMessage>();
            }
        }

        private class PanicException : Exception
        {

        }

        private class SubroutineScope
        {
            public Dictionary<string, Variable> Variables = new Dictionary<string, Variable>();
            public SubroutineScope ParentScope { get; }

            public SubroutineScope(SubroutineScope parentScope)
            {
                this.ParentScope = parentScope;
            }
        }

        private abstract class Variable
        {
            public string Name { get; }

            public Variable(string name)
            {
                this.Name = name;
            }

            public abstract uint EmitRead(SubroutineContext context);
            public abstract uint EmitWrite(SubroutineContext context, Expression value, object expressionContext);
        }

        private class StandardVariable : Variable
        {
            public ushort Index { get; }

            public StandardVariable(string name, ushort index) : base(name)
            {
                this.Index = index;
            }

            public override uint EmitRead(SubroutineContext context)
            {
                uint size = 0;
                size += context.EmitBCLInstruction(BCLOpcode.GetVariableValue, Index);
                return size;
            }

            public override uint EmitWrite(SubroutineContext context, Expression value, object expressionContext)
            {
                uint size = 0;
                size += value.AcceptVisitor(context, expressionContext);
                size += context.EmitBCLInstruction(BCLOpcode.SetVariableValue, Index);
                return size;
            }
        }

        private class IterationVariable : Variable
        {
            public Variable IndexVariable { get; }
            public Expression Collection { get; }

            public IterationVariable(string name, Variable indexVariable, Expression collection) : base(name)
            {
                this.IndexVariable = indexVariable;
                this.Collection = collection;
            }

            public override uint EmitRead(SubroutineContext context)
            {
                uint size = 0;
                size += Collection.AcceptVisitor(context, null);
                size += IndexVariable.EmitRead(context);
                size += context.EmitBCLInstruction(BCLOpcode.GetArrayValue);
                return size;
            }

            public override uint EmitWrite(SubroutineContext context, Expression value, object expressionContext)
            {
                Error(context.Messages, "Cannot modify the contents of an iteration variable.", "LSS057", value.Span, true);
                return 0; // Never reached
            }
        }

        private class SubroutineContext : ExpressionVisitor<uint, object>, StatementVisitor<uint, object>
        {
            private const string ITERATION_INDEX_LOCAL_NAME = "@";
            public List<Instruction> Instructions { get; } = new List<Instruction>();
            private OSIFile OSI { get; }
            public bool IsFinalized { get; private set; }
            public bool IsInstanceMethod { get; }
            private ushort ParameterCount { get; } // Including 'this'
            private ushort VariableCount = 0; // The locals (which includes iteration indices, even though they aren't directly accessed by the user)
            private SubroutineScope BaseScope { get; }
            private SubroutineScope CurrentScope { get; set; }
            private Settings CompileSettings { get; }
            public List<CompileMessage> Messages { get; }

            public SubroutineContext(List<CompileMessage> messages, OSIFile osi, Settings compileSettings, bool isInstanceMethod, IEnumerable<string> parameterNames)
            {
                this.Messages = messages;
                this.OSI = osi;
                this.CompileSettings = compileSettings;
                BaseScope = new SubroutineScope(null);
                CurrentScope = BaseScope;
                foreach (string param in parameterNames) {
                    AddLocal(param);
                }
                ParameterCount = VariableCount;
                this.IsInstanceMethod = isInstanceMethod;
            }

            public void FinalizeInstructions()
            {
                if (IsFinalized)
                    throw new InvalidOperationException("Cannot modify a SubroutineContext after it has been finalized.");
                if (VariableCount > ParameterCount)
                {
                    Instructions.Insert(0, new BCLInstruction(BCLOpcode.CreateStackVariables, (sbyte)(VariableCount - ParameterCount)));
                }
                if (this.IsInstanceMethod && ParameterCount > 0)
                {
                    Instructions.Insert(0, new BCLInstruction(BCLOpcode.MemberFunctionArgumentCheck, (sbyte)ParameterCount));
                }
                if (Instructions.Count == 0 || ((Instructions[Instructions.Count - 1] as BCLInstruction)?.Opcode != BCLOpcode.Return))
                {
                    EmitBCLInstruction(BCLOpcode.PushNothing);
                    EmitBCLInstruction(BCLOpcode.Return);
                }
            }

            public uint EmitInstruction(Instruction instruction)
            {
                Instructions.Add(instruction);
                return instruction.Size;
            }

            public uint EmitBCLInstruction(BCLOpcode opcode, params object[] operands)
            {
                return EmitInstruction(new BCLInstruction(opcode, operands));
            }

            public uint EmitBCLInstruction(out BCLInstruction instruction, BCLOpcode opcode, params object[] operands)
            {
                instruction = new BCLInstruction(opcode, operands);
                return EmitInstruction(instruction);
            }

            private StandardVariable AddLocal(string localName)
            {
                if (IsFinalized)
                    throw new InvalidOperationException("Cannot modify a SubroutineContext after it has been finalized.");

                if (CurrentScope.Variables.ContainsKey(localName))
                    throw new ArgumentException("Variable is already declared, and would conflict.");

                StandardVariable result = new StandardVariable(localName, VariableCount);
                CurrentScope.Variables.Add(localName, result);
                VariableCount++;
                return result;
            }

            private IterationVariable AddIterationVariable(string name, Variable indexVariable, Expression collection)
            {
                if (IsFinalized)
                    throw new InvalidOperationException("Cannot modify a SubroutineContext after it has been finalized.");
                SubroutineScope scope = CurrentScope;
                while (scope != null)
                {
                    if (scope.Variables.ContainsKey(name))
                        throw new ArgumentException("Variable is already declared, and would conflict.");
                    scope = scope.ParentScope;
                }
                IterationVariable result = new IterationVariable(name, indexVariable, collection);
                CurrentScope.Variables.Add(name, result);
                return result;
            }

            // Could be a local in one of the current scopes or a global.
            private Variable FindVariable(string name)
            {
                Variable result = null;

                // Local?
                SubroutineScope scope = CurrentScope;
                while (scope != null)
                {
                    if (scope.Variables.ContainsKey(name))
                    {
                        result = scope.Variables[name];
                        break;
                    }
                    scope = scope.ParentScope;
                }

                // Global?
                if (result == null)
                {
                    if (OSI.Globals.Contains(name))
                    {
                        // Highest bit (0x8000) means global
                        // TODO: Optimization - Keep these around so we don't constantly create them all the time
                        result = new StandardVariable(name, (ushort)(OSI.Globals.IndexOf(name) | (1 << 15)));
                    }
                }

                return result;
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

            private ushort AddOrGetSymbol(string value)
            {
                if (IsFinalized)
                    throw new InvalidOperationException("Cannot modify a SubroutineContext after it has been finalized.");
                int index = OSI.Symbols.IndexOf(value);
                if (index == -1)
                {
                    index = OSI.Symbols.Count;
                    OSI.Symbols.Add(value);
                }
                return (ushort)index;
            }

            private uint EmitLineNumberAlt1(ushort lineNumber, string filename)
            {
                if (!CompileSettings.EmitLineNumbers)
                    return 0;

                uint size = 0;
                int sourceFileIndex = OSI.SourceFilenames.IndexOf(filename);
                if (sourceFileIndex == -1)
                {
                    sourceFileIndex = OSI.SourceFilenames.Count;
                    OSI.SourceFilenames.Add(filename);
                }
                size += EmitBCLInstruction(BCLOpcode.LineNumberAlt1, lineNumber, (ushort)sourceFileIndex);

                return size;
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

            #region Expressions
            public uint VisitArrayAccessExpression(ArrayAccessExpression expr, object context)
            {
                uint size = 0;

                size += expr.Array.AcceptVisitor(this, context);

                size += expr.Index.AcceptVisitor(this, context);

                size += EmitBCLInstruction(BCLOpcode.GetArrayValue);


                return size;
            }

            public uint VisitArrayExpression(ArrayExpression expr, object context)
            {
                uint size = 0;

                size += EmitBCLInstruction(BCLOpcode.CreateArray);


                foreach (Expression value in expr.Elements)
                {
                    size += EmitBCLInstruction(BCLOpcode.Dup);


                    size += value.AcceptVisitor(this, context);

                    size += EmitBCLInstruction(BCLOpcode.AppendToArray);

                }

                return size;
            }

            public uint VisitBinaryExpression(BinaryExpression expr, object context)
            {
                uint size = 0;
                if (expr.Operation.Type == TokenType.Period)
                {
                    if (expr.Right is VariableExpression symbol)
                    {

                        if (symbol.Symbol.Type == TokenType.Symbol)
                        {
                            // Getting a member
                            ushort memberSymbol = AddOrGetSymbol(symbol.Symbol.Content);
                            if (expr.Left is VariableExpression thisExpr && thisExpr.Symbol.Type == TokenType.KeywordThis)
                            {
                                if (!this.IsInstanceMethod)
                                    Error(Messages, "'this' keyword is only valid in an instance method.", "LSS059", expr.Left.Span, true);
                                size += EmitBCLInstruction(BCLOpcode.GetThisMemberValue, memberSymbol);
                            }
                            else
                            {
                                size += expr.Left.AcceptVisitor(this, null);
                                size += EmitBCLInstruction(BCLOpcode.GetMemberValue, memberSymbol);
                            }
                        }
                        else
                        {
                            // Getting a builtin
                            if (expr.Left is VariableExpression thisExpr && thisExpr.Symbol.Type == TokenType.KeywordThis)
                            {
                                Error(Messages, "No builtin members are useable on 'this'.", "LSS060", expr.Right.Span, true);
                            }
                            else
                            {
                                size += expr.Left.AcceptVisitor(this, null);
                                if (symbol.Symbol.Type == TokenType.KeywordLength)
                                {
                                    size += EmitBCLInstruction(BCLOpcode.ElementsInArray);
                                }
                                else if (symbol.Symbol.Type == TokenType.KeywordRed)
                                {
                                    size += EmitBCLInstruction(BCLOpcode.GetRedValue);
                                }
                                else if (symbol.Symbol.Type == TokenType.KeywordGreen)
                                {
                                    size += EmitBCLInstruction(BCLOpcode.GetGreenValue);
                                }
                                else if (symbol.Symbol.Type == TokenType.KeywordBlue)
                                {
                                    size += EmitBCLInstruction(BCLOpcode.GetBlueValue);
                                }
                                else if (symbol.Symbol.Type == TokenType.KeywordAlpha)
                                {
                                    size += EmitBCLInstruction(BCLOpcode.GetAlphaValue);
                                }
                                else
                                {
                                    Error(Messages, "'" + symbol.Symbol.Content + "' of type '" + symbol.Symbol.Type.ToString() + "' is not a valid builtin property.", "LSS061", symbol.Span, true);
                                }
                            }
                        }
                    }
                    else
                    {
                        Error(Messages, "Right side of period must be a member name", "LSS062", expr.Right.Span, true);
                    }
                }
                else if (expr.Operation.Type == TokenType.PeriodDollarSign)
                {
                    size += expr.Left.AcceptVisitor(this, null);
                    size += expr.Right.AcceptVisitor(this, null);

                    size += EmitBCLInstruction(BCLOpcode.GetMemberValueFromString);
                }
                else if (expr.Operation.Type == TokenType.ColonColon)
                {
                    if (expr.Left is VariableExpression ns && ns.Symbol.Type == TokenType.Symbol
                        && expr.Right is VariableExpression name && name.Symbol.Type == TokenType.Symbol)
                    {
                        ushort nsIndex = AddOrGetString(ns.Symbol.Content);
                        ushort nameIndex = AddOrGetString(name.Symbol.Content);

                        size += EmitBCLInstruction(BCLOpcode.GetGameVariable, nsIndex, nameIndex);
                    }
                    else
                    {
                        Error(Messages, "Native game access must use a name on both sides of the double-colon", "LSS063", expr.Span, true);
                    }
                }
                else if (expr.Operation.Type == TokenType.ColonColonDollarSign)
                {
                    Error(Messages, "Dynamic-lookup game variable are not implemented in SAGE.", "LSS064", expr.Span, true);
                }
                else
                {
                    size += expr.Left.AcceptVisitor(this, context);
                    size += expr.Right.AcceptVisitor(this, context);
                    if (expr.Operation.Type == TokenType.Ampersand)
                    {
                        size += EmitBCLInstruction(BCLOpcode.BitwiseAnd);
                    }
                    else if (expr.Operation.Type == TokenType.AmpersandAmpersand)
                    {
                        size += EmitBCLInstruction(BCLOpcode.And);
                    }
                    else if (expr.Operation.Type == TokenType.Asterisk)
                    {
                        size += EmitBCLInstruction(BCLOpcode.Multiply);
                    }
                    else if (expr.Operation.Type == TokenType.Caret)
                    {
                        size += EmitBCLInstruction(BCLOpcode.Power);
                    }
                    else if (expr.Operation.Type == TokenType.Dash)
                    {
                        size += EmitBCLInstruction(BCLOpcode.Subtract);
                    }
                    else if (expr.Operation.Type == TokenType.EqualsEquals)
                    {
                        size += EmitBCLInstruction(BCLOpcode.EqualTo);
                    }
                    else if (expr.Operation.Type == TokenType.ExclamationEquals)
                    {
                        size += EmitBCLInstruction(BCLOpcode.EqualTo);
                        size += EmitBCLInstruction(BCLOpcode.Not);
                    }
                    else if (expr.Operation.Type == TokenType.Greater)
                    {
                        size += EmitBCLInstruction(BCLOpcode.GreaterThan);
                    }
                    else if (expr.Operation.Type == TokenType.GreaterEquals)
                    {
                        size += EmitBCLInstruction(BCLOpcode.GreaterOrEqual);
                    }
                    else if (expr.Operation.Type == TokenType.GreaterGreater)
                    {
                        size += EmitBCLInstruction(BCLOpcode.ShiftRight);
                    }
                    else if (expr.Operation.Type == TokenType.Less)
                    {
                        size += EmitBCLInstruction(BCLOpcode.LessThan);
                    }
                    else if (expr.Operation.Type == TokenType.LessEquals)
                    {
                        size += EmitBCLInstruction(BCLOpcode.LessOrEqual);
                    }
                    else if (expr.Operation.Type == TokenType.LessLess)
                    {
                        size += EmitBCLInstruction(BCLOpcode.ShiftLeft);
                    }
                    else if (expr.Operation.Type == TokenType.Octothorpe)
                    {
                        size += EmitBCLInstruction(BCLOpcode.BitwiseXor);
                    }
                    else if (expr.Operation.Type == TokenType.Percent)
                    {
                        size += EmitBCLInstruction(BCLOpcode.Modulus);
                    }
                    else if (expr.Operation.Type == TokenType.Pipe)
                    {
                        size += EmitBCLInstruction(BCLOpcode.BitwiseOr);
                    }
                    else if (expr.Operation.Type == TokenType.PipePipe)
                    {
                        size += EmitBCLInstruction(BCLOpcode.Or);
                    }
                    else if (expr.Operation.Type == TokenType.Plus)
                    {
                        size += EmitBCLInstruction(BCLOpcode.Add);
                    }
                    else if (expr.Operation.Type == TokenType.Slash)
                    {
                        size += EmitBCLInstruction(BCLOpcode.Divide);
                    }
                    else
                    {
                        Error(Messages, "Invalid binary operator: " + expr.Operation.Type, "LSS065", expr.Operation.Span, true);
                    }
                }
                return size;
            }

            private uint PushCallArguments(CallExpression expr)
            {
                uint size = 0;
                
                // Push call arguments left to right
                for (int i = 0; i < expr.Arguments.Count; i++)
                {
                    size += expr.Arguments[i].AcceptVisitor(this, null);
                }

                return size;
            }

            public uint VisitCallExpression(CallExpression expr, object context)
            {
                uint size = 0;
                if (expr.Target is VariableExpression varExp)
                {
                    if (varExp.Symbol.Type == TokenType.Symbol)
                    {
                        // Static function
                        bool found = false;
                        foreach (OSIFile.FunctionInfo func in OSI.Functions)
                        {
                            if (func.Name == varExp.Symbol.Content)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            Error(Messages, "Static function '" + varExp.Symbol.Content + "' not found.", "LSS066", varExp.Span, true);
                        }

                        size += PushCallArguments(expr);

                        size += EmitInstruction(new JumpStaticInstruction(varExp.Symbol.Content, (sbyte)expr.Arguments.Count));
                    }
                    else
                    {
                        if (varExp.Symbol.Type == TokenType.KeywordRGBA)
                        {
                            if (expr.Arguments.Count != 4)
                            {
                                Error(Messages, "Builtin function '" + varExp.Symbol.Content + "' requires exactly 4 arguments.", "LSS067", varExp.Span, true);
                            }
                            else
                            {
                                uint initialColor = 0x00000000;
                                bool complexR = false;
                                bool complexG = false;
                                bool complexB = false;
                                bool complexA = false;
                                if (expr.Arguments[0] is LiteralExpression rLiteral && rLiteral.Value.Type == TokenType.IntegerLiteral)
                                {
                                    int value = Int32.Parse(rLiteral.Value.Content);
                                    if (value > Byte.MaxValue || value < Byte.MinValue)
                                    {
                                        Error(Messages, "Literal value is out of the acceptable range (0 - 255) for R component.", "LSS068", expr.Arguments[0].Span, true);
                                    }
                                    else
                                    {
                                        initialColor |= ((uint)value & 0xFF) << 24;
                                    }
                                }
                                else
                                {
                                    complexR = true;
                                }

                                if (expr.Arguments[1] is LiteralExpression gLiteral && gLiteral.Value.Type == TokenType.IntegerLiteral)
                                {
                                    int value = Int32.Parse(gLiteral.Value.Content);
                                    if (value > Byte.MaxValue || value < Byte.MinValue)
                                    {
                                        Error(Messages, "Literal value is out of the acceptable range (0 - 255) for G component.", "LSS068", expr.Arguments[1].Span, true);
                                    }
                                    else
                                    {
                                        initialColor |= ((uint)value & 0xFF) << 16;
                                    }
                                }
                                else
                                {
                                    complexG = true;
                                }

                                if (expr.Arguments[2] is LiteralExpression bLiteral && bLiteral.Value.Type == TokenType.IntegerLiteral)
                                {
                                    int value = Int32.Parse(bLiteral.Value.Content);
                                    if (value > Byte.MaxValue || value < Byte.MinValue)
                                    {
                                        Error(Messages, "Literal value is out of the acceptable range (0 - 255) for B component.", "LSS068", expr.Arguments[2].Span, true);
                                    }
                                    else
                                    {
                                        initialColor |= ((uint)value & 0xFF) << 8;
                                    }
                                }
                                else
                                {
                                    complexB = true;
                                }

                                if (expr.Arguments[3] is LiteralExpression aLiteral && aLiteral.Value.Type == TokenType.IntegerLiteral)
                                {
                                    int value = Int32.Parse(aLiteral.Value.Content);
                                    if (value > Byte.MaxValue || value < Byte.MinValue)
                                    {
                                        Error(Messages, "Literal value is out of the acceptable range (0 - 255) for A component.", "LSS068", expr.Arguments[3].Span, true);
                                    }
                                    else
                                    {
                                        initialColor |= ((uint)value & 0xFF);
                                    }
                                }
                                else
                                {
                                    complexA = true;
                                }

                                size += EmitBCLInstruction(BCLOpcode.PushConstantColor8888, (uint)initialColor);

                                if (complexR)
                                {
                                    size += expr.Arguments[0].AcceptVisitor(this, context);
                                    size += EmitBCLInstruction(BCLOpcode.SetRedValue);
                                }
                                if (complexG)
                                {
                                    size += expr.Arguments[1].AcceptVisitor(this, context);
                                    size += EmitBCLInstruction(BCLOpcode.SetGreenValue);
                                }
                                if (complexB)
                                {
                                    size += expr.Arguments[2].AcceptVisitor(this, context);
                                    size += EmitBCLInstruction(BCLOpcode.SetBlueValue);
                                }
                                if (complexA)
                                {
                                    size += expr.Arguments[3].AcceptVisitor(this, context);
                                    size += EmitBCLInstruction(BCLOpcode.SetAlphaValue);
                                }
                            }
                        }
                        else
                        {
                            if (varExp.Symbol.Type == TokenType.KeywordToString
                                || varExp.Symbol.Type == TokenType.KeywordToFloat
                                || varExp.Symbol.Type == TokenType.KeywordToInt
                                || varExp.Symbol.Type == TokenType.KeywordIsInt
                                || varExp.Symbol.Type == TokenType.KeywordIsFloat
                                || varExp.Symbol.Type == TokenType.KeywordIsString
                                || varExp.Symbol.Type == TokenType.KeywordIsInstance
                                || varExp.Symbol.Type == TokenType.KeywordIsObject
                                || varExp.Symbol.Type == TokenType.KeywordIsArray
                                || varExp.Symbol.Type == TokenType.KeywordClassID)
                            {
                                if (expr.Arguments.Count != 1)
                                {
                                    Error(Messages, "Builtin function '" + varExp.Symbol.Content + "' requires exactly 1 argument.", "LSS069", expr.Span, true);
                                }
                                size += expr.Arguments[0].AcceptVisitor(this, context);
                                if (varExp.Symbol.Type == TokenType.KeywordToString)
                                {
                                    size += EmitBCLInstruction(BCLOpcode.ConvertToString);
                                }
                                else if (varExp.Symbol.Type == TokenType.KeywordToFloat)
                                {
                                    size += EmitBCLInstruction(BCLOpcode.ConvertToFloat);
                                }
                                else if (varExp.Symbol.Type == TokenType.KeywordToInt)
                                {
                                    size += EmitBCLInstruction(BCLOpcode.ConvertToInteger);
                                }
                                else if (varExp.Symbol.Type == TokenType.KeywordIsInt)
                                {
                                    size += EmitBCLInstruction(BCLOpcode.IsInteger);
                                }
                                else if (varExp.Symbol.Type == TokenType.KeywordIsFloat)
                                {
                                    size += EmitBCLInstruction(BCLOpcode.IsFloat);
                                }
                                else if (varExp.Symbol.Type == TokenType.KeywordIsString)
                                {
                                    size += EmitBCLInstruction(BCLOpcode.IsString);
                                }
                                else if (varExp.Symbol.Type == TokenType.KeywordIsInstance)
                                {
                                    size += EmitBCLInstruction(BCLOpcode.IsAnObject);
                                }
                                else if (varExp.Symbol.Type == TokenType.KeywordIsObject)
                                {
                                    size += EmitBCLInstruction(BCLOpcode.IsGameObject);
                                }
                                else if (varExp.Symbol.Type == TokenType.KeywordIsArray)
                                {
                                    size += EmitBCLInstruction(BCLOpcode.IsArray);
                                }
                                else if (varExp.Symbol.Type == TokenType.KeywordClassID)
                                {
                                    size += EmitBCLInstruction(BCLOpcode.GetObjectClassID);
                                }
                                else
                                {
                                    throw new ArgumentException("What the hecc???");
                                }
                            }
                            else
                            {
                                Error(Messages, "'" + varExp.Symbol.Content + "' is not callable.", "LSS070", varExp.Span, true);
                            }
                        }
                    }
                }
                else if (expr.Target is BinaryExpression binExp)
                {
                    if (binExp.Right is VariableExpression nameExp)
                    {
                        // Standard symbol
                        if (binExp.Operation.Type == TokenType.Period)
                        {
                            if (nameExp.Symbol.Type == TokenType.Symbol)
                            {
                                ushort methodSymbol = AddOrGetSymbol(nameExp.Symbol.Content);
                                if (binExp.Left is VariableExpression thisExp && thisExp.Symbol.Type == TokenType.KeywordThis)
                                {
                                    // This method

                                    if (!IsInstanceMethod)
                                    {
                                        Error(Messages, "'this' not allowed in static function.", "LSS071", binExp.Span, true);
                                    }

                                    size += EmitBCLInstruction(BCLOpcode.GetVariableValue, (ushort)0);


                                    size += PushCallArguments(expr);

                                    size += EmitBCLInstruction(BCLOpcode.GetThisMemberFunction, AddOrGetSymbol(nameExp.Symbol.Content));

                                    size += EmitBCLInstruction(BCLOpcode.JumpAbsolute, (sbyte)(expr.Arguments.Count + 1)); // Add the implicit 'this' argument
                                }
                                else
                                {
                                    // Standard method
                                    size += binExp.Left.AcceptVisitor(this, context);

                                    size += PushCallArguments(expr);

                                    if (expr.Arguments.Count == 0)
                                    {
                                        size += EmitBCLInstruction(BCLOpcode.Dup);
                                    }
                                    else
                                    {
                                        size += EmitBCLInstruction(BCLOpcode.Pull, (sbyte)(expr.Arguments.Count + 1));
                                    }

                                    size += EmitBCLInstruction(BCLOpcode.GetMemberFunction, AddOrGetSymbol(nameExp.Symbol.Content));

                                    size += EmitBCLInstruction(BCLOpcode.JumpAbsolute, (sbyte)(expr.Arguments.Count + 1)); // Add the implicit 'this' argument
                                }
                            }
                            else
                            {
                                // Builtin function (all for arrays for now)
                                if (nameExp.Symbol.Type == TokenType.KeywordAppend)
                                {
                                    if (expr.Arguments.Count != 1)
                                        Error(Messages, "Builtin method 'append' requires 1 argument.", "LSS072", expr.Span, true);

                                    size += binExp.Left.AcceptVisitor(this, context); // Push array

                                    size += EmitBCLInstruction(BCLOpcode.Dup); // Make a copy, so we 'return' the original array


                                    size += expr.Arguments[0].AcceptVisitor(this, context); // Push value

                                    size += EmitBCLInstruction(BCLOpcode.AppendToArray);

                                }
                                else if (nameExp.Symbol.Type == TokenType.KeywordRemoveAt)
                                {
                                    if (expr.Arguments.Count != 1)
                                        Error(Messages, "Builtin method 'removeat' requires 1 argument.", "LSS072", expr.Span, true);

                                    size += binExp.Left.AcceptVisitor(this, context); // Push array

                                    size += EmitBCLInstruction(BCLOpcode.Dup); // Make a copy, so we 'return' the original array


                                    size += expr.Arguments[0].AcceptVisitor(this, context); // Push index

                                    size += EmitBCLInstruction(BCLOpcode.RemoveFromArray);

                                }
                                else if (nameExp.Symbol.Type == TokenType.KeywordInsertAt)
                                {
                                    if (expr.Arguments.Count != 2)
                                        Error(Messages, "Builtin method 'insertat' requires 2 arguments.", "LSS072", expr.Span, true);

                                    size += binExp.Left.AcceptVisitor(this, context); // Push array

                                    size += EmitBCLInstruction(BCLOpcode.Dup); // Make a copy, so we 'return' the original array


                                    size += expr.Arguments[0].AcceptVisitor(this, context); // Push index

                                    size += expr.Arguments[1].AcceptVisitor(this, context); // Push value

                                    size += EmitBCLInstruction(BCLOpcode.InsertIntoArray);

                                }
                                else if (nameExp.Symbol.Type == TokenType.KeywordWithRed)
                                {
                                    if (expr.Arguments.Count != 1)
                                        Error(Messages, "Builtin method 'withred' requires 1 argument.", "LSS072", expr.Span, true);

                                    size += binExp.Left.AcceptVisitor(this, context); // Target

                                    size += expr.Arguments[0].AcceptVisitor(this, context); // Value

                                    size += EmitBCLInstruction(BCLOpcode.SetRedValue);
                                }
                                else if (nameExp.Symbol.Type == TokenType.KeywordWithGreen)
                                {
                                    if (expr.Arguments.Count != 1)
                                        Error(Messages, "Builtin method 'withgreen' requires 1 argument.", "LSS072", expr.Span, true);

                                    size += binExp.Left.AcceptVisitor(this, context); // Target

                                    size += expr.Arguments[0].AcceptVisitor(this, context); // Value

                                    size += EmitBCLInstruction(BCLOpcode.SetGreenValue);
                                }
                                else if (nameExp.Symbol.Type == TokenType.KeywordWithBlue)
                                {
                                    if (expr.Arguments.Count != 1)
                                        Error(Messages, "Builtin method 'withblue' requires 1 argument.", "LSS072", expr.Span, true);

                                    size += binExp.Left.AcceptVisitor(this, context); // Target

                                    size += expr.Arguments[0].AcceptVisitor(this, context); // Value

                                    size += EmitBCLInstruction(BCLOpcode.SetBlueValue);
                                }
                                else if (nameExp.Symbol.Type == TokenType.KeywordWithAlpha)
                                {
                                    if (expr.Arguments.Count != 1)
                                        Error(Messages, "Builtin 'withalpha' requires 1 argument.", "LSS072", expr.Span, true);

                                    size += binExp.Left.AcceptVisitor(this, context); // Target

                                    size += expr.Arguments[0].AcceptVisitor(this, context); // Value

                                    size += EmitBCLInstruction(BCLOpcode.SetAlphaValue);
                                }
                                else
                                {
                                    Error(Messages, "Expected name of method, found " + nameExp.Symbol.Type + ".", "LSS073", nameExp.Span, true);
                                }
                            }
                        }
                        else if (binExp.Operation.Type == TokenType.ColonColon)
                        {
                            if (binExp.Left is VariableExpression ns)
                            {
                                // Game function lookup
                                size += PushCallArguments(expr);

                                size += EmitBCLInstruction(BCLOpcode.CallGameFunction, AddOrGetString(ns.Symbol.Content), AddOrGetString(nameExp.Symbol.Content), (sbyte)expr.Arguments.Count);

                            }
                            else
                            {
                                Error(Messages, "Game function namespace must be a symbol.", "LSS074", binExp.Left.Span, true);
                            }
                        }
                        else
                        {
                            Error(Messages, "Expression is not callable.", "LSS075", binExp.Span, true);
                        }
                    }
                    else
                    {
                        // Dynamic lookups
                        if (binExp.Operation.Type == TokenType.PeriodDollarSign)
                        {
                            // Dynamic method lookup
                            size += binExp.Left.AcceptVisitor(this, context);

                            size += PushCallArguments(expr);

                            if (expr.Arguments.Count == 0)
                            {
                                size += EmitBCLInstruction(BCLOpcode.Dup);
                            }
                            else
                            {
                                size += EmitBCLInstruction(BCLOpcode.Pull, (sbyte)(expr.Arguments.Count + 1));
                            }

                            size += binExp.Right.AcceptVisitor(this, context); // Method name

                            size += EmitBCLInstruction(BCLOpcode.GetMemberFunctionFromString);


                            size += EmitBCLInstruction(BCLOpcode.JumpAbsolute, (sbyte)(expr.Arguments.Count + 1)); // Add the implicit 'this' argument

                        }
                        else if (binExp.Operation.Type == TokenType.ColonColonDollarSign)
                        {
                            if (binExp.Left is VariableExpression ns)
                            {
                                // Dynamic game function lookup
                                size += PushCallArguments(expr);

                                size += binExp.Right.AcceptVisitor(this, context);

                                size += EmitBCLInstruction(BCLOpcode.CallGameFunction, AddOrGetString(ns.Symbol.Content), (sbyte)expr.Arguments.Count);

                            }
                            else
                            {
                                Error(Messages, "Game function namespace must be a symbol.", "LSS076", binExp.Left.Span, true);
                            }
                        }
                        else
                        {
                            Error(Messages, "Expression is not dynamically-callable.", "LSS077", binExp.Span, true);
                        }
                    }
                }
                else
                {
                    Error(Messages, "Expression is not callable.", "LSS078", expr.Span, true);
                }
                return size;
            }

            public uint VisitConstructorExpression(ConstructorExpression expr, object context)
            {
                uint size = 0;

                ushort? classIndex = null;
                for (int i = 0; i < OSI.Classes.Count; i++)
                {
                    if (OSI.Classes[i].Name == expr.TypeName.Content)
                    {
                        classIndex = (ushort)i;
                        break;
                    }
                }

                if (!classIndex.HasValue)
                    Error(Messages, "Type name '" + expr.TypeName.Content + "' is not a valid class!", "LSS079", expr.TypeName.Span, true);

                // Create the new instance
                size += EmitBCLInstruction(BCLOpcode.CreateObject, classIndex.Value);


                // The first constructor argument is the new instance
                size += EmitBCLInstruction(BCLOpcode.Dup);


                // Push the rest of the arguments
                foreach (Expression arg in expr.Arguments)
                {
                    size += arg.AcceptVisitor(this, context);
                }

                // Get the new instance to get the constructor method by pulling from earlier in the stack
                size += EmitBCLInstruction(BCLOpcode.Pull, (sbyte)(expr.Arguments.Count() + 1));


                // Get the constructor function on the new instance that we just copied to the top of the stack
                size += EmitBCLInstruction(BCLOpcode.GetMemberFunction, AddOrGetSymbol(expr.TypeName.Content));


                // Call the constructor
                size += EmitBCLInstruction(BCLOpcode.JumpAbsolute, (sbyte)(expr.Arguments.Count() + 1));


                // Pop the result - the constructor doesn't return anything useful, and we still have the new instance on the stack
                size += EmitBCLInstruction(BCLOpcode.Pop);


                return size;
            }

            public uint VisitGroupingExpression(GroupingExpression expr, object context)
            {
                return expr.Contents.AcceptVisitor(this, context);
            }

            public uint VisitLiteralExpression(LiteralExpression expr, object context)
            {
                uint size = 0;
                if (expr.Value.Type == TokenType.StringLiteral)
                {
                    ushort index = AddOrGetString(UnescapeString(expr.Value.Content.Substring(1, expr.Value.Content.Length - 2)));
                    size += EmitBCLInstruction(BCLOpcode.PushConstantString, index);
                }
                else if (expr.Value.Type == TokenType.IntegerLiteral)
                {
                    int value = Int32.Parse(expr.Value.Content);
                    if (value == 0)
                    {
                        size += EmitBCLInstruction(BCLOpcode.PushConstant0);
                    }
                    else if (value <= SByte.MaxValue && value >= SByte.MinValue)
                    {
                        size += EmitBCLInstruction(BCLOpcode.PushConstanti8, (sbyte)value);
                    }
                    else if (value <= Int16.MaxValue && value >= Int16.MinValue)
                    {
                        size += EmitBCLInstruction(BCLOpcode.PushConstanti16, (short)value);
                    }
                    else
                    {
                        size += EmitBCLInstruction(BCLOpcode.PushConstanti32, value);
                    }
                }
                else if (expr.Value.Type == TokenType.FloatLiteral)
                {
                    size += EmitBCLInstruction(BCLOpcode.PushConstantf32, Single.Parse(expr.Value.Content));
                }
                else if (expr.Value.Type == TokenType.KeywordTrue)
                {
                    size += EmitBCLInstruction(BCLOpcode.PushConstanti8, (sbyte)1);
                }
                else if (expr.Value.Type == TokenType.KeywordFalse)
                {
                    size += EmitBCLInstruction(BCLOpcode.PushConstant0);
                }
                else if (expr.Value.Type == TokenType.KeywordNull)
                {
                    size += EmitBCLInstruction(BCLOpcode.PushNothing);
                }
                else
                {
                    Error(Messages, "Invalid literal type: " + expr.Value.Type, "LSS080", expr.Value.Span, true);
                }
                return size;
            }

            public uint VisitUnaryExpression(UnaryExpression expr, object context)
            {
                uint size = 0;
                size += expr.Contents.AcceptVisitor(this, context);
                if (expr.Operation.Type == TokenType.Exclamation)
                {
                    size += EmitBCLInstruction(BCLOpcode.Not);
                }
                else if (expr.Operation.Type == TokenType.Tilde)
                {
                    size += EmitBCLInstruction(BCLOpcode.BitwiseNot);
                }
                else if (expr.Operation.Type == TokenType.Dash)
                {
                    size += EmitBCLInstruction(BCLOpcode.PushConstanti8, (sbyte)-1);
                    size += EmitBCLInstruction(BCLOpcode.Multiply);
                }
                else
                {
                    Error(Messages, "Invalid unary operator " + expr.Operation.Type, "LSS081", expr.Operation.Span, true);
                }
                return size;
            }

            public uint VisitVariableExpression(VariableExpression expr, object context)
            {
                uint size = 0;
                Variable variable = FindVariable(expr.Symbol.Content);

                if (variable != null)
                {
                    size += variable.EmitRead(this);
                }
                else
                {
                    Error(Messages, "Variable '" + expr.Symbol.Content + "' is not a valid local or global.", "LSS082", expr.Span, true);
                }

                return size;
            }
            #endregion

            #region Statements
            public uint VisitBlockStatement(BlockStatement s, object context)
            {
                uint size = 0;
                EnterScope();
                foreach (InstructionStatement childStatement in s.Instructions)
                {
                    try
                    {
                        size += childStatement.AcceptVisitor(this, context);
                    }
                    catch (PanicException)
                    {

                    }
                }
                LeaveScope();
                return size;
            }

            public uint VisitClassStatement(ClassStatement s, object context)
            {
                throw new InvalidOperationException("Class statements are not allowed in a subroutine body.");
            }

            public uint VisitPropertyStatement(PropertyStatement s, object context)
            {
                throw new InvalidOperationException("Property statements are not allowed in a subroutine body.");
            }

            public uint VisitSubroutineStatement(SubroutineStatement s, object context)
            {
                throw new InvalidOperationException("Subroutine statements are not allowed inside the body of another subroutine.");
            }

            public uint VisitGlobalStatement(GlobalStatement s, object context)
            {
                throw new InvalidOperationException("Global statements are not allowed inside a subroutine body.");
            }

            public uint VisitExpressionStatement(ExpressionStatement s, object context)
            {
                uint size = 0;
                if (s.Span.Start.Line.HasValue)
                    size += EmitLineNumberAlt1((ushort)s.Span.Start.Line.Value, s.Span.Start.Filename);
                size += s.Expression.AcceptVisitor(this, context);
                size += EmitBCLInstruction(BCLOpcode.Pop);
                return size;
            }

            public uint VisitReturnStatement(ReturnStatement s, object context)
            {
                uint size = 0;
                if (s.Span.Start.Line.HasValue)
                    size += EmitLineNumberAlt1((ushort)s.Span.Start.Line.Value, s.Span.Start.Filename);
                if (s.Value != null)
                {
                    size += s.Value.AcceptVisitor(this, context);
                }
                else
                {
                    size += EmitBCLInstruction(BCLOpcode.PushNothing);
                }
                size += EmitBCLInstruction(BCLOpcode.Return);
                return size;
            }

            public uint VisitIfStatement(IfStatement s, object context)
            {
                uint size = 0;
                if (s.Span.Start.Line.HasValue)
                    size += EmitLineNumberAlt1((ushort)s.Span.Start.Line.Value, s.Span.Start.Filename);

                BCLInstruction mainBranchInstruction = null;

                // 'else' statements have no condition
                if (s.Condition != null)
                {
                    size += s.Condition.AcceptVisitor(this, context);
                    size += EmitBCLInstruction(out mainBranchInstruction, BCLOpcode.CompareAndBranchIfFalse);
                }

                uint bodySize = s.Body.AcceptVisitor(this, context);
                size += bodySize;

                if (s.ElseStatement != null)
                {
                    size += EmitBCLInstruction(out BCLInstruction branchToEndInstruction, BCLOpcode.BranchAlways);
                    bodySize += branchToEndInstruction.Size;

                    uint elseSize = s.ElseStatement.AcceptVisitor(this, context);
                    size += elseSize;
                    branchToEndInstruction.Arguments[0].Value = (short)elseSize;
                }

                if (s.Condition != null)
                {
                    mainBranchInstruction.Arguments[0].Value = (short)bodySize;
                }

                return size;
            }

            public uint VisitWhileStatement(WhileStatement s, object context)
            {
                uint size = 0;
                if (s.Span.Start.Line.HasValue)
                    size += EmitLineNumberAlt1((ushort)s.Span.Start.Line.Value, s.Span.Start.Filename);

                size += s.Condition.AcceptVisitor(this, context);
                size += EmitBCLInstruction(out BCLInstruction exitBranch, BCLOpcode.CompareAndBranchIfFalse);

                uint bodySize = 0;
                bodySize += s.Body.AcceptVisitor(this, context);
                bodySize += EmitBCLInstruction(out BCLInstruction loopBranch, BCLOpcode.BranchAlways);

                size += bodySize;
                loopBranch.Arguments[0].Value = (short)(-1 * (short)size);
                exitBranch.Arguments[0].Value = (short)bodySize;

                return size;
            }

            public uint VisitDoWhileStatement(DoWhileStatement s, object context)
            {
                uint size = 0;

                size += s.Body.AcceptVisitor(this, context);

                size += s.Condition.AcceptVisitor(this, context);

                // Invert the condition to produce a 'false' to branch
                size += EmitBCLInstruction(BCLOpcode.Not);

                size += EmitBCLInstruction(out BCLInstruction branch, BCLOpcode.CompareAndBranchIfFalse);
                branch.Arguments[0].Value = (short)(-1 * (short)size);

                return size;
            }

            public uint VisitAssignmentStatement(AssignmentStatement s, object context)
            {
                uint size = 0;
                if (s.Span.Start.Line.HasValue)
                    size += EmitLineNumberAlt1((ushort)s.Span.Start.Line.Value, s.Span.Start.Filename);

                if (s.Target is VariableExpression varExpr)
                {
                    // Local variable or global variable assignment
                    if (varExpr.Symbol.Type == TokenType.KeywordThis)
                    {
                        Error(Messages, "'this' is not allowed to be modified.", "LSS083", s.Span, true);
                    }

                    Variable variable = FindVariable(varExpr.Symbol.Content);

                    if (variable != null)
                    {
                        size += variable.EmitWrite(this, s.Value, context);
                        return size;
                    }
                    else
                    {
                        Error(Messages, "Variable '" + varExpr.Symbol.Content + "' is not a valid local or global.", "LSS084", varExpr.Span, true);
                        return 0; // Unreachable
                    }
                }
                else if (s.Target is BinaryExpression memberExpr && memberExpr.Operation.Type == TokenType.Period && memberExpr.Right is VariableExpression member)
                {
                    // Member assignment (.)
                    if (member.Symbol.Type == TokenType.Symbol)
                    {
                        size += s.Value.AcceptVisitor(this, context); // Value
                        ushort memberSymbol = AddOrGetSymbol(member.Symbol.Content);
                        if (memberExpr.Left is VariableExpression instance && instance.Symbol.Type == TokenType.KeywordThis)
                        {
                            size += EmitBCLInstruction(BCLOpcode.SetThisMemberValue, (ushort)memberSymbol);
                        }
                        else
                        {
                            size += memberExpr.Left.AcceptVisitor(this, context); // Target

                            size += EmitBCLInstruction(BCLOpcode.SetMemberValue, (ushort)memberSymbol);
                        }
                    }
                    else
                    {
                        // Setting a builtin
                        if (memberExpr.Left is VariableExpression instance && instance.Symbol.Type == TokenType.KeywordThis)
                        {
                            Error(Messages, "No builtin members are allowed to be set on 'this'.", "LSS085", s.Span, true);
                        }
                        else
                        {
                            Error(Messages, "Symbol of type '" + member.Symbol.Type + "' is not a member nor a builtin and therefore cannot be assigned.", "LSS086", member.Span, true);
                        }
                    }
                    return size;
                }
                else if (s.Target is BinaryExpression lookupExpr && lookupExpr.Operation.Type == TokenType.PeriodDollarSign)
                {
                    // Dynamic member assignment (.$)
                    // Push value
                    size += s.Value.AcceptVisitor(this, context);

                    // Push target
                    size += lookupExpr.Left.AcceptVisitor(this, context);

                    // Push member name
                    size += lookupExpr.Right.AcceptVisitor(this, context);

                    size += EmitBCLInstruction(BCLOpcode.SetMemberValueFromString);

                    return size;
                }
                else if (s.Target is BinaryExpression gameExpr && gameExpr.Operation.Type == TokenType.ColonColon && gameExpr.Left is VariableExpression ns && gameExpr.Right is VariableExpression name)
                {
                    // Game variable assignment (::)
                    size += s.Value.AcceptVisitor(this, context);

                    size += EmitBCLInstruction(BCLOpcode.SetGameVariable, AddOrGetString(ns.Symbol.Content), AddOrGetString(name.Symbol.Content));

                    return size;
                }
                else if (s.Target is ArrayAccessExpression arrayExpr)
                {
                    // Array assignment ([ ])

                    size += arrayExpr.Array.AcceptVisitor(this, context);

                    size += arrayExpr.Index.AcceptVisitor(this, context);

                    size += s.Value.AcceptVisitor(this, context);

                    size += EmitBCLInstruction(BCLOpcode.SetArrayValue);

                    return size;
                }
                else
                {
                    Error(Messages, "Cannot assign into " + s.Target.ToString(), "LSS087", s.Span, true);
                    return 0; // Unreachable
                }
            }

            public uint VisitVariableDeclarationStatement(VariableDeclarationStatement s, object context)
            {
                if (s.Name.Type == TokenType.KeywordThis)
                {
                    Error(Messages, "The name 'this' is reserved and cannot be used for locals.", "LSS088", s.Name.Span, true);
                }

                uint size = 0;
                if (s.Span.Start.Line.HasValue)
                    size += EmitLineNumberAlt1((ushort)s.Span.Start.Line.Value, s.Span.Start.Filename);

                SubroutineScope scope = CurrentScope;
                while (scope != null)
                {
                    if (scope.Variables.ContainsKey(s.Name.Content))
                        Error(Messages, "Local variable '" + s.Name.Content + "' is already declared.", "LSS058", s.Span, true);
                    scope = scope.ParentScope;
                }

                Variable local = AddLocal(s.Name.Content);
                if (s.Initializer != null)
                {
                    size += local.EmitWrite(this, s.Initializer, context);
                }
                return size;
            }

            public uint VisitForEachStatement(ForEachStatement s, object context)
            {
                uint size = 0;
                EnterScope(); // A scope just for storing the index variable

                // Create the index variable
                StandardVariable indexVar = AddLocal(ITERATION_INDEX_LOCAL_NAME);

                // Push the max index = <collection>.length - 1
                size += s.Collection.AcceptVisitor(this, context);
                size += EmitBCLInstruction(BCLOpcode.ElementsInArray);
                size += EmitBCLInstruction(BCLOpcode.PushConstanti8, (sbyte)1);
                size += EmitBCLInstruction(BCLOpcode.Subtract);

                // Initialize the index variable
                size += EmitBCLInstruction(BCLOpcode.PushConstant0);
                size += EmitBCLInstruction(BCLOpcode.SetVariableValue, indexVar.Index);

                // The iteration condition that is checked each loop
                uint conditionStart = size;
                size += EmitBCLInstruction(BCLOpcode.Dup);
                size += indexVar.EmitRead(this);
                size += EmitBCLInstruction(BCLOpcode.GreaterOrEqual);
                size += EmitBCLInstruction(out BCLInstruction branchToEnd, BCLOpcode.CompareAndBranchIfFalse, (short)0); // Set this after we know the size of the loop body
                int conditionSize = (int)size - (int)conditionStart; // len1

                SubroutineScope scope = CurrentScope;
                while (scope != null)
                {
                    if (scope.Variables.ContainsKey(s.Variable.Content))
                        Error(Messages, "Local variable '" + s.Variable.Content + "' is already declared.", "LSS058", s.Span, true);
                    scope = scope.ParentScope;
                }

                // Set up the iteration variable
                IterationVariable iterationVar = AddIterationVariable(s.Variable.Content, indexVar, s.Collection);

                // The body of the loop
                uint bodyStart = size;
                size += s.Body.AcceptVisitor(this, context);
                size += EmitBCLInstruction(BCLOpcode.IncrementVariable, indexVar.Index);
                size += EmitBCLInstruction(out BCLInstruction branchBack, BCLOpcode.BranchAlways, (short)0);
                int bodySize = (int)size - (int)bodyStart;

                // Now we know the size of the body
                branchToEnd.Arguments[0].SetValue((short)bodySize);
                branchBack.Arguments[0].SetValue((short)-(conditionSize + bodySize));

                // Pop the stored max index
                size += EmitBCLInstruction(BCLOpcode.Pop);

                LeaveScope();
                return size;
            }
            #endregion
        }

        public static string UnescapeString(string value)
        {
            return value.Replace("\\\"", "\"").Replace("\\n", "\n");
        }

        public static string EscapeString(string value)
        {
            return value.Replace("\"", "\\\"").Replace("\n", "\\n");
        }

        private static void Error(Result result, string message, string errorCode, SourceSpan span, bool panic)
        {
            Error(result.Messages, message, errorCode, span, panic);
        }

        private static void Error(List<CompileMessage> messages, string message, string errorCode, SourceSpan span, bool panic)
        {
            messages.Add(new CompileMessage(message, errorCode, CompileMessage.MessageSeverity.Error, span));
            if (panic)
            {
                throw new PanicException();
            }
        }

        // Compiles a single source string into an OSI.
        public static Result Compile(string source, string filename, Settings settings = null)
        {
            if (settings == null)
                settings = new Settings();

            OSIFile osi = new OSIFile((ushort)settings.VersionMajor, (ushort)settings.VersionMinor);
            Result result = new Result(osi);
            Parser.Result parseResults;
            Parser p = new Parser();

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(Encoding.ASCII.GetBytes(source)))
            using (System.IO.StreamReader reader = new System.IO.StreamReader(ms))
            {
                List<CompileMessage> scanMessages = new List<CompileMessage>();
                List<Token> tokens = Scanner.Scan(source, filename, scanMessages, true, true);
                if (scanMessages.Count == 0)
                {
                    parseResults = p.Parse(tokens);
                    if (parseResults.Messages.Count == 0)
                    {
                        CompileInto(result, settings, parseResults);
                    }
                    else
                    {
                        result.Messages = parseResults.Messages;
                    }
                }
                else
                {
                    result.Messages = scanMessages;
                }
            }

            return result;
        }

        // Compiles the given files into an OSI.
        public static Result CompileFiles(IEnumerable<string> filenames, Settings settings = null)
        {
            if (settings == null)
                settings = new Settings();

            OSIFile osi = new OSIFile((ushort)settings.VersionMajor, (ushort)settings.VersionMinor);
            Result result = new Result(osi);
            Parser p = new Parser();
            List<Parser.Result> parseResults = new List<Parser.Result>();

            foreach (string filename in filenames)
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(filename))
                {
                    List<CompileMessage> scanErrors = new List<CompileMessage>();
                    List<Token> tokens = Scanner.Scan(reader.ReadToEnd(), filename, scanErrors, true, true);
                    if (scanErrors.Count == 0)
                    {
                        Parser.Result parseResult = p.Parse(tokens);
                        parseResults.Add(parseResult);
                        if (parseResult.Messages.Count == 0)
                        {
                            
                        }
                        else
                        {
                            result.Messages.AddRange(parseResult.Messages);
                        }
                    }
                    else
                    {
                        result.Messages.AddRange(scanErrors);
                    }
                }
            }
            CompileInto(result, settings, parseResults.ToArray());
            return result;
        }

        public static Result CompileParsed(Parser.Result parseResult, Settings settings = null)
        {
            if (settings == null)
                settings = new Settings();

            Result result = new Result(new OSIFile((ushort)settings.VersionMajor, (ushort)settings.VersionMinor));
            CompileInto(result, settings, parseResult);
            return result;
        }

        private static void CompileInto(Result result, Settings settings = null, params Parser.Result[] parseResults)
        {
            if (settings == null)
                settings = new Settings();

            Dictionary<string, ushort> strings = new Dictionary<string, ushort>();
            Dictionary<string, ushort> symbols = new Dictionary<string, ushort>();
            Dictionary<string, ushort> globals = new Dictionary<string, ushort>();

            Dictionary<string, OSIFile.FunctionInfo> functions = new Dictionary<string, OSIFile.FunctionInfo>();
            Dictionary<string, OSIFile.ClassInfo> classes = new Dictionary<string, OSIFile.ClassInfo>();
            Dictionary<string, ClassStatement> lssClassesByName = new Dictionary<string, ClassStatement>();

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
                        Error(result, "Class already exists with same name.", "LSS005", cls.Name.Span, false);
                        continue;
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

                    lssClassesByName.Add(cls.Name.Content, cls);
                }

                foreach (Statements.SubroutineStatement func in parseResult.Functions)
                {
                    if (functions.ContainsKey(func.Name.Content))
                    {
                        Error(result, "Function already exists with same name.", "LSS006", func.Name.Span, false);
                        continue;
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
                        result.Messages.Add(new CompileMessage("Global already exists with same name.", "LSS054", CompileMessage.MessageSeverity.Warning, global.Name.Span));
                    }
                    else
                    {
                        ushort globalIndex = (ushort)result.OSI.Globals.Count;
                        result.OSI.Globals.Add(global.Name.Content);
                        globals.Add(global.Name.Content, globalIndex);
                    }
                }

            }

            // Compile subroutines
            foreach (Parser.Result parseResult in parseResults)
            {
                // Compile the functions
                foreach (SubroutineStatement function in parseResult.Functions)
                {
                    SubroutineContext context = new SubroutineContext(result.Messages, result.OSI, settings, false, function.Parameters.Select(token => token.Content));
                    foreach (InstructionStatement stmt in function.Body.Instructions)
                    {
                        try
                        {
                            stmt.AcceptVisitor(context, null);
                        }
                        catch (PanicException)
                        {

                        }
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
                        SubroutineContext context = new SubroutineContext(result.Messages, result.OSI, settings, true, parameters);
                        foreach (InstructionStatement stmt in method.Body.Instructions)
                        {
                            try
                            {
                                stmt.AcceptVisitor(context, null);
                            }
                            catch (PanicException)
                            {

                            }
                        }
                        context.FinalizeInstructions();
                        OSIFile.MethodInfo destination = classes[cls.Name.Content].Methods.Find(m => m.NameSymbol == symbols[method.Name.Content]);
                        destination.Instructions.Clear();
                        destination.Instructions.AddRange(context.Instructions);
                    }
                }

            }

            // Apply inheritance
            // Until all the classes are applied, take a class, find the root-est that has not been applied of the tree, and apply it.
            HashSet<ClassStatement> classesToApply = new HashSet<ClassStatement>(lssClassesByName.Values.Where(c => c.SuperclassName != null));
            Stack<ClassStatement> loopDetector = new Stack<ClassStatement>();
            while (classesToApply.Count > 0)
            {
                // Pick an arbitrary element to start with
                ClassStatement toApply = classesToApply.ElementAt(0);
                loopDetector.Clear();
                loopDetector.Push(toApply);

                // Find the parent-est LSS class that needs to be applied
                bool failed = false;
                while (toApply.SuperclassName != null && lssClassesByName.ContainsKey(toApply.SuperclassName.Content)
                    && classesToApply.Contains(lssClassesByName[toApply.SuperclassName.Content]))
                {
                    toApply = lssClassesByName[toApply.SuperclassName.Content];
                    if (loopDetector.Contains(toApply))
                    {
                        Error(result, "Infinite loop in class inheritance. (" + String.Join(" -> ", loopDetector.Select(c => c.Name)) + ")", "LSS056", toApply.SuperclassName.Span, false);
                        // The whole inheritance chain can't be processed properly, so skip them
                        foreach (ClassStatement chainMember in loopDetector)
                        {
                            if (classesToApply.Contains(chainMember))
                                classesToApply.Remove(chainMember);
                        }
                        failed = true;
                    }
                    else
                    {
                        loopDetector.Push(toApply);
                    }
                }
                if (failed)
                    continue;

                // Apply the inheritance
                OSIFile.ClassInfo compiledClass = classes[toApply.Name.Content];
                OSIFile.ClassInfo superclass = classes[toApply.SuperclassName.Content];
                foreach (ushort propertySymbol in superclass.PropertySymbols)
                {
                    if (compiledClass.PropertySymbols.Contains(propertySymbol))
                    {
                        Error(result, "Class " + toApply.Name.Content + " cannot declare property " + result.OSI.Symbols[propertySymbol] + " because its superclass " + superclass.Name + " already declares it.", "LSS055", toApply.Properties.First(prop => prop.Name.Content == result.OSI.Symbols[propertySymbol]).Name.Span, false);
                        continue;
                    }
                    compiledClass.PropertySymbols.Add(propertySymbol);
                }

                foreach (OSIFile.MethodInfo method in superclass.Methods)
                {
                    if (!compiledClass.Methods.Any(m => m.NameSymbol == method.NameSymbol))
                    {
                        compiledClass.Methods.Add(new OSIFile.MethodInfo(method.NameSymbol, method));
                    }
                }

                classesToApply.Remove(toApply);
            }
        }
    }
}
