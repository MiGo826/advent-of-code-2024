using static System.Console;

//using var file = File.OpenText("input_sample.txt");
using var file = File.OpenText("input.txt");

var garden = new List<List<char>>();

var mapSize = 0;
while (!file.EndOfStream) {
    var line = file.ReadLine();

    garden.Add([.. line!]);

    mapSize++;
}

var totalPrice = 0;
var newTotalPrice = 0;
var visitsMap = new bool[mapSize,mapSize];
for (int i = 0; i < mapSize; i++) {
    for (int j = 0; j < mapSize; j++) {
        if (visitsMap[i,j]) {
            continue;
        }
        
        var regionTiles = new HashSet<(int i, int j)>();
        var regionCorners = new HashSet<(int i, int j, bool inner)>();
        var (area, perimeter) = VisitRegionTile(regionTiles, i,j);
        var price = area * perimeter;

        var regionName = garden[i][j];
        foreach (var tile in regionTiles)
        {
            var tileCorners = FindTileCorners(regionTiles, tile.i, tile.j, regionName).ToList();
            tileCorners.ForEach(a => regionCorners.Add(a));
        }
        var sides = regionCorners.Count;
        var newPrice = area * sides;

        totalPrice += price;
        newTotalPrice += newPrice;

        WriteLine($"Region: {regionName}: Area - {area}, Perimeter - {perimeter}, Price - {price}, Sides - {sides}, New price - {newPrice}");
    }
}

WriteLine($"Total price - {totalPrice}");
WriteLine($"New total price - {newTotalPrice}");

(int area, int perimeter) VisitRegionTile(HashSet<(int i, int j)> tiles, int i, int j) {
    if (tiles.Contains((i, j))) {
        return (0, 0);
    }
    tiles.Add((i,j));
    visitsMap[i, j] = true;

    var regionName = garden[i][j];
    var neighbors = FindTileNeighbors(i, j, regionName);

    var totalArea = 1;
    var totalPerimeter = 4 - neighbors.Count();
    foreach (var neighbor in neighbors)
    {
        (int neighborArea, int neighborPerimeter) = VisitRegionTile(tiles, neighbor.i, neighbor.j);
        totalArea += neighborArea;
        totalPerimeter += neighborPerimeter;
    }

    return (totalArea, totalPerimeter);
}

IEnumerable<(int i, int j)> FindTileNeighbors(int i, int j, char regionName) {
    // Up
    if (i > 0 && garden[i-1][j] == regionName) {
        yield return (i-1, j);
    }

    // Right
    if (j < mapSize - 1 && garden[i][j+1] == regionName) {
        yield return (i, j+1);
    }

    // Down
    if (i < mapSize - 1 && garden[i+1][j] == regionName) {
        yield return (i+1, j);
    }

    // Left
    if (j > 0 && garden[i][j-1] == regionName) {
        yield return (i, j-1);
    }
}

IEnumerable<(int i, int j, bool inner)> FindTileCorners(HashSet<(int i, int j)> regionTiles, int i, int j, char regionName) {
    if (IsCornerWithinMap(i, j) && IsRegionCorner(i, j)) {
        yield return (i, j, false);
        if (IsInnerCorner(i, j)) {
            yield return (i, j, true);
        }
    }

    if (IsCornerWithinMap(i, j + 1) && IsRegionCorner(i, j + 1)) {
        yield return (i, j + 1, false);
        if (IsInnerCorner(i, j + 1)) {
            yield return (i, j + 1, true);
        }
    }

    if (IsCornerWithinMap(i + 1, j + 1) && IsRegionCorner(i + 1, j + 1)) {
        yield return (i + 1, j + 1, false);
        if (IsInnerCorner(i + 1, j + 1)) {
            yield return (i + 1, j + 1, true);
        }
    }

    if (IsCornerWithinMap(i + 1, j) && IsRegionCorner(i + 1, j)) {
        yield return (i + 1, j, false);
        if (IsInnerCorner(i + 1, j)) {
            yield return (i + 1, j, true);
        }
    }

    bool IsRegionCorner(int y, int x) {
        var plants = GetPlantsAroundCorner(y, x).Where(a => regionTiles.Contains((a.y, a.x))).ToList();
        
        return plants.Count is 1 or 3 || plants.DistinctBy(a => a.x).Count() > 1 && plants.DistinctBy(a => a.y).Count() > 1 && plants.Count != 4;
    }

    bool IsInnerCorner(int y, int x) {
        var plants = GetPlantsAroundCorner(y, x).Where(a => regionTiles.Contains((a.y, a.x))).ToList();

        return plants.Count == 2 && plants.DistinctBy(a => a.x).Count() > 1 && plants.DistinctBy(a => a.y).Count() > 1;
    }

    IEnumerable<(int y, int x, char regionName)> GetPlantsAroundCorner(int i, int j) {
        if (IsWithinMap(i - 1, j - 1)) {
            yield return (i - 1, j - 1, garden[i-1][j-1]);
        }
        if (IsWithinMap(i - 1, j)) {
            yield return (i - 1, j, garden[i-1][j]);
        }
        if (IsWithinMap(i, j)) {
            yield return (i, j, garden[i][j]);
        }
        if (IsWithinMap(i, j - 1)) {
            yield return (i, j - 1, garden[i][j-1]);
        }
    }
}

bool IsWithinMap(int i, int j) {
    return i >= 0 && i < mapSize && j >= 0 && j < mapSize;
}

bool IsCornerWithinMap(int i, int j) {
    return i >= 0 && i < mapSize + 1 && j >= 0 && j < mapSize + 1;
}