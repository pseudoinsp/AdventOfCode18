using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Day18
{
    class Program
    {
        static void Main(string[] args)
        {
            Task<string[]> inputReadingTask = File.ReadAllLinesAsync("..\\..\\..\\input.txt");

            _rules = new List<Rule>()
            {
                new Rule(current => current == AreaTypes.OpenGround,
                         adjacents => adjacents.Count(a => a == AreaTypes.Trees) >= 3,
                         AreaTypes.Trees,
                         AreaTypes.OpenGround),
                new Rule(current => current == AreaTypes.Trees,
                         adjacents => adjacents.Count(a => a == AreaTypes.Lumberyard) >= 3,
                         AreaTypes.Lumberyard,
                         AreaTypes.Trees),
                new Rule(current => current == AreaTypes.Lumberyard,
                         adjacents => adjacents.Count(a => a == AreaTypes.Trees) >= 1 && adjacents.Count(a => a == AreaTypes.Lumberyard) >= 1,
                         AreaTypes.Lumberyard,
                         AreaTypes.OpenGround),
            };

            ParseInput(inputReadingTask.Result);

            int minute;
            Map previousMapState = new Map();
            var results = new List<int>();

            for (minute = 0; minute < 1000000000; minute++)
            {
                Stopwatch sw1 = Stopwatch.StartNew();
                previousMapState.Acres = new AreaTypes[50, 50];
                CopyMap(_map, previousMapState);
                sw1.Stop();
                //Console.WriteLine($"Array copy: {sw1.ElapsedMilliseconds}");

                Stopwatch sw2 = Stopwatch.StartNew();
                for (int x = 0; x < 50; x++)
                {
                    for (int y = 0; y < 50; y++)
                    {
                        var neighbours = GetNeighbours(previousMapState, x, y);
                        var ruleApplied = false;
                        foreach (var rule in _rules)
                        {
                            if (rule.IsApplicable(previousMapState.Acres[x, y]) && !ruleApplied)
                            {
                                _map.Acres[x, y] = rule.Apply(neighbours);
                                ruleApplied = true;
                            }
                        }
                    }
                }

                int lumberjardCount = 0;
                int treeCount = 0;

                for (int x = 0; x < _map.Acres.GetLength(0); x++)
                {
                    for (int y = 0; y < _map.Acres.GetLength(1); y++)
                    {
                        if (_map.Acres[x, y] == AreaTypes.Lumberyard)
                            lumberjardCount++;
                        else if (_map.Acres[x, y] == AreaTypes.Trees)
                            treeCount++;
                    }
                }

                var sum = lumberjardCount * treeCount;
               
                results.Add(sum);

                if(results.Count == 10)
                {
                    Console.WriteLine($"Part 1 - After 10: {sum}");
                }

                // try to search for a pattern
               if(results.Count > 1000)
                {
                    var frequentOccurences = results.GroupBy(r => r).Where(g => g.Count() > 3);

                    int loopStart = results.IndexOf(frequentOccurences.First().Select(occ => occ).First());

                    int loopLength = frequentOccurences.Count();

                    int indexInLoop = (1000000000 - 1 - loopStart) % loopLength;

                    Console.WriteLine($"Part 2 - Result of 1Bth loop: {results[loopStart + indexInLoop]}");

                    Console.ReadKey();
                }
            }
        }

        static AreaTypes[] GetNeighbours(Map map, int x, int y)
        {
            var neighbours = new AreaTypes[8];

            if (x > 0 && y > 0)
                neighbours[0] = map.Acres[x - 1, y - 1];
            if (y > 0)
                neighbours[1] = map.Acres[x, y - 1];
            if (x < 49 && y > 0)
                neighbours[2] = map.Acres[x + 1, y - 1];
            if (x > 0)
                neighbours[3] = map.Acres[x - 1, y];
            if (x < 49)
                neighbours[4] = map.Acres[x + 1, y];
            if (x > 0 && y < 49)
                neighbours[5] = map.Acres[x - 1, y + 1];
            if (y < 49)
                neighbours[6] = map.Acres[x, y + 1];
            if (x < 49 && y < 49)
                neighbours[7] = map.Acres[x + 1, y + 1];

            return neighbours;
        }

        static void ParseInput(string[] input)
        {
            for (int lineIndex = 0; lineIndex < input.Length; lineIndex++)
            {
                string line = input[lineIndex];
                for (int inlineIndex = 0; inlineIndex < line.Length; inlineIndex++)
                {
                    switch (line[inlineIndex])
                    {
                        case '.':
                            _map.Acres[lineIndex, inlineIndex] = AreaTypes.OpenGround;
                            break;
                        case '|':
                            _map.Acres[lineIndex, inlineIndex] = AreaTypes.Trees;
                            break;
                        case '#':
                            _map.Acres[lineIndex, inlineIndex] = AreaTypes.Lumberyard;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        static void PrintMap(int roundCount)
        {
            int minX = 0;
            int maxX = 50;
            int minY = 0;
            int maxY = 50;

            Console.WriteLine($"After {roundCount} minutes:");
            for (int x = minX; x < maxX; x++)
            {
                Console.WriteLine();
                for (int y = minY; y < maxY; y++)
                {
                    switch (_map.Acres[x, y])
                    {
                        case AreaTypes.None:
                            break;
                        case AreaTypes.OpenGround:
                            Console.Write('.');
                            break;
                        case AreaTypes.Trees:
                            Console.Write('|');
                            break;
                        case AreaTypes.Lumberyard:
                            Console.Write('#');
                            break;
                        default:
                            break;
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine();
        }

        static void CopyMap(Map from, Map to)
        {
            for (int x = 0; x < 50; x++)
            {
                for (int y = 0; y < 50; y++)
                {
                    to.Acres[x, y] = from.Acres[x, y];
                }
            }
        }

        static int loopStart = int.MinValue;
        static Map _map = new Map();
        static List<Rule> _rules;
    }

    class Rule
    {
        public Rule(Func<AreaTypes, bool> applicable, Func<AreaTypes[], bool> condition, AreaTypes yesResultOutput, AreaTypes noResultOutput)
        {
            Applicable = applicable;
            Condition = condition;
            YesResultOutput = yesResultOutput;
            NoResultOutput = noResultOutput;
        }

        public bool IsApplicable(AreaTypes current)
        {
            return Applicable(current);
        }

        public AreaTypes Apply(AreaTypes[] neighbours)
        {
            return Condition(neighbours) ? YesResultOutput : NoResultOutput;
        }

        private readonly Func<AreaTypes, bool> Applicable;
        private readonly Func<AreaTypes[], bool> Condition;
        private readonly AreaTypes YesResultOutput;
        private readonly AreaTypes NoResultOutput;
    }

    enum AreaTypes
    {
        None,
        OpenGround,
        Trees,
        Lumberyard
    }

    class Map
    {
        public AreaTypes[,] Acres = new AreaTypes[50, 50];
    }
}
