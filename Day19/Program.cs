using System.Xml;
using static System.Console;

//using var file = File.OpenText("input_sample.txt");
using var file = File.OpenText("input.txt");

var towelsInput = file.ReadLine();
var towels = towelsInput.Split(", ").ToHashSet();
var maxTowelLength = towels.MaxBy(a => a.Length).Length;
file.ReadLine();

var designs = new List<string>();
while (!file.EndOfStream) {
    var input = file.ReadLine();
    designs.Add(input);
}

var possibleCache = new Dictionary<string, long>();
var impossiblesCache = new HashSet<string>();
var possibleDesignsCount = 0;
foreach (var design in designs)
{
    var possible = IsPossibleRecursive(design);
    if (possible) {
        possibleDesignsCount++;
    }

    WriteLine($"Design {design} is {(possible ? "Possible" : "Impossible")}");
}

WriteLine($"Possible designs: {possibleDesignsCount}");

long totalVariations = 0;
foreach (var design in designs)
{
    var possible = FindAllPossibleCounts(design);
    totalVariations += possible;

    WriteLine($"Possible {possible} designs for {design}");
}

WriteLine($"Possible designs variations: {totalVariations}");


bool IsPossibleRecursive(string design) {
    var count = Math.Min(maxTowelLength, design.Length);

    for (int i = 1; i <= count; i++) {
        var substr = design.Substring(0, i);

        if (towels.Contains(substr)) {
            var restOfString = design.Substring(i);

            if (impossiblesCache.Contains(restOfString)) {
                continue;
            }
            
            if (string.IsNullOrEmpty(restOfString)){
                return true;
            }
            
            if (IsPossibleRecursive(restOfString)) {
                return true;
            }
        }
    }

    impossiblesCache.Add(design);
    return false;
}

long FindAllPossibleCounts(string design) {
    if (string.IsNullOrEmpty(design)){
        return 1;
    }

    if (possibleCache.TryGetValue(design, out var amount)) {
        return amount;
    }

    var count = Math.Min(maxTowelLength, design.Length);

    long possibleSubDesigns = 0;
    for (int i = 1; i <= count; i++) {
        var substr = design.Substring(0, i);
        
        if (towels.Contains(substr)) {
            var restOfString = design.Substring(i);

            if (impossiblesCache.Contains(restOfString)) {
                continue;
            }
            
            possibleSubDesigns += FindAllPossibleCounts(restOfString);
        }
    }

    if (possibleSubDesigns == 0) {
        impossiblesCache.Add(design);
    } else {
        possibleCache[design] = possibleSubDesigns;
    }

    return possibleSubDesigns;
}