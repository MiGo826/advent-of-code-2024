using System;

namespace Day25;

public static class ParseHelpers
{
    public static int[] ParseLock(List<string> lines) {
        var lineSize = lines[0].Length;
        var lck = new int[lineSize];
        for (int i = 1; i < lines.Count - 1; i++)
        {
            for (int j = 0; j < lineSize; j++)
            {
                lck[j] = lines[i][j] == '#' ? lck[j] + 1 : lck[j];
            }
        }

        return lck;
    }

    public static int[] ParseKey(List<string> lines) {
        var lineSize = lines[0].Length;
        var key = new int[lineSize];
        for (int i = 1; i < lines.Count - 1; i++)
        {
            for (int j = 0; j < lineSize; j++)
            {
                key[j] = lines[i][j] == '#' ? key[j] + 1 : key[j];
            }
        }

        return key;
    }
}
