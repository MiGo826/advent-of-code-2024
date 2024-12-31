using static System.Console;

//using var file = File.OpenText("input_sample.txt");
using var file = File.OpenText("input.txt");

var map = new List<List<int>>();

var mapSize = 0;
while (!file.EndOfStream) {
    var str = file.ReadLine();
    var heights = str!.Select(a => (int)char.GetNumericValue(a)).ToList();
    map.Add(heights);

    mapSize++;
}

var visits = new HashSet<(int, int)>();
var totalScore = 0;
for (int i = 0; i < mapSize; i++) {
    for (int j = 0; j < mapSize; j++) {
        if (map[i][j] == 0) {
            totalScore += GetTrailheadScore(i, j);
            visits.Clear();
        }
    }
}

WriteLine($"Total score: {totalScore}");

int GetTrailheadScore(int i, int j) {
    var height = map[i][j];
    if (height == 9) {
        return 1;
    }

    var score = 0;
    var steps = FindNextSteps(i, j, height);
    foreach (var step in steps)
    {
        if (!visits.Contains(step)) {
            score += GetTrailheadScore(step.i, step.j);
            //visits.Add(step);
        }
    }

    return score;
}

IEnumerable<(int i, int j)> FindNextSteps(int i, int j, int height) {
        // Up
        if (i > 0 && map[i-1][j] == height + 1) {
            yield return (i-1, j);
        }

        // Right
        if (j < mapSize - 1 && map[i][j+1] == height + 1) {
            yield return (i, j+1);
        }

        // Down
        if (i < mapSize - 1 && map[i+1][j] == height + 1) {
            yield return (i+1, j);
        }

        // Left
        if (j > 0 && map[i][j-1] == height + 1) {
            yield return (i, j-1);
        }
}

