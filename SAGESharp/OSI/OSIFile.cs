using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SAGESharp.OSI
{
    public class OSIFile
    {
        public class FunctionInfo
        {
            public string Name { get; }
            public List<Instruction> Instructions { get; }
            public uint BytecodeOffset { get; }
            public ushort ParameterCount { get; }

            public FunctionInfo(string name, ushort parameterCount)
            {
                this.Name = name;
                this.ParameterCount = parameterCount;
                this.BytecodeOffset = 0;
                this.Instructions = new List<Instruction>();
            }

            public FunctionInfo(BinaryReader reader)
            {
                byte nameLength = reader.ReadByte();
                this.Name = Encoding.ASCII.GetString(reader.ReadBytes(nameLength));
                this.BytecodeOffset = reader.ReadUInt32();
                this.ParameterCount = reader.ReadUInt16();
                this.Instructions = OSIFile.ReadBytecode(reader, BytecodeOffset);
            }
        }

        public class MethodInfo
        {
            public ushort NameSymbol { get; }
            public List<Instruction> Instructions { get; }
            public uint BytecodeOffset { get; }

            public MethodInfo(ushort nameSymbol)
            {
                this.NameSymbol = nameSymbol;
                this.BytecodeOffset = 0;
                this.Instructions = new List<Instruction>();
            }

            public MethodInfo(BinaryReader reader)
            {
                this.NameSymbol = reader.ReadUInt16();
                this.BytecodeOffset = reader.ReadUInt32();
                this.Instructions = OSIFile.ReadBytecode(reader, BytecodeOffset);
            }
        }

        public class ClassInfo
        {
            public string Name { get; private set; }
            public List<ushort> PropertySymbols { get; }
            public List<MethodInfo> Methods { get; }

            public ClassInfo(string name, List<ushort> propertySymbols, List<MethodInfo> methods)
            {
                this.Name = name;
                this.PropertySymbols = propertySymbols;
                this.Methods = methods;
            }

            public ClassInfo(BinaryReader reader, ushort versionMajor)
            {
                // Properties
                ushort propertyCount = versionMajor < 6 ? reader.ReadByte() : reader.ReadUInt16();
                PropertySymbols = new List<ushort>();
                for (int i = 0; i < propertyCount; i++)
                {
                    PropertySymbols.Add(reader.ReadUInt16());
                }

                // Methods
                ushort methodCount = versionMajor < 6 ? reader.ReadByte() : reader.ReadUInt16();
                Methods = new List<MethodInfo>();
                for (int i = 0; i < methodCount; i++)
                {
                    Methods.Add(new MethodInfo(reader));
                }
            }

            public void LoadName(string name)
            {
                if (Name != null)
                    throw new InvalidOperationException("You may only load a class name to a class with no loaded name.");
                this.Name = name;
            }
        }

        ushort VersionMajor { get; }
        ushort VersionMinor { get; }
        public List<string> Strings { get; }
        public List<string> Globals { get; }
        public List<FunctionInfo> Functions { get; }
        public List<ClassInfo> Classes { get; }
        public List<string> Symbols { get; }
        public List<string> SourceFilenames { get; }

        public OSIFile(ushort versionMajor = 4, ushort versionMinor = 1)
        {
            this.VersionMajor = versionMajor;
            this.VersionMinor = versionMinor;
            this.Strings = new List<string>();
            this.Globals = new List<string>();
            this.Functions = new List<FunctionInfo>();
            this.Classes = new List<ClassInfo>();
            this.Symbols = new List<string>();
            this.SourceFilenames = new List<string>();
        }

        public OSIFile(BinaryReader reader)
        {
            // Header
            if (Encoding.ASCII.GetString(reader.ReadBytes(4)) != "OSI\0")
            {
                throw new FormatException("Incorrect magic.");
            }
            VersionMajor = reader.ReadUInt16();
            VersionMinor = reader.ReadUInt16();

            // String table
            ushort stringCount = reader.ReadUInt16();
            Strings = new List<string>();
            for (int i = 0; i < stringCount; i++)
            {
                byte length = reader.ReadByte();
                Strings.Add(Encoding.ASCII.GetString(reader.ReadBytes(length)));
                reader.ReadByte(); // Null terminator
            }

            // Global table
            ushort globalCount = reader.ReadUInt16();
            Globals = new List<string>();
            for (int i = 0; i < globalCount; i++)
            {
                byte length = reader.ReadByte();
                Globals.Add(Encoding.ASCII.GetString(reader.ReadBytes(length)));
                reader.ReadByte(); // Null terminator
            }

            // Function table
            ushort functionCount = reader.ReadUInt16();
            Functions = new List<FunctionInfo>();
            for (int i = 0; i < functionCount; i++)
            {
                Functions.Add(new FunctionInfo(reader));
            }

            // Class table
            ushort classCount = reader.ReadUInt16();
            Classes = new List<ClassInfo>();
            for (int i = 0; i < classCount; i++)
            {
                Classes.Add(new ClassInfo(reader, VersionMajor));
            }
            for (int i = 0; i < classCount; i++)
            {
                byte length = reader.ReadByte();
                Classes[i].LoadName(Encoding.ASCII.GetString(reader.ReadBytes(length)));
                reader.ReadByte(); // Null terminator
            }

            // Symbol table
            ushort symbolCount = reader.ReadUInt16();
            Symbols = new List<string>();
            for (int i = 0; i < symbolCount; i++)
            {
                byte length = reader.ReadByte();
                Symbols.Add(Encoding.ASCII.GetString(reader.ReadBytes(length)));
                reader.ReadByte(); // Null terminator
            }

            // Source table
            if (VersionMajor == 4)
            {
                ushort sourceCount = reader.ReadUInt16();
                SourceFilenames = new List<string>();
                for (int i = 0; i < sourceCount; i++)
                {
                    byte length = reader.ReadByte();
                    SourceFilenames.Add(Encoding.ASCII.GetString(reader.ReadBytes(length)));
                    reader.ReadByte(); // Null terminator
                }
            }

            TransformToJumpStatic();
        }

        private static List<Instruction> ReadBytecode(BinaryReader reader, uint startOffset)
        {
            List<Instruction> result = new List<Instruction>();
            // Read bytecode until the last return
            long offset = reader.BaseStream.Position;

            reader.BaseStream.Position = startOffset;

            long lastBranchTarget = 0;
            bool keepReading = true;
            result = new List<Instruction>();
            while (keepReading)
            {
                BCLInstruction instruction = new BCLInstruction((BCLOpcode)reader.ReadByte(), reader);
                result.Add(instruction);

                if (instruction.Opcode == BCLOpcode.BranchAlways || instruction.Opcode == BCLOpcode.CompareAndBranchIfFalse)
                {
                    int delta = (short)instruction.Arguments[0].Value;
                    long branchTo = reader.BaseStream.Position + delta;
                    if (branchTo > lastBranchTarget)
                        lastBranchTarget = branchTo;
                }
                else if (instruction.Opcode == BCLOpcode.Return && reader.BaseStream.Position > lastBranchTarget)
                {
                    keepReading = false;
                }
            }

            reader.BaseStream.Position = offset;
            return result;
        }

        private void InsertJumpStatic(List<Instruction> instructions)
        {
            for (int i = instructions.Count - 2; i > 0; i--)
            {
                if (instructions[i] is BCLInstruction addr && addr.Opcode == BCLOpcode.PushConstanti32
                    && instructions[i + 1] is BCLInstruction jmp && jmp.Opcode == BCLOpcode.JumpRelative)
                {
                    JumpStaticInstruction jmpStatic = new JumpStaticInstruction(this, addr, jmp);
                    instructions.RemoveAt(i + 1);
                    instructions[i] = jmpStatic;
                }
            }

        }

        public void TransformToJumpStatic()
        {
            foreach (FunctionInfo func in Functions)
            {
                InsertJumpStatic(func.Instructions);
            }
            foreach (ClassInfo cls in Classes)
            {
                foreach (MethodInfo method in cls.Methods)
                {
                    InsertJumpStatic(method.Instructions);
                }
            }
        }

        private void RemoveJumpStatic(List<Instruction> instructions)
        {
            for (int i = instructions.Count - 1; i > 0; i--)
            {
                if (instructions[i] is JumpStaticInstruction jmpStatic)
                {
                    uint? destination = null;
                    foreach (FunctionInfo func in Functions)
                    {
                        if (func.Name == jmpStatic.FunctionName)
                        {
                            destination = func.BytecodeOffset;
                        }
                    }
                    if (!destination.HasValue)
                    {
                        throw new ArgumentException("Function '" + jmpStatic.FunctionName + "' does not exist.");
                    }

                    BCLInstruction addr = new BCLInstruction(BCLOpcode.PushConstanti32, (int)destination.Value);
                    BCLInstruction jmp = new BCLInstruction(BCLOpcode.JumpRelative, (sbyte)jmpStatic.ArgumentCount);
                    instructions[i] = jmp;
                    instructions.Insert(i, addr);
                }
            }
        }

        public void TransformRemoveJumpStatic()
        {
            foreach (FunctionInfo func in Functions)
            {
                RemoveJumpStatic(func.Instructions);
            }
            foreach (ClassInfo cls in Classes)
            {
                foreach (MethodInfo method in cls.Methods)
                {
                    RemoveJumpStatic(method.Instructions);
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Globals [" + this.Globals.Count + "]:");
            for (int i = 0; i < this.Globals.Count; i++)
            {
                sb.AppendLine("  [" + i.ToString().PadLeft(4, ' ') + "]: " + this.Globals[i]);
            }
            sb.AppendLine("Symbols [" + this.Symbols.Count + "]:");
            for (int i = 0; i < this.Symbols.Count; i++)
            {
                sb.AppendLine("  [" + i.ToString().PadLeft(4, ' ') + "]: " + this.Symbols[i]);
            }
            sb.AppendLine("Strings [" + this.Strings.Count + "]:");
            for (int i = 0; i < this.Strings.Count; i++)
            {
                sb.AppendLine("  [" + i.ToString().PadLeft(4, ' ') + "]: \"" + this.Strings[i] + "\"");
            }
            sb.AppendLine("Sources [" + this.SourceFilenames.Count + "]: ");
            for (int i = 0; i < this.SourceFilenames.Count; i++)
            {
                sb.AppendLine("  [" + i.ToString().PadLeft(4, ' ') + "]: \"" + this.SourceFilenames[i] + "\"");
            }
            sb.AppendLine("Functions [" + this.Functions.Count + "]: ");
            for (int i = 0; i < this.Functions.Count; i++)
            {
                sb.AppendLine("  [" + i.ToString().PadLeft(4, ' ') + "]: " + this.Functions[i].Name + "(" + this.Functions[i].ParameterCount + ") -> 0x" + this.Functions[i].BytecodeOffset.ToString("X8"));
                uint offset = 0;
                for (int j = 0; j < this.Functions[i].Instructions.Count; j++)
                {
                    sb.AppendLine("    [" + offset.ToString().PadLeft(8, '0') + "]: " + this.Functions[i].Instructions[j].ToString());
                    offset += this.Functions[i].Instructions[j].Size;
                }
            }
            sb.AppendLine("Classes [" + this.Classes.Count + "]: ");
            for (int i = 0; i < this.Classes.Count; i++)
            {
                sb.AppendLine("  [" + i.ToString().PadLeft(4, ' ') + "]: " + this.Classes[i].Name);
                sb.AppendLine("    Properties [" + this.Classes[i].PropertySymbols.Count + "]:");
                for (int j = 0; j < this.Classes[i].PropertySymbols.Count; j++)
                {
                    sb.AppendLine("      [" + j.ToString().PadLeft(4, ' ') + "]: " + this.Symbols[this.Classes[i].PropertySymbols[j]]);
                }
                sb.AppendLine("    Methods [" + this.Classes[i].Methods.Count + "]:");
                for (int j = 0; j < this.Classes[i].Methods.Count; j++)
                {
                    sb.AppendLine("      [" + j.ToString().PadLeft(4, ' ') + "]: " + this.Symbols[this.Classes[i].Methods[j].NameSymbol] + " -> 0x" + this.Classes[i].Methods[j].BytecodeOffset.ToString("X8"));
                    uint offset = 0;
                    for (int k = 0; k < this.Classes[i].Methods[j].Instructions.Count; k++)
                    {
                        sb.AppendLine("        [" + offset.ToString().PadLeft(8, '0') + "]: " + this.Classes[i].Methods[j].Instructions[k].ToString());
                        offset += this.Classes[i].Methods[j].Instructions[k].Size;
                    }
                }
            }
            return sb.ToString();
        }
    }
}
