using System.Numerics;
using static System.Console;

//using var file = File.OpenText("input_sample.txt");
using var file = File.OpenText("input.txt");

const int startBytesCount = 1024;

char[,] field = null;
int size = 0;
Vector2 startPosition = new Vector2();
Vector2 endPosition = new Vector2();

int currentLine = 0;
do {
    var line = file.ReadLine()!;

    if (field == null) {
        size = line.Length;
        field = new char[size, size];
    }

    for (int i = 0; i < size; i++)
    {
        if (line[i] == '#') {
            field[currentLine, i] = '#';
        } else if (line[i] == 'S') {
            startPosition = new Vector2(currentLine, i);
        } else if (line[i] == 'E') {
            endPosition = new Vector2(currentLine, i);
        }
    }
    
    currentLine++;
} while (!file.EndOfStream);

// var cheats = GetPossibleCheatsLong(field, startPosition);
// var height = field.GetLength(0);
// var width = field.GetLength(1);
// var distances = new int[height,width];
// foreach (var item in cheats)
// {
//     distances[item.end.Y, item.end.X] = item.length;
// }
// PrintField(field, null);
// PrintDistances(distances);

Task2();

void Task1() {
    PrintField(field, null);
    var distancesFromStart = FindDistances(field, startPosition, endPosition);
    var distancesFromEnd = FindDistances(field, endPosition, startPosition);
    var path = FindShortesPath(distancesFromEnd, field, endPosition, startPosition);

    var pathLength = path.Count;
    var cheats = new Dictionary<Vector2, int>();
    var visited = new HashSet<Vector2>();
    foreach (var step in path) {
        var possibleCheats = GetPossibleCheats(field, step).ToList();
        visited.Add(step);

        foreach (var cheat in possibleCheats)
        {
            if (visited.Contains(cheat.end)) {
                continue;
            }

            var distanceFromStart = distancesFromStart[step.Y, step.X];
            var distanceFromEnd = distancesFromEnd[cheat.end.Y, cheat.end.X];

            // Clear();
            // PrintDistances(distancesFromStart, cheat.start, cheat.end);
            // ReadKey();

            var newDistance = distanceFromStart + distanceFromEnd + 2;
            if (newDistance < pathLength) {
                var saving = pathLength - newDistance;
                cheats[cheat.start] = saving;
            }

        }
    }

    var result = cheats.GroupBy(a => a.Value)
                        .Select(a => new { SavedPicoseconds = a.Key, CheatsCount = a.Count()})
                        .OrderBy(a => a.SavedPicoseconds)
                        .ToList();

    // foreach (var item in result)
    // {
    //     WriteLine($"There are {item.CheatsCount} possible cheats that save {item.SavedPicoseconds} picoseconds");
    // }

    var taskResult = result.Where(a => a.SavedPicoseconds >= 100).Sum(a => a.CheatsCount);
    WriteLine($"There are {taskResult} cheats that save at least 100 picoseconds");
}

void Task2() {
    PrintField(field, null);
    var distancesFromStart = FindDistances(field, startPosition, endPosition);
    var distancesFromEnd = FindDistances(field, endPosition, startPosition);
    var path = FindShortesPath(distancesFromEnd, field, endPosition, startPosition);

    var pathLength = path.Count;
    var cheats = new Dictionary<(Vector2 start, Vector2 end), int>();
    var visited = new HashSet<Vector2>();
    foreach (var step in path) {
        var possibleCheats = GetPossibleCheatsLong(field, step, 20).ToList();
        visited.Add(step);

        foreach (var cheat in possibleCheats)
        {
            if (visited.Contains(cheat.end)) {
                continue;
            }

            var distanceFromStart = distancesFromStart[step.Y, step.X];
            var distanceFromEnd = distancesFromEnd[cheat.end.Y, cheat.end.X];

            // Clear();
            // PrintDistances(distancesFromStart, cheat.start, cheat.end);
            // ReadKey();

            var newDistance = distanceFromStart + distanceFromEnd + cheat.length;
            if (newDistance < pathLength) {
                var saving = pathLength - newDistance;
                cheats[(cheat.start, cheat.end)] = saving;
            }

        }
    }

    var result = cheats.GroupBy(a => a.Value)
                        .Select(a => new { SavedPicoseconds = a.Key, CheatsCount = a.Count()})
                        .Where(a => a.SavedPicoseconds >= 50)
                        .OrderBy(a => a.SavedPicoseconds)
                        .ToList();

    // foreach (var item in result)
    // {
    //     WriteLine($"There are {item.CheatsCount} possible cheats that save {item.SavedPicoseconds} picoseconds");
    // }

    var taskResult = result.Where(a => a.SavedPicoseconds >= 100).Sum(a => a.CheatsCount);
    WriteLine($"There are {taskResult} cheats that save at least 100 picoseconds");
}

