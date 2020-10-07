using System;
using System.Collections.Generic;
using System.Text;

namespace Zem80.Core.Instructions
{
    public class SCF : IMicrocode
    {
        public ExecutionResult Execute(Processor cpu, ExecutionPackage package)
        {
            Flags flags = cpu.Registers.Flags;
            flags.Carry = true;
            flags.HalfCarry = false;
            flags.Subtract = false;
            flags.X = (cpu.Registers.A & 0x08) > 0; // copy bit 3
            flags.Y = (cpu.Registers.A & 0x20) > 0; // copy bit 5
            return new ExecutionResult(package, flags);
        }

        public SCF()
        {
        }
    }
}
