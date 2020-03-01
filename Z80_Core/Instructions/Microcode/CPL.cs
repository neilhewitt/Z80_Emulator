using System;
using System.Collections.Generic;
using System.Text;

namespace Z80.Core
{
    public class CPL : IMicrocode
    {
        public ExecutionResult Execute(Processor cpu, InstructionPackage package)
        {
            Flags flags = cpu.Registers.Flags;
            flags.HalfCarry = true;
            flags.Subtract = true;
            cpu.Registers.A ^= cpu.Registers.A;

            return new ExecutionResult(package, flags, false);
        }

        public CPL()
        {
        }
    }
}
