using System.Numerics;
using static System.Console;

//using var file = File.OpenText("input_sample.txt");
using var file = File.OpenText("input.txt");

var maze = new List<List<char>>();

while (!file.EndOfStream) {
    var line = file.ReadLine();
    maze.Add(line.ToList());
}

var startTile = new Vector2(maze.Count - 2, 1);
var endTile = new Vector2(1, maze[1].Count - 2);

var costs = new int[maze.Count, maze[1].Count];
var steps = new (int tiles, int turns, bool visited)[maze.Count, maze[1].Count];

var score = FindPath();
WriteLine($"Score: {score}");

//var bestTiles = FindAllShortesPaths(startTile, endTile, score);
var bestTiles = TraversePath(endTile, Direction.Down, score);
PrintScores(costs);
WriteLine($"Best tiles count: {bestTiles}");

///////////////////////////////////////////////
///               WARNING!                  ///
///                                         ///
///  ПОСЛЕ АДВЕНТА СЖЕЧЬ ВСЕ ЭТО НАХУЙ!!!1  ///
///                                         ///
///////////////////////////////////////////////

int FindPath() {
    var currentPosition = startTile;
    var currentDirection = Direction.Right;
    var currentScore = 0;
    var queue = new Queue<(Vector2, Direction, int)>();
    queue.Enqueue((currentPosition, currentDirection, 0));

    do {
        (currentPosition, currentDirection, currentScore) = queue.Dequeue();

        var neighbors = GetNeighbors(currentPosition);
        foreach (var neighbor in neighbors)
        {
            var newScore = currentDirection == neighbor.direction
                ? currentScore + 1 
                : currentScore + 1001;
            if (neighbor.score == 0 || neighbor.score > newScore) {
                var pos = neighbor.point;
                costs[pos.Y, pos.X] = newScore;
                queue.Enqueue((neighbor.point, neighbor.direction, newScore));

                var tiles = steps[currentPosition.Y, currentPosition.X].tiles + 1;
                var turns = currentDirection == neighbor.direction 
                                ? steps[currentPosition.Y, currentPosition.X].turns
                                : steps[currentPosition.Y, currentPosition.X].turns + 1;
                steps[pos.Y, pos.X] = (tiles, turns, false);
            }
        }
    } while (queue.Count != 0);

    var endSum = costs[endTile.Y, endTile.X];

    return endSum;
}

int FindAllShortesPaths(Vector2 startPosition, Vector2 endPosition, int finalScore) {
    var bestPath = new HashSet<Vector2>();
    var visited = new HashSet<Vector2>();

    Visit(startPosition, Direction.Right, 0);

    bool Visit(Vector2 position, Direction direction, int score) {
        visited.Add(position);
        steps[position.Y, position.X].visited = true;
        //Clear();
        //PrintScores(costs);
        //Thread.Sleep(500);

        if (position == endPosition && score == finalScore) {
            bestPath.Add(position);
            visited.Remove(position);
            return true;
        }

        if (position == endPosition || score > finalScore) {
            visited.Remove(position);
            steps[position.Y, position.X].visited = false;
            return false;
        }

        var neighbors = GetNeighbors(position).ToList();

        var havePath = false;
        foreach (var neighbor in neighbors)
        {
            if (visited.Contains(neighbor.point)) {
                continue;
            }
            var newScore = direction == neighbor.direction
                ? score + 1 
                : score + 1001;

            havePath = Visit(neighbor.point, neighbor.direction, newScore) || havePath;
        }

        if (havePath) {
            bestPath.Add(position);
            visited.Remove(position);
            return true;
        }

        visited.Remove(position);
        steps[position.Y, position.X].visited = false;
        return false;
    }

    foreach (var pos in bestPath)
    {
        steps[pos.Y, pos.X].visited = true;
    }

    return bestPath.Count;
} 

