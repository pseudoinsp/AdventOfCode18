using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Day16
{
    class Program
    {
        static void Main(string[] args)
        {
            Task<string[]> inputReadingTask = File.ReadAllLinesAsync("..\\..\\..\\input.txt");

            _opCodes = new List<IOPCode>()
            {
                new Addr(), new Addi(),
                new Mulr(), new Muli(),
                new Banr(), new Bani(),
                new Borr(), new Bori(),
                new Setr(), new Seti(),
                new Gtir(), new Gtri(), new Gtrr(),
                new Eqir(), new Eqri(), new Eqrr()
            };

            ParseInput(inputReadingTask.Result);

            var applicableOPCodesOfSamples = new Dictionary<(List<int>, List<int>, List<int>), IList<IOPCode>>();

            foreach (var sample in _samples)
            {
                applicableOPCodesOfSamples.Add(sample, new List<IOPCode>());

                foreach (var opcode in _opCodes)
                {
                    if (opcode.CanBehave(sample.Item1, sample.Item2, sample.Item3))
                        applicableOPCodesOfSamples[sample].Add(opcode);
                }
            }

            var samplesWith3applicableOPcodes = applicableOPCodesOfSamples.Where(kv => kv.Value.Count >= 3);

            Console.WriteLine($"Samples with 3 or more applicable OpCodes: {samplesWith3applicableOPcodes.Count()}");

            Console.ReadLine();
        }

        static void ParseInput(string[] lines)
        {
            for (int i = 0; i < lines.Length; i += 4)
            {
                List<int> before = new List<int>()
                {
                    int.Parse(lines[i][9].ToString()),
                    int.Parse(lines[i][12].ToString()),
                    int.Parse(lines[i][15].ToString()),
                    int.Parse(lines[i][18].ToString()),
                };

                var instructionParts = lines[i + 1].Split(' ');

                List<int> instructions = new List<int>()
                {
                    int.Parse(instructionParts[0].ToString()),
                    int.Parse(instructionParts[1].ToString()),
                    int.Parse(instructionParts[2].ToString()),
                    int.Parse(instructionParts[3].ToString()),
                };

                List<int> after = new List<int>()
                {
                    int.Parse(lines[i+2][9].ToString()),
                    int.Parse(lines[i+2][12].ToString()),
                    int.Parse(lines[i+2][15].ToString()),
                    int.Parse(lines[i+2][18].ToString()),
                };

                _samples.Add((before, instructions, after));
            }
        }

        static IList<IOPCode> _opCodes;

        static readonly List<(List<int>, List<int>, List<int>)> _samples = new List<(List<int>, List<int>, List<int>)>();
    }

    interface IOPCode
    {
        bool CanBehave(List<int> registerBefore, List<int> instruction, List<int> registerAfter);
    }

    class Addr : IOPCode
    {
        public bool CanBehave(List<int> registerBefore, List<int> instruction, List<int> registerAfter)
        {
            return registerAfter[instruction[3]] == registerBefore[instruction[1]] + registerBefore[instruction[2]];
        }
    }

    class Addi : IOPCode
    {
        public bool CanBehave(List<int> registerBefore, List<int> instruction, List<int> registerAfter)
        {
            return registerAfter[instruction[3]] == registerBefore[instruction[1]] + instruction[2];
        }
    }

    class Mulr : IOPCode
    {
        public bool CanBehave(List<int> registerBefore, List<int> instruction, List<int> registerAfter)
        {
            return registerAfter[instruction[3]] == registerBefore[instruction[1]] * registerBefore[instruction[2]];
        }
    }

    class Muli : IOPCode
    {
        public bool CanBehave(List<int> registerBefore, List<int> instruction, List<int> registerAfter)
        {
            return registerAfter[instruction[3]] == registerBefore[instruction[1]] * instruction[2];
        }
    }

    class Banr : IOPCode
    {
        public bool CanBehave(List<int> registerBefore, List<int> instruction, List<int> registerAfter)
        {
            return registerAfter[instruction[3]] == (registerBefore[instruction[1]] & registerBefore[instruction[2]]);
        }
    }

    class Bani : IOPCode
    {
        public bool CanBehave(List<int> registerBefore, List<int> instruction, List<int> registerAfter)
        {
            return registerAfter[instruction[3]] == (registerBefore[instruction[1]] & instruction[2]);
        }
    }

    class Borr : IOPCode
    {
        public bool CanBehave(List<int> registerBefore, List<int> instruction, List<int> registerAfter)
        {
            return registerAfter[instruction[3]] == (registerBefore[instruction[1]] | registerBefore[instruction[2]]);
        }
    }

    class Bori : IOPCode
    {
        public bool CanBehave(List<int> registerBefore, List<int> instruction, List<int> registerAfter)
        {
            return registerAfter[instruction[3]] == (registerBefore[instruction[1]] | instruction[2]);
        }
    }

    class Setr : IOPCode
    {
        public bool CanBehave(List<int> registerBefore, List<int> instruction, List<int> registerAfter)
        {
            return registerAfter[instruction[3]] == registerBefore[instruction[1]];
        }
    }

    class Seti : IOPCode
    {
        public bool CanBehave(List<int> registerBefore, List<int> instruction, List<int> registerAfter)
        {
            return registerAfter[instruction[3]] == instruction[1];
        }
    }

    class Gtir : IOPCode
    {
        public bool CanBehave(List<int> registerBefore, List<int> instruction, List<int> registerAfter)
        {
            var AgreaterThanB = instruction[1] > registerBefore[instruction[2]];

            return (registerAfter[instruction[3]] == 1 && AgreaterThanB) || (registerAfter[instruction[3]] == 0 && !AgreaterThanB);
        }
    }

    class Gtri : IOPCode
    {
        public bool CanBehave(List<int> registerBefore, List<int> instruction, List<int> registerAfter)
        {
            var AgreaterThanB = registerBefore[instruction[1]] > instruction[2];

            return (registerAfter[instruction[3]] == 1 && AgreaterThanB) || (registerAfter[instruction[3]] == 0 && !AgreaterThanB);
        }
    }

    class Gtrr : IOPCode
    {
        public bool CanBehave(List<int> registerBefore, List<int> instruction, List<int> registerAfter)
        {
            var AgreaterThanB = registerBefore[instruction[1]] > registerBefore[instruction[2]];

            return (registerAfter[instruction[3]] == 1 && AgreaterThanB) || (registerAfter[instruction[3]] == 0 && !AgreaterThanB);
        }
    }

    class Eqir : IOPCode
    {
        public bool CanBehave(List<int> registerBefore, List<int> instruction, List<int> registerAfter)
        {
            var AEqualsB = instruction[1] == registerBefore[instruction[2]];

            return (registerAfter[instruction[3]] == 1 && AEqualsB) || (registerAfter[instruction[3]] == 0 && !AEqualsB);
        }
    }

    class Eqri : IOPCode
    {
        public bool CanBehave(List<int> registerBefore, List<int> instruction, List<int> registerAfter)
        {
            var AEqualsB = registerBefore[instruction[1]] == instruction[2];

            return (registerAfter[instruction[3]] == 1 && AEqualsB) || (registerAfter[instruction[3]] == 0 && !AEqualsB);
        }
    }

    class Eqrr : IOPCode
    {
        public bool CanBehave(List<int> registerBefore, List<int> instruction, List<int> registerAfter)
        {
            var AEqualsB = registerBefore[instruction[1]] == registerBefore[instruction[2]];

            return (registerAfter[instruction[3]] == 1 && AEqualsB) || (registerAfter[instruction[3]] == 0 && !AEqualsB);
        }
    }
}
