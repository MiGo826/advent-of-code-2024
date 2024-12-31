using static System.Console;

//using var file = File.OpenText("input_sample.txt");
using var file = File.OpenText("input.txt");

var map = new List<List<char>>();

var antennas = new Dictionary<char, List<Point>>();

var line = 0;
while (!file.EndOfStream) {
    var str = file.ReadLine();
    map.Add(str.ToList());

    for (int x = 0; x < str.Length; x++) {
        if (char.IsLetterOrDigit(str[x])) {
            if (antennas.TryGetValue(str[x], out var values)) 
            {
                values.Add(new Point(line, x));
            }
            else 
            {
                antennas[str[x]] = new List<Point> { new Point(line, x) };
            }
        }
    }
    line++;
}

var mapSize = line;
var antinodesCount = 0;

foreach (var (key, values) in antennas) {
    // Попарно проверяем все антенны - ищем для каждой пары возможные положения антинод
    for (int i = 0; i < values.Count - 1; i++) {
        for (int j = i + 1; j < values.Count; j++) {
            var first = values[i];
            var second = values[j];

            var firstAntinodes = FindAntinodes(first, second);
            foreach (var antinode in firstAntinodes)
            {
                if (!IsAlreadyAntinode(antinode)) {
                    map[antinode.Y][antinode.X] = '#';
                    antinodesCount++;
                }
            }

            var secondAntinodes = FindAntinodes(second, first);
            foreach (var antinode in secondAntinodes)
            {
                if (!IsAlreadyAntinode(antinode)) {
                    map[antinode.Y][antinode.X] = '#';
                    antinodesCount++;
                }
            }
        }
    }
}

WriteLine($"Antinodes count: {antinodesCount}");

using var outputFile = File.OpenWrite("output.txt");
using var writer = new StreamWriter(outputFile);
foreach (var item in map)
{
    var str = new string(item.ToArray());
    writer.WriteLine(str);
}

List<Point> FindAntinodes(Point point1, Point point2) {
    var antinodes = new List<Point> { point1, point2 };

    var i = 2;
    var antinodesWithin = true;
    do {
        var antinode = FindAntinode(point1, point2, i);
        if (IsWithinMap(antinode)) {
            antinodesWithin = true;
            i++;
            antinodes.Add(antinode);
        } else {
            antinodesWithin = false;
        }
    } while (antinodesWithin);

    return antinodes;
}

Point FindAntinode(Point point1, Point point2, int scalar) {
    var vector = point2 - point1;
    return point1 + vector * scalar;
}

bool IsWithinMap(Point point) {
    return point.X >= 0 && point.X < mapSize && point.Y >= 0 && point.Y < mapSize;
}

bool IsAlreadyAntinode(Point point) {
    return map[point.Y][point.X] == '#';
}

struct Point {
    public int X { get; }

    public int Y { get; }
    
    public Point(int y, int x) {
        X = x;
        Y = y;
    } 

    public static Point operator+(Point a, Point b) {
        return new Point(a.Y + b.Y, a.X + b.X);
    }

    public static Point operator-(Point a, Point b) {
        return new Point(a.Y - b.Y, a.X - b.X);
    }

    public static Point operator*(Point a, int scalar) {
        return new Point(a.Y * scalar, a.X * scalar);
    }

    public override string ToString()
    {
        return $"(Y:{Y}, X:{X})";
    }
}