using static System.Console;

using var file = File.OpenText("input_sample.txt");
//using var file = File.OpenText("input.txt");

var registerA = ParseRegisterValue(file.ReadLine());
var registerB = ParseRegisterValue(file.ReadLine());
var registerC = ParseRegisterValue(file.ReadLine());
var registers = new Registers(registerA, (int)registerB, (int)registerC);

file.ReadLine();

var program = ParseProgram(file.ReadLine());

RunProgram();
//FindReverse();

void RunProgram() {
    var computer = new Computer(registers);
    var output = computer.Run(program);
    WriteLine("Output:");
    output.ForEach(a => Write($"{a},"));
}

void FindReverse() {
    Clear();
    var foundA = ReverseSearch(program);
    WriteLine($"Found A - {foundA}");
}
 

long ReverseSearch(List<int> program) {
    var registers = new Registers(0,0,0);
    var currentA = 0;
    var queue = new Queue<long>();
    queue.Enqueue(currentA);

    while (queue.Count > 0) {
        var value = queue.Dequeue();

        for (int j = 0; j <= 7; j++) {
            var temp = value + j;

            registers.A = temp;
            registers.B = 0;
            registers.C = 0;

            var computer = new Computer(registers);
            var output = computer.Run(program);

            if (output.Count > program.Count) {
                break;
            }

            var outputCorrect = true;
            for (int i = 0; i < output.Count; i++) {
                var commandIndex = program.Count - output.Count + i;
                if (output[i] != program[commandIndex]) {
                    outputCorrect = false;
                    break;
                }
            }

            if (outputCorrect) {
                WriteLine($"Found potential value {temp}");
                queue.Enqueue(temp << 3);
            }
        }
    }

    return currentA;
}

long ParseRegisterValue(string input) {
    var value = input.Split(':')[1];
    return long.Parse(value);
}

List<int> ParseProgram(string input) {
    var value = input.Split(':')[1];
    var codes = value.Split(',');
    var parsedCodes = codes.Select(int.Parse).ToList();

    return parsedCodes;
}

class Registers {
    public long A { get; set; }
    public long B { get; set; }
    public long C { get; set; }

    public Registers(long a, long b, long c) {
        A = a;
        B = b;
        C = c;
    }

    public void Reset() {
        A = 0; B = 0; C = 0;
    }
}

delegate bool InstructionDelegate(int operand);

class Computer {
    private Registers _registers;
    private OperandReader _operandReader;
    private int _instructionPointer = 0;
    private long _ticks = 0;
    private List<int> _output = new List<int>();

    public Computer(Registers registers) {
        _registers = registers;
        _operandReader = new OperandReader(registers);
    }

    public List<int> Run(List<int> program) {
        while (_instructionPointer < program.Count) {
            var command = program[_instructionPointer];
            var operand = program[_instructionPointer + 1];

            //PrintState(program, operand);
            //Thread.Sleep(500);

            var instrucionDelegate = GetInstructionDelegate(command);

            var dontMoveInstructionPointer = instrucionDelegate(operand);

            if (!dontMoveInstructionPointer) {
                _instructionPointer += 2;
            }

            _ticks++;
        }

        return _output;
    }

    public long FindCorrectA(List<int> program) {
        // last was 2268201
        // 49589305
        var currentA = 0;
        var maxCountFount = 0;
        while (_output.Count < program.Count) {
            Clear();

            currentA++;
            WriteLine($"Trying {currentA}");
            WriteLine($"Max output count: {maxCountFount}");
            Write("Last output: ");
            _output.ForEach(a => Write($"{a},"));
            
            _registers.Reset();
            _output.Clear();

            _registers.A = currentA;
            _instructionPointer = 0;
            
            while (_instructionPointer < program.Count) {
                var command = program[_instructionPointer];
                var operand = program[_instructionPointer + 1];

                //PrintState(program, operand);
                //Thread.Sleep(500);
                var countBeforeCall = _output.Count;

                var instrucionDelegate = GetInstructionDelegate(command);
                var dontMoveInstructionPointer = instrucionDelegate(operand);
                if (!dontMoveInstructionPointer) {
                    _instructionPointer += 2;
                }
                _ticks++;

                if (_output.Count > countBeforeCall) {
                    if (_output[_output.Count - 1] != program[_output.Count - 1]) {
                        break;
                    }

                    if (_output.Count > maxCountFount) {
                        maxCountFount = _output.Count;
                        currentA *= 8;
                    }
                }
            }
            
        }
        
        return currentA;
    }

