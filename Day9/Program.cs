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
            var marblesInCircle = new LinkedList<Marble>();

            marblesInCircle.AddFirst(new Marble(0));
            marblesInCircle.AddLast(new Marble(1));
            
            var playerScores = new Dictionary<int, long>();
            for (int i = 0; i < playerNumber; i++)
            {
                playerScores[i] = 0;
            }

            bool gameEnd = false;
            int roundCount = 2;
            LinkedListNode<Marble> currentMarble = marblesInCircle.Last;

            var sw = Stopwatch.StartNew();
            while (!gameEnd)
            {
                // point score
                if (roundCount % 23 == 0)
                {
                    var additionalMarbleToTake = GetNthCounterClockwiseElement(currentMarble, 7);

                    if (additionalMarbleToTake.Next != null)
                        currentMarble = additionalMarbleToTake.Next;
                    else
                        currentMarble = marblesInCircle.First;

                    marblesInCircle.Remove(additionalMarbleToTake);

                    int currentPlayerIndex = roundCount % playerNumber;
                    playerScores[currentPlayerIndex] += additionalMarbleToTake.Value.Value + roundCount;
                }
                else // standard placing
                {
                    var newMarbleToAdd = new Marble(roundCount);

                    if (currentMarble.Next == null)
                        currentMarble = marblesInCircle.AddAfter(marblesInCircle.First, newMarbleToAdd);
                    else
                        currentMarble = marblesInCircle.AddAfter(currentMarble.Next, newMarbleToAdd);
                }

                if (roundCount == lastMarbleValue)
                    gameEnd = true;

                roundCount++;
            }
            sw.Stop();

            var playerWithMaxPoint = playerScores.Aggregate((l, r) => l.Value > r.Value ? l : r);
            Console.WriteLine($"Winner: player {playerWithMaxPoint.Key} with value {playerWithMaxPoint.Value}");
            Console.WriteLine($"Time: {sw.ElapsedMilliseconds} ms");
            Console.ReadKey();
        }

        static LinkedListNode<Marble> GetNthCounterClockwiseElement(LinkedListNode<Marble> from, int n)
        {
            var currentNode = from;

            for (int i = 0; i < n; i++)
            {
                currentNode = currentNode.Previous;

                if (currentNode == null)
                    currentNode = from.List.Last;
            }

            return currentNode;
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

