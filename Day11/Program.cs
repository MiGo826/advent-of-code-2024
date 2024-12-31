using static System.Console;

//using var file = File.OpenText("input_sample.txt");
using var file = File.OpenText("input.txt");

var stones = new LinkedList<string>();

var input = file.ReadToEnd();
var splittedInput = input.Split(' ');
foreach (var item in splittedInput)
{
    stones.AddLast(item);
}


// Old solution
// for (int i = 0; i < 10; i++) {
//     PrintStones(i);

//     //WriteLine($"Iteration: {i}");

//     var current = stones.First;
//     while (current != null) {
//         var value = current.Value;
//         if (value == "0") 
//         {
//             current.Value = "1";
//         }
//         else if (value.Length % 2 == 0) 
//         {
//             var left = value.Substring(0, value.Length/2);
//             var right = value.Substring(value.Length/2, value.Length / 2);

//             stones.AddBefore(current, left);
//             current.Value = RemoveLeadingZeroes(right);
//         } 
//         else 
//         {
//             var num = long.Parse(value);
//             current.Value = (num * 2024).ToString();
//         }
//         current = current.Next;
//     }
// }

long totalCount = 0;
var memo = new long[2048,76];
foreach (var stone in stones) {
    totalCount += StonesCount(stone, 75);
}

WriteLine($"Stones count: {totalCount}");

long StonesCount(string value, int iteration) {
    if (iteration == 0) {
        return 1;
    }

    var num = long.Parse(value);
    if (num < 2048 && memo[num, iteration] > 0) {
        return memo[num, iteration];
    }

    long sum = 0;
    if (value == "0") 
    {
        sum = StonesCount("1", iteration - 1);
    }
    else if (value.Length % 2 == 0) 
    {
        var left = value.Substring(0, value.Length/2);
        var right = value.Substring(value.Length/2, value.Length / 2);
        right = RemoveLeadingZeroes(right);

        sum = StonesCount(left, iteration - 1) + StonesCount(right, iteration - 1);
    } 
    else 
    {
        var newVal = (num * 2024).ToString();
        sum = StonesCount(newVal, iteration - 1);
    }

    if (num < 2048) {
        memo[num, iteration] = sum;
    }

    return sum;
}

string RemoveLeadingZeroes(string value) {
    var num = long.Parse(value);

    return num.ToString();
}

void PrintStones(int iteration) {
    WriteLine($"Iteration: {iteration}");
    foreach (var item in stones)
    {
        Write(item);
        Write(' ');
    }
    WriteLine();
    WriteLine($"Stones count: {stones.Count}");
    WriteLine();
}