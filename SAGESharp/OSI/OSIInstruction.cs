using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGESharp.OSI
{
    public interface OSIGenericInstructionArgument
    {
        Type Type { get; }
        R GetValue<R>();
        void SetValue<R>(R value);
        object Value { get; set; }
        uint Size { get; }
        void ReadValue(BinaryReader reader);
        void WriteValue(BinaryWriter writer);
    }

    public class OSIInstructionArgument<T> : OSIGenericInstructionArgument
    {
        public Type Type { get; }
        private T _value;
        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (!Type.IsAssignableFrom(value.GetType()))
                    throw new ArgumentException("You cannot assign an instance of type '" + value.GetType().Name + "' to an argument of type '" + Type.Name + "'.");
                _value = (T)value;
            }
        }

        public uint Size
        {
            get
            {
                if (Type == typeof(byte) || Type == typeof(sbyte))
                {
                    return 1;
                }
                else if (Type == typeof(short) || Type == typeof(ushort))
                {
                    return 2;
                }
                else if (Type == typeof(int) || Type == typeof(uint) || Type == typeof(float))
                {
                    return 4;
                }
                else if (Type == typeof(string))
                {
                    return 0; // HACK: All virtual instructions have size zero, and string arguments are only present in virtual instructions. 
                    // There is no canonical way to write string arguments into instructions because it isn't done in real BCL instructions.
                }
                else
                {
                    throw new InvalidOperationException("Invalid argument type '" + typeof(T).Name + "'!");
                }
            }
        }

        public OSIInstructionArgument(T value)
        {
            this.Type = typeof(T);
            _value = value;
        }

        public void ReadValue(BinaryReader reader)
        {
            if (Type == typeof(byte))
                Value = reader.ReadByte();
            else if (Type == typeof(sbyte))
                Value = reader.ReadSByte();
            else if (Type == typeof(short))
                Value = reader.ReadInt16();
            else if (Type == typeof(ushort))
                Value = reader.ReadUInt16();
            else if (Type == typeof(int))
                Value = reader.ReadInt32();
            else if (Type == typeof(uint))
                Value = reader.ReadUInt32();
            else if (Type == typeof(float))
                Value = reader.ReadSingle();
        }

        public void WriteValue(BinaryWriter writer)
        {
            if (Type == typeof(byte))
                writer.Write(GetValue<byte>());
            else if (Type == typeof(sbyte))
                writer.Write(GetValue<sbyte>());
            else if (Type == typeof(short))
                writer.Write(GetValue<short>());
            else if (Type == typeof(ushort))
                writer.Write(GetValue<ushort>());
            else if (Type == typeof(int))
                writer.Write(GetValue<int>());
            else if (Type == typeof(uint))
                writer.Write(GetValue<uint>());
            else if (Type == typeof(float))
                writer.Write(GetValue<float>());
        }

        public R GetValue<R>()
        {
            if (typeof(R) != Type)
                throw new ArgumentException("This instruction argument is not of type '" + typeof(R).Name + "'.");
            else
                return (R)Value;
        }

        public void SetValue<R>(R value)
        {
            if (typeof(R) != Type)
                throw new ArgumentException("This instruction argument is not of type '" + typeof(R).Name + "'.");
            else
                Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public abstract class OSIInstruction
    {
        public abstract uint Size { get; }
        public IReadOnlyList<OSIGenericInstructionArgument> Arguments { get; protected set; }
        public abstract void Write(System.IO.BinaryWriter writer);
    }
}
