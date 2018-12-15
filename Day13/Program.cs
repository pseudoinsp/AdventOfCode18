using System;
using System.Collections.Generic;
using System.Linq;

namespace Day13
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    class Track
    {
        public List<Curve> Curves { get; }

        public List<Intersection> Intersections { get; }
    }

    class Intersection : MapElement
    {
        public IDictionary<Direction, Track> IntersectingTracks { get; }
    }

    class Curve : MapElement
    {
        public CurveType Type { get; set; }
    }

    class Cart : MapElement
    {
        public Track OnTrack { get; set; }

        public Direction Direction { get; set; }

        public void MoveTick()
        {
            var nextLocation = CalculateNextLocation();
            
            foreach (Curve curve in OnTrack.Curves)
            {
                if(curve.Location == nextLocation)
                {
                    HandleCurve(curve);
                }
            }

            foreach (Intersection intersection in OnTrack.Intersections)
            {
                if (intersection.Location == nextLocation)
                {

                }
            }
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
                nextLocation = new Coordinate(Location.Y - 1, Location.Y);
            }
            else
            {
                nextLocation = new Coordinate(Location.Y + 1, Location.Y);
            }

            return nextLocation;
        }

        private void HandleCurve(Curve curve)
        {
            if(Direction == Direction.Down || Direction == Direction.Up)
            {
                Direction = curve.Type.HorizontalComponent;
            }
            else
            {
                Direction = curve.Type.VerticalComponent;
            }
        }

        private void HandleIntersection(Intersection intersection)
        {
            Direction enteringDirection = intersection.IntersectingTracks.First(t => t.Value == OnTrack).Key;
            Direction nextDirection = DirectionHelper.CalculateNextDirection(enteringDirection, _intersectionSeed);


        }

        private int _intersectionSeed = 0;
    }

    abstract class MapElement
    {
        public Coordinate Location { get; set; }
    }

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
            return other is Coordinate && Equals((Coordinate) other);
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
        public  static Direction CalculateNextDirection(Direction previousDirection, int seed)
        {
            // left
            if(seed % 3 == 0)
            {
                if(previousDirection == Direction.Left)
                {
                    return Direction.Up;
                }

                return previousDirection + 3;
            }
            // straigth
            else if(seed % 3 == 1)
            {
                if (previousDirection == Direction.Left || previousDirection == Direction.Down)
                {
                    return previousDirection - 6;
                }

                return previousDirection + 6;
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
