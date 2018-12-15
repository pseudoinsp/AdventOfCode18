using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Day10
{
    class Program
    {
        static void Main(string[] args)
        {
            Task<string[]> linesReadingTask = System.IO.File.ReadAllLinesAsync("..\\..\\..\\input.txt");

            var coordinates = ParseInput(linesReadingTask.Result);

            int seconds = 0;
            while(true)
            {
                if (DetectMessageCandidate(coordinates))
                {
                    PrintMessageCandidate(coordinates, seconds);
                }

                seconds++;

                foreach (var coordinate in coordinates)
                {
                    coordinate.Move();
                }
            }
        }

        static List<Coordinate> ParseInput(string[] input)
        {
            var parsedCoordinates = new List<Coordinate>();

            foreach (var line in input)
            {
                var parts = line.Split(',');

                int startingX = int.Parse(parts[0].Substring(parts[0].IndexOf('<') + 1));
                int startingY = int.Parse(parts[1].Substring(0, parts[1].IndexOf('>')).TrimStart());

                int velocityX = int.Parse(parts[1].Substring(parts[1].Length - 2).TrimStart());
                int velocityY = int.Parse(parts[2].Substring(0, 3).TrimStart());

                parsedCoordinates.Add(new Coordinate(startingX, startingY, velocityX, velocityY));
            }

            return parsedCoordinates;
        }

        // TODO use unusually low boundrectangle size to detect!
        static bool DetectMessageCandidate(List<Coordinate> coordinates)
        {
            IEnumerable<IGrouping<int, int>> yCoordinatesGrouped = coordinates.Select(c => c.X).GroupBy(x => x);
            int candidateGroups = 0;

            foreach (var group in yCoordinatesGrouped)
            {
                if(group.Count() >= 8)
                {
                    candidateGroups++;
                }
            }

            return candidateGroups >= 12;
        }

        static void PrintMessageCandidate(List<Coordinate> coordinates, int second)
        {
            var boundingRectangle = CalculateBoundingRectangle(coordinates);

            Console.WriteLine();
            Console.WriteLine($"Second: {second}");

            for (int y = boundingRectangle.Top - 1; y < boundingRectangle.Bottom + 1; y++)
            {
                Console.WriteLine();
                for (int x = boundingRectangle.Left - 1; x < boundingRectangle.Right + 1; x++)
                {
                    if (coordinates.Any(c => c.X == x && c.Y == y))
                        Console.Write("#");
                    else
                        Console.Write('.');
                }
            }
        }

        static Rectangle CalculateBoundingRectangle(List<Coordinate> coordinates)
        {
            var xMin = coordinates.Min(c => c.X);
            var yMin = coordinates.Min(c => c.Y);
            var xMax = coordinates.Max(c => c.X);
            var yMax = coordinates.Max(c => c.Y);

            return new Rectangle(xMin, yMin, xMax - xMin, yMax - yMin);
        }
    }

    [DebuggerDisplay("Position: ({X},{Y}), Velocity: ({VelocityX},{VelocityY})")]
    class Coordinate
    {
        public Coordinate(int startingX, int startingY, int velocityX, int velocityY)
        {
            X = startingX;
            Y = startingY;
            VelocityX = velocityX;
            VelocityY = velocityY;
        }

        public int X { get; set; }

        public int Y { get; set; }

        public int VelocityX { get; }

        public int VelocityY { get; }

        public void Move()
        {
            X += VelocityX;
            Y += VelocityY;
        }
    }
}
