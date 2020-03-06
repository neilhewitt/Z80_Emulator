using System;
using System.Collections.Generic;
using System.Text;

namespace Z80.Core
{
    public class DEC : IMicrocode
    {
        public ExecutionResult Execute(Processor cpu, ExecutionPackage package)
        {
            Instruction instruction = package.Instruction;
            InstructionData data = package.Data;
            IRegisters r = cpu.Registers;
            byte offset = data.Argument1;
            Flags flags = cpu.Registers.Flags;

            ushort decw(ushort value)
            {
                return (ushort)(value - 1);
            }

            byte dec(byte value)
            {
                int result = value - 1;
                sbyte subtract = -1;
                flags = FlagLookup.FlagsFromArithmeticOperation(value, (byte)subtract, false, true);
                return (byte)result;
            }

            switch (instruction.Prefix)
            {
                case InstructionPrefix.Unprefixed:
                    switch (instruction.Opcode)
                    {
                        case 0x05: // DEC B
                            r.B = dec(r.B);
                            break;
                        case 0x0B: // DEC BC
                            r.BC = decw(r.BC);
                            break;
                        case 0x0D: // DEC C
                            r.C = dec(r.C);
                            break;
                        case 0x15: // DEC D
                            r.D = dec(r.D);
                            break;
                        case 0x1B: // DEC DE
                            r.DE = decw(r.DE);
                            break;
                        case 0x1D: // DEC E
                            r.E = dec(r.E);
                            break;
                        case 0x25: // DEC H
                            r.H = dec(r.H);
                            break;
                        case 0x2B: // DEC HL
                            r.HL = decw(r.HL);
                            break;
                        case 0x2D: // DEC L
                            r.L = dec(r.L);
                            break;
                        case 0x35: // DEC (HL)
                            cpu.Memory.WriteByteAt(r.HL, dec(cpu.Memory.ReadByteAt(r.HL)));
                            break;
                        case 0x3B: // DEC SP
                            r.SP = decw(r.SP);
                            break;
                        case 0x3D: // DEC A
                            r.A = dec(r.A);
                            break;

                    }
                    break;

                case InstructionPrefix.DD:
                    switch (instruction.Opcode)
                    {
                        case 0x25: // DEC IXh
                            r.IXh = dec(r.IXh);
                            break;
                        case 0x2D: // DEC IXl
                            r.IXl = dec(r.IXl);
                            break;
                        case 0x2B: // DEC IX
                            r.IX = decw(r.IX);
                            break;
                        case 0x35: // DEC (IX+o)
                            cpu.Memory.WriteByteAt((ushort)(r.IX + (sbyte)offset), dec(cpu.Memory.ReadByteAt((ushort)(r.IX + (sbyte)offset))));
                            break;

                    }
                    break;

                case InstructionPrefix.FD:
                    switch (instruction.Opcode)
                    {
                        case 0x25: // DEC IYh
                            r.IYh = dec(r.IYh);
                            break;
                        case 0x2D: // DEC IYl
                            r.IYl = dec(r.IYl);
                            break;
                        case 0x2B: // DEC IY
                            r.IY = decw(r.IY);
                            break;
                        case 0x35: // DEC (IY+o)
                            cpu.Memory.WriteByteAt((ushort)(r.IY + (sbyte)offset), dec(cpu.Memory.ReadByteAt((ushort)(r.IY + (sbyte)offset))));
                            break;
                    }
                    break;
            }

            return new ExecutionResult(package, flags, false);
        }

        public DEC()
        {
        }
    }
}
