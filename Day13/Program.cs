using System.Text.RegularExpressions;
using static System.Console;

//using var file = File.OpenText("input_sample.txt");
using var file = File.OpenText("input.txt");

long totalTokens = 0;
while (!file.EndOfStream) {
    var buttonAStr = file.ReadLine();
    var buttonA = ParseCoords(buttonAStr);

    var buttonBStr = file.ReadLine();
    var buttonB = ParseCoords(buttonBStr);

    var prizeStr = file.ReadLine();
    var prize = ParseCoords(prizeStr);

    file.ReadLine(); // Just an empty read

    var tokens = CalculateTokens(buttonA, buttonB, prize);
    WriteLine($"Tokens: {tokens}");
    totalTokens += tokens;
}

WriteLine($"Total tokens {totalTokens}");


(int x, int y) ParseCoords(string line) 
{
    var regex = "X(?:\\+|=)(?<X>\\d+),\\sY(?:\\+|=)(?<Y>\\d+)";
    var match = Regex.Match(line, regex);

    var x = int.Parse(match.Groups["X"].Value);
    var y = int.Parse(match.Groups["Y"].Value);

    return (x,y);
}

long CalculateTokens((int x, int y) a, (int x, int y) b, (long x, long y) prize) {

    var A = new int[2,2] {
        { a.x, b.x },
        { a.y, b.y }
    };
    var determinant = a.x * b.y - b.x * a.y;

    if (determinant == 0) return 0;

    var adjT = new int[2,2] {
        { A[1,1], -A[0,1] },
        { -A[1,0], A[0,0] }
    };

    prize.x += 10000000000000;
    prize.y += 10000000000000;

    (long x, long y) mul = (
        adjT[0,0] * prize.x + adjT[0,1] * prize.y,
        adjT[1,0] * prize.x + adjT[1,1] * prize.y
    );

    var isIntegerResult = mul.x % determinant == 0 && mul.y % determinant == 0;

    (long x, long y) result = (mul.x / determinant, mul.y / determinant);

    WriteLine($"Button presses: A - {result.x}, B - {result.y}");

    return isIntegerResult
            ? result.x * 3 + result.y * 1
            : 0;
}