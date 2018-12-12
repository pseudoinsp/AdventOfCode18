using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Day5
{
    class Program
    {
        static void Main(string[] args)
        {
            Task<string> textReadingTask = System.IO.File.ReadAllTextAsync("..\\..\\..\\input.txt");

            string polymer = textReadingTask.Result;

            Console.WriteLine($"Original length: {polymer.Length}");

            // Part1
            Console.WriteLine($"Remaining units in the polymer: {RemoveUnstableElements(polymer).Length}");

            // Part2
            List<char> unitTypes = new List<char>();

            for (char i = 'a'; i <= 'z'; i++)
            {
                unitTypes.Add(i);
            }

            List<int> shortestPolymers = new List<int>();
            foreach (var type in unitTypes)
            {
                string input = RemoveTypeFromPolymer(polymer, type);

                string reactResult = RemoveUnstableElements(input);

                shortestPolymers.Add(reactResult.Length);
            }

            Console.WriteLine($"Shortest polymer after removing a specific unit type: {shortestPolymers.Min()}");


            Console.ReadLine();
        }

        public static string RemoveTypeFromPolymer(string polymer, char type)
        {
            for (int i = 0; i < polymer.Length; i++)
            {
                if(char.ToLower(polymer[i]) == type)
                {
                    polymer = polymer.Remove(i, 1);
                    i--;
                }
            }

            return polymer;
        }

        /// <summary>
        /// Removes unstable elements next to each other
        /// </summary>
        public static string RemoveUnstableElements(string input)
        {
            bool unstabilityFound = false;

            for (int i = 0; i < input.Length - 1; i++)
            {
                if (input[i] != input[i + 1] &&
                   char.ToLower(input[i]) == char.ToLower(input[i + 1]))
                {
                    input = input.Remove(i, 2);
                    unstabilityFound = true;

                    // to continue with the character next to the unstable pair in the result string
                    i--;
                }
            }

            string res = input;
            if (unstabilityFound)
            {
                res = RemoveUnstableElements(input);
            }

            return res;
        }
    }
}
