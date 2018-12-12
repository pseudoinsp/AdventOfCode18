using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Day2
{
    class Program
    {
        static void Main(string[] args)
        {
            Task<string[]> linesReadingTask = System.IO.File.ReadAllLinesAsync("..\\..\\..\\input.txt");

            int wordsWithDoubleLetters = 0;
            int wordsWithTripleLetters = 0;

            string[] lines = linesReadingTask.Result;

            foreach (string line in lines)
            {
                IDictionary<char, int> letterCounts = new Dictionary<char, int>();

                foreach (char letter in line)
                {
                    if(letterCounts.ContainsKey(letter))
                    {
                        letterCounts[letter]++;
                    }
                    else
                    {
                        letterCounts[letter] = 1;
                    }
                }

                bool doubleIncremented = false;
                bool tripleIncremented = false;

                foreach (int frequency in letterCounts.Values)
                {
                    if(frequency == 2 && !doubleIncremented)
                    {
                        wordsWithDoubleLetters++;
                        doubleIncremented = true;
                    }

                    if(frequency == 3 && !tripleIncremented)
                    {
                        wordsWithTripleLetters++;
                        tripleIncremented = true;
                    }
                }
            }

            // Part 1
            Console.WriteLine($"Double x triple frequencies: {wordsWithDoubleLetters * wordsWithTripleLetters}");

            // Part 2
            var commonLetters = CommonLettersInMostSimilarBoxIds(lines);
            Console.Write($"Most matches in an id pair: ");
            foreach (var letter in commonLetters)
            {
                Console.Write(letter);
            }

            Console.ReadLine();
        }

        public static List<char> CommonLettersInMostSimilarBoxIds(string[] boxIds)
        {
            List<char> matchesInLongestPair = new List<char>();

            // TODO reduce comparisionCount
            for (int i = 0; i < boxIds.Length - 1; i++)
            {
                for (int j = i+1; j < boxIds.Length; j++)
                {
                    var commonLetters = CommonLettersInBoxIdPairs(boxIds[i], boxIds[j]);
                    
                    if(commonLetters.Count > matchesInLongestPair.Count)
                    {
                        matchesInLongestPair = commonLetters;
                    }
                }
            }

            return matchesInLongestPair;
        }

        // TODO pass current record, abort if the common part cannot be larger
        public static List<char> CommonLettersInBoxIdPairs(string id1, string id2)
        {
            List<char> commonLetters = new List<char>();
            for (int i = 0; i < id1.Length; i++)
            {
                if(id1[i] == id2[i])
                {
                    commonLetters.Add(id1[i]);
                }
            }

            return commonLetters;
        }
    }
}
