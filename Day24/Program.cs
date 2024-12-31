using System.Collections;
using Day24;
using static System.Console;

using var file = File.OpenText("input_sample.txt");
//using var file = File.OpenText("input.txt");

var inputs = InputParser.ReadInputs(file);
var gates = InputParser.ReadGates(file);

var inputsX = inputs.Where(a => a.Name.StartsWith('x')).ToList();
var inputsY = inputs.Where(a => a.Name.StartsWith('y')).ToList();

var x = InputsHelper.InputToInt64(inputsX);
var y = InputsHelper.InputToInt64(inputsY);
WriteLine($"X = {x}");
WriteLine($"Y = {y}");

var z = x + y;
WriteLine($"Z = {z}");
var zbytes = BitConverter.GetBytes(z);
var zbits = new BitArray(zbytes);

var computer = new Computer(inputs, gates);
computer.SwapWires("z10", "kmb");
computer.SwapWires("z15", "tvp");
computer.SwapWires("z25", "dpg");
computer.SwapWires("vdk", "mmf");

var result = computer.GetOutputAsDecimal();
WriteLine($"Actual Z = {result}");

WriteLine($"Difference = {Convert.ToString(z ^ result, 2)}");

foreach (var output in computer.ResultingOutput)
{
    var id = InputsHelper.GetNum(output);
    WriteLine($"Output {output.Name} is connected to gate {output.Parent.Name} - ({ToNum(output.Value)} {(zbits[id] != output.Value ? $"but should be {ToNum(zbits[id])}": "")})");
}

WriteLine();

foreach (var output in computer.GetOutputs(BadXors))
{
    Write($"Output {output.Name} is connected to gate {output.Parent.Name} ");
    if (output.Parent is Gate gate) {
        var parent1 = (gate.Input1 as Output)?.Parent;
        var parent2 = (gate.Input2 as Output)?.Parent;
        Write($"which connected to gates {parent1?.Name} and {parent2?.Name}");
    }
    WriteLine();
}

var list = new List<string> {"z10", "kmb", "z15", "tvp", "z25", "dpg", "vdk", "mmf"};
list.Sort();
WriteLine($"Task2 result: {string.Join(',', list)}");

bool IsOutputConnectedToOr(Output output) {
    return output.Parent is Gate gate && gate.Operation == GateOp.Or;  
}

bool MmfOrTsw(Output output) {
    return output.Name is "mmf" or "tsw";
}

bool BadXors(Output output) {
    var parent = output.Parent;

    if (parent is Gate gate && gate.Operation == GateOp.Xor) {
        // if (gate.Input1 is Input && gate.Input2 is Input) {
        //     return false;
        // }

        // if (gate.Input1 is Gate left && gate.Input2 is Gate right && left.Operation is GateOp.Or or GateOp.Xor && right.Operation is GateOp.Or or GateOp.Xor) {
        //     return false;
        // }

        return true;
    }

    return false;
}

string ToNum(bool b) => b ? "1" : "0";  

class Computer {
    private Dictionary<string, INode> _inputsAndOutputs = new Dictionary<string, INode>();

    private List<Output> _resultingOutputs = new List<Output>();

    public IReadOnlyCollection<Output> ResultingOutput => _resultingOutputs;

    public Computer(List<Input> inputs, List<GateInfo> gates) {
        inputs.ForEach(a => _inputsAndOutputs[a.Name] = a);
        BuildGates(gates);
    }

    public long GetOutputAsDecimal() {
        var outputBinary = new BitArray(_resultingOutputs.Count);
        foreach (var output in _resultingOutputs)
        {
            var value = output.Value;
            var id = int.Parse(output.Name.Substring(1));
            outputBinary[id] = value;
        }

        long result = 0;
        for (int i = _resultingOutputs.Count - 1; i >= 0; i--) {
            Write(outputBinary[i] ? '1' : '0');
            if (outputBinary[i]) {
                result += (long)Math.Pow(2, i);
            }
        }
        WriteLine();
        return result;
    }

