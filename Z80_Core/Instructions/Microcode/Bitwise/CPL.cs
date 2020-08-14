using System;
using System.Collections.Generic;
using System.Text;

namespace Z80.Core
{
    public class CPL : IMicrocode
    {
        public ExecutionResult Execute(Processor cpu, ExecutionPackage package)
        {
            Flags flags = cpu.Registers.Flags;
            flags.HalfCarry = true;
            flags.Subtract = true;
            cpu.Registers.A = (byte)(~cpu.Registers.A);
            flags.X = (cpu.Registers.A & 0x08) > 0; // copy bit 3
            flags.Y = (cpu.Registers.A & 0x20) > 0; // copy bit 5

            return new ExecutionResult(package, flags);
        }

        public CPL()
        {
        }
    }
}
