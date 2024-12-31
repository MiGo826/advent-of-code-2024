using System.Text.RegularExpressions;
using static System.Console;

//using var file = File.OpenText("input_sample.txt");
using var file = File.OpenText("input.txt");

var robots = new List<Robot>();

while (!file.EndOfStream) {
    var input = file.ReadLine();
    var robot = ParseRobot(input!);
    robots.Add(robot);
}

const int time = 10000;
const int width = 101;
const int height = 103;
// const int width = 11;
// const int height = 7;



for (var t = 0; t <= time; t++) {
    var bathroom = new int[height, width];
    foreach (var robot in robots)
    {
        var position = robot.FindPositionAfter(t);
        var i = position.Y % height >= 0 ? position.Y % height : height + position.Y % height;
        var j = position.X % width >= 0 ? position.X % width : width + position.X % width;
        bathroom[i, j]++;
    }
    SaveBathroomToFile(bathroom, t);
}


// PrintBathroom(bathroom);
// CalculateSafetyFactor(bathroom);

Robot ParseRobot(string input) {
    var pattern = @"p=(?<px>-?\d+),(?<py>-?\d+)\sv=(?<vx>-?\d+),(?<vy>-?\d+)";
    var match = Regex.Match(input, pattern);
    var groups = match.Groups;

    var position = new Vector2(int.Parse(groups["px"].Value), int.Parse(groups["py"].Value));
    var velocity = new Vector2(int.Parse(groups["vx"].Value), int.Parse(groups["vy"].Value));

    return new Robot(position, velocity);
}

int CalculateSafetyFactor(int[,] bathroom) {
    var height = bathroom.GetLength(0);
    var width = bathroom.GetLength(1);

    var quadrants = new int[4];

    for (int i = 0; i < height; i++)
    {
        for (int j = 0; j < width; j++)
        {
            var value = bathroom[i,j];
            quadrants[0] += i < height/2 && j < width/2 ? value : 0;
            quadrants[1] += i < height/2 && j > width/2 ? value : 0;
            quadrants[2] += i > height/2 && j > width/2 ? value : 0;
            quadrants[3] += i > height/2 && j < width/2 ? value : 0;
        }
    }

    var totalSafetyScore = 1;
    for (int i = 0; i < quadrants.Length; i++)
    {
        WriteLine($"Quadrant {i} score - {quadrants[i]}");
        totalSafetyScore *= quadrants[i];
    }

    WriteLine($"Total safety score - {totalSafetyScore}");
    return totalSafetyScore;
}

void PrintBathroom(int[,] bathroom) {
    for (int i = 0; i < bathroom.GetLength(0); i++)
    {
        for (int j = 0; j < bathroom.GetLength(1); j++)
        {
            var value = bathroom[i,j];
            var valueToPrint = value == 0 ? "." : value.ToString();
            Write(valueToPrint);
        }
        WriteLine();
    }
}

void SaveBathroomToFile(int[,] bathroom, int time) {
    using var file = File.Open("output.txt", FileMode.Append);
    using var writer = new StreamWriter(file);

    writer.WriteLine($"Time: {time}");

    for (int i = 0; i < bathroom.GetLength(0); i++)
    {
        for (int j = 0; j < bathroom.GetLength(1); j++)
        {
            var value = bathroom[i,j];
            var valueToPrint = value == 0 ? "." : value.ToString();
            writer.Write(valueToPrint);
        }
        writer.WriteLine();
    }

    writer.WriteLine();
}

class Robot(Vector2 Position, Vector2 Velocity) {

    public Vector2 FindPositionAfter(int time) {
        var x = Velocity.X * time + Position.X;
        var y = Velocity.Y * time + Position.Y;
        
        return new Vector2(x,y);
    }
}

record struct Vector2(int X, int Y);