    public IEnumerable<Output> GetOutputs(Func<Output, bool> predicate) {
        foreach (var item in _inputsAndOutputs)
        {
            if (item.Value is Output output && predicate(output)) {
                yield return output;
            }
        }
    }

    public void SwapWires(string first, string second) {
        var firstOutput = _inputsAndOutputs[first] as Output;
        var secondOutput = _inputsAndOutputs[second] as Output;

        if (firstOutput is null || secondOutput is null) {
            throw new InvalidOperationException("Some of the outputs is not Output");
        }

        firstOutput.SwapWires(secondOutput);
    }

    private void BuildGates(List<GateInfo> gates) {
        var linkedList = new LinkedList<GateInfo>(gates);
        while (linkedList.Any()) {
            var current = linkedList.First!.Value;

            if (_inputsAndOutputs.TryGetValue(current.Input1, out var inputNode1) 
                    && _inputsAndOutputs.TryGetValue(current.Input2, out var inputNode2)) {
                
                var gate = new Gate(inputNode1, current.Operation, inputNode2, current.Output);
                _inputsAndOutputs[gate.Output.Name] = gate.Output;

                if (gate.Output.Name.StartsWith('z')) {
                    _resultingOutputs.Add(gate.Output);
                }

            } else {
                linkedList.AddLast(linkedList.First.Value);
            }

            linkedList.RemoveFirst();
        }

        _resultingOutputs.Sort((input1, input2) => input1.Name.CompareTo(input2.Name));
    }
}

public interface INode {
    string Name { get; }

    bool Value { get; }
}

public class Input : INode
{
    public string Name { get; }

    public bool Value { get; }

    public Input(string name, bool value) {
        Name = name;
        Value = value;
    }
}

public class Gate: INode {
    private bool? _savedValue;

    public INode Input1 { get;}

    public INode Input2 { get; }

    public GateOp Operation { get; }

    public Output Output { get; private set; }

    public string Name => ToString();

    public bool Value => GetValue();

    public Gate(INode input1, GateOp operation, INode input2, string outputName) {
        Input1 = input1;
        Operation = operation;
        Input2 = input2;
        Output = new Output(this, outputName);
    }

    public void ChangeOutput(Output output) {
        Output = output;
    }

    private bool GetValue() {
        if (_savedValue != null) {
            return _savedValue.Value;
        }

        _savedValue = Operation switch
        {
            GateOp.And => Input1.Value && Input2.Value,
            GateOp.Or => Input1.Value || Input2.Value,
            GateOp.Xor => Input1.Value ^ Input2.Value,
        };

        return _savedValue.Value;
    }

    public override string ToString()
    {
        return $"{Input1.Name} {Operation} {Input2.Name} -> {Output.Name}";
    }
}

public class Output: INode {
    private INode _node;

    public string Name { get; }

    public bool Value => _node.Value;

    public INode Parent => _node;

    public Output(INode input, string name) {
        _node = input;
        Name = name;
    }

    public void SwapWires(Output other) {
        (_node as Gate)?.ChangeOutput(other);
        (other._node as Gate)?.ChangeOutput(this);
        (_node, other._node) = (other._node, _node);
    }
}

public enum GateOp {
    And, Or, Xor
}

public record struct GateInfo(string Input1, GateOp Operation, string Input2, string Output);

public static class InputsHelper {
    public static long InputToInt64(List<Input> inputs) {
        long result = 0;
        inputs.Sort((input1, input2) => input1.Name.CompareTo(input2.Name));

        for (int i = inputs.Count - 1; i >= 0; i--) {
            if (inputs[i].Value) {
                result += (long)Math.Pow(2, i);
            }
        }

        return result;
    }

    public static int GetNum(Output output) {
        return int.Parse(output.Name.Substring(1));
    }
}