int[,] FindDistances(char[,] field, Vector2 startPosition, Vector2 endPosition) {
    var height = field.GetLength(0);
    var width = field.GetLength(1);

    var distances = new int[height,width];

    var queue = new Queue<Vector2>();
    queue.Enqueue(startPosition);

    while (queue.Count > 0) {
        var position = queue.Dequeue();
        var currentDistance = distances[position.Y, position.X];

        var neighbors = GetNeighborPositions(position, field);
        foreach (var neighbor in neighbors)
        {
            if (neighbor == startPosition) continue;

            var neighborDistance = distances[neighbor.Y, neighbor.X];
            if (neighborDistance == 0 || neighborDistance > currentDistance + 1) {
                distances[neighbor.Y, neighbor.X] = currentDistance + 1;
                queue.Enqueue(neighbor);
            }
        }
    }

    return distances;
}

HashSet<Vector2> FindShortesPath(int[,] distances, char[,] field, Vector2 startPosition, Vector2 endPosition) {
    var height = field.GetLength(0);
    var width = field.GetLength(1);

    var path = new HashSet<Vector2>();

    var currentPosition = endPosition;

    while (currentPosition != startPosition) {
        path.Add(currentPosition);

        var neighbors = GetNeighborPositions(currentPosition, field);
        var currentDistance = distances[currentPosition.Y, currentPosition.X];

        var next = neighbors
                        .Select(a => (a, distances[a.Y, a.X]))
                        .Where(a => a.Item2 < currentDistance)
                        .ToList();
        
        currentPosition = next.First().a;
    }

    return path;
}

void AddPath(char[,] field, IEnumerable<Vector2> path) {
    foreach (var item in path)
    {
        field[item.Y, item.X] = 'O';
    }
}

void PrintField(char[,] field, HashSet<Vector2>? path) {
    var height = field.GetLength(0);
    var width = field.GetLength(1);

    for (int i = 0; i < height; i++) {
        for (int j = 0; j < width; j++) {
            if (field[i,j] is '#') {
                Write(field[i,j]);
            } else if (path?.Contains(new Vector2(i,j)) ?? false) {
                Write('O');
            } else {
                Write('.');
            }
        }

        WriteLine();
    }
}

void PrintDistances(int[,] distances, params Vector2[] highlightPositions) {
    var height = distances.GetLength(0);
    var width = distances.GetLength(1);

    for (int i = 0; i < height; i++) {
        for (int j = 0; j < width; j++) {
            if (highlightPositions.Contains(new Vector2(i,j))) {
                ForegroundColor = ConsoleColor.Yellow;
                Write($"{distances[i,j],3}");
                ForegroundColor = ConsoleColor.Black;
            } else if (distances[i,j] > 0) {
                ForegroundColor = ConsoleColor.Red;
                Write($"{distances[i,j],3}");
                ForegroundColor = ConsoleColor.Black;
            } else {
                Write($"{distances[i,j],3}");
            }
        }

        WriteLine();
    }

}

