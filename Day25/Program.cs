using Day25;
using static System.Console;

//using var file = File.OpenText("input_sample.txt");
using var file = File.OpenText("input.txt");

var keys = new List<int[]>();
var locks = new List<int[]>();
var lines = new List<string>();
while (!file.EndOfStream) {
    
    var line = file.ReadLine();
    while (line != string.Empty && !file.EndOfStream) {
        lines.Add(line);
        line = file.ReadLine();
    }

    if (lines[0].All(a => a == '#')) {
        var lck = ParseHelpers.ParseLock(lines);
        locks.Add(lck);
    } else {
        var key = ParseHelpers.ParseKey(lines);
        keys.Add(key);
    }
    lines.Clear();
}


var fittingPairs = 0;
foreach (var lck in locks)
{
    foreach (var key in keys)
    {
        if (IsFit(key, lck)) {
            fittingPairs++;
        }
    }
}

WriteLine($"Fitting pairs: {fittingPairs}");

bool IsFit(int[] key, int[] lck) {
    for (int i = 0; i < key.Length; i++)
    {
        if (key[i] + lck[i] > 5) return false;
    }

    return true;
}