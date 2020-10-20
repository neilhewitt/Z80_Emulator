﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Globalization;

namespace Zem80.Core.Instructions
{
    public class Instruction
    {
        public InstructionPrefix Prefix { get; private set; }
        public byte Opcode { get; private set; }
        public string FullOpcode { get; private set; }
        public string Mnemonic { get; private set; }
        public Condition Condition { get; private set; }
        public byte SizeInBytes { get; private set; }
        public IReadOnlyList<MachineCycle> Timing { get; private set; }
        public TimingExceptions TimingExceptions { get; private set; }
        public bool IsIndexed => Prefix >= InstructionPrefix.DD && Prefix <= InstructionPrefix.FDCB;
        public bool IsConditional => Condition != Condition.None;
        public IMicrocode Microcode { get; private set; }
        public InstructionElement Target { get; private set; }
        public InstructionElement Source { get; private set; }
        public InstructionElement Argument1 { get; private set; }
        public InstructionElement Argument2 { get; private set; }
        public bool TargetsByteRegister => Target >= InstructionElement.A && Target <= InstructionElement.IYl;
        public bool TargetsWordRegister => Target >= InstructionElement.AF && Target <= InstructionElement.SP;
        public bool TargetsByteInMemory => (Target >= InstructionElement.AddressFromHL && Target <= InstructionElement.AddressFromIYAndOffset);
        public ByteRegister? CopyResultTo { get; private set; }

        public Instruction(string fullOpcode, string mnemonic, Condition condition, InstructionElement target, InstructionElement source, InstructionElement arg1, InstructionElement arg2, byte sizeInBytes, MachineCycle[] machineCycles, ByteRegister? copyResultTo = ByteRegister.None, IMicrocode microcode = null)
        {
            FullOpcode = fullOpcode;
            CopyResultTo = copyResultTo;

            Prefix = fullOpcode.Length == 2 ? InstructionPrefix.Unprefixed : (InstructionPrefix)Enum.Parse(typeof(InstructionPrefix), fullOpcode[..^2], true);
            Opcode = byte.Parse(fullOpcode[^2..], NumberStyles.HexNumber);
            
            Mnemonic = mnemonic;
            SizeInBytes = sizeInBytes;
            Target = target;
            Source = source;
            Argument1 = arg1;
            Argument2 = arg2;
            Condition = condition;

            // this is expensive, but only done once at startup; binds the Instruction directly to the method instance implementing it
            if (microcode == null)
            {
                Type microcodeType = Assembly.GetExecutingAssembly().GetTypes().SingleOrDefault(x => x.Name == mnemonic.Split(' ')[0]);
                Microcode = (IMicrocode)Activator.CreateInstance(microcodeType);
            }
            else
            {
                Microcode = microcode;
            }

            // deal with timing + any exceptions
            Timing = new List<MachineCycle>(machineCycles);
            bool odh4 = false, mr4 = false, mw5 = false;
            if (Microcode is CALL)
            {
                // specifically for CALL instructions, the high byte operand read is 4 clock cycles rather than 3 *if* the condition is true (or there is no condition)
                odh4 = true;
            }
            if (Timing.Any(x => x.Type == MachineCycleType.MemoryRead && x.TStates == 4)) mr4 = true;
            if (Timing.Any(x => x.Type == MachineCycleType.MemoryWrite && x.TStates == 5)) mw5 = true;
            TimingExceptions = new TimingExceptions(odh4, mr4, mw5);
        }
    }
}
