using System;

namespace Day11
{
    class Program
    {
        static void Main(string[] args)
        {
            var grid = new int[300, 300];
            FillGrid(grid, 6392);

            int topLeftX, topLeftY, squareSize;

            FindMaxSquaresTopLeftElement(grid, out topLeftX, out topLeftY, out squareSize);

            // Part 2
            Console.WriteLine($"Top left coordinate of the square with the max value: ({topLeftX},{topLeftY}) in a square with size: {squareSize}");

            Console.ReadLine();
        }


        static void FillGrid(int[,] grid, int gridNumber)
        {
            int xLength = grid.GetLength(0);
            int yLength = grid.GetLength(1);

            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    grid[x, y] = CalculateFuelCellPowerLevel(x, y, gridNumber);
                }
            }
        }

        static int CalculateFuelCellPowerLevel(int x, int y, int gridNumber)
        {
            int rackId = x + 10;
            int tempRes = (rackId * y + gridNumber) * rackId;
            int thirdDigit = tempRes / 100 % 10;

            return thirdDigit - 5;
        }

        static void FindMaxSquaresTopLeftElement(int[,] grid, out int xOut, out int yOut, out int squareSizeOut)
        {
            int xLength = grid.GetLength(0);
            int yLength = grid.GetLength(1);

            int maxValue = Int32.MinValue;
            int maxTopLeftX = -1;
            int maxTopLeftY = -1;
            int maxSquareSize = -1;

            for (int squareSize = 0; squareSize < xLength; squareSize++)
            {
                for (int x = 0; squareSize + x < xLength; x++)
                {
                    for (int y = 0; squareSize + y < yLength; y++)
                    {
                        int value = GetSquareValue(grid, x, y, squareSize);
                        if (value > maxValue)
                        {
                            maxValue = value;
                            maxTopLeftX = x;
                            maxTopLeftY = y;
                            maxSquareSize = squareSize;
                        }
                    }
                }
            }

            xOut = maxTopLeftX;
            yOut = maxTopLeftY;
            squareSizeOut = maxSquareSize;
        }

        static int GetSquareValue(int[,] grid, int topLeftXCoordinate, int topLeftYCoordinate, int squareSize)
        {
            int value = 0;

            for (int x = topLeftXCoordinate; x < topLeftXCoordinate + squareSize; x++)
            {
                for (int y = topLeftYCoordinate; y < topLeftYCoordinate + squareSize; y++)
                {
                    value += grid[x, y];
                }
            }

            return value;
        }
    }
}
