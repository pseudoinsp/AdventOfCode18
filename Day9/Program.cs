using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Day9
{
    class Program
    {
        static void Main(string[] args)
        {
            int playerNumber = 455;
            int lastMarbleValue = 7122300;
            var marblesInCircle = new List<Marble>(lastMarbleValue*(21/23))
            {
                new Marble(0),
                new Marble(1)
            };


            bool gameEnd = false;
            int roundCount = 2;
            int currentMarbleIndex = 1;


            var playerScores = new Dictionary<int, int>();

            for (int i = 0; i < playerNumber; i++)
            {
                playerScores[i] = 0;
            }

            while (!gameEnd)
            {
                var newMarbleToAdd = new Marble(roundCount);
                int currentPlayerIndex = roundCount % playerNumber;

                // point score
                if (roundCount % 23 == 0)
                {
                    int additionalMarbleToTakeIndex;
                    if (currentMarbleIndex > 7)
                    {
                        additionalMarbleToTakeIndex = currentMarbleIndex - 7;
                    }
                    else
                    {
                        additionalMarbleToTakeIndex = marblesInCircle.Count - 7 + currentMarbleIndex;
                    }

                    var additionalMarbleToTake = marblesInCircle[additionalMarbleToTakeIndex];

                    marblesInCircle.RemoveAt(additionalMarbleToTakeIndex);

                    playerScores[currentPlayerIndex] += additionalMarbleToTake.Value + roundCount;

                    // not index + 1 since the clockwise neighbour takes the removed marble's place
                    //currentMarbleIndex = additionalMarbleToTakeIndex + 1 == marblesInCircle.Count ? 0 : additionalMarbleToTakeIndex;
                    currentMarbleIndex = additionalMarbleToTakeIndex;
                }
                else // standard placing
                {
                    int positionOfNewMarble;

                    if (currentMarbleIndex + 2 == marblesInCircle.Count)
                        positionOfNewMarble = marblesInCircle.Count;
                    else
                        positionOfNewMarble = (currentMarbleIndex + 2) % marblesInCircle.Count;

                    marblesInCircle.Insert(positionOfNewMarble, newMarbleToAdd);
                    currentMarbleIndex = positionOfNewMarble;
                }

                if (roundCount == lastMarbleValue)
                    gameEnd = true;

                roundCount++;
            }

            var playerWithMaxPoint = playerScores.Aggregate((l, r) => l.Value > r.Value ? l : r);
            Console.WriteLine($"Winner: player {playerWithMaxPoint.Key} with value {playerWithMaxPoint.Value}");
            Console.ReadKey();
        }
    }

    [DebuggerDisplay("Id: {Id}")]
    class Marble
    {
        public Marble(int id)
        {
            Id = id;
        }

        public int Id { get; }

        public int Value => Id;
    }
}

