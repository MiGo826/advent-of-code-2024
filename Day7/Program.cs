using static System.Console;

//using var file = File.OpenText("input_sample.txt");
using var file = File.OpenText("input.txt");

long sum = 0;
while (!file.EndOfStream) 
{
    var line = file.ReadLine();
    var splittedString = line.Split(' ');

    var testValueChars = splittedString.First().Where(char.IsDigit).ToArray();
    var testValue = long.Parse(new string(testValueChars));

    var values = splittedString.Skip(1).Select(int.Parse).ToList();

    sum += CheckEquation(testValue, values);
}

WriteLine($"Sum: {sum}");

long CheckEquation(long testValue, List<int> values) {

    return CheckRecursive(values[0], 1) ? testValue : 0;


    bool CheckRecursive(long currentTotal, int currentItemIndex) {
        if (currentTotal > testValue) return false;

        if (currentItemIndex == values.Count) {
            return currentTotal == testValue;
        }

        var addition = CheckRecursive(currentTotal + values[currentItemIndex], currentItemIndex + 1);
        var multiplication = CheckRecursive(currentTotal * values[currentItemIndex], currentItemIndex + 1);
        var concatenation = CheckRecursive(Concat(currentTotal, values[currentItemIndex]), currentItemIndex + 1);
        return addition || multiplication || concatenation;

        long Concat(long left, int right) {
            var str = left.ToString() + right.ToString();
            return long.Parse(str);
        }
    }
}