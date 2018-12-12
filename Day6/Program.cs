using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            var coordinates = new List<Coordinate>();

            foreach (var line in linesReadingTask.Result)
            {
                string[] splitR = line.Split(',');

                coordinates.Add(new Coordinate(int.Parse(splitR[0]), int.Parse(splitR[1].TrimStart())));
            }

            var boundingRectangle = CalculateBoundingRectangle(coordinates);

            var boundingCoordinates = CalculateBoundingCoordinates(boundingRectangle);

            var coordinatesWithInfiniteSpace = CoordinatesWithInfiniteSpace(coordinates, boundingCoordinates);

            var coordinateTerritories = CoordinateTerritoriesInBoundingRectangle(coordinates, boundingRectangle);

            var coordinateWithMaxFiniteTerritory = coordinateTerritories.Item1.OrderByDescending(kv => kv.Value).Where(kv => !coordinatesWithInfiniteSpace.Contains(kv.Key)).First();



            Console.WriteLine($"Coordinate with max finite territory: ({coordinateWithMaxFiniteTerritory.Key.X},{coordinateWithMaxFiniteTerritory.Key.Y})" +
                              $" with territory {coordinateWithMaxFiniteTerritory.Value}");

            // Task 2
            var safePlaces = coordinateTerritories.Item2.Where(kv => kv.Value < 10000);

            Console.ReadLine();
        }

        static Rectangle CalculateBoundingRectangle(List<Coordinate> coordinates)
        {
            var xMin = coordinates.Min(c => c.X);
            var yMin = coordinates.Min(c => c.Y);
            var xMax = coordinates.Max(c => c.X);
            var yMax = coordinates.Max(c => c.Y);

            return new Rectangle(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        static List<Coordinate> CalculateBoundingCoordinates(Rectangle boundingRectangle)
        {
            var boundingCoordinates = new List<Coordinate>();

            // horizontal points
            int underYMin = boundingRectangle.Bottom + 1;
            int overYMax = boundingRectangle.Top - 1;
            for (int i = boundingRectangle.Left - 1; i <= boundingRectangle.Right + 1; i++)
            {
                boundingCoordinates.Add(new Coordinate(i, underYMin));
                boundingCoordinates.Add(new Coordinate(i, overYMax));
            }

            // vertical points
            int leftOfXMin = boundingRectangle.Left - 1;
            int rightOfXMax = boundingRectangle.Right - 1;
            for (int i = boundingRectangle.Top + 1; i < boundingRectangle.Bottom - 1; i++)
            {
                boundingCoordinates.Add(new Coordinate(leftOfXMin, i));
                boundingCoordinates.Add(new Coordinate(rightOfXMax, i));
            }

            return boundingCoordinates;
        }

        static HashSet<Coordinate> CoordinatesWithInfiniteSpace(List<Coordinate> coordinates, List<Coordinate> boundingCoordinates)
        {
            var sw = Stopwatch.StartNew();
            var coordinatesWithInfiniteSpace = new HashSet<Coordinate>();

            foreach (var boundCoordinate in boundingCoordinates)
            {
                int minDistance = int.MaxValue;
                Coordinate closestCoordinate = null;

                // TODO narrow the possible coordinates
                foreach (var coordinate in coordinates)
                {
                    if(coordinate.DistanceTo(boundCoordinate) < minDistance)
                    {
                        minDistance = coordinate.DistanceTo(boundCoordinate);
                        closestCoordinate = coordinate;
                    }
                }

                coordinatesWithInfiniteSpace.Add(closestCoordinate);
            }

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);

            return coordinatesWithInfiniteSpace;
        }

        static (IDictionary<Coordinate, int>, IDictionary<Coordinate, int>)CoordinateTerritoriesInBoundingRectangle(List<Coordinate> coordinates, Rectangle boundingRectangle)
        {
            var sw = Stopwatch.StartNew();
            var coordinatesWithInfiniteSpace = new HashSet<Coordinate>();

            var coordinateTerritories = new Dictionary<Coordinate, int>();
            var locationSafeness = new Dictionary<Coordinate, int>();

            for (int i = 0; i < coordinates.Count; i++)
            {
                coordinateTerritories[coordinates[i]] = 0;
            }

            for (int x = boundingRectangle.Left; x <= boundingRectangle.Right; x++)
            {
                for (int y = boundingRectangle.Top; y <= boundingRectangle.Bottom; y++)
                {
                    int minDistance = int.MaxValue;
                    Coordinate closestCoordinate = null;
                    int sumDistance = 0;
                    foreach (var coordinate in coordinates)
                    {
                        int distance = coordinate.DistanceTo(x, y);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestCoordinate = coordinate;
                        }

                        sumDistance += distance;
                    }

                    coordinateTerritories[closestCoordinate]++;
                    locationSafeness.Add(new Coordinate(x, y), sumDistance);
                }
            }

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);

            return (coordinateTerritories, locationSafeness);
        }

        
    }

    [DebuggerDisplay("X: {X}, Y: {Y}")]
    class Coordinate
    {
        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }

        public int Y { get; }

        public int DistanceTo(Coordinate other)
        {
            return DistanceTo(other.X, other.Y);
        }

        public int DistanceTo(int otherX, int otherY)
        {
            return Math.Abs(otherX - X) + Math.Abs(otherY - Y);
        }

        public bool IsNeighbour(Coordinate other)
        {
            return Math.Abs(other.X - X) <= 1 && Math.Abs(other.Y - Y) <= 1;
        }
    }
}

