using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day12
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputReadingTask = File.ReadAllLinesAsync("..\\..\\..\\input.txt");

            long generationsToSimulate = 10000;

            List<bool> firstGeneration;
            List<Rule> rules;
            List<int> values = new List<int>();

            ParseInput(inputReadingTask.Result, out firstGeneration, out rules);

            var currentGeneration = firstGeneration;
            
            for (int gen = 0; gen <= generationsToSimulate; gen++)
            {
                Debug.WriteLine($"Gen {gen}");
                if (gen == generationsToSimulate)
                    PrintGeneration(gen, currentGeneration);

                var nextGeneration = new List<bool>(firstGeneration.Count);
                for (int plantIndex = -2; plantIndex < currentGeneration.Count + 2; plantIndex++)
                {
                    bool nextGenerationValue = false;
                    foreach (var rule in rules)
                    {
                        if (!nextGenerationValue && rule.IsApplicable(currentGeneration, plantIndex))
                        {
                            nextGenerationValue = true;
                        }
                    }

                    nextGeneration.Add(nextGenerationValue);
                }

                int value = GetValue(gen, currentGeneration);
                values.Add(value);

                if(values.Count >2)
                    Console.WriteLine($"{values[values.Count - 1] - values[values.Count - 2]}");

                currentGeneration = nextGeneration;
            }

            Console.ReadKey();
        }

        static void ParseInput(string[] input, out List<bool> firstGeneration, out List<Rule> rules)
        {
            firstGeneration = new List<bool>();
            var firstGenLine = input[0].Split(' ')[2];
            foreach (var c in firstGenLine)
            {
                firstGeneration.Add(c == '#');
            }

            rules = new List<Rule>();
            for (int i = 2; i < input.Length; i++)
            {
                rules.Add(new Rule(input[i]));
            }
        }

        static void PrintGeneration(int genNumber, List<bool> plants)
        {
            var value = GetValue(genNumber, plants);

            Console.WriteLine($"Generation: {genNumber} - value: {value}");
            for (int i = 0; i < 20 - genNumber; i++)
            {
                Console.Write('.');
            }
            foreach (bool plant in plants)
            {
                Console.Write(plant ? '#' : '.');
            }
            for (int i = 0; i < 20 - genNumber; i++)
            {
                Console.Write('.');
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        static int GetValue(int genNumber, List<bool> plants)
        {
            int value = 0;
            int newElementsOnTheLeft = genNumber * 2;
            int leftValue = -1;
            for (int i = newElementsOnTheLeft - 1; i >= 0; i--)
            {
                if (plants[i])
                    value += leftValue;

                leftValue--;
            }
            for (int i = newElementsOnTheLeft; i < plants.Count; i++)
            {
                if (plants[i])
                    value += i - newElementsOnTheLeft;
            }

            return value;
        }

    }

    class Rule
    {
        public Rule(string ruleString)
        {
            var patternLeftSide = new List<bool>(5);

            for (int i = 0; i < 5; i++)
            {
                patternLeftSide.Add(ruleString[i] == '#');
            }

            var patternRightSide = ruleString.Last() == '#';

            _Pattern = new KeyValuePair<List<bool>, bool>(patternLeftSide, patternRightSide);
        }

        // TODO currently also returns false if applicable but value is false
        public bool IsApplicable(List<bool> plants, int plantIndex)
        {
            List<bool> comparableLeftSide;
            if (plantIndex == -2)
            {
                comparableLeftSide = new List<bool>() { false, false, false, false };
                comparableLeftSide.AddRange(plants.Take(1));
            }
            else if (plantIndex == -1)
            {
                comparableLeftSide = new List<bool>() { false, false, false };
                comparableLeftSide.AddRange(plants.Take(2));
            }
            else if (plantIndex == 0)
            {
                comparableLeftSide = new List<bool> { false, false };
                comparableLeftSide.AddRange(plants.Take(3));
            }
            else if (plantIndex == 1)
            {
                comparableLeftSide = new List<bool> { false };
                comparableLeftSide.AddRange(plants.Take(4));
            }
            else if (plantIndex == plants.Count - 2)
            {
                comparableLeftSide = new List<bool>(plants.Skip(plants.Count - 4).Take(4)) { false };
            }
            else if (plantIndex == plants.Count - 1)
            {
                comparableLeftSide = new List<bool>(plants.Skip(plants.Count - 3).Take(3)) { false, false };
            }
            else if (plantIndex == plants.Count)
            {
                comparableLeftSide = new List<bool>(plants.Skip(plants.Count - 2).Take(2)) { false, false, false };
            }
            else if (plantIndex == plants.Count + 1)
            {
                comparableLeftSide = new List<bool>(plants.Skip(plants.Count - 1).Take(1)) { false, false, false, false };
            }
            else
                comparableLeftSide = plants.Skip(plantIndex - 2).Take(5).ToList();

            if (_Pattern.Key.SequenceEqual(comparableLeftSide))
            {
                return _Pattern.Value;
            }

            return false;
        }

        private readonly KeyValuePair<List<bool>, bool> _Pattern;
    }
}

