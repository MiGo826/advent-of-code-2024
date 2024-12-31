using System.Collections;
using static System.Console;

//using var file = File.OpenText("input_sample.txt");
using var file = File.OpenText("input.txt");

var initialSecretNumbers = new List<int>();

while (!file.EndOfStream) {
    var line = file.ReadLine();
    var num = int.Parse(line);
    initialSecretNumbers.Add(num);
}

Task2();

void Task2() {
    var cache = new Dictionary<Sequence, int>();

    for (int i = 0; i < initialSecretNumbers.Count; i++)
    {
        int secretNumber = initialSecretNumbers[i];
        FillSequences(secretNumber, i, 2000, cache);
        var max = cache.MaxBy(a => a.Value);
        var seq = new Sequence(-2,1,-1,3);
        //WriteLine($"For number {secretNumber} sequence {seq} gives {cache[seq]} bananas");
    }

    var maxSequence = cache.MaxBy(a => a.Value);

    WriteLine($"Best sequence: {maxSequence.Key} with best sum of {maxSequence.Value}");
}

void FillSequences(long secretNumber, int buyerId, int evolveTimes, Dictionary<Sequence, int> cache) {
    var diffs = new LinkedList<int>();
    var foundSequences = new HashSet<Sequence>();

    var previousDigit = secretNumber % 10;
    for (int i = 1; i < evolveTimes; i++)
    {
        secretNumber = Evolve(secretNumber);
        var digit = (int)(secretNumber % 10);
        var diff = (int)(digit - previousDigit);
        diffs.AddLast(diff);
        
        if (diffs.Count == 4) {
            var seq = Sequence.ReadFrom(diffs);
            if (!foundSequences.Contains(seq)) {
                if (cache.TryGetValue(seq, out var sum)) {
                    cache[seq] = sum + digit;
                } else {
                    cache[seq] = digit;
                }
                foundSequences.Add(seq);
            }
            diffs.RemoveFirst();
        }

        previousDigit = digit;
    }
}

void Task1() {
    long sum = 0;
    foreach (var secretNumber in initialSecretNumbers)
    {
        var newNumber = EvolveMultiple(secretNumber, 2000);
        WriteLine($"{secretNumber}: {newNumber}");

        sum += newNumber;
    }
    WriteLine($"Sum: {sum}");
}

long EvolveMultiple(long secretNumber, int times) {
    for (int i = 0; i < times; i++)
    {
        secretNumber = Evolve(secretNumber);
    }
    return secretNumber;
}


long Evolve(long secretNumber) {
    var mul = secretNumber * 64;
    secretNumber = Mix(secretNumber, mul);
    secretNumber = Prune(secretNumber);

    var div = secretNumber / 32;
    secretNumber = Mix(secretNumber, div);
    secretNumber = Prune(secretNumber);

    mul = secretNumber * 2048;
    secretNumber = Mix(secretNumber, mul);
    secretNumber = Prune(secretNumber);

    return secretNumber;
}

long Mix(long first, long second) {
    return first ^ second;
}

long Prune(long number) {
    return number % 16777216;
}

record struct Sequence(int first, int second, int third, int fourth) {
    public static Sequence ReadFrom(IEnumerable<int> enumerable) {
        var nums = enumerable.Take(4).ToArray();
        return new Sequence(nums[0], nums[1], nums[2], nums[3]);
    }

    public override string ToString()
    {
        return $"{first}, {second}, {third}, {fourth}";
    }
};