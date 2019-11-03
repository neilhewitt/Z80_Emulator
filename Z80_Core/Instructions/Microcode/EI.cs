using System;
using System.Collections.Generic;
using System.Text;

namespace Z80.Core
{
    public class EI : IInstructionImplementation
    {
        public ExecutionResult Execute(Processor cpu, InstructionPackage package)
        {
            cpu.EnableInterrupts();
            return new ExecutionResult(package, new Flags(), false);
        }

        public EI()
        {
        }
    }
}
