﻿using System.Drawing;

namespace Zem80.ZXSpectrumVM
{
    public class ColourValue
    {
        public string Name { get; private set; }
        public byte Bits { get; private set; }
        public Color Normal { get; private set; }
        public Color Bright { get; private set; }

        public override string ToString()
        {
            return Name;
        }

        public ColourValue(string name, byte bits, Color normal, Color bright)
        {
            Name = name;
            Bits = bits;
            Normal = normal;
            Bright = bright;
        }
    }
}
