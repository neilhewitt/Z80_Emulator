﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace ZexNext.Core
{
    public class TestSet
    {
        public string Name { get; private set; }
        public IReadOnlyList<Test> Tests { get; private set; }

        private IReadOnlyList<Test> Setup(string dataPath)
        {
            if (!File.Exists(dataPath))
            {
                throw new FileNotFoundException("Specified file could not be found at path \"" + dataPath + "\".");
            }
            else
            {
                List<Test> tests = new List<Test>();

                string testName = String.Empty;
                Test test = null;
                IEnumerable<string> file = File.ReadLines(dataPath);
                TestState before = null, after = null;
                foreach (string line in file)
                {
                    if (line.StartsWith("T:"))
                    {
                        // it's a section title
                        string[] testNameParts = line.Split('|');
                        testName = testNameParts[0].Substring(2);
                        byte mask = 0xFF;
                        if (testNameParts.Length > 1)
                        {
                            mask = byte.Parse(testNameParts[1], System.Globalization.NumberStyles.HexNumber);
                        }
                        test = new Test(testName, mask);
                        tests.Add(test);
                    }
                    else
                    {
                        string left = line.Substring(0, line.IndexOf(']') + 1);
                        string mid = line.Substring(left.Length + 1, line.IndexOf('[', left.Length + 1));
                        string right = line.Substring(line.IndexOf('[', left.Length + 1));

                        string mode = left.Substring(0, left.IndexOf(':'));
                        string mnemonic = left.Substring(3, (left.IndexOf('"', 4) - 3));
                        string opcode = left.Substring(mode.Length + mnemonic.Length + 4);

                        string[] registers = mid.Split(',');
                        string[] opcodeParts = opcode.Replace("]", null).Replace("[", null).Split(',');
                        string[] dataParts = right.Substring(1, right.Length - 2).Split(',');

                        byte[] opcodeBytes = new byte[opcodeParts.Length];
                        for (int i = 0; i < opcodeBytes.Length; i++)
                        {
                            opcodeBytes[i] = byte.Parse(opcodeParts[i], System.Globalization.NumberStyles.HexNumber);
                        }

                        byte[] dataBytes = new byte[dataParts.Length];
                        for (int i = 0; i < dataBytes.Length; i++)
                        {
                            dataBytes[i] = byte.Parse(dataParts[i], System.Globalization.NumberStyles.HexNumber);
                        }

                        TestState state = new TestState(
                            opcodeBytes,
                            mnemonic,
                            dataBytes,
                            ushort.Parse(registers[0], System.Globalization.NumberStyles.HexNumber),
                            ushort.Parse(registers[1], System.Globalization.NumberStyles.HexNumber),
                            ushort.Parse(registers[2], System.Globalization.NumberStyles.HexNumber),
                            ushort.Parse(registers[3], System.Globalization.NumberStyles.HexNumber),
                            ushort.Parse(registers[4], System.Globalization.NumberStyles.HexNumber),
                            ushort.Parse(registers[5], System.Globalization.NumberStyles.HexNumber),
                            ushort.Parse(registers[6], System.Globalization.NumberStyles.HexNumber),
                            ushort.Parse(registers[7], System.Globalization.NumberStyles.HexNumber)
                            );

                        if (mode == "I")
                        {
                            before = state;
                        }
                        else
                        {
                            after = state;
                        }

                        if (before != null && after != null)
                        {
                            test.Add(new TestCycle(test, before, after));
                            before = null;
                            after = null;
                        }
                    }
                }

                return tests;
            }
        }

        public TestSet(string name, string dataPath)
        {
            Name = name;
            Tests = Setup(dataPath);
        }
    }
}
