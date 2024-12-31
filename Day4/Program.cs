using var file = File.OpenText("input.txt");

var rows = new List<List<char>>();

while (!file.EndOfStream) {
    var line = file.ReadLine();
    rows.Add(line.ToList());
}

var count = 0;
for (int i = 0; i < rows.Count; i++) {
    for (int j = 0; j < rows[i].Count; j++) {
        // Horizontal search
        try {
            var test = MakeString(rows[i][j], rows[i][j+1], rows[i][j+2], rows[i][j+3]);
            if (test is "XMAS" or "SAMX") {
                count++;
            }
        } catch {
            // nothing to see here
        }

        // Vertical search
        try {
            var test = MakeString(rows[i][j], rows[i+1][j], rows[i+2][j], rows[i+3][j]);
            if (test is "XMAS" or "SAMX") {
                count++;
            }
        } catch {
            // nothing to see here
        }

        // \-diagonal search
        try {
            var test = MakeString(rows[i][j], rows[i+1][j+1], rows[i+2][j+2], rows[i+3][j+3]);
            if (test is "XMAS" or "SAMX") {
                count++;
            }
        } catch {
            // nothing to see here
        }

        // /-diagonal search
        try {
            var test = MakeString(rows[i][j], rows[i-1][j+1], rows[i-2][j+2], rows[i-3][j+3]);
            if (test is "XMAS" or "SAMX") {
                count++;
            }
        } catch {
            // nothing to see here
        }
    } 
}

Console.WriteLine($"XMAS count: {count}");

var masCount = 0;
for (int i = 0; i < rows.Count; i++) {
    for (int j = 0; j < rows[i].Count; j++) {
        if (rows[i][j] != 'A') 
        {
            continue;
        }

        try {
            var first = MakeString(rows[i-1][j-1], rows[i][j], rows[i+1][j+1]) is "MAS" or "SAM";
            var second = MakeString(rows[i+1][j-1], rows[i][j], rows[i-1][j+1]) is "MAS" or "SAM";
            if (first && second) {
                masCount++;
            }
        } catch {
            // nothing to see here
        }
    }
}

Console.WriteLine($"X-MAS count: {masCount}");


string MakeString(params char[] chars) {
    return new string(chars);
}