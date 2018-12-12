using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace AdventOfCode18
{
    class Program
    {
        static void Main(string[] args)
        {
            Task<string[]> linesReadingTask = System.IO.File.ReadAllLinesAsync("..\\..\\..\\input.txt");

            int temperature = 0;

            string[] lines = linesReadingTask.Result;

            foreach (string line in lines)
            {
                if (line[0] == '+')
                {
                    int value = int.Parse(line.Substring(1));
                    temperature += value;
                }
                else
                {
                    temperature -= int.Parse(line.Substring(1));
                }
            }

            //Part1
            Console.WriteLine($"Ending temperature: {temperature}");

            //Part2
            Console.WriteLine($"First duplicate temperature: {FirstDuplicateTemperature(lines)}");

            
            Console.ReadLine();
        }

        public static int FirstDuplicateTemperature(string[] lines)
        {
            int temperature = 0;
            HashSet<int> historicTemperatures = new HashSet<int>();

            while(true)
            {
                foreach (string line in lines)
                {
                    if (line[0] == '+')
                    {
                        int value = int.Parse(line.Substring(1));
                        temperature += value;
                    }
                    else
                    {
                        temperature -= int.Parse(line.Substring(1));
                    }

                    if (historicTemperatures.Contains(temperature))
                    {
                        return temperature;
                    }
                    else
                    {
                        historicTemperatures.Add(temperature);
                    }
                }
            }
        }
    }
}