IEnumerable<Vector2> GetNeighborPositions(Vector2 position, char[,] field) {
    var height = field.GetLength(0);
    var width = field.GetLength(1);

    // Up
    if (position.Y > 0 && field[position.Y-1, position.X] != '#') {
        yield return new Vector2(position.Y-1, position.X);
    }

    // Right
    if (position.X < width - 1 && field[position.Y, position.X + 1] != '#') {
        yield return new Vector2(position.Y, position.X + 1);
    }

    // Down
    if (position.Y < height - 1 && field[position.Y+1, position.X] != '#') {
        yield return new Vector2(position.Y+1, position.X);
    }

    // Left
    if (position.X > 0 && field[position.Y, position.X - 1] != '#') {
        yield return new Vector2(position.Y, position.X - 1);
    }
}

IEnumerable<(Vector2 start, Vector2 end)> GetPossibleCheats(char[,] field, Vector2 position) {
    var height = field.GetLength(0);
    var width = field.GetLength(1);

    // Up
    if (position.Y > 2 && field[position.Y-1, position.X] == '#' && field[position.Y-2, position.X] != '#') {
        yield return (new Vector2(position.Y-1, position.X), new Vector2(position.Y-2, position.X));
    }

    // Right
    if (position.X < width - 2 && field[position.Y, position.X + 1] == '#' && field[position.Y, position.X + 2] != '#') {
        yield return (new Vector2(position.Y, position.X + 1), new Vector2(position.Y, position.X + 2));
    }

    // Down
    if (position.Y < height - 2 && field[position.Y+1, position.X] == '#' && field[position.Y+2, position.X] != '#') {
        yield return (new Vector2(position.Y+1, position.X), new Vector2(position.Y+2, position.X));
    }

    // Left
    if (position.X > 2 && field[position.Y, position.X - 1] == '#' && field[position.Y, position.X - 2] != '#') {
        yield return (new Vector2(position.Y, position.X - 1), new Vector2(position.Y, position.X - 2));
    }
}

IEnumerable<(Vector2 start, Vector2 end, int length)> GetPossibleCheatsLong(char[,] field, Vector2 position, int maxDistance) {
    var queue = new Queue<(Vector2 pos, int distance)>();

    var visited = new HashSet<Vector2>();
    visited.Add(position);

    queue.Enqueue((position, 0));

    var distances = new List<(Vector2 pos, int dist)>();
    while (queue.Count > 0) {
        var (pos, distance) = queue.Dequeue();

        var neighbors = GetNeighbors(pos, field);
        foreach (var neigbor in neighbors)
        {
            if (visited.Contains(neigbor.pos)) {
                continue;
            }
            visited.Add(neigbor.pos);

            if (neigbor.type == '#' && distance < maxDistance) {
                queue.Enqueue((neigbor.pos, distance + 1));
            } else if (distance < maxDistance) {
                distances.Add((neigbor.pos, distance + 1));
                queue.Enqueue((neigbor.pos, distance + 1));
            }
        }
    }

    return distances.Select(a => (position, a.pos, a.dist));
}

IEnumerable<(Vector2 pos, char type)> GetNeighbors(Vector2 position, char[,] field) {
    var height = field.GetLength(0);
    var width = field.GetLength(1);

    // Up
    if (position.Y > 0) {
        yield return (new Vector2(position.Y-1, position.X), field[position.Y-1, position.X]);
    }

    // Right
    if (position.X < width - 1) {
        yield return (new Vector2(position.Y, position.X + 1), field[position.Y, position.X + 1]);
    }

    // Down
    if (position.Y < height - 1) {
        yield return (new Vector2(position.Y+1, position.X), field[position.Y+1, position.X]);
    }

    // Left
    if (position.X > 0) {
        yield return (new Vector2(position.Y, position.X - 1), field[position.Y, position.X - 1]);
    }
}

enum Direction {
    Up,
    Right,
    Down,
    Left
}

record struct Vector2(int Y, int X);