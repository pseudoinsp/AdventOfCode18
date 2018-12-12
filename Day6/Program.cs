using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Day6
{
    class Program
    {
        static void Main(string[] args)
        {
            Task<string[]> linesReadingTask = System.IO.File.ReadAllLinesAsync("..\\..\\..\\input.txt");

            var coordinates = new List<Point>();

            foreach (var line in linesReadingTask.Result)
            {
                string[] splitR = line.Split(',');

                coordinates.Add(new Point(int.Parse(splitR[0]), int.Parse(splitR[1].TrimStart())));
            }

            var boundingRectangle = CalculateBoundingRectangle(coordinates);

            var boundingCoordinates = 

            Console.ReadLine();
        }

        static Rectangle CalculateBoundingRectangle(List<Point> coordinates)
        {
            var xMin = coordinates.Min(c => c.X);
            var yMin = coordinates.Min(c => c.Y);
            var xMax = coordinates.Max(c => c.X);
            var yMax = coordinates.Max(c => c.Y);

            return new Rectangle(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        static List<Point> Bounding
    }


}
