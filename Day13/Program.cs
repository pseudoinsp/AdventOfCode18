using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Day13
{
    class Program
    {
        static void Main(string[] args)
        {
            Task<string[]> inputReadingTask = File.ReadAllLinesAsync("..\\..\\..\\input.txt");

            ParseInput(inputReadingTask.Result);

            do
            {
                SimulateTick();
            }
            while (_map.Carts.Count != 1);

            // Task 2
            Console.WriteLine($"Position of the last remaining cart: ({_map.Carts.First().Location.Y},{_map.Carts.First().Location.X})");

            Console.ReadLine();
        }

        static void ParseInput(string[] input)
        {
            for (int lineIndex = 0; lineIndex < input.Length; lineIndex++)
            {
                string line = input[lineIndex];
                for (int inlineIndex = 0; inlineIndex < line.Length; inlineIndex++)
                {
                    if (line[inlineIndex] == '/')
                    {
                        // case _/ 
                        if (inlineIndex != 0 &&
                            (line[inlineIndex - 1] == '-' || line[inlineIndex - 1] == '>' || line[inlineIndex - 1] == '<' || line[inlineIndex - 1] == '+' || line[inlineIndex - 1] == '\\') &&
                            (input[lineIndex - 1][inlineIndex] == '^' || input[lineIndex - 1][inlineIndex] == 'v' || input[lineIndex - 1][inlineIndex] == '+' || input[lineIndex - 1][inlineIndex] == '\\' || input[lineIndex - 1][inlineIndex] == '|'))
                        {
                            _map.Curves.Add(new Curve()
                            {
                                Location = new Coordinate(lineIndex, inlineIndex),
                                Type = new CurveType()
                                {
                                    HorizontalComponent = Direction.Left,
                                    VerticalComponent = Direction.Down
                                }
                            });
                        }
                        else // case /-
                        {
                            _map.Curves.Add(new Curve()
                            {
                                Location = new Coordinate(lineIndex, inlineIndex),
                                Type = new CurveType()
                                {
                                    HorizontalComponent = Direction.Right,
                                    VerticalComponent = Direction.Up
                                }
                            });
                        }
                    }
                    if (line[inlineIndex] == '\\')
                    {
                        // case -\
                        if (inlineIndex != 0 &&
                            lineIndex + 1 < input.Length &&
                            (line[inlineIndex - 1] == '-' || line[inlineIndex - 1] == '>' || line[inlineIndex - 1] == '<' || line[inlineIndex - 1] == '+' || line[inlineIndex - 1] == '/') &&
                            (input[lineIndex + 1][inlineIndex] == '|' || input[lineIndex + 1][inlineIndex] == '^' || input[lineIndex + 1][inlineIndex] == 'v' || input[lineIndex + 1][inlineIndex] == '+' || input[lineIndex + 1][inlineIndex] == '/'))
                        {
                            _map.Curves.Add(new Curve()
                            {
                                Location = new Coordinate(lineIndex, inlineIndex),
                                Type = new CurveType()
                                {
                                    HorizontalComponent = Direction.Left,
                                    VerticalComponent = Direction.Up
                                }
                            });
                        }
                        else // case \-
                        {
                            _map.Curves.Add(new Curve()
                            {
                                Location = new Coordinate(lineIndex, inlineIndex),
                                Type = new CurveType()
                                {
                                    HorizontalComponent = Direction.Right,
                                    VerticalComponent = Direction.Down
                                }
                            });
                        }
                    }
                    if (line[inlineIndex] == '+')
                    {
                        _map.Intersections.Add(new Coordinate(lineIndex, inlineIndex));
                    }
                    if (line[inlineIndex] == 'v')
                    {
                        _map.Carts.Add(new Cart()
                        {
                            Direction = Direction.Up,
                            Location = new Coordinate(lineIndex, inlineIndex),
                            Map = _map
                        });
                    }
                    if (line[inlineIndex] == '^')
                    {
                        _map.Carts.Add(new Cart()
                        {
                            Direction = Direction.Down,
                            Location = new Coordinate(lineIndex, inlineIndex),
                            Map = _map
                        });
                    }
                    if (line[inlineIndex] == '>')
                    {
                        _map.Carts.Add(new Cart()
                        {
                            Direction = Direction.Right,
                            Location = new Coordinate(lineIndex, inlineIndex),
                            Map = _map
                        });
                    }
                    if (line[inlineIndex] == '<')
                    {
                        _map.Carts.Add(new Cart()
                        {
                            Direction = Direction.Left,
                            Location = new Coordinate(lineIndex, inlineIndex),
                            Map = _map
                        });
                    }
                }
            }
        }


        static void SimulateTick()
        {
           // PrintTrack();

            var cartsInMoveOrder = _map.Carts.OrderBy(c => c.Location.X).ThenBy(c => c.Location.Y);

            foreach (var cart in cartsInMoveOrder)
            {
                cart.MoveTick();
                // TODO ...
                if (DetectCollision(out collisionLocation))
                {
                    _map.Carts.RemoveAll(c => c.Location == collisionLocation);
                }
            }

            ticks++;
        }

        static bool DetectCollision(out Coordinate Where)
        {
            IEnumerable<Coordinate> duplicates = _map.Carts.Select(c => c.Location).GroupBy(l => l).Where(g => g.Count() > 1).Select(g => g.Key);

            if (duplicates.Any())
            {
                Where = duplicates.First();
                return true;
            }

            Where = new Coordinate();
            return false;
        }

        static void PrintTrack()
        {
            int minX = _map.Curves.Select(c => c.Location.X).Union(_map.Intersections.Select(i => i.X)).Min();
            int maxX = _map.Curves.Select(c => c.Location.X).Union(_map.Intersections.Select(i => i.X)).Max();
            int minY = _map.Curves.Select(c => c.Location.Y).Union(_map.Intersections.Select(i => i.Y)).Min();
            int maxY = _map.Curves.Select(c => c.Location.Y).Union(_map.Intersections.Select(i => i.Y)).Max();

            Console.WriteLine();
            for (int x = minX; x <= maxX; x++)
            {
                Console.WriteLine();
                Console.Write($"{x + 1} ");
                for (int y = minY; y <= maxY; y++)
                {
                    if (_map.Carts.Any(c => c.Location == new Coordinate(x, y)))
                    {
                        var cart = _map.Carts.First(c => c.Location == new Coordinate(x, y));

                        switch (cart.Direction)
                        {
                            case Direction.Up:
                                Console.Write('v');
                                break;
                            case Direction.Down:
                                Console.Write('^');
                                break;
                            case Direction.Left:
                                Console.Write('<');
                                break;
                            case Direction.Right:
                                Console.Write('>');
                                break;
                            default:
                                throw new ArgumentException();
                        }
                    }
                    else if (_map.Curves.Any(c => c.Location == new Coordinate(x, y)))
                    {
                        var curve = _map.Curves.First(c => c.Location == new Coordinate(x, y));

                        if (curve.Type.HorizontalComponent == Direction.Left && curve.Type.VerticalComponent == Direction.Up)
                            Console.Write('a');
                        else if(curve.Type.HorizontalComponent == Direction.Right && curve.Type.VerticalComponent == Direction.Down)
                            Console.Write('b');
                        else if(curve.Type.HorizontalComponent == Direction.Left && curve.Type.VerticalComponent == Direction.Down)
                            Console.Write('1');
                        else if (curve.Type.HorizontalComponent == Direction.Right && curve.Type.VerticalComponent == Direction.Up)
                            Console.Write('2');
                    }
                    else if (_map.Intersections.Any(c => c == new Coordinate(x, y)))
                    {
                        Console.Write('+');
                    }

                    else
                        Console.Write(' ');
                }

                Console.Write($"  {x + 1}");
            }
        }

        readonly static Map _map = new Map();
        static Coordinate collisionLocation;
        static int ticks = 0;
    }

    class Map
    {
        public Map()
        {
            Curves = new List<Curve>();
            Intersections = new List<Coordinate>();
            Carts = new List<Cart>();
        }

        public List<Curve> Curves { get; }

        public List<Coordinate> Intersections { get; }

        public List<Cart> Carts { get; }
    }

    class Curve : MapElement
    {
        public CurveType Type { get; set; }
    }

    [DebuggerDisplay("X: {Location.X}, Y: {Location.Y}")]
    class Cart : MapElement
    {
        public Map Map { get; set; }

        public Direction Direction { get; set; }

        public void MoveTick()
        {
            var nextLocation = CalculateNextLocation();

            foreach (Curve curve in Map.Curves)
            {
                if (curve.Location == nextLocation)
                {
                    HandleCurve(curve);
                }
            }

            foreach (var intersection in Map.Intersections)
            {
                if (intersection == nextLocation)
                {
                    HandleIntersection(intersection);
                }
            }

            Location = nextLocation;
        }

        private Coordinate CalculateNextLocation()
        {
            Coordinate nextLocation;
            if (Direction == Direction.Up)
            {
                nextLocation = new Coordinate(Location.X + 1, Location.Y);
            }
            else if (Direction == Direction.Down)
            {
                nextLocation = new Coordinate(Location.X - 1, Location.Y);
            }
            else if (Direction == Direction.Left)
            {
                nextLocation = new Coordinate(Location.X, Location.Y - 1);
            }
            else
            {
                nextLocation = new Coordinate(Location.X, Location.Y + 1);
            }

            return nextLocation;
        }

        private void HandleCurve(Curve curve)
        {
            if (Direction == Direction.Down || Direction == Direction.Up)
            {
                Direction = curve.Type.HorizontalComponent;
            }
            else
            {
                Direction = curve.Type.VerticalComponent;
            }
        }

        private void HandleIntersection(Coordinate intersection)
        {
            Direction nextDirection = DirectionHelper.CalculateNextDirection(Direction, _intersectionSeed);

            Direction = nextDirection;

            _intersectionSeed++;
        }

        private int _intersectionSeed = 0;
    }

    abstract class MapElement
    {
        public Coordinate Location { get; set; }
    }

    [DebuggerDisplay("X: {X}, Y: {Y}")]
    internal struct Coordinate : IEquatable<Coordinate>
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

    class CurveType
    {
        public Direction HorizontalComponent
        {
            get => _horizontalComponent;
            set
            {
                if (value == Direction.Up || value == Direction.Down)
                {
                    throw new ArgumentException();
                }

                _horizontalComponent = value;
            }
        }
        public Direction VerticalComponent
        {
            get => _verticalComponent;
            set
            {
                if (value == Direction.Left || value == Direction.Right)
                {
                    throw new ArgumentException();
                }

                _verticalComponent = value;
            }
        }

        private Direction _horizontalComponent;
        private Direction _verticalComponent;
    }

    enum Direction
    {
        Up = 0,
        Down = 6,
        Left = 9,
        Right = 3
    }

    // TODO .................................
    static class DirectionHelper
    {
        public static Direction CalculateNextDirection(Direction previousDirection, int seed)
        {
            // left
            if (seed % 3 == 0)
            {
                if (previousDirection == Direction.Left)
                {
                    return Direction.Up;
                }

                return previousDirection + 3;
            }
            // straight
            else if (seed % 3 == 1)
            {
                return previousDirection;
            }
            // right
            else if (seed % 3 == 2)
            {
                if (previousDirection == Direction.Up)
                {
                    return Direction.Left;
                }

                return previousDirection - 3;
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}