int TraversePath(Vector2 endPosition, Direction endDirection, int endScore) {
    var currentPosition = endTile;
    var currentScore = endScore;
    var previousScore = endScore;
    var currentDirection = endDirection;
    var queue = new Queue<(Vector2, Direction, int, int)>();
    queue.Enqueue((currentPosition, currentDirection, currentScore, previousScore));
    var bestPath = new HashSet<Vector2>();
    bestPath.Add(currentPosition);
    
    do {
        (currentPosition, currentDirection, currentScore, previousScore) = queue.Dequeue();
        bestPath.Add(currentPosition);
        WriteLine($"Queue size: {queue.Count}");
        //Clear();
        //PrintScores(costs);
        //Thread.Sleep(100);

        var neighbors = GetNeighbors(currentPosition);
        foreach (var neighbor in neighbors)
        {
            var pos = neighbor.point;
            var neighborSteps = steps[pos.Y, pos.X];
            var neighborScore = costs[pos.Y, pos.X];
            // var possibleNeighborScore = neighbor.direction == currentDirection 
            //                             ? currentScore - 1
            //                             : currentScore - 1000;

            if (neighborScore < currentScore || neighborScore == previousScore - 2) {
                queue.Enqueue((neighbor.point, neighbor.direction, neighborScore, currentScore));
                steps[pos.Y, pos.X].visited = true;
            }
        }
    } while (queue.Count != 0);

    return bestPath.Count;
}

void PrintMaze(List<List<char>> maze) {
    for (int i = 0; i < maze.Count; i++) {
        for (int j = 0; j < maze[i].Count; j++) {
            Write(maze[i][j]);
        }
        WriteLine();
    }
}

void PrintScores(int[,] scores) {
    var height = scores.GetLength(0);
    var width = scores.GetLength(1);

    var visitedCount = 0;
    for (int i = 0; i < height; i++) {
        for (int j = 0; j < width; j++) {
            ForegroundColor = steps[i,j].visited ? ConsoleColor.Red : ConsoleColor.Black;
            Write($"{scores[i,j],6}");
            visitedCount = steps[i,j].visited ? visitedCount + 1 : visitedCount;
        }
        WriteLine();
    }

    WriteLine($"Visited count: {visitedCount}");
} 

IEnumerable<(Vector2 point, Direction direction, int score)> GetNeighbors(Vector2 position) {
    // Up
    if (IsWithinMap(new Vector2(position.Y-1, position.X)) && maze[position.Y-1][position.X] != '#') {
        var pos = new Vector2(position.Y-1, position.X);
        var cost = costs[position.Y-1,position.X];;
        yield return (pos, Direction.Up, cost);
    }

    // Right
    if (IsWithinMap(new Vector2(position.Y, position.X + 1)) && maze[position.Y][position.X + 1] != '#') {
        var pos = new Vector2(position.Y, position.X + 1);
        var cost = costs[position.Y,position.X + 1];
        yield return (pos, Direction.Right, cost);
    }

    // Down
    if (IsWithinMap(new Vector2(position.Y+1, position.X)) && maze[position.Y+1][position.X] != '#') {
        var pos = new Vector2(position.Y+1, position.X);
        var cost = costs[position.Y+1,position.X];
        yield return (pos, Direction.Down, cost);
    }

    // Left
    if (IsWithinMap(new Vector2(position.Y, position.X - 1)) && maze[position.Y][position.X - 1] != '#') {
        var pos = new Vector2(position.Y, position.X - 1);
        var cost = costs[position.Y,position.X - 1];
        yield return (pos, Direction.Left, cost);
    }
}

bool IsWithinMap(Vector2 point) {
    return point.X >= 0 && point.X < maze.Count && point.Y >= 0 && point.Y < maze.Count;
}

Direction TurnRight(Direction direction) {
    return (Direction)((int)(direction + 1) % 4);
}

Direction TurnLeft(Direction direction) {
    return (Direction)((int)(direction - 1 + 4) % 4);
}

enum Direction {
    Up,
    Right,
    Down,
    Left
}

record struct Vector2(int Y, int X);