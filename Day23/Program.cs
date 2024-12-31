using System.Collections;
using System.Text;
using static System.Console;

//using var file = File.OpenText("input_sample.txt");
using var file = File.OpenText("input.txt");

var connections = new List<(string node1, string node2)>();
while (!file.EndOfStream) {
    var line = file.ReadLine()!;
    var split = line.Split('-');
    var connection = (split[0], split[1]);
    connections.Add(connection);
}

var distinctHosts = connections.SelectMany(a => new string[] { a.node1, a.node2 }).Distinct().Order().ToList();
var hostsCount = distinctHosts.Count;

var hostNamesMappings = new Dictionary<string, int>();
for (int i = 0; i < distinctHosts.Count; i++) {
    hostNamesMappings[distinctHosts[i]] = i;
}

var connectionMatrix = new int[distinctHosts.Count, distinctHosts.Count];
foreach (var connection in connections)
{
    var (left, right) = connection;
    var leftId = hostNamesMappings[left];
    var rightId = hostNamesMappings[right];
    connectionMatrix[leftId, rightId] = 1;
    connectionMatrix[rightId, leftId] = 1;

    connectionMatrix[leftId, leftId] = 1;
    connectionMatrix[rightId, rightId] = 1;
}

var bitmaps = new BitArray[hostsCount];
for (int i = 0; i < hostsCount; i++)
{
    bitmaps[i] = new BitArray(hostsCount);
    for (int j = 0; j < hostsCount; j++)
    {
        bitmaps[i][j] = connectionMatrix[i,j] == 1;
    }
}

var triplets = new Dictionary<string, BitArray>();
var tripletsWithT = new HashSet<string>();
for (int i = 0; i < hostsCount - 1; i++) {
    for (int j = i + 1; j < hostsCount; j++) {
        if (connectionMatrix[i, j] == 1) {
            for (int k = 0; k < hostsCount; k++) {
                if (k != i && k != j && connectionMatrix[j,k] == 1 && connectionMatrix[i,k] == 1) {
                    var first = distinctHosts[i];
                    var second = distinctHosts[j];
                    var third = distinctHosts[k];
                    var bitmap = bitmaps[i].And(bitmaps[j].And(bitmaps[k]));

                    var triplet = MakeTriplet(first, second, third);
                    triplets.TryAdd(triplet, bitmap);

                    if (first.StartsWith('t') || second.StartsWith('t') || second.StartsWith('t') ) {
                        tripletsWithT.Add(triplet);
                    }
                }
            }
        }
    }
}

//PrintMatrix(connectionMatrix, hostNamesMappings["co"],hostNamesMappings["de"],hostNamesMappings["ka"],hostNamesMappings["ta"]);

WriteLine($"Found {triplets.Count} triplets");
// foreach (var triplet in triplets)
// {
//     WriteLine(triplet);
// }

WriteLine($"Found {tripletsWithT.Count} triplets that have one host starting with t");
// foreach (var triplet in tripletsWithT)
// {
//     WriteLine(triplet);
// }

var orderedTriplets = triplets
.OrderByDescending(a => TrueBitsCount(a.Value)).ThenBy(a => a.Key)
.Select(a => new { triplet = a.Key, count = TrueBitsCount(a.Value), bitmap = a.Value})
.Where(a => a.count == 13);
foreach (var item in orderedTriplets)
{
    WriteLine($"Triplet {item.triplet} have {item.count} items in common: {StringConnections(item.bitmap)}");
}



string MakeTriplet(string first, string second, string third) {
    var arr = new string[] {first, second, third};
    Array.Sort(arr);
    return string.Join(',', arr);
}

void PrintMatrix(int[,] matrix, params int[] highlightLines) {
    var size = matrix.GetLength(0);
    Write("   ");
    for (int i = 0; i < size; i++)
    {
        Write($"{distinctHosts[i],3}");
    }
    WriteLine();

    for (int i = 0; i < size; i++)
    {
        
        if (highlightLines.Contains(i)) {
            ForegroundColor = ConsoleColor.Red;
        }

        Write(distinctHosts[i]);
        for (int j = 0; j < size; j++)
        {
            Write($"{matrix[i,j],3}");
        }
        WriteLine();

        ForegroundColor = ConsoleColor.Black;
    }
}

void PrintConnectionsCounts() {
    var connectionCounts = new int[hostsCount];
    for (int i = 0; i < hostsCount; i++) {
        for (int j = 0; j < hostsCount; j++) {
            if (connectionMatrix[i, j] == 1 && i != j) {
                connectionCounts[i]++;
            }
        }
    }

    for (int i = 0; i < connectionCounts.Length; i++)
    {
        WriteLine($"{distinctHosts[i]}={connectionCounts[i]}, ");
    }
}

int TrueBitsCount(BitArray bitArray) {
    // var ints = new uint[(bitArray.Count >> 5) + 1];
    // bitArray.CopyTo(ints, 0);
    //var count = 0;
    // for (var i = 0; i < ints.Length; i++) {
    //     count += BitOperations.PopCount(ints[i]);
    // }

    var count = 0;
    for (var i = 0; i < bitArray.Count; i++) {
        count = bitArray[i] == true ? count + 1 : count;
    }
    return count;
}

string StringConnections(BitArray bitArray) {
    var builder = new StringBuilder();
    for (var i = 0; i < bitArray.Count; i++) {
        if (bitArray[i] == true) {
            builder.Append(distinctHosts[i]);
            builder.Append(',');
        }
    }

    return builder.ToString();
}