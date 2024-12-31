using System.Text.RegularExpressions;

using var file = File.OpenText("input.txt");

var text = file.ReadToEnd();

var regex = new Regex("mul\\((\\d+),(\\d+)\\)|do\\(\\)|don't\\(\\)");

var matches = regex.Matches(text).ToList();

var sum = 0;
var enabled = true;
foreach (var match in matches) {
    var command = match.Groups[0].Value;

    if (command.StartsWith("mul")) {
        if (enabled) {
            var arg1 = int.Parse(match.Groups[1].Value);
            var arg2 = int.Parse(match.Groups[2].Value);
            sum += arg1 * arg2;
        }
    } else if (command.StartsWith("don't")) {
        enabled = false;
    } else if (command.StartsWith("do")) {
        enabled = true;
    } else {
        Console.WriteLine($"Evaluation failed for command {command}");
    }
}

Console.WriteLine(sum);