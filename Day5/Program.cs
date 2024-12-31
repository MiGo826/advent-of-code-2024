using var file = File.OpenText("input_sample.txt");
//using var file = File.OpenText("input.txt");

var comparisonMatrix = new int[100,100];
var sum = 0;
var fixedSum = 0;

var readingRules = true;
while (!file.EndOfStream) {
    var line = file.ReadLine();

    if (string.IsNullOrWhiteSpace(line)) {
        readingRules = false;
        continue;
    }

    if (readingRules) {
        var split = line.Split('|');
        var left = int.Parse(split[0]);
        var right = int.Parse(split[1]);

        // Means left must be before right
        comparisonMatrix[left,right] = -1;
        comparisonMatrix[right,left] = 1;
    } else {
        var split = line.Split(',');
        var updates = split.Select(int.Parse).ToList();
        var result = CheckUpdates(updates);
        if (result > 0) {
            sum += result;
        } else {
            fixedSum += FixUpdates(updates);
        }
    }
}

Console.WriteLine($"Correst updates sum: {sum}");
Console.WriteLine($"Fixed updates sum: {fixedSum}");

int CheckUpdates(List<int> data) {
    for (int i = 0; i < data.Count - 1; i++) {
        for (int j = i + 1; j < data.Count; j++) {
            var left = data[i];
            var right = data[j];
            if (comparisonMatrix[left, right] > 0) {
                return 0;
            }
        }
    }

    var middle = data.Count / 2;
    return data[middle];
}

int FixUpdates(List<int> data) {
    for (int i = 0; i < data.Count - 1; i++) {
        for (int j = i + 1; j < data.Count; j++) {
            var left = data[i];
            var right = data[j];
            if (comparisonMatrix[left, right] > 0) {
                (data[j], data[i]) = (data[i], data[j]);
            }
        }
    }

    var middle = data.Count / 2;
    return data[middle];
}