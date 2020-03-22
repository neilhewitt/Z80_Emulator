using System;
using System.Collections.Generic;
using System.Text;

namespace Z80.Core
{
    public class OUT : IMicrocode
    {
        public ExecutionResult Execute(Processor cpu, ExecutionPackage package)
        {
            Instruction instruction = package.Instruction;
            InstructionData data = package.Data;
            Flags flags = cpu.Registers.Flags;
            IRegisters r = cpu.Registers;

            void @out(byte portNumber, RegisterByte addressRegister, RegisterByte dataRegister)
            {
                IPort port = cpu.Ports[portNumber];
                byte output = r[dataRegister];
                cpu.SetAddressBus(portNumber, r[addressRegister]);
                port.SignalWrite();
                cpu.SetDataBus(output);
                port.WriteByte(output);
            }

            if (instruction.Prefix == InstructionPrefix.Unprefixed)
            {
                @out(data.Argument1, RegisterByte.A, RegisterByte.A);
            }
            else
            {
                @out(r.C, RegisterByte.B, instruction.OperandRegister);
            }

            return new ExecutionResult(package, flags, false);
        }

        public OUT()
        {
        }
    }
}