    private InstructionDelegate GetInstructionDelegate(int command) {
        return command switch {
            0 => Adv,
            1 => Bxl,
            2 => Bst,
            3 => Jnz,
            4 => Bxc,
            5 => Out,
            6 => Bdv,
            7 => Cdv,
            _ => throw new ArgumentOutOfRangeException(nameof(command), $"Invalid command value {command}")
        };
    }

    private bool Adv(int operand) {
        var numerator = (double)_registers.A;
        var value = ReadComboOperand(operand);
        var denominator = Math.Pow(2, value);

        var result = (long)Math.Truncate(numerator / denominator);
        _registers.A = result;
        return false;
    }

    private bool Bxl(int operand) {
        _registers.B = _registers.B ^ operand;
        return false;
    }

    private bool Bst(int operand) {
        var value = ReadComboOperand(operand);
        _registers.B = value % 8;
        return false;
    }

    private bool Jnz(int operand) {
        if (_registers.A != 0) {
            _instructionPointer = operand;
            return true;
        }
        return false;
    }

    private bool Bxc(int operand) {
        _registers.B = _registers.B ^ _registers.C;
        return false;
    }

    private bool Out(int operand) {
        var value = ReadComboOperand(operand);
        var result = value % 8;
        _output.Add((int)result);
        return false;
    }

    private bool Bdv(int operand) {
        var numerator = (double)_registers.A;
        var value = ReadComboOperand(operand);
        var denominator = Math.Pow(2, value);

        var result = (long)Math.Truncate(numerator / denominator);
        _registers.B = result;
        return false;
    }

    private bool Cdv(int operand) {
        var numerator = (double)_registers.A;
        var value = ReadComboOperand(operand);
        var denominator = Math.Pow(2, value);

        var result = (long)Math.Truncate(numerator / denominator);
        _registers.C = result;
        return false;
    }

    private long ReadComboOperand(int operand) {
        return operand switch {
            >=0 and <=3 => operand,
            4 => _registers.A,
            5 => _registers.B,
            6 => _registers.C,
            _ => throw new ArgumentOutOfRangeException(nameof(operand), $"Invalid operand value {operand}")
        };
    }

    private void PrintState(List<int> program, int operand) {
        
        ForegroundColor = ConsoleColor.Black;

        Clear();
        WriteLine($"Register A: {_registers.A}");
        WriteLine($"Register B: {_registers.B}");
        WriteLine($"Register C: {_registers.C}");
        WriteLine();
        WriteLine($"Instruction pointer: {_instructionPointer}");
        WriteLine($"Ticks: {_ticks}");
        WriteLine();
        WriteLine($"Operand raw: {operand}");
        WriteLine($"Operand value: {_operandReader.Read(operand)}");
        WriteLine();

        for (int i = 0; i < program.Count; i += 2) {
            var dlg = GetInstructionDelegate(program[i]);
            var name = dlg.Method.Name;

            var color = i / 2 == _instructionPointer ? ConsoleColor.Red : ConsoleColor.Black;
            ForegroundColor = color;
            Write($"{name} ");
        }
    }
}

class OperandReader {
    private Registers _registers;

    public OperandReader(Registers registers) {
        _registers = registers;
    }

    public long Read(int operand) {
        return operand switch {
            >=0 and <=3 => operand,
            4 => _registers.A,
            5 => _registers.B,
            6 => _registers.C,
            _ => throw new ArgumentOutOfRangeException(nameof(operand), $"Invalid operand value {operand}")
        };
    }
}

