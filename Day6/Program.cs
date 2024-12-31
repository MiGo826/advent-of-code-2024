using System.Data.Common;
using static System.Console;

//using var file = File.OpenText("input_sample.txt");
using var file = File.OpenText("input.txt");

var map = new List<List<char>>();
var startingPosition = (0,0);

var line = 0;
while (!file.EndOfStream) {
    var str = file.ReadLine();
    map.Add(str.ToList());

    if (startingPosition == (0,0)) {
        for (int i = 0; i < str.Length; i++) {
            if (str[i] == '^') {
                startingPosition = (line, i);
            }
        }
    }

    line++;
}

var possibleLoopObstructions = new HashSet<(int y, int x)>();
var mainGuard = new Guard(startingPosition, Direction.Up);
while (mainGuard.TryMove(map)) {
    var nextPosition = mainGuard.NextPosition(mainGuard.Position, mainGuard.Direction);
    if (map.CanObstacleBePlacesAt(nextPosition)) {
        map.PutObstacle(nextPosition);
        var ghost = new Guard(startingPosition, Direction.Up);
        while (ghost.TryMove(map)) {
            //  ᯓᯓ༼ つ ╹ ╹ ༽つ
        }
        if (ghost.StuckInLoop && nextPosition != startingPosition) {
            possibleLoopObstructions.Add(nextPosition);
        }
        map.RemoveObstacle(nextPosition);
    }

    Clear();
    WriteLine($"Distance traveled: {mainGuard.DistanceTraveled}");
    WriteLine($"Points visited: {mainGuard.PositionsVisited}");
    WriteLine($"Possible loop obstructions: {possibleLoopObstructions.Count}");
}

// for (int i = 0; i < map.Count; i++)
// {
//     for (int j = 0; j < map[i].Count; j++)
//     {
//         (int y, int x) position = (i,j);
//         if (map.CanObstacleBePlacesAt(position) && position != startingPosition) {
//             map.PutObstacle(position);
//             var guard = new Guard(startingPosition, Direction.Up);
//             while (guard.TryMove(map))
//             {
                
//             }
//             if (guard.StuckInLoop) {
//                 possibleLoopObstructions.Add(position);
//             }
//             map.RemoveObstacle(position);
//         }
//         Clear();
//         WriteLine($"Position ({position.y}, {position.x})");
//         WriteLine($"Possible loop obstructions: {possibleLoopObstructions.Count}");
//     }
// }

WriteLine("Traversal complete!");

foreach (var obstructions in possibleLoopObstructions) {
    map[obstructions.y][obstructions.x] = 'O';
}

using var output = File.OpenWrite("output.txt");
using var writer = new StreamWriter(output);
foreach (var item in map) {
    var str = new string(item.ToArray());
    writer.WriteLine(str);
}

enum Direction {
    Up,
    Right,
    Down,
    Left
}

class Guard {
    private (int y, int x) _startingPosition;

    public Direction Direction { get; private set; }

    public (int y, int x) Position { get; private set; }

    public int DistanceTraveled { get; private set; } = 1;

    public HashSet<(int y, int x)> PossibleLoopObstructions {get; private set;}

    public HashSet<(int y, int x, Direction direction)> VisitedPositions { get; }

    public bool FoundEdge { get; private set; }

    public bool StuckInLoop { get; private set; }

    private int _samePositionsVisited = 0;

    public int PositionsVisited => VisitedPositions.Select(a => (a.y, a.x)).Distinct().Count();

    public Guard((int,int) startingPosition, Direction startingDirection) {
        Direction = startingDirection;
        Position = startingPosition;
        _startingPosition = startingPosition;
        PossibleLoopObstructions = new HashSet<(int y, int x)>();
        VisitedPositions = new HashSet<(int, int, Direction)> { (Position.y, Position.x, Direction) };
    }

    public bool TryMove(List<List<char>> map)
    {
        if (StuckInLoop || FoundEdge) {
            return false;
        }

        if (IsMapEdgeAhead(map, Position, Direction)) {
            FoundEdge = true;
            return false;
        }

        if (HaveObstacleAhead(map, Position, Direction))
        {
            VisitedPositions.Add((Position.y, Position.x, Direction));
            Direction = TurnRight(Direction);
            //map[Position.y][Position.x] = '+';
            return true;
        }

        VisitedPositions.Add((Position.y, Position.x, Direction));
        Position = NextPosition(Position, Direction);
        DistanceTraveled++;

        //CheckPossibleLoop(map);

        if (VisitedPositions.Contains((Position.y, Position.x, Direction))) {
            _samePositionsVisited++;
        }

        if (_samePositionsVisited > 10000) {
            StuckInLoop = true;
            return false;
        }

        // if (VisitedPositions.Contains((Position.y, Position.x, Direction))) 
        // {
        //     StuckInLoop = true;
        //     return false;
        // }

        

        return true;
    }

