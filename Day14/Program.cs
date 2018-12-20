using System;
using System.Collections.Generic;
using System.Linq;

namespace Day14
{
    class Program
    {
        static void Main(string[] args)
        {
            int endRecipeNumber = 825401;

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

            while (recipeScores.Count <= endRecipeNumber + 10)
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
                    recipeScores.Add(selectedRecipeScoresSum % 10);
                }
                else
                {
                    recipeScores.Add(selectedRecipeScoresSum);
                }

                for (int i = 0; i < selectedRecipeIndexes.Count; i++)
                {
                    int newIndex = CalculateNewSelectionIndex(recipeScores, selectedRecipeIndexes[i]);
                    selectedRecipeIndexes[i] = newIndex;
                }
            }

            var last10Scores = recipeScores.Skip(endRecipeNumber).Take(10);

            Console.WriteLine("Last 10 scores: ");

            foreach (var score in last10Scores)
            {
                Console.Write(score);
            }

            Console.ReadKey();
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
    }
}
