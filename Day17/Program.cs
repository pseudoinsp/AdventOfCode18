using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Day17
{
    class Program
    {
        static void Main(string[] args)
        {
            Task<string[]> inputReadingTask = File.ReadAllLinesAsync("..\\..\\..\\input.txt");

            ParseInput(inputReadingTask.Result);
        }

        static void ParseInput(string[] lines)
        {
            foreach (var line in lines)
            {
                int xDefStart = line.IndexOf("x=");
                int yDefStart = line.IndexOf("y=");
                int endOfXDeforYDef = line.IndexOfAny(new[] { ' ', ',' });

                int xStart = -1;
                int xEnd = -1;
                int yStart = -1;
                int yEnd = -1;

                // parse first part -- could be either x or y
                if (xDefStart < endOfXDeforYDef)
                {
                    var firstPartDef = int.Parse(line.Substring(xDefStart + 2, endOfXDeforYDef - 2));
                    xStart = xEnd = firstPartDef;
                }
                else // y is at the start of the line
                {
                    var firstPartDef = int.Parse(line.Substring(yDefStart + 2, endOfXDeforYDef - 2));
                    yStart = yEnd = firstPartDef;
                }

                // parse second part
                if (xDefStart > endOfXDeforYDef)
                {
                    var splits = line.Substring(xDefStart + 2).Split("..");
                    xStart = int.Parse(splits[0]);
                    xEnd = int.Parse(splits[1]);
                }
                else // y as second part
                {
                    var splits = line.Substring(yDefStart + 2).Split("..");
                    yStart = int.Parse(splits[0]);
                    yEnd = int.Parse(splits[1]);
                }

                for (int x = xStart; x <= xEnd; x++)
                {
                    for (int y = yStart; y <= yEnd; y++)
                    {
                        _map.Clays.Add(new Clay() { Location = new Coordinate(x, y) });    
                    }
                }
            }

            // TODO solve duplicate clays if necessary
        }

        static Map _map = new Map();
    }

    class Map
    {
        public Map()
        {
            WaterSource = new WaterSource();
            Clays = new List<Clay>();
        }

        public WaterSource WaterSource { get; }

        public List<Clay> Clays { get; }
    }

    class MapElement
    {
        public Coordinate Location { get; set; }

        public bool WaterTouched { get; set; }
    }

    class WaterSource : MapElement
    {
        public WaterSource()
        {
            Location = new Coordinate(500, 0);
            WaterTouched = false;
        }
    }

    [DebuggerDisplay("{Location.X} {Location.Y}")]
    class Clay : MapElement
    {
        public Clay()
        {
            WaterTouched = false;
        }
    }

    [DebuggerDisplay("X: {X}, Y: {Y}")]
    public struct Coordinate : IEquatable<Coordinate>
    {
        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }

        public override bool Equals(object other)
        {
            return other is Coordinate && Equals((Coordinate)other);
        }

        public bool Equals(Coordinate other)
        {
            return other.X == X && other.Y == Y;
        }

        public static bool operator ==(Coordinate c1, Coordinate c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(Coordinate c1, Coordinate c2)
        {
            return !c1.Equals(c2);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
    }
}
