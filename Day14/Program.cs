using System;
using System.Collections.Generic;
using System.Linq;

namespace Day14
{
    class Program
    {
        static int[] scoreSequenceToMatch = new int[6] { 8, 2, 5, 4, 0, 1 };
        static bool matchFound = false;

        static void Main(string[] args)
        {
            

            var recipeScores = new List<int>()
            {
                3,
                7
            };

            var selectedRecipeIndexes = new List<int>()
            {
                0,
                1
            };

            
            List<int> last6Digits = new List<int> { 3,7,0,0,0,0 };

            while (!matchFound)
            {
                int selectedRecipeScoresSum = 0;
                foreach (var index in selectedRecipeIndexes)
                {
                    selectedRecipeScoresSum += recipeScores[index];
                }

                // TODO debug this lol
                //int prevCount = recipeScores.Count;
                //while (selectedRecipeScoresSum > 0)
                //{
                //    recipeScores.Insert(prevCount, selectedRecipeScoresSum % 10);
                //    selectedRecipeScoresSum /= 10;
                //}

                if(selectedRecipeScoresSum > 9)
                {
                    recipeScores.Add(selectedRecipeScoresSum / 10);

                    //AddNewScoreToLastScores(recipeScores, selectedRecipeScoresSum / 10);
                    last6Digits = last6Digits.Skip(1).Take(5).ToList();
                    last6Digits.Add(selectedRecipeScoresSum / 10);
                    if (last6Digits.SequenceEqual(scoreSequenceToMatch))
                    {
                        matchFound = true;
                    }

                    recipeScores.Add(selectedRecipeScoresSum % 10);

                    //AddNewScoreToLastScores(recipeScores, selectedRecipeScoresSum % 10);
                    last6Digits = last6Digits.Skip(1).Take(5).ToList();
                    last6Digits.Add(selectedRecipeScoresSum % 10);
                    if (last6Digits.SequenceEqual(scoreSequenceToMatch))
                    {
                        matchFound = true;
                    }
                }
                else
                {
                    recipeScores.Add(selectedRecipeScoresSum);
                    //AddNewScoreToLastScores(recipeScores, selectedRecipeScoresSum);
                    last6Digits = last6Digits.Skip(1).Take(5).ToList();
                    last6Digits.Add(selectedRecipeScoresSum);
                    if (last6Digits.SequenceEqual(scoreSequenceToMatch))
                    {
                        matchFound = true;
                    }
                }

                for (int i = 0; i < selectedRecipeIndexes.Count; i++)
                {
                    int newIndex = CalculateNewSelectionIndex(recipeScores, selectedRecipeIndexes[i]);
                    selectedRecipeIndexes[i] = newIndex;
                }
            }

            int scoresToRemove = last6Digits.SequenceEqual(scoreSequenceToMatch) ? 6 : 7;
            Console.WriteLine($"Digit count before match: {recipeScores.Count - scoresToRemove}");

            Console.ReadKey();
        }

        static void AddNewScoreToLastScores(List<int> last6Digits, int newDigit)
        {
            last6Digits = last6Digits.Skip(1).Take(5).ToList();
            last6Digits.Add(newDigit);
            if (last6Digits.SequenceEqual(scoreSequenceToMatch))
            {
                matchFound = true;
            }
        }

        static int CalculateNewSelectionIndex(List<int> recipeScores, int currentSelectionIndex)
        {
            int currentSelectionValue = recipeScores[currentSelectionIndex];
            int recipeCount = recipeScores.Count;

            int indexToMoveRight = (1 + currentSelectionValue) % recipeCount;

            if (currentSelectionIndex + indexToMoveRight >= recipeCount)
                return 0 + currentSelectionIndex + indexToMoveRight - recipeCount;
            else
                return currentSelectionIndex + indexToMoveRight;
        }

        static int GetFirstDigit(int number)
        {
            if (number < 10)
            {
                return number;
            }
            return GetFirstDigit((number - (number % 10)) / 10);
        }
    }
}
