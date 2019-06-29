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
            public List<SyntaxError> Errors;

            public Result(OSIFile osi)
            {
                this.OSI = osi;
                this.Errors = new List<SyntaxError>();
            }
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
                // TODO: Panic & Sync
                throw new InvalidOperationException("Cannot set an iteration variable.");
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

            public SubroutineContext(OSIFile osi, Settings compileSettings, bool isInstanceMethod, IEnumerable<string> parameterNames)
            {
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
                SubroutineScope scope = CurrentScope;
                while (scope != null)
                {
                    if (scope.Variables.ContainsKey(localName))
                        throw new ArgumentException("Variable is already declared, and would conflict.");
                    scope = scope.ParentScope;
                }
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
                                    throw new ArgumentException("'this' keyword is only valid in an instance method.");
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
                                throw new ArgumentException("Cannot get value of type '" + symbol.Symbol.Type.ToString() + "' on 'this'.");
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
                                    throw new ArgumentException("'" + symbol.Symbol.Content + "' of type '" + symbol.Symbol.Type.ToString() + "' is not a valid builtin property.");
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Member access must use a member name.");
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
                        throw new ArgumentException("Game variable access must use a namespace and name symbol.");
                    }
                }
                else if (expr.Operation.Type == TokenType.ColonColonDollarSign)
                {
                    throw new NotImplementedException("Dynamic-lookup game variable are not implemented in SAGE.");
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
                        throw new InvalidOperationException("Invalid binary operator: " + expr.Operation.Type);
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
                            throw new ArgumentException("Static function '" + varExp.Symbol.Content + "' not found.");
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
                                throw new ArgumentException("Builtin function '" + varExp.Symbol.Content + "' requires exactly 4 arguments.");
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
                                        throw new ArgumentException("Value is out of the acceptable range (0 - 255) for R component.");
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
                                        throw new ArgumentException("Value is out of the acceptable range (0 - 255) for G component.");
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
                                        throw new ArgumentException("Value is out of the acceptable range (0 - 255) for B component.");
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
                                        throw new ArgumentException("Value is out of the acceptable range (0 - 255) for A component.");
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
                            if (expr.Arguments.Count != 1)
                            {
                                throw new ArgumentException("Builtin function '" + varExp.Symbol.Content + "' requires exactly 1 argument.");
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
                                throw new ArgumentException("Not invokable.");
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
                                        throw new InvalidOperationException("'this' not allowed in static function.");
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
                                        throw new ArgumentException("Builtin 'append' requires 1 argument.");

                                    size += binExp.Left.AcceptVisitor(this, context); // Push array

                                    //size += EmitBCLInstruction(BCLOpcode.Dup); // Make a copy, so we 'return' the original array


                                    size += expr.Arguments[0].AcceptVisitor(this, context); // Push value

                                    size += EmitBCLInstruction(BCLOpcode.AppendToArray);

                                }
                                else if (nameExp.Symbol.Type == TokenType.KeywordRemoveAt)
                                {
                                    if (expr.Arguments.Count != 1)
                                        throw new ArgumentException("Builtin 'removeat' requires 1 argument.");

                                    size += binExp.Left.AcceptVisitor(this, context); // Push array

                                    size += EmitBCLInstruction(BCLOpcode.Dup); // Make a copy, so we 'return' the original array


                                    size += expr.Arguments[0].AcceptVisitor(this, context); // Push index

                                    size += EmitBCLInstruction(BCLOpcode.RemoveFromArray);

                                }
                                else if (nameExp.Symbol.Type == TokenType.KeywordInsertAt)
                                {
                                    if (expr.Arguments.Count != 2)
                                        throw new ArgumentException("Builtin 'insertat' requires 2 arguments.");

                                    size += binExp.Left.AcceptVisitor(this, context); // Push array

                                    size += EmitBCLInstruction(BCLOpcode.Dup); // Make a copy, so we 'return' the original array


                                    size += expr.Arguments[0].AcceptVisitor(this, context); // Push index

                                    size += expr.Arguments[1].AcceptVisitor(this, context); // Push value

                                    size += EmitBCLInstruction(BCLOpcode.InsertIntoArray);

                                }
                                else
                                {
                                    throw new ArgumentException("Member expression not invokable.");
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
                                throw new ArgumentException("Game function namespace must be a symbol.");
                            }
                        }
                        else
                        {
                            throw new ArgumentException("Expression is not standard-invokable.");
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
                                throw new ArgumentException("Game function namespace must be a symbol.");
                            }
                        }
                        else
                        {
                            throw new ArgumentException("Expression is not dynamic-invokable.");
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Expression is not invokable.");
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
                    throw new ArgumentException("Type name '" + expr.TypeName.Content + "' is not a valid class!");

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
                    ushort index = AddOrGetString(expr.Value.Content.Substring(1, expr.Value.Content.Length - 2));
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
                    throw new InvalidOperationException("Invalid literal type: " + expr.Value.Type);
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
                    throw new InvalidOperationException("Invalid unary operator " + expr.Operation.Type);
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
                    throw new ArgumentException("Variable '" + expr.Symbol.Content + "' is not a valid local or global.");
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
                    size += childStatement.AcceptVisitor(this, context);
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
                        throw new ArgumentException("'this' is not allowed to be modified.");
                    }

                    Variable variable = FindVariable(varExpr.Symbol.Content);

                    if (variable != null)
                    {
                        size += variable.EmitWrite(this, s.Value, context);
                        return size;
                    }
                    else
                    {
                        throw new ArgumentException("Variable '" + varExpr.Symbol.Content + "' is not a valid local or global.");
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
                            throw new ArgumentException("Cannot set a builtin member of 'this'.");
                        }
                        size += memberExpr.Left.AcceptVisitor(this, context); // Target

                        size += s.Value.AcceptVisitor(this, context); // Value

                        if (member.Symbol.Type == TokenType.KeywordRed)
                        {
                            size += EmitBCLInstruction(BCLOpcode.SetRedValue);
                            size += EmitBCLInstruction(BCLOpcode.Pop); // Because SetRedValue pushes the resulting color
                        }
                        else if (member.Symbol.Type == TokenType.KeywordGreen)
                        {
                            size += EmitBCLInstruction(BCLOpcode.SetGreenValue);
                            size += EmitBCLInstruction(BCLOpcode.Pop); // Because SetGreenValue pushes the resulting color
                        }
                        else if (member.Symbol.Type == TokenType.KeywordBlue)
                        {
                            size += EmitBCLInstruction(BCLOpcode.SetBlueValue);
                            size += EmitBCLInstruction(BCLOpcode.Pop); // Because SetBlueValue pushes the resulting color
                        }
                        else if (member.Symbol.Type == TokenType.KeywordAlpha)
                        {
                            size += EmitBCLInstruction(BCLOpcode.SetAlphaValue);
                            size += EmitBCLInstruction(BCLOpcode.Pop); // Because SetAlphaValue pushes the resulting color
                        }
                        else
                        {
                            throw new ArgumentException("Symbol of type '" + member.Symbol.Type + "' is not a member nor a builtin and therefore cannot be assigned.");
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
                    throw new ArgumentException("Cannot assign into " + s.Target.ToString());
                }
            }

            public uint VisitVariableDeclarationStatement(VariableDeclarationStatement s, object context)
            {
                if (s.Name.Type == TokenType.KeywordThis)
                {
                    throw new ArgumentException("Cannot create local variables with reserved name 'this'.");
                }

                uint size = 0;
                if (s.Span.Start.Line.HasValue)
                    size += EmitLineNumberAlt1((ushort)s.Span.Start.Line.Value, s.Span.Start.Filename);

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

        //private class DecompileSubroutineContext : ExpressionVisitor<object, object>

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
                List<SyntaxError> scanErrors = new List<SyntaxError>();
                List<Token> tokens = Scanner.Scan(source, filename, scanErrors, true, true);
                if (scanErrors.Count == 0)
                {
                    parseResults = p.Parse(tokens);
                    if (parseResults.Errors.Count == 0)
                    {
                        CompileInto(result, settings, parseResults);
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
                    List<SyntaxError> scanErrors = new List<SyntaxError>();
                    List<Token> tokens = Scanner.Scan(reader.ReadToEnd(), filename, scanErrors, true, true);
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
                        result.Errors.Add(new SyntaxError("Class already exists with same name.", cls.Name.Span));
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
                        result.Errors.Add(new SyntaxError("Function already exists with same name.", func.Name.Span));
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
                    SubroutineContext context = new SubroutineContext(result.OSI, settings, false, function.Parameters.Select(token => token.Content));
                    foreach (InstructionStatement stmt in function.Body.Instructions)
                    {
                        stmt.AcceptVisitor(context, null);
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
                        SubroutineContext context = new SubroutineContext(result.OSI, settings, true, parameters);
                        foreach (InstructionStatement stmt in method.Body.Instructions)
                        {
                            stmt.AcceptVisitor(context, null);
                        }
                        context.FinalizeInstructions();
                        OSIFile.MethodInfo destination = classes[cls.Name.Content].Methods.Find(m => m.NameSymbol == symbols[method.Name.Content]);
                        destination.Instructions.Clear();
                        destination.Instructions.AddRange(context.Instructions);
                    }
                }
            }
        }
    }
}
