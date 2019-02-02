using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.OSI
{
    public enum BCLOpcode : byte
    {
        Nop = 0x00,
        DebugOn = 0x02,
        DebugOff = 0x03,
        LineNumber = 0x04,
        LineNumberAlt1 = 0x05,
        LineNumberAlt2 = 0x06,
        SetMemberValue = 0x10,
        GetMemberValue = 0x11,
        GetMemberFunction = 0x12,
        CreateObject = 0x13,
        MemberFunctionArgumentCheck = 0x14,
        SetThisMemberValue = 0x15,
        GetThisMemberValue = 0x16,
        GetThisMemberFunction = 0x17,
        GetMemberValueFromString = 0x18,
        GetMemberFunctionFromString = 0x19,
        SetMemberValueFromString = 0x1A,
        GetVariableValue = 0x21,
        SetVariableValue = 0x22,
        CreateStackVariables = 0x23,
        IncrementVariable = 0x24,
        DecrementVariable = 0x25,
        Pop = 0x30,
        PopN = 0x31,
        Swap = 0x32,
        Pull = 0x33,
        DupN = 0x34,
        Dup = 0x35,
        PushConstanti32 = 0x40,
        PushConstanti24 = 0x41,
        PushConstanti16 = 0x42,
        PushConstanti8 = 0x43,
        PushConstantf32 = 0x44,
        PushConstant0 = 0x45,
        PushConstantString = 0x46,
        PushNothing = 0x47,
        PushConstantColor8888 = 0x48,
        PushConstantColor5551 = 0x49,
        JumpRelative = 0x50,
        JumpAbsolute = 0x51,
        Return = 0x52,
        CompareAndBranchIfFalse = 0x53,
        BranchAlways = 0x54,
        EqualTo = 0x60,
        LessThan = 0x61,
        GreaterThan = 0x62,
        LessOrEqual = 0x63,
        GreaterOrEqual = 0x64,
        And = 0x6A,
        Or = 0x6B,
        Not = 0x6C,
        BitwiseAnd = 0x6D,
        BitwiseOr = 0x6E,
        BitwiseXor = 0x6F,
        Add = 0x70,
        Subtract = 0x71,
        Multiply = 0x72,
        Divide = 0x73,
        Power = 0x74,
        Modulus = 0x75,
        BitwiseNot = 0x76,
        ShiftLeft = 0x77,
        ShiftRight = 0x78,
        Increment = 0x7A,
        Decrement = 0x7B,
        GetGameVariable = 0x80,
        SetGameVariable = 0x81,
        CallGameFunction = 0x82,
        CallGameFunctionFromString = 0x83,
        CallGameFunctionDirect = 0x84,
        CreateArray = 0x90,
        GetArrayValue = 0x91,
        ElementsInArray = 0x92,
        SetArrayValue = 0x93,
        AppendToArray = 0x94,
        RemoveFromArray = 0x95,
        InsertIntoArray = 0x96,
        SetRedValue = 0xA0,
        SetGreenValue = 0xA1,
        SetBlueValue = 0xA2,
        SetAlphaValue = 0xA3,
        GetRedValue = 0xA4,
        GetGreenValue = 0xA5,
        GetBlueValue = 0xA6,
        GetAlphaValue = 0xA7,
        ConvertToString = 0xB0,
        ConvertToFloat = 0xB1,
        ConvertToInteger = 0xB2,
        IsInteger = 0xB8,
        IsFloat = 0xB9,
        IsString = 0xBA,
        IsAnObject = 0xBB,
        IsGameObject = 0xBC,
        IsArray = 0xBD,
        GetObjectClassID = 0xBF,
        Halt = 0xFF,
    }

    public class BCLInstruction : OSIInstruction
    {
        public BCLOpcode Opcode { get; }

        public override uint Size
        {
            get
            {
                uint result = 1;
                foreach (OSIGenericInstructionArgument arg in this.Arguments)
                {
                    result += arg.Size;
                }
                return result;
            }
        }

        public BCLInstruction(BCLOpcode opcode, params object[] arguments)
        {
            this.Opcode = opcode;
            if (opcode == BCLOpcode.LineNumber) InitArguments("i16u i16u");
            else if (opcode == BCLOpcode.LineNumberAlt1) InitArguments("i16u i16u");
            else if (opcode == BCLOpcode.LineNumberAlt2) InitArguments("i16u i16u");
            else if (opcode == BCLOpcode.SetMemberValue) InitArguments("i16u");
            else if (opcode == BCLOpcode.GetMemberValue) InitArguments("i16u");
            else if (opcode == BCLOpcode.GetMemberFunction) InitArguments("i16u");
            else if (opcode == BCLOpcode.CreateObject) InitArguments("i16u");
            else if (opcode == BCLOpcode.MemberFunctionArgumentCheck) InitArguments("i8s");
            else if (opcode == BCLOpcode.SetThisMemberValue) InitArguments("i16u");
            else if (opcode == BCLOpcode.GetThisMemberValue) InitArguments("i16u");
            else if (opcode == BCLOpcode.GetThisMemberFunction) InitArguments("i16u");
            else if (opcode == BCLOpcode.GetVariableValue) InitArguments("i16u");
            else if (opcode == BCLOpcode.SetVariableValue) InitArguments("i16u");
            else if (opcode == BCLOpcode.CreateStackVariables) InitArguments("i8s");
            else if (opcode == BCLOpcode.IncrementVariable) InitArguments("i16u");
            else if (opcode == BCLOpcode.DecrementVariable) InitArguments("i16u");
            else if (opcode == BCLOpcode.PopN) InitArguments("i8s");
            else if (opcode == BCLOpcode.Pull) InitArguments("i8s");
            else if (opcode == BCLOpcode.DupN) InitArguments("i8s");
            else if (opcode == BCLOpcode.PushConstanti32) InitArguments("i32s");
            else if (opcode == BCLOpcode.PushConstanti24) InitArguments("i24s");
            else if (opcode == BCLOpcode.PushConstanti16) InitArguments("i16s");
            else if (opcode == BCLOpcode.PushConstanti8) InitArguments("i8s");
            else if (opcode == BCLOpcode.PushConstantf32) InitArguments("f32");
            else if (opcode == BCLOpcode.PushConstantString) InitArguments("i16u");
            else if (opcode == BCLOpcode.PushConstantColor8888) InitArguments("i32u");
            else if (opcode == BCLOpcode.PushConstantColor5551) InitArguments("i16u");
            else if (opcode == BCLOpcode.JumpRelative) InitArguments("i8s");
            else if (opcode == BCLOpcode.JumpAbsolute) InitArguments("i8s");
            else if (opcode == BCLOpcode.CompareAndBranchIfFalse) InitArguments("i16s");
            else if (opcode == BCLOpcode.BranchAlways) InitArguments("i16s");
            else if (opcode == BCLOpcode.GetGameVariable) InitArguments("i16u i16u");
            else if (opcode == BCLOpcode.SetGameVariable) InitArguments("i16u i16u");
            else if (opcode == BCLOpcode.CallGameFunction) InitArguments("i16u i16u i8s");
            else if (opcode == BCLOpcode.CallGameFunctionFromString) InitArguments("i16u i8s");
            else if (opcode == BCLOpcode.CallGameFunctionDirect) InitArguments("i32s i8s");
            else this.Arguments = new List<OSIGenericInstructionArgument>();

            if (arguments.Length > this.Arguments.Count)
                throw new ArgumentException("Too many argument values!");

            for (int i = 0; i < arguments.Length; i++)
            {
                this.Arguments[i].Value = arguments[i];
            }
        }

        public BCLInstruction(BCLOpcode opcode, BinaryReader reader) : this(opcode)
        {
            foreach (OSIGenericInstructionArgument arg in this.Arguments)
            {
                arg.ReadValue(reader);
            }
        }

        private void InitArguments(string argString)
        {
            List<OSIGenericInstructionArgument> args = new List<OSIGenericInstructionArgument>();
            foreach (string arg in argString.Split(' '))
            {
                if (arg == "i8s")
                    args.Add(new OSIInstructionArgument<sbyte>(0));
                else if (arg == "i8u")
                    args.Add(new OSIInstructionArgument<byte>(0));
                else if (arg == "i16u")
                    args.Add(new OSIInstructionArgument<ushort>(0));
                else if (arg == "i16s")
                    args.Add(new OSIInstructionArgument<short>(0));
                else if (arg == "i32s")
                    args.Add(new OSIInstructionArgument<int>(0));
                else if (arg == "i32u")
                    args.Add(new OSIInstructionArgument<uint>(0));
                else if (arg == "f32")
                    args.Add(new OSIInstructionArgument<float>(0.0f));
                else
                    throw new ArgumentException("Invalid arg type string: '" + arg + "'!");
            }
            Arguments = args;
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write((byte)this.Opcode);
            foreach (OSIGenericInstructionArgument arg in this.Arguments)
            {
                arg.WriteValue(writer);
            }
        }

        public override string ToString()
        {
            return this.Opcode.ToString() + " " + String.Join(", ", this.Arguments);
        }
    }
}
