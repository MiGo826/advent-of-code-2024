using System;
using System.Text.RegularExpressions;

namespace Day24;

public static class InputParser
{
    public static List<Input> ReadInputs(StreamReader file) {
        var inputs = new List<Input>();
        var line = file.ReadLine();
        do {
            var split = line.Split(':');
            
            var name = split[0];
            var value = split[1] == " 1";

            inputs.Add(new Input(name, value));
            line = file.ReadLine()!;
        } while (!string.IsNullOrEmpty(line));

        return inputs;
    }

    public static List<GateInfo> ReadGates(StreamReader file) {
        var gates = new List<GateInfo>();
        while (!file.EndOfStream) {
            var line = file.ReadLine();
            var gate = ParseGate(line);
            gates.Add(gate);
        }

        return gates;
    }

    private static GateInfo ParseGate(string line) 
    {
        var regex = "(?<input1>\\w+)\\s(?<operation>\\w+)\\s(?<input2>\\w+)\\s->\\s(?<output>\\w+)";
        var match = Regex.Match(line, regex);

        var input1 = match.Groups["input1"].Value;
        var input2 = match.Groups["input2"].Value;

        var operation = Enum.Parse<GateOp>(match.Groups["operation"].Value, true);

        var output = match.Groups["output"].Value;

        return new GateInfo(input1, operation, input2, output);
    }
}
