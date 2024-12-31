using var file = File.OpenText("input.txt");

var leftBag = new PriorityQueue<int, int>();
var rightBah = new PriorityQueue<int, int>();
var rightDictionary = new Dictionary<int, int>();
var count = 0;

while (!file.EndOfStream) {
    var line = file.ReadLine();
    var split = line!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var left = int.Parse(split[0]);
    var right = int.Parse(split[1]);

    leftBag.Enqueue(left, left);
    rightBah.Enqueue(right, right);

    if (rightDictionary.ContainsKey(right)) 
    {
        rightDictionary[right]++;
    } 
    else 
    {
        rightDictionary[right] = 1;
    }

    count++;
}

var sum = 0;
var similarityScore = 0;
for (var i = 0; i < count; i++) {
    var left = leftBag.Dequeue();
    var right = rightBah.Dequeue();

    var distance = Math.Abs(left - right);
    sum += distance;

    var similarity = left * (rightDictionary.TryGetValue(left, out var rightCount) ? rightCount : 0);
    similarityScore += similarity;
}

Console.WriteLine($"Total distance: {sum}");
Console.WriteLine($"Similarity score: {similarityScore}");