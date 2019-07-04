using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SAGESharp.OSI
{
    /// <summary>
    /// A file which contains compiled classes and functions for use with the SAGE game engine.
    /// </summary>
    public class OSIFile
    {
        /// <summary>
        /// Metadata for a static OSI function.
        /// </summary>
        public class FunctionInfo
        {
            /// <summary>
            /// The name of the function.
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// The instructions that the function executes.
            /// </summary>
            public List<Instruction> Instructions { get; }

            /// <summary>
            /// The offset in the OSI file where the <see cref="Instructions"/> were read from.
            /// Do not assume this offset will be the location where the Instructions will be written to,
            /// unless <see cref="UpdateBytecodeLayout"/> has just been called.
            /// </summary>
            public uint BytecodeOffset { get; set; }

            /// <summary>
            /// The number of parameters that the function has.
            /// </summary>
            public ushort ParameterCount { get; }

            /// <summary>
            /// Creates a new FunctionInfo with the given name and parameter count.
            /// </summary>
            /// <param name="name">The name of the function.</param>
            /// <param name="parameterCount">The number of parameters the function has.</param>
            public FunctionInfo(string name, ushort parameterCount)
            {
                this.Name = name;
                this.ParameterCount = parameterCount;
                this.BytecodeOffset = 0;
                this.Instructions = new List<Instruction>();
            }

            /// <summary>
            /// Reades a FunctionInfo from the given <see cref="BinaryReader"/>.
            /// </summary>
            /// <param name="reader">The BinaryReader to read the function from.</param>
            public FunctionInfo(BinaryReader reader)
            {
                byte nameLength = reader.ReadByte();
                this.Name = Encoding.ASCII.GetString(reader.ReadBytes(nameLength));
                this.BytecodeOffset = reader.ReadUInt32();
                this.ParameterCount = reader.ReadUInt16();
                this.Instructions = OSIFile.ReadBytecode(reader, BytecodeOffset);
            }

            /// <summary>
            /// Writes this FunctionInfo, excluding the bytecode, to the given <see cref="BinaryWriter"/>.
            /// </summary>
            /// <param name="writer">The BinaryWriter to write to.</param>
            public void Write(BinaryWriter writer)
            {
                byte[] nameBytes = Encoding.ASCII.GetBytes(Name);
                if (nameBytes.Length > Byte.MaxValue)
                    throw new FormatException("Function name '" + Name + "' may not be more than " + Byte.MaxValue + " bytes!");
                writer.Write((byte)nameBytes.Length);
                writer.Write(nameBytes);
                writer.Write(BytecodeOffset);
                writer.Write(ParameterCount);
            }

            /// <summary>
            /// Writes the bytecode of the function to the given <see cref="BinaryWriter"/>.
            /// </summary>
            /// <param name="writer">The BinaryWriter to write the bytecode to.</param>
            /// <exception cref="ArgumentException">Thrown if the given BinaryWriter fails to seek to the location specified in <see cref="BytecodeOffset"/>.</exception>
            public void WriteBytecode(BinaryWriter writer)
            {
                long startPosition = writer.BaseStream.Position;
                try
                {
                    writer.BaseStream.Position = BytecodeOffset;
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("The given writer must be able to seek to the BytecodeOffset of this FunctionInfo.", ex);
                }
                foreach (Instruction i in Instructions)
                    i.Write(writer);
                writer.BaseStream.Position = startPosition;
            }
        }

        /// <summary>
        /// Metadata for an OSI class' instance method.
        /// </summary>
        public class MethodInfo
        {
            /// <summary>
            /// The index of the OSI symbol that contains the name of the method.
            /// </summary>
            public ushort NameSymbol { get; }

            /// <summary>
            /// The instructions that make up the content of the method.
            /// </summary>
            public List<Instruction> Instructions { get; private set; } = new List<Instruction>();

            /// <summary>
            /// The offset in the OSI file where the <see cref="Instructions"/> were read from.
            /// Do not assume this offset will be the location where the Instructions will be written to,
            /// unless <see cref=UpdateBytecodeLayout"/> has just been called.
            /// </summary>
            public uint BytecodeOffset { get; set; }

            /// <summary>
            /// The method that this method inherits its instructions from, instead of having its own instructions.
            /// This is used when a class inherits a method from a superclass.
            /// </summary>
            public MethodInfo SourceMethod
            {
                get => _sourceMethod;
                set
                {
                    if (_sourceMethod != value)
                    {
                        if (value == null)
                        {
                            Instructions = new List<Instruction>();
                        }
                        else
                        {
                            Instructions = value.Instructions;
                        }
                        _sourceMethod = value;
                    }
                }
            }
            private MethodInfo _sourceMethod = null;

            /// <summary>
            /// Creates a new MethodInfo with the given name symbol index and optional source method.
            /// </summary>
            /// <param name="nameSymbol">The index of the OSI symbol that contains the name for this method.</param>
            /// <param name="sourceMethod">The optional method that contains the instructions for this method.</param>
            public MethodInfo(ushort nameSymbol, MethodInfo sourceMethod = null)
            {
                this.NameSymbol = nameSymbol;
                this.BytecodeOffset = 0;
                this.SourceMethod = sourceMethod;
            }

            /// <summary>
            /// Reads a MethodInfo from the given <see cref="BinaryReader"/>.
            /// </summary>
            /// <param name="reader">The BinaryReader to read the method info and instructions from.</param>
            public MethodInfo(BinaryReader reader)
            {
                this.NameSymbol = reader.ReadUInt16();
                this.BytecodeOffset = reader.ReadUInt32();
                this.Instructions = OSIFile.ReadBytecode(reader, BytecodeOffset);
            }

            /// <summary>
            /// Writes this MethodInfo, excluding the bytecode, to the given <see cref="BinaryWriter"/>.
            /// </summary>
            /// <param name="writer">The BinaryWriter to write to.</param>
            public void Write(BinaryWriter writer)
            { 
                writer.Write(NameSymbol);
                writer.Write(BytecodeOffset);
            }

            /// <summary>
            /// Writes the bytecode of the method to the given <see cref="BinaryWriter"/>.
            /// </summary>
            /// <param name="writer">The BinaryWriter to write the bytecode to.</param>
            /// <exception cref="ArgumentException">Thrown if the given BinaryWriter fails to seek to the location specified in <see cref="BytecodeOffset"/>.</exception>
            public void WriteBytecode(BinaryWriter writer)
            {
                long startPosition = writer.BaseStream.Position;
                try
                {
                    writer.BaseStream.Position = BytecodeOffset;
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("The given writer must be able to seek to the BytecodeOffset of this FunctionInfo.", ex);
                }
                foreach (Instruction i in Instructions)
                    i.Write(writer);
                writer.BaseStream.Position = startPosition;
            }
        }

        /// <summary>
        /// A class in an OSI file.
        /// </summary>
        public class ClassInfo
        {
            /// <summary>
            /// The name of the class.
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// The indices of the OSI symbols that contain the names of the properties of this class.
            /// </summary>
            public List<ushort> PropertySymbols { get; }

            /// <summary>
            /// The instance methods of this class.
            /// </summary>
            public List<MethodInfo> Methods { get; }

            /// <summary>
            /// Creates a new ClassInfo with the given name, property symbols, and methods.
            /// </summary>
            /// <param name="name">The name for the class.</param>
            /// <param name="propertySymbols">The indices of the OSI symbols that contain the names of the properties for the class.</param>
            /// <param name="methods">The methods for the class.</param>
            public ClassInfo(string name, List<ushort> propertySymbols, List<MethodInfo> methods)
            {
                this.Name = name;
                this.PropertySymbols = propertySymbols;
                this.Methods = methods;
            }

            /// <summary>
            /// Reads a ClassInfo instance from the given <see cref="BinaryReader"/>, using the format for the given version of SAGE.
            /// The name is not loaded, and must be set exactly once later, using the <see cref="LoadName"/> function.
            /// </summary>
            /// <param name="reader">The BinaryReader to read the class from.</param>
            /// <param name="versionMajor">The major component of the version number of the OSI file format.</param>
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

            /// <summary>
            /// Sets the name of the class to the given value. Cannot be called once the name has been set.
            /// </summary>
            /// <param name="name">The new name for this class.</param>
            /// <exception cref="InvalidOperationException">Thrown if the name has already been set.</exception>
            public void LoadName(string name)
            {
                if (Name != null)
                    throw new InvalidOperationException("You may only load a class name to a class with no loaded name.");
                this.Name = name;
            }

            /// <summary>
            /// Writes this class, except the <see cref="Name"/> and the method <see cref="MethodInfo.Instructions"/>, to the given <see cref="BinaryWriter"/>.
            /// </summary>
            /// <param name="writer">The BinaryWriter to write to.</param>
            /// <param name="versionMajor">The major version of the OSI file sytax to write.</param>
            public void Write(BinaryWriter writer, ushort versionMajor)
            {
                if (versionMajor < 6)
                    writer.Write((byte)PropertySymbols.Count);
                else
                    writer.Write((ushort)PropertySymbols.Count);

                foreach (ushort symbol in PropertySymbols)
                    writer.Write(symbol);

                if (versionMajor < 6)
                    writer.Write((byte)Methods.Count);
                else
                    writer.Write((ushort)Methods.Count);

                foreach (MethodInfo m in Methods)
                    m.Write(writer);
            }
        }

        /// <summary>
        /// The major component of the version of SAGE that this OSI is built for.
        /// </summary>
        ushort VersionMajor { get; }

        /// <summary>
        /// The minor component of the version of SAGE the this OSI is built for.
        /// </summary>
        ushort VersionMinor { get; }

        /// <summary>
        /// The constant strings that are used in this OSI.
        /// </summary>
        public List<string> Strings { get; }

        /// <summary>
        /// The names of the globals that this OSI defines.
        /// </summary>
        public List<string> Globals { get; }

        /// <summary>
        /// The static functions that this OSI contains.
        /// </summary>
        public List<FunctionInfo> Functions { get; }
        
        /// <summary>
        /// The classes that this OSI contains.
        /// </summary>
        public List<ClassInfo> Classes { get; }

        /// <summary>
        /// The names of the symbols that are used in this OSI.
        /// </summary>
        public List<string> Symbols { get; }

        /// <summary>
        /// The filenames that are referenced in LineNumber diagnostic instructions in this OSI.
        /// </summary>
        public List<string> SourceFilenames { get; }

        /// <summary>
        /// Creates an empty OSI file that targets the given SAGE version.
        /// </summary>
        /// <param name="versionMajor">The major component of the SAGE version number.</param>
        /// <param name="versionMinor">The minor component of the SAGE version number.</param>
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

        /// <summary>
        /// Reads an OSI file from the given <see cref="BinaryReader"/>.
        /// </summary>
        /// <param name="reader">The BinaryReader to read from.</param>
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

            // Find linked methods
            Dictionary<uint, MethodInfo> methodOwners = new Dictionary<uint, MethodInfo>();
            foreach (ClassInfo c in Classes)
            {
                foreach (MethodInfo m in c.Methods)
                {
                    if (methodOwners.ContainsKey(m.BytecodeOffset))
                    {
                        // Make m be a child of whatever is already at that offset
                        // NOTE: this does not determine which way the inheritance relationship works. Inheritance direction is irrelevant for baseline OSI functionality.
                        m.SourceMethod = methodOwners[m.BytecodeOffset];
                    }
                    else
                    {
                        // m is the owner of that bytecode
                        methodOwners.Add(m.BytecodeOffset, m);
                    }
                }
            }

            TransformToJumpStatic();
        }

        /// <summary>
        /// Reads <see cref="Instruction"/>s from the given <see cref="BinaryReader"/> starting at the given offset, and ending when the end of the subroutine is detected.
        /// </summary>
        /// <param name="reader">The BinaryReader to read Instructions from.</param>
        /// <param name="startOffset">The offset to begin reading Instructions at.</param>
        /// <returns>The Instructions that were read from the given reader.</returns>
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

        /// <summary>
        /// Writes the contents of this OSI file to the given <see cref="BinaryWriter"/>.
        /// </summary>
        /// <param name="writer">The BinaryWriter to write to.</param>
        public void Write(BinaryWriter writer)
        {
            TransformRemoveJumpStatic();

            // Header
            writer.Write(Encoding.ASCII.GetBytes("OSI\0"));
            writer.Write(VersionMajor);
            writer.Write(VersionMinor);

            // String table
            WriteStringList(writer, Strings);

            // Global table
            WriteStringList(writer, Globals);

            // Function table
            writer.Write((ushort)Functions.Count);
            foreach (FunctionInfo f in Functions)
                f.Write(writer);

            // Class table
            writer.Write((ushort)Classes.Count);
            foreach (ClassInfo c in Classes)
                c.Write(writer, VersionMajor);
            foreach (ClassInfo c in Classes)
            {
                byte[] nameBytes = Encoding.ASCII.GetBytes(c.Name);
                if (nameBytes.Length > Byte.MaxValue)
                    throw new FormatException("Class name '" + c.Name + "' may not be more than " + Byte.MaxValue + " bytes!");
                writer.Write((byte)nameBytes.Length);
                writer.Write(nameBytes);
                writer.Write((byte)0);
            }

            // Symbol table
            WriteStringList(writer, Symbols);

            // Source table
            if (VersionMajor == 4)
            {
                WriteStringList(writer, SourceFilenames);
            }

            // Function contents
            foreach (FunctionInfo f in Functions)
                f.WriteBytecode(writer);

            // Method contents
            foreach (ClassInfo c in Classes)
                foreach (MethodInfo m in c.Methods)
                    m.WriteBytecode(writer);
        }

        private static void WriteStringList(BinaryWriter writer, List<string> strings)
        {
            writer.Write((ushort)strings.Count);
            foreach (string s in strings)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(s);
                if (bytes.Length > Byte.MaxValue)
                    throw new FormatException("Value '" + s + "' may not be more than " + Byte.MaxValue + " bytes!");
                writer.Write((byte)bytes.Length);
                writer.Write(bytes);
                writer.Write((byte)0);
            }
        }

        // Refreshes the offsets where the bytecode will be.
        /// <summary>
        /// Refreshes the offsets of where the bytecode will be written in all the functions and methods in this OSI file.
        /// </summary>
        public void UpdateBytecodeLayout()
        {
            uint offset = 18; // 4 magic, 4 versions, 2 string count, 2 global count, 2 function count, 2 class count, 2 symbol count

            uint countSize = VersionMajor < 6 ? 1u : 2u;

            foreach (string s in Strings)
                offset += (uint)s.Length + 2; // plus 1 length, 1 null terminator

            foreach (string g in Globals)
                offset += (uint)g.Length + 2; // plus 1 length, 1 null terminator

            foreach (FunctionInfo f in Functions)
                offset += (uint)f.Name.Length + 7; // plus 1 name length, 4 bytecode, 2 parameters

            foreach (ClassInfo c in Classes)
                offset += (uint)c.Name.Length + countSize * 2 + (uint)c.PropertySymbols.Count * 2 + (uint)c.Methods.Count * 6 + 2; // method: 2 name, 4 offset, class: count of properties, count of methods, 1 name length, 1 null terminator

            foreach (string s in Symbols)
                offset += (uint)s.Length + 2; // plus 1 length, 1 null terminator

            if (VersionMajor == 4)
            {
                offset += 2;
                foreach (string s in SourceFilenames)
                    offset += (uint)s.Length + 2; // plus 1 length, 1 null terminator
            }

            foreach (FunctionInfo f in Functions)
            {
                f.BytecodeOffset = offset;
                foreach (Instruction i in f.Instructions)
                    offset += i.Size;
            }
            foreach (ClassInfo c in Classes)
            {
                foreach (MethodInfo m in c.Methods)
                {
                    if (m.SourceMethod == null)
                    {
                        m.BytecodeOffset = offset;
                        foreach (Instruction i in m.Instructions)
                            offset += i.Size;
                    }
                }
            }
            foreach (ClassInfo c in Classes)
            {
                foreach (MethodInfo m in c.Methods)
                {
                    if (m.SourceMethod != null)
                    {
                        m.BytecodeOffset = m.SourceMethod.BytecodeOffset;
                    }
                }
            }
        }

        private void InsertJumpStatic(List<Instruction> instructions)
        {
            for (int i = instructions.Count - 2; i >= 0; i--)
            {
                if (instructions[i + 1] is BCLInstruction jmp && jmp.Opcode == BCLOpcode.JumpRelative)
                {
                    if (instructions[i] is BCLInstruction addr && addr.Opcode == BCLOpcode.PushConstanti32)
                    {
                        JumpStaticInstruction jmpStatic = new JumpStaticInstruction(this, addr, jmp);
                        instructions.RemoveAt(i + 1);
                        instructions[i] = jmpStatic;
                    }
                    else
                    {
                        Console.WriteLine("[ERROR]: JumpRelative not preceeded by a PushConstanti32!");
                    }
                }
                    
            }

        }

        /// <summary>
        /// Replaces pairs of PushConstanti32 and JumpRelative instructions with the abstract JumpStatic instruction.
        /// </summary>
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
                    if (method.SourceMethod == null)
                        InsertJumpStatic(method.Instructions);
                }
            }
        }

        private void RemoveJumpStatic(List<Instruction> instructions)
        {
            for (int i = instructions.Count - 1; i >= 0; i--)
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

        /// <summary>
        /// Replaces abstract JumpStatic instructions with pairs of PushConstanti32 and JumpRelative instructions.
        /// </summary>
        public void TransformRemoveJumpStatic()
        {
            UpdateBytecodeLayout();
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
