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

            var applicableOPCodesOfSamples = new Dictionary<Sample, IList<Type>>();

            HashSet<Type> remainingCodeIds = new HashSet<Type>()
            {
                typeof(Addr), typeof(Addi),
                typeof(Mulr), typeof(Muli),
                typeof(Banr), typeof(Bani),
                typeof(Borr), typeof(Bori),
                typeof(Setr), typeof(Seti),
                typeof(Gtir), typeof(Gtri), typeof(Gtrr),
                typeof(Eqir), typeof(Eqri), typeof(Eqrr)
            };

            IDictionary<IOPCode, int> opcodeIds = new Dictionary<IOPCode, int>();

            foreach (var sample in _samples)
            {
                applicableOPCodesOfSamples.Add(sample, new List<Type>());

                foreach (var opcode in _opCodes)
                {
                    if (opcode.CanBehave(sample))
                        applicableOPCodesOfSamples[sample].Add(opcode.GetType());
                }

                //if(applicableOPCodesOfSamples[sample].Count == 1)
                //{
                //    IOPCode opcode = applicableOPCodesOfSamples[sample].First();

                //    if(remainingCodeIds.Contains(opcode.GetType()))
                //    {
                //        opcodeIds.Add(applicableOPCodesOfSamples[sample].First(), sample.Item2[0]);
                //        remainingCodeIds.Remove(opcode.GetType());
                //    }
                //}
            }

            IEnumerable<IGrouping<int, KeyValuePair<Sample, IList<Type>>>> samplesByOpcodes = applicableOPCodesOfSamples.GroupBy(kv => kv.Key.OpCode);

            var codeCandidates = new Dictionary<int, List<Type>>();
            foreach (var sampleGroup in samplesByOpcodes)
            {
                var candidates = new List<Type>()
                {
                    typeof(Addr), typeof(Addi),
                    typeof(Mulr), typeof(Muli),
                    typeof(Banr), typeof(Bani),
                    typeof(Borr), typeof(Bori),
                    typeof(Setr), typeof(Seti),
                    typeof(Gtir), typeof(Gtri), typeof(Gtrr),
                    typeof(Eqir), typeof(Eqri), typeof(Eqrr)
                };

                foreach (var samplesAndCodes in sampleGroup)
                {
                    candidates = candidates.Intersect(samplesAndCodes.Value).ToList();
                }

                codeCandidates.Add(sampleGroup.Key, candidates);
            }

            //var samplesWith1applicableOPcodes = applicableOPCodesOfSamples.Where(kv => kv.Value.Count == 1);

            var samplesWith3applicableOPcodes = applicableOPCodesOfSamples.Where(kv => kv.Value.Count >= 3);

            Console.WriteLine($"Samples with 3 or more applicable OpCodes: {samplesWith3applicableOPcodes.Count()}");

            Console.ReadLine();
        }

        static void ParseInput(string[] lines)
        {
            for (int i = 1; i < lines.Length; i += 4)
            {
                _samples.Add(Sample.FromString(lines.Skip(i - 1).Take(3).ToArray()));
            }
        }

        static IList<IOPCode> _opCodes;

        static readonly List<Sample> _samples = new List<Sample>();
    }

    class Sample
    {
        private Sample(List<int> registerBefore, List<int> instruction, List<int> registerAfter)
        {
            RegisterBefore = registerBefore;
            RegisterAfter = registerAfter;
            Instruction = instruction;
            OpCode = instruction[0];
        }

        public static Sample FromString(string[] lines)
        {
            List<int> before = new List<int>()
                {
                    int.Parse(lines[0][9].ToString()),
                    int.Parse(lines[0][12].ToString()),
                    int.Parse(lines[0][15].ToString()),
                    int.Parse(lines[0][18].ToString()),
                };

            var instructionParts = lines[1].Split(' ');

            List<int> instructions = new List<int>()
                {
                    int.Parse(instructionParts[0].ToString()),
                    int.Parse(instructionParts[1].ToString()),
                    int.Parse(instructionParts[2].ToString()),
                    int.Parse(instructionParts[3].ToString()),
                };

            List<int> after = new List<int>()
                {
                    int.Parse(lines[2][9].ToString()),
                    int.Parse(lines[2][12].ToString()),
                    int.Parse(lines[2][15].ToString()),
                    int.Parse(lines[2][18].ToString()),
                };

            return new Sample(before, instructions, after);
        }

        public List<int> RegisterBefore { get; }
        public List<int> Instruction { get; }
        public List<int> RegisterAfter { get; }

        public int OpCode { get; set; }
    }

    interface IOPCode
    {
        bool CanBehave(Sample sample);

        //void Execute(List<int> register, List<int> instruction);

        //public int Id { get; set; }
    }

    class Addr : IOPCode
    {
        public bool CanBehave(Sample sample)
        {
            return sample.RegisterAfter[sample.Instruction[3]] == sample.RegisterBefore[sample.Instruction[1]] + sample.RegisterBefore[sample.Instruction[2]];
        }
    }

    class Addi : IOPCode
    {
        public bool CanBehave(Sample sample)
        {
            return sample.RegisterAfter[sample.Instruction[3]] == sample.RegisterBefore[sample.Instruction[1]] + sample.Instruction[2];
        }
    }

    class Mulr : IOPCode
    {
        public bool CanBehave(Sample sample)
        {
            return sample.RegisterAfter[sample.Instruction[3]] == sample.RegisterBefore[sample.Instruction[1]] * sample.RegisterBefore[sample.Instruction[2]];
        }
    }

    class Muli : IOPCode
    {
        public bool CanBehave(Sample sample)
        {
            return sample.RegisterAfter[sample.Instruction[3]] == sample.RegisterBefore[sample.Instruction[1]] * sample.Instruction[2];
        }
    }

    class Banr : IOPCode
    {
        public bool CanBehave(Sample sample)
        {
            return sample.RegisterAfter[sample.Instruction[3]] == (sample.RegisterBefore[sample.Instruction[1]] & sample.RegisterBefore[sample.Instruction[2]]);
        }
    }

    class Bani : IOPCode
    {
        public bool CanBehave(Sample sample)
        {
            return sample.RegisterAfter[sample.Instruction[3]] == (sample.RegisterBefore[sample.Instruction[1]] & sample.Instruction[2]);
        }
    }

    class Borr : IOPCode
    {
        public bool CanBehave(Sample sample)
        {
            return sample.RegisterAfter[sample.Instruction[3]] == (sample.RegisterBefore[sample.Instruction[1]] | sample.RegisterBefore[sample.Instruction[2]]);
        }
    }

    class Bori : IOPCode
    {
        public bool CanBehave(Sample sample)
        {
            return sample.RegisterAfter[sample.Instruction[3]] == (sample.RegisterBefore[sample.Instruction[1]] | sample.Instruction[2]);
        }
    }

    class Setr : IOPCode
    {
        public bool CanBehave(Sample sample)
        {
            return sample.RegisterAfter[sample.Instruction[3]] == sample.RegisterBefore[sample.Instruction[1]];
        }
    }

    class Seti : IOPCode
    {
        public bool CanBehave(Sample sample)
        {
            return sample.RegisterAfter[sample.Instruction[3]] == sample.Instruction[1];
        }
    }

    class Gtir : IOPCode
    {
        public bool CanBehave(Sample sample)
        {
            var AgreaterThanB = sample.Instruction[1] > sample.RegisterBefore[sample.Instruction[2]];

            return (sample.RegisterAfter[sample.Instruction[3]] == 1 && AgreaterThanB) || (sample.RegisterAfter[sample.Instruction[3]] == 0 && !AgreaterThanB);
        }
    }

    class Gtri : IOPCode
    {
        public bool CanBehave(Sample sample)
        {
            var AgreaterThanB = sample.RegisterBefore[sample.Instruction[1]] > sample.Instruction[2];

            return (sample.RegisterAfter[sample.Instruction[3]] == 1 && AgreaterThanB) || (sample.RegisterAfter[sample.Instruction[3]] == 0 && !AgreaterThanB);
        }
    }

    class Gtrr : IOPCode
    {
        public bool CanBehave(Sample sample)
        {
            var AgreaterThanB = sample.RegisterBefore[sample.Instruction[1]] > sample.RegisterBefore[sample.Instruction[2]];

            return (sample.RegisterAfter[sample.Instruction[3]] == 1 && AgreaterThanB) || (sample.RegisterAfter[sample.Instruction[3]] == 0 && !AgreaterThanB);
        }
    }

    class Eqir : IOPCode
    {
        public bool CanBehave(Sample sample)
        {
            var AEqualsB = sample.Instruction[1] == sample.RegisterBefore[sample.Instruction[2]];

            return (sample.RegisterAfter[sample.Instruction[3]] == 1 && AEqualsB) || (sample.RegisterAfter[sample.Instruction[3]] == 0 && !AEqualsB);
        }
    }

    class Eqri : IOPCode
    {
        public bool CanBehave(Sample sample)
        {
            var AEqualsB = sample.RegisterBefore[sample.Instruction[1]] == sample.Instruction[2];

            return (sample.RegisterAfter[sample.Instruction[3]] == 1 && AEqualsB) || (sample.RegisterAfter[sample.Instruction[3]] == 0 && !AEqualsB);
        }
    }

    class Eqrr : IOPCode
    {
        public bool CanBehave(Sample sample)
        {
            var AEqualsB = sample.RegisterBefore[sample.Instruction[1]] == sample.RegisterBefore[sample.Instruction[2]];

            return (sample.RegisterAfter[sample.Instruction[3]] == 1 && AEqualsB) || (sample.RegisterAfter[sample.Instruction[3]] == 0 && !AEqualsB);
        }
    }
}
