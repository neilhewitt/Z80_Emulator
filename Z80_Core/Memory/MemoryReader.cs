﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Z80.Core
{
    public class MemoryReader
    {
        private MemoryMap _map;

        public byte ReadByteAt(uint address)
        {
            IMemory memory = _map.MemoryFor(address);
            return memory?.ReadByteAt(address) ?? 255; // default value if page is unallocated
        }

        public ushort ReadWordAt(uint address)
        {
            IMemory memory = _map.MemoryFor(address);
            return memory?.ReadWordAt(address) ?? 65535; // default value if page is unallocated
        }

        public MemoryReader(MemoryMap map)
        {
            _map = map;
        }
    }
}
