using static System.Console;

//using var file = File.OpenText("input_sample.txt");
using var file = File.OpenText("input.txt");

var codes = new List<string>();
while (!file.EndOfStream) {
    var line = file.ReadLine()!;
    codes.Add(line);
}

// +---+---+---+
// | 7 | 8 | 9 |
// +---+---+---+
// | 4 | 5 | 6 |
// +---+---+---+
// | 1 | 2 | 3 |
// +---+---+---+
//     | 0 | A |
//     +---+---+
var numpad = new char[4,3] {
    {'#', '0', 'A'},
    {'1', '2', '3'},
    {'4', '5', '6'},
    {'7', '8', '9'},
};

//     +---+---+
//     | ^ | A |
// +---+---+---+
// | < | v | > |
// +---+---+---+
var pad = new char[2,3] {
    {'<', 'v', '>'},
    {'#', '^', 'A'},
};

// var previousButton = 'A';
// var firstRobotMovements = new List<Movement>();
// foreach (var button in codes[0])
// {
//     WriteLine($"Current button {button}");
//     var paths = FindAllShortPaths(numpad, previousButton, button);
//     if (paths.Count == 1) {
//         var cost = CalculateCost(paths[0], 0);
//         PrintPath(paths[0]);
//         Write($" Cost: {cost}");
//         WriteLine();
//     } else {
//         foreach (var path in paths)
//         {
//             var cost = CalculateCost(path, 0);
//             PrintPath(path);
//             Write($" Cost: {cost}");
//             WriteLine();
//         }
//         var bestPath = FindBestAmongShortPaths(paths, 0);
//         firstRobotMovements.AddRange(bestPath);
//     }
    
//     WriteLine();
//     previousButton = button;
// }

var cache = new Dictionary<(string path, int level), long>();
Task2();

void Task1() {
    var totalComplexity = 0;
    foreach (var code in codes)
    {
        var movements = FindMyMovements(code);
        var codeNumericValue = int.Parse(code.Substring(0, code.Length-1));
        var complexity = movements.Count * codeNumericValue;
        WriteLine($"Complexity for code {code} is {complexity}");
        totalComplexity += complexity;
    }

    WriteLine($"Total complexity is {totalComplexity}");
}

void Task2() {
    long totalComplexity = 0;
    foreach (var code in codes)
    {
        var minLength = GetMinimalCodeLength(code);
        var codeNumericValue = int.Parse(code.Substring(0, code.Length-1));
        var complexity = minLength * codeNumericValue;
        WriteLine($"Complexity for code {code} is {complexity}");
        totalComplexity += complexity;
    }

    WriteLine($"Total complexity is {totalComplexity}");
}

long GetMinimalCodeLength(string code) {
    var previousButton = 'A';
    long totalLength = 0;
    foreach (var button in code)
    {
        var paths = FindAllShortPaths(numpad, previousButton, button);
        if (paths.Count == 1) {
            totalLength += CalculateCost(paths[0], 0, 24);
        } else {
            var (bestPath, cost) = FindBestAmongShortPaths(paths, 0, 24);
            totalLength += cost;
        }
        
        previousButton = button;
    }

    return totalLength;
}

List<Movement> FindMyMovements(string code) {
    var previousButton = 'A';
    var firstRobotMovements = new List<Movement>();
    foreach (var button in code)
    {
        var paths = FindAllShortPaths(numpad, previousButton, button);
        if (paths.Count == 1) {
            firstRobotMovements.AddRange(paths[0]);
        } else {
            var (bestPath, _) = FindBestAmongShortPaths(paths, 0, 3);
            firstRobotMovements.AddRange(bestPath);
        }
        
        previousButton = button;
    }

    WriteLine("First robot movements: ");
    PrintPath(firstRobotMovements);
    WriteLine();

    previousButton = 'A';
    var secondRobotMovements = new List<Movement>();
    foreach (var movement in firstRobotMovements)
    {
        var button = MovementToChar(movement);
        var paths = FindAllShortPaths(pad, previousButton, button);
        if (paths.Count == 1) {
            secondRobotMovements.AddRange(paths[0]);
        } else {
            var (bestPath, _) = FindBestAmongShortPaths(paths, 1, 3);
            secondRobotMovements.AddRange(bestPath);
        }
        previousButton = button;
    }
    WriteLine("Second robot movements: ");
    PrintPath(secondRobotMovements);
    WriteLine();

    previousButton = 'A';
    var myMovements = new List<Movement>();
    foreach (var movement in secondRobotMovements)
    {
        var button = MovementToChar(movement);
        var paths = FindAllShortPaths(pad, previousButton, button);
        myMovements.AddRange(paths[0]);
        previousButton = button;
    }
    WriteLine("My movements: ");
    PrintPath(myMovements);
    WriteLine();

    return myMovements;
}

