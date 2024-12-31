//using var file = File.OpenText("input_test.txt");
using var file = File.OpenText("input.txt");

var safeCount = 0;
while (!file.EndOfStream) {
    var line = file.ReadLine();
    var items = line!.Split(' ').Select(int.Parse).ToList()!;

    var isSafe = IsSafe(items);
    if (isSafe)
    {
        safeCount++;
    } 
    else 
    {
        for (int i = 0; i < items.Count; i++) {
            var itemsWithoutCurrent = items.Where((val, pos) => pos != i).ToList();
            if (IsSafeV2(itemsWithoutCurrent)){
                safeCount++;
                break;
            }
        }
    }
}

Console.WriteLine(safeCount);

// Virgin IsSafe
bool IsSafe(List<int> items) {
    var diffSum = 0;
    for (int i = 1; i < items.Count; i++) {
        var diff = items[i] - items[i-1];

        if (Math.Abs(diffSum + diff) <= Math.Abs(diffSum)
            || Math.Abs(diff) is (< 1) or (> 3)) 
        {
            return false;
        }

        diffSum += diff;
    }

    return true;
}

// Chad IsSafeV2
bool IsSafeV2(List<int> items) {
    var diffs = new List<int>();
    for (int i = 0; i < items.Count - 1; i++) {
        diffs.Add(items[i+1] - items[i]);
    }

    var isSafe = (diffs.All(a => a > 0) || diffs.All(a => a < 0)) && diffs.All(a => Math.Abs(a) <= 3);

    return isSafe;
}