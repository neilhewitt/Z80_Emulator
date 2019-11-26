using System;
using System.Collections.Generic;
using System.Text;

namespace Z80.Core
{
    public class RETN : IInstructionImplementation
    {
        public ExecutionResult Execute(Processor cpu, InstructionPackage package)
        {
            cpu.Registers.PC = cpu.Memory.Stack.Pop();
            return new ExecutionResult(package, cpu.Registers.Flags, false, true);
        }

        public RETN()
        {
        }
    }
}