//<v<A>>^AvA^A<v<A>>^A<vA<A>>^AAvA<^A>AvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A
//<v<A>>^AvA^A<vA<AA>>^AAvA<^A>AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A.

(List<Movement>, long) FindBestAmongShortPaths(List<List<Movement>> paths, int level, int maxLevel) {
    var lengths = new long[paths.Count];
    for (int i = 0; i < paths.Count; i++)
    {
        var path = paths[i];
        var cost = CalculateCost(path, level, maxLevel);
        lengths[i] = cost;

        //var previousButton = 'A';
        // foreach (var movement in path)
        // {
        //     var button = MovementToChar(movement);
        //     var movements = FindAllShortPaths(pad, previousButton, button);
        //     if (movements.Count == 1) {
        //         lengths[i] += movements.Count;
        //     } else if (level < 3) {
        //         var bestLength = FindBestAmongShortPaths(movements, level + 1).Count;
        //         lengths[i] += bestLength;
        //     } else {
        //         lengths[i] += movements[0].Count;
        //     }
        //     previousButton = button;
        // }
    }

    var minLength = lengths.Min();
    var minPos = lengths.Select((val, pos) => new {val,pos}).First(a => a.val == minLength);

    return (paths[minPos.pos], minPos.val);
}

long CalculateCost(List<Movement> path, int level, int maxLevel) {
    if (cache.TryGetValue((PathToString(path), level), out var cached)) {
        return cached;
    }

    var previousButton = 'A';
    long totalCost = 0;
    foreach (var movement in path)
    {
        var button = MovementToChar(movement);
        var movements = FindAllShortPaths(pad, previousButton, button);
        if (movements.Count == 1 && level < maxLevel) {
            var cost = CalculateCost(movements[0], level + 1, maxLevel);
            totalCost += cost;
        } else if (level < maxLevel) {
            var costs = movements.Select(a => CalculateCost(a, level+1, maxLevel));
            var bestLength = costs.Min();
            totalCost += bestLength;
        } else {
            totalCost += movements[0].Count;
        }
        previousButton = button;
    }

    cache[(PathToString(path), level)] = totalCost;

    return totalCost;
}

string PathToString(List<Movement> path) {
    var chars = path.Select(a => MovementToChar(a)).ToArray();
    return new string(chars);
}

List<List<Movement>> FindAllShortPaths(char[,] pad, char from, char to) {
    var start = GetButtonPosition(pad, from);
    var end = GetButtonPosition(pad, to);

    var height = pad.GetLength(0);
    var width = pad.GetLength(1);
    var distances = new int[height, width];

    var queue = new Queue<(Vector2 pos, int dist)>();
    queue.Enqueue((start, 0));

    while (queue.Count > 0) {
        var (pos, dist) = queue.Dequeue();

        var neighbors = GetNeighborPositions(pos, pad);
        foreach (var neigbor in neighbors)
        {
            if (neigbor.pos == start) continue;

            if (distances[neigbor.pos.Y, neigbor.pos.X] == 0 || distances[neigbor.pos.Y, neigbor.pos.X] > dist + 1) {
                distances[neigbor.pos.Y, neigbor.pos.X] = dist + 1;
                queue.Enqueue((neigbor.pos, dist + 1));
            }
        }
    }

    // var movements = new List<Movement>();
    // var currentPosition = end;
    // while (currentPosition != start) {
    //     var currentDistance = distances[currentPosition.Y, currentPosition.X];
    //     var neighbors = GetNeighborPositions(currentPosition, pad);

    //     var neigbor = neighbors.OrderBy(a => a.movement)
    //                             .First(neigbor => distances[neigbor.pos.Y, neigbor.pos.X] == currentDistance - 1);
    //     currentPosition = neigbor.pos;
    //     movements.Add(neigbor.movement);
    // }

    // movements.Reverse();

    var paths = GetAllPaths(distances, pad, start, end);
    var minPathLength = paths.MinBy(a => a.Count).Count;
    var shortestPaths = paths.Where(a => a.Count == minPathLength).ToList();
    
    // WriteLine($"Paths from {from} to {to}:");
    // foreach (var path in shortestPaths)
    // {
    //     PrintPath(path);
    //     WriteLine();
    // }
    // WriteLine();

    return shortestPaths;
}

