using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Day15
{
    class Program
    {
        static void Main(string[] args)
        {
            Task<string[]> inputReadingTask = File.ReadAllLinesAsync("..\\..\\..\\input.txt");

            int elfAttackPower = 19;
            int roundCount = 0;
            //do
            //{
            ParseInput(inputReadingTask.Result);
            foreach (var elf in _map.Players.Where(p => p.Team == Teams.Elf))
            {
                elf.AttackValue = elfAttackPower;
            }

            //roundCount = 0;
            //_map.ElfDied = false;

            do
            {
                //PrintMap(roundCount);
                SimulateRound();
                roundCount++;
            }
            while (_map.Players.GroupBy(p => p.Team).Count() != 1);

            //elfAttackPower++;
            //}
            //while (_map.ElfDied == false);

            int remainingPlayersHitPoint = _map.Players.Sum(p => p.HP);

            Console.WriteLine($"Round number * remaining hit points: {(roundCount - 1) * remainingPlayersHitPoint}");

            Console.ReadLine();
        }

        static void SimulateRound()
        {
            var orderOfPlayersToMove = _map.Players.OrderBy(p => p.Location.X).ThenBy(p => p.Location.Y);

            foreach (var player in orderOfPlayersToMove)
            {
                if (player.HP > 0)
                    player.ExecuteRound();
            }
        }

        static void ParseInput(string[] input)
        {
            _map.Players.Clear();
            _map.Walls.Clear();

            for (int lineIndex = 0; lineIndex < input.Length; lineIndex++)
            {
                string line = input[lineIndex];
                int lineLength = line.Length;
                for (int inlineIndex = 0; inlineIndex < lineLength; inlineIndex++)
                {
                    if (line[inlineIndex] == 'G')
                    {
                        _map.Players.Add(new Player(_map, Teams.Goblin, new Coordinate(lineIndex, inlineIndex)));
                    }
                    if (line[inlineIndex] == 'E')
                    {
                        _map.Players.Add(new Player(_map, Teams.Elf, new Coordinate(lineIndex, inlineIndex)));
                    }
                    if (line[inlineIndex] == '#')
                    {
                        _map.Walls.Add(new Wall(new Coordinate(lineIndex, inlineIndex)));
                    }
                }
            }
        }

        static void PrintMap(int roundCount)
        {
            int minX = _map.Walls.Select(w => w.Location.X).Min();
            int maxX = _map.Walls.Select(w => w.Location.X).Max();
            int minY = _map.Walls.Select(w => w.Location.Y).Min();
            int maxY = _map.Walls.Select(w => w.Location.Y).Max();

            Console.WriteLine($"Round count: {roundCount}");
            for (int x = minX; x <= maxX; x++)
            {
                Console.WriteLine();
                for (int y = minY; y <= maxY; y++)
                {
                    if (_map.Walls.Any(c => c.Location == new Coordinate(x, y)))
                    {
                        Console.Write('#');
                    }
                    else if (_map.Players.Any(c => c.Location == new Coordinate(x, y)))
                    {
                        var player = _map.Players.First(c => c.Location == new Coordinate(x, y));
                        if (player.Team == Teams.Elf)
                            Console.Write('E');
                        else
                            Console.Write('G');
                    }
                    else
                        Console.Write('.');
                }
            }

            Console.WriteLine();
            Console.WriteLine();
        }

        public static Map _map = new Map();
    }

    public class Map
    {
        public Map()
        {
            Players = new List<Player>();
            Walls = new List<Wall>();
        }

        public List<Player> Players { get; set; }

        public List<Wall> Walls { get; set; }

        public bool ElfDied { get; set; } = false;
    }

    public abstract class MapElement
    {
        public Coordinate Location { get; set; }
    }

    [DebuggerDisplay("{Team}, X: {Location.X}, Y: {Location.Y}, HP: {HP}")]
    public class Player : MapElement
    {
        public Player(Map map, Teams team, Coordinate initialLocation)
        {
            HP = 200;
            AttackValue = 3;
            Map = map;
            Team = team;
            Location = initialLocation;
        }

        public Teams Team { get; }

        public Map Map { get; }

        public int HP { get; set; }

        public int AttackValue { get; set; }

        public void ExecuteRound()
        {
            // find all possible targets, if no possible target -> return
            var reachableEnemiesWithWhortestPaths = BreadthFirstShortestPath.ReachableEnemiesAndShortestPaths(Map, this);

            if (!reachableEnemiesWithWhortestPaths.Any())
            {
                return;
            }

            // if already standing next to an enemy -> attack it
            // if standing next to multiple enemies -> choose based on HP -> X -> Y
            var adjacentEnemies = reachableEnemiesWithWhortestPaths.Keys.Where(e => e.AdjacentTo(this.Location));
            if (adjacentEnemies.Any())
            {
                var adjacentEnemiesInOrder = adjacentEnemies.OrderBy(e => e.HP).ThenBy(e => e.Location.X).ThenBy(e => e.Location.Y);
                Attack(adjacentEnemiesInOrder.First());
                return;
            }

            // otherwise, find the square next to an enemy which can be reached with the fewest steps (if tied, first in reading order)-> 
            int shortestPath = reachableEnemiesWithWhortestPaths.Min(e => e.Value.First().Length);

            var enemiesWithShortestPath = reachableEnemiesWithWhortestPaths.Where(e => e.Value.First().Length == shortestPath);

            var enemyToStepTowards = enemiesWithShortestPath.OrderBy(e => e.Key.Location.X).ThenBy(e => e.Key.Location.Y).First();

            IEnumerable<Path> possiblePaths = enemyToStepTowards.Value;
            var pathToUse = possiblePaths.OrderBy(p => p.Steps.First().X).ThenBy(p => p.Steps.First().Y).First();
            var stepToTake = pathToUse.Steps.First();

            // move a step along the shortest path (if multiple shortest paths, use step first that is first in reading order) 
            this.Location = stepToTake;

            // and if canattack->attack
            if (pathToUse.Length == 1)
            {
                var adjacentEnemiesAfterMove = reachableEnemiesWithWhortestPaths.Keys.Where(e => e.AdjacentTo(this.Location));

                if (adjacentEnemiesAfterMove.Any())
                {
                    var adjacentEnemiesInOrder = adjacentEnemiesAfterMove.OrderBy(e => e.HP).ThenBy(e => e.Location.X).ThenBy(e => e.Location.Y);
                    Attack(adjacentEnemiesInOrder.First());
                    return;
                }
            }
        }

        public void Attack(Player attackedPlayer)
        {
            attackedPlayer.HP -= this.AttackValue;

            if (attackedPlayer.HP <= 0)
            {
                if (attackedPlayer.Team == Teams.Elf)
                {
                    throw new InvalidOperationException();
                }
                Map.Players.Remove(attackedPlayer);
            }
        }

        public bool AdjacentTo(Coordinate coordinate)
        {
            return this.Location.X == coordinate.X && Math.Abs(this.Location.Y - coordinate.Y) == 1 ||
                this.Location.Y == coordinate.Y && Math.Abs(this.Location.X - coordinate.X) == 1;
        }
    }

    public class Wall : MapElement
    {
        public Wall(Coordinate location)
        {
            Location = location;
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

    public enum Teams
    {
        Elf,
        Goblin
    }

    public class Path
    {
        public Path()
        {
            Steps = new LinkedList<Coordinate>();
        }

        public LinkedList<Coordinate> Steps { get; set; }

        public int Length => Steps.Count;
    }

    // https://youtu.be/KiCBXu4P-2Y?t=480
    public static class BreadthFirstShortestPath
    {
        /// <summary>
        /// Searches for the shortest path for each reachable enemy's adjacent square.
        /// </summary>
        static public IDictionary<Player, List<Path>> ReachableEnemiesAndShortestPaths(Map map, Player fromPlayer)
        {
            bool newSquareVisitedThisStep = true;
            List<VisitedSquare> visitedSquares = new List<VisitedSquare>()
            {
                new VisitedSquare()
                {
                    Coordinate = fromPlayer.Location,
                    VisitedFrom = null,
                    InStep = 0
                }
            };
            int currentStep = 1;

            var wallLocations = map.Walls.Select(w => w.Location);
            var playerLocations = map.Players.Select(p => p.Location);

            // Visit the map starting from the given location
            // Create a structure where the visited squares are stored
            // also store that in which step they have been visited and from which square
            while (newSquareVisitedThisStep)
            {
                newSquareVisitedThisStep = false;

                var squaresFromLastStep = visitedSquares.Where(vs => vs.InStep == currentStep - 1).ToList();
                var squaresAlreadyVisitedBeforeCurrentStep = visitedSquares.Select(s => s.Coordinate).ToList();

                foreach (var square in squaresFromLastStep)
                {
                    var possiblenewCoordinates = new List<Coordinate>()
                    {
                        new Coordinate(square.Coordinate.X + 1, square.Coordinate.Y),
                        new Coordinate(square.Coordinate.X - 1, square.Coordinate.Y),
                        new Coordinate(square.Coordinate.X, square.Coordinate.Y + 1),
                        new Coordinate(square.Coordinate.X, square.Coordinate.Y - 1),
                    };

                    foreach (var newCoordinate in possiblenewCoordinates)
                    {
                        if (wallLocations.Contains(newCoordinate) || playerLocations.Contains(newCoordinate) || squaresAlreadyVisitedBeforeCurrentStep.Contains(newCoordinate))
                        {
                            continue;
                        }

                        var visitedSquare = visitedSquares.FirstOrDefault(sq => sq.Coordinate == newCoordinate);

                        if (visitedSquare == null)
                        {
                            var squareToAdd = new VisitedSquare()
                            {
                                Coordinate = newCoordinate,
                                InStep = currentStep,
                            };
                            squareToAdd.VisitedFrom.Add(square);

                            visitedSquares.Add(squareToAdd);
                        }
                        else // square has been already visited from an other square in this step
                        {
                            visitedSquare.VisitedFrom.Add(square);
                        }

                        newSquareVisitedThisStep = true;
                    }
                }

                currentStep++;
            }

            // Calculate the squares which are adjacent to enemies
            // and they can be possibly reached (no wall, no other player unless its the current player already standing there)
            var adjacentPossiblyReachableSquaresToEnemies = new Dictionary<Player, List<Coordinate>>();

            var enemies = map.Players.Where(p => p.Team != fromPlayer.Team);
            foreach (var enemy in enemies)
            {
                var enemyLocation = enemy.Location;
                adjacentPossiblyReachableSquaresToEnemies.Add(enemy, new List<Coordinate>());

                var rightSquare = new Coordinate(enemyLocation.X + 1, enemyLocation.Y);
                if ((!wallLocations.Contains(rightSquare) && !playerLocations.Contains(rightSquare)) ||
                    rightSquare == fromPlayer.Location)
                {
                    adjacentPossiblyReachableSquaresToEnemies[enemy].Add(rightSquare);
                }

                var leftSquare = new Coordinate(enemyLocation.X - 1, enemyLocation.Y);
                if ((!wallLocations.Contains(leftSquare) && !playerLocations.Contains(leftSquare)) ||
                    leftSquare == fromPlayer.Location)
                {
                    adjacentPossiblyReachableSquaresToEnemies[enemy].Add(leftSquare);
                }

                var upSquare = new Coordinate(enemyLocation.X, enemyLocation.Y + 1);
                if ((!wallLocations.Contains(upSquare) && !playerLocations.Contains(upSquare)) ||
                    upSquare == fromPlayer.Location)
                {
                    adjacentPossiblyReachableSquaresToEnemies[enemy].Add(upSquare);
                }

                var downSquare = new Coordinate(enemyLocation.X, enemyLocation.Y - 1);
                if ((!wallLocations.Contains(downSquare) && !playerLocations.Contains(downSquare)) ||
                    downSquare == fromPlayer.Location)
                {
                    adjacentPossiblyReachableSquaresToEnemies[enemy].Add(downSquare);
                }
            }

            // search these adjacent square in the visit structure.
            // Create the optimal path to each of them.
            var enemiesWithPathsToAdjacentSquares = new Dictionary<Player, List<Path>>();

            foreach (KeyValuePair<Player, List<Coordinate>> enemyWithAdjacentSqures in adjacentPossiblyReachableSquaresToEnemies)
            {
                foreach (Coordinate adjSquare in enemyWithAdjacentSqures.Value)
                {
                    var visited = visitedSquares.FirstOrDefault(vs => vs.Coordinate == adjSquare);

                    if (visited != null)
                    {
                        var paths = SearchOptimalPath(visited);

                        if (!enemiesWithPathsToAdjacentSquares.ContainsKey(enemyWithAdjacentSqures.Key))
                        {
                            enemiesWithPathsToAdjacentSquares.Add(enemyWithAdjacentSqures.Key, new List<Path>());
                        }

                        enemiesWithPathsToAdjacentSquares[enemyWithAdjacentSqures.Key].Add(paths);
                    }
                }
            }

            // Out of all reachable adjacent squares of an enemy, search the shortest ones, and return them
            var enemiesWithPathsToClosestAdjacentSquares = new Dictionary<Player, List<Path>>();
            foreach (var item in enemiesWithPathsToAdjacentSquares)
            {
                int minLength = item.Value.Min(p => p.Length);
                enemiesWithPathsToClosestAdjacentSquares.Add(item.Key, item.Value.Where(p => p.Length == minLength).ToList());
            }

            return enemiesWithPathsToClosestAdjacentSquares;
        }

        /// <summary>
        /// If multiple paths are existing to a square, at a branch, always chose the first one in the X,Y order.
        /// </summary>
        public static Path SearchOptimalPath(VisitedSquare to)
        {
            var ret = new Path();

            var current = to;
            while (current.VisitedFrom != null)
            {
                ret.Steps.AddFirst(current.Coordinate);
                current = current.VisitedFrom.OrderBy(sq => sq.Coordinate.X).ThenBy(sq => sq.Coordinate.Y).First();
            }

            return ret;
        }
    }

    /// <summary>
    /// Helper structure for the visiting algorythm.
    /// </summary>
    public class VisitedSquare
    {
        public VisitedSquare()
        {
            VisitedFrom = new List<VisitedSquare>();
        }

        public int InStep { get; set; }

        public Coordinate Coordinate { get; set; }

        public List<VisitedSquare> VisitedFrom { get; set; }
    }
}