    private bool CheckPossibleLoop(List<List<char>> map) {
        if (IsMapEdgeAhead(map, Position, Direction)) return false;

        var possibleObstaclePosition = NextPosition(Position, Direction);
        if (possibleObstaclePosition == _startingPosition) return false;

        var temp = map[possibleObstaclePosition.y][possibleObstaclePosition.x];
        map[possibleObstaclePosition.y][possibleObstaclePosition.x] = '#';

        var startingPosition = Position;
        var currentPosition = Position;
        var currentDirection = TurnRight(Direction);
        var obstaclesVisited = 0;

        var path = new HashSet<(int y, int x, Direction direction)> {
            (startingPosition.y, startingPosition.x, currentDirection)
        };
        
        while (!IsMapEdgeAhead(map, currentPosition, currentDirection)) {

            if (HaveObstacleAhead(map, currentPosition, currentDirection)) {
                currentDirection = TurnRight(currentDirection);
                obstaclesVisited++;
                continue;
            }

            currentPosition = NextPosition(currentPosition, currentDirection);

            if (path.Contains((currentPosition.y, currentPosition.x, currentDirection))) {
                PossibleLoopObstructions.Add((possibleObstaclePosition.y, possibleObstaclePosition.x));

                map[possibleObstaclePosition.y][possibleObstaclePosition.x] = temp;
                return true;
            }

            path.Add((currentPosition.y, currentPosition.x, currentDirection));
        }

        map[possibleObstaclePosition.y][possibleObstaclePosition.x] = temp;
        return false;
    }

    public bool HaveObstacleAhead(List<List<char>> map, (int y, int x) position, Direction direction) {
        (int y, int x) positionAhead = direction switch
        {
            Direction.Up => (position.y - 1, position.x),
            Direction.Right => (position.y, position.x + 1),
            Direction.Down => (position.y + 1, position.x),
            Direction.Left => (position.y, position.x - 1)
        };

        try
        {
            return map[positionAhead.y][positionAhead.x] == '#';
        }
        catch
        {
            return false;
        }
    }

    public bool IsMapEdgeAhead(List<List<char>> map, (int y, int x) position, Direction direction) {
        return direction switch
        {
            Direction.Up => position.y == 0,
            Direction.Right => position.x == map[0].Count - 1,
            Direction.Down => position.y == map.Count - 1,
            Direction.Left => position.x == 0
        };
    }

    public (int y, int x) NextPosition((int y, int x) position, Direction direction) {
        return direction switch
        {
            Direction.Up => (position.y - 1, position.x),
            Direction.Right => (position.y, position.x + 1),
            Direction.Down => (position.y + 1, position.x),
            Direction.Left => (position.y, position.x - 1)
        };
    }    

    public Direction TurnRight(Direction direction) {
        return (Direction)((int)(direction + 1) % 4);
    }

    private char GetDirectionSymbol(Direction direction) {
        return direction switch
        {
            Direction.Up => '^',
            Direction.Right => '>',
            Direction.Down => 'v',
            Direction.Left => '<',
        };
    }

    private Direction GetDirectionFromSymbol(char symbol) {
        return symbol switch {
            '^' => Direction.Up,
            '>' => Direction.Right,
            'v' => Direction.Down,
            '<' => Direction.Left,
            _ => throw new ArgumentOutOfRangeException(nameof(symbol))
        };
    }
}

static class MapHelpers {
    public static bool CanObstacleBePlacesAt(this List<List<char>> map, (int y, int x) pos) {

        return pos.y >= 0 && pos.y < map.Count && pos.x >= 0 && pos.x < map[0].Count && map[pos.y][pos.x] != '#';
    }

    public static void PutObstacle(this List<List<char>> map, (int y, int x) pos) {
        map[pos.y][pos.x] = '#';
    }

    public static void RemoveObstacle(this List<List<char>> map, (int y, int x) pos) {
        map[pos.y][pos.x] = '.';
    }
}