using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.OSI
{
    public class JumpStaticInstruction : Instruction
    {
        private uint _size = 7; // 5bytes PushConstanti32, 2bytes JumpRelative
        public override uint Size => _size;

        public string FunctionName { get; set; } = null;
        public sbyte ArgumentCount { get; set; } = 0;

        public JumpStaticInstruction(string functionName, sbyte argumentCount)
        {
            this.FunctionName = functionName;
            this.ArgumentCount = argumentCount;
        }

        public JumpStaticInstruction(OSIFile osi, BCLInstruction addressInstruction, BCLInstruction jumpInstruction)
        {
            if (addressInstruction.Opcode != BCLOpcode.PushConstanti32 || jumpInstruction.Opcode != BCLOpcode.JumpRelative)
            {
                throw new ArgumentException("Must take a pair of PushConstanti32 and JumpRelative instructions.");
            }

            _size = addressInstruction.Size + jumpInstruction.Size;
            int targetAddress = addressInstruction.Arguments[0].GetValue<int>();
            ArgumentCount = jumpInstruction.Arguments[0].GetValue<sbyte>();
            foreach (OSIFile.FunctionInfo func in osi.Functions)
            {
                if (func.BytecodeOffset == targetAddress)
                {
                    FunctionName = func.Name;
                    break;
                }
            }
            if (FunctionName == null)
            {
                throw new ArgumentException("Could not find function for address 0x" + targetAddress.ToString("X8"));
            }
        }

        public override void Write(BinaryWriter writer)
        {
            throw new InvalidOperationException("Abstract instructions cannot be serialized. Transform them into BCL instructions first.");
        }

        public override string ToString()
        {
            return "JumpStatic " + FunctionName + ", " + ArgumentCount;
        }
    }
}
