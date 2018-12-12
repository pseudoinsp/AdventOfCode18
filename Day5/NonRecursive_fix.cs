using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Day5
{
    class NonRecursive_fix
    {
        static void Main_(string[] args)
        {
            Task<string> textReadingTask = System.IO.File.ReadAllTextAsync("..\\..\\..\\input.txt");

            string polymer = textReadingTask.Result;

            Console.WriteLine($"Original length: {polymer.Length}");

            var res = RemoveUnstableElements(polymer);
            Console.WriteLine($"Remaining units in the polymer: {res.Length}");
            Console.ReadLine();

        }

        /// <summary>
        /// Removes unstable elements next to each other
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveUnstableElements(string input)
        {

            for (int i = 0; i < input.Length - 1; i++)
            {
                if (input[i] != input[i + 1] &&
                   char.ToLower(input[i]) == char.ToLower(input[i + 1]))
                {
                    input = input.Remove(i, 2);

                    // to continue with the character before the unstable pair in the result string
                    if (i >= 2)
                    {
                        i -= 2;
                    }
                    else
                    {
                        i = 0;
                    }
                }
            }

            return input;
        }
    }
}