List<List<Movement>> GetAllPaths(int[,] distances, char[,] pad, Vector2 start, Vector2 end) {
    var paths = new List<List<Movement>>();
    var visited = new List<(Vector2, Movement)>();

    Visit(end, Movement.Press);

    void Visit(Vector2 pos, Movement movement) {
        visited.Add((pos, movement));
        var currentDistance = distances[pos.Y, pos.X];

        if (pos == start) {
            var newPath = new List<Movement>();
            foreach (var item in visited)
            {
                newPath.Add(item.Item2);
            }
            newPath.Reverse();
            paths.Add(newPath);
            visited.Remove((pos, movement));
            return;
        }

        var neighbors = GetNeighborPositions(pos, pad);
        foreach (var neighbor in neighbors)
        {
            if (distances[neighbor.pos.Y, neighbor.pos.X] == currentDistance - 1) {
                Visit(neighbor.pos, neighbor.movement);
            }
        }

        visited.Remove((pos, movement));
    }

    return paths;
}

Vector2 GetButtonPosition(char[,] pad, char button) {
    var height = pad.GetLength(0);
    var width = pad.GetLength(1);

    for (int i = 0; i < height; i++) {
        for (int j = 0; j < width; j++) {
            if (pad[i,j] == button) {
                return new Vector2(i,j);
            }
        }
    }

    throw new Exception("Button not found");
}

IEnumerable<(Vector2 pos, Movement movement)> GetNeighborPositions(Vector2 position, char[,] field) {
    var height = field.GetLength(0);
    var width = field.GetLength(1);

    // Up
    if (position.Y > 0 && field[position.Y-1, position.X] != '#') {
        yield return (new Vector2(position.Y-1, position.X), Movement.Up);
    }

    // Down
    if (position.Y < height - 1 && field[position.Y+1, position.X] != '#') {
        yield return (new Vector2(position.Y+1, position.X), Movement.Down);
    }

    // Left
    if (position.X > 0 && field[position.Y, position.X - 1] != '#') {
        yield return (new Vector2(position.Y, position.X - 1), Movement.Right);
    }

    // Right
    if (position.X < width - 1 && field[position.Y, position.X + 1] != '#') {
        yield return (new Vector2(position.Y, position.X + 1), Movement.Left);
    }
}

void PrintPath(List<Movement> movements) {
    foreach (var movement in movements)
    {
        Write(MovementToChar(movement));
    }
}

char MovementToChar(Movement direction) {
    return direction switch
    {
        Movement.Up => '^',
        Movement.Right => '>',
        Movement.Down => 'v',
        Movement.Left => '<',
        Movement.Press => 'A'
    };
}

Movement CharToMovement(char symbol) {
    return symbol switch {
        '^' => Movement.Up,
        '>' => Movement.Right,
        'v' => Movement.Down,
        '<' => Movement.Left,
        'A' => Movement.Press,
        _ => throw new ArgumentOutOfRangeException(nameof(symbol))
    };
}

enum Movement {
    Up,
    Left,
    Down,
    Right,
    
    Press,
}

record struct Vector2(int Y, int X);