using System;
using System.Collections.Generic;
using System.Text;

namespace Z80.Core
{
    public class RRC : IMicrocode
    {
        public ExecutionResult Execute(Processor cpu, ExecutionPackage package)
        {
            Instruction instruction = package.Instruction;
            InstructionData data = package.Data;
            Flags flags = cpu.Registers.Flags;
            IRegisters r = cpu.Registers;
            sbyte offset = (sbyte)(data.Argument1);
            RegisterByte register = instruction.OperandRegister;

            byte original, shifted;
            if (register != RegisterByte.None)
            {
                original = r[register];
                shifted = (byte)(original >> 1);
                shifted = shifted.SetBit(7, original.GetBit(0));
                shifted = setFlags(original, shifted);
                r[register] = shifted;
            }
            else
            {
                ushort address = instruction.Prefix switch
                {
                    InstructionPrefix.CB => r.HL,
                    InstructionPrefix.DDCB => (ushort)(r.IX + offset),
                    InstructionPrefix.FDCB => (ushort)(r.IY + offset),
                    _ => (ushort)0xFFFF
                };
                original = cpu.Memory.ReadByteAt(address);
                shifted = (byte)(original >> 1);
                shifted = shifted.SetBit(7, original.GetBit(0));
                shifted = setFlags(original, shifted);
                cpu.Memory.WriteByteAt(address, shifted);
            }

            byte setFlags(byte original, byte shifted)
            {
                flags = FlagLookup.FlagsFromBitwiseOperation(original, BitwiseOperation.RotateRight);
                flags.HalfCarry = false;
                flags.Subtract = false;
                return shifted;
            }

            return new ExecutionResult(package, flags, false);
        }

        public RRC()
        {
        }
    }
}
