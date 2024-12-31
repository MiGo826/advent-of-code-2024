using static System.Console;

//using var file = File.OpenText("input_sample.txt");
using var file = File.OpenText("input.txt");

const int size = 71;
const int startBytesCount = 1024;

var field = new char[size, size];
var bytes = new List<Vector2>();

while (!file.EndOfStream) {
    var line = file.ReadLine();
    var coords = line.Split(',').Select(int.Parse).ToArray();
    var item = new Vector2(coords[1], coords[0]);
    bytes.Add(item);
}

var startPosition = new Vector2(0,0);
var endPosition = new Vector2(size - 1, size - 1);
AddBytes(field, bytes, startBytesCount);


Task2();

void Task1() {
    var distances = FindDistances(field, startPosition, endPosition);
    PrintField(field);
    WriteLine();
    PrintDistances(distances);
    WriteLine($"Minimum steps: {distances[endPosition.Y, endPosition.X]}");
}

void Task2() {
    var currentByteIndex = startBytesCount;

    var distances = FindDistances(field, startPosition, endPosition);
    var path = FindShortesPath(distances, field, startPosition, endPosition);
    var havePath = true;
    
    while (havePath && currentByteIndex < bytes.Count) {
        var currentByte = bytes[currentByteIndex];

        AddByte(field, currentByte);
        if (path.Contains(currentByte)) {
            distances = FindDistances(field, startPosition, endPosition);
            
            if (distances[endPosition.Y, endPosition.X] == 0) {
                WriteLine($"No paths. Last byte fell: ({currentByte.X},{currentByte.Y})");
                break;
            }

            path = FindShortesPath(distances, field, startPosition, endPosition);
        }

        currentByteIndex++;
    }
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

void AddByte(char[,] field, Vector2 fallenByte) {
    field[fallenByte.Y, fallenByte.X] = '#';
} 

void AddBytes(char[,] field, List<Vector2> bytes, int count) {
    for (int i = 0; i < count; i++) {
        AddByte(field, bytes[i]);
    }
}

void PrintField(char[,] field) {
    var height = field.GetLength(0);
    var width = field.GetLength(1);

    for (int i = 0; i < height; i++) {
        for (int j = 0; j < width; j++) {
            if (field[i,j] is '#' or 'O') {
                Write(field[i,j]);
            } else {
                Write('.');
            }
        }

        WriteLine();
    }
}

void PrintDistances(int[,] distances) {
    var height = distances.GetLength(0);
    var width = distances.GetLength(1);

    for (int i = 0; i < height; i++) {
        for (int j = 0; j < width; j++) {
            Write($"{distances[i,j],3}");
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

enum Direction {
    Up,
    Right,
    Down,
    Left
}

record struct Vector2(int Y, int X);