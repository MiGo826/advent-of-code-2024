using static System.Console;

//using var file = File.OpenText("input_sample.txt");
using var file = File.OpenText("input.txt");

var warehouse = new IWarehouseObject[52,102];

var lineNum = 0;
var line = file.ReadLine();
Robot robot = null;
do {
    for (int i = 0; i < line.Length; i++) {
        var obj = line[i] switch {
            '#' => new Wall(new Vector2(lineNum, i*2), warehouse),
            'O' => new Box(new Vector2(lineNum, i*2), warehouse),
            '@' => new Robot(new Vector2(lineNum, i*2), warehouse),
            '.' => (IWarehouseObject?)null
        };
        if (obj is Wall) {
            var secondWall = new Wall(new Vector2(lineNum, i*2 + 1), warehouse);
        } 
        if (obj is Robot rbt) {
            robot = rbt;
        }
    }

    line = file.ReadLine();
    lineNum++;

} while (!string.IsNullOrEmpty(line));

if (robot is null) throw new Exception("Robot is null!!!");


var movements = new List<Direction>();
while (!file.EndOfStream) {
    var movementsStr = file.ReadLine();
    foreach (var item in movementsStr!)
    {
        var direction =  ParseDirection(item);
        movements.Add(direction);
    }
}

warehouse.Print();
foreach (var movement in movements) {
    warehouse.Print();
    WriteLine($"Next movement: {movement}");
    Thread.Sleep(400);
    //ReadKey();

    robot.TryPush(movement);
    Clear();
}

var coordsSum = warehouse.CalculateCoords();
WriteLine($"Coords sum: {coordsSum}");

char GetDirectionChar(Direction direction) {
    return direction switch
    {
        Direction.Up => '^',
        Direction.Right => '>',
        Direction.Down => 'v',
        Direction.Left => '<',
    };
}

Direction ParseDirection(char symbol) {
    return symbol switch {
        '^' => Direction.Up,
        '>' => Direction.Right,
        'v' => Direction.Down,
        '<' => Direction.Left,
        _ => throw new ArgumentOutOfRangeException(nameof(symbol))
    };
}

enum Direction {
    Up,
    Right,
    Down,
    Left
}

record struct Vector2(int Y, int X);

interface IWarehouseObject {
    
    Vector2 Position { get; }

    char Id { get; }
}

class Wall: IWarehouseObject
{
    public static char Name = '#';

    public Vector2 Position { get; }

    public char Id => Name;

    public IWarehouseObject[,] Warehouse { get; }

    public Wall(Vector2 position, IWarehouseObject[,] warehouse) {
        Position = position;
        Warehouse = warehouse;
        warehouse.Put(this);
    }
}

abstract class MovableObject: IWarehouseObject {
    public Vector2 Position { get; protected set;}

    public virtual char Id { get; }

    public IWarehouseObject[,] Warehouse { get; }

    public MovableObject(Vector2 position, IWarehouseObject[,] warehouse) {
        Position = position;
        Warehouse = warehouse;
        warehouse.Put(this);
    }

    public bool TryPush(Direction direction) {
        var position = GetNextPosition(Position, direction);
        var nextObj = Warehouse.Get(position);

        var canMove = nextObj switch {
            Wall => false,
            Box box => box.CanMove(direction),
            null => true
        };

        if (canMove) {
            Move(direction);
            return true;
        } 
        
        return false;
    }

    protected virtual bool CanMove(Direction direction) {
        var position = GetNextPosition(Position, direction);
        var nextObj = Warehouse.Get(position);

        return nextObj switch {
            Wall => false,
            Box box => box.CanMove(direction),
            null => true
        };
    }

    protected virtual void Move(Direction direction) {
        var position = GetNextPosition(Position, direction);
        var nextObj = Warehouse.Get(position);

        var movableObject = nextObj as MovableObject;
        movableObject?.Move(direction);

        Warehouse.PutEmpty(Position);
        Position = position;
        Warehouse.Put(this);
    } 

    protected Vector2 GetNextPosition(Vector2 position, Direction direction) {
        return direction switch
        {
            Direction.Up => new Vector2(position.Y - 1, position.X),
            Direction.Right => new Vector2(position.Y, position.X + 1),
            Direction.Down => new Vector2(position.Y + 1, position.X),
            Direction.Left => new Vector2(position.Y, position.X - 1)
        };
    } 
}

class Box: MovableObject
{
    private char _id;

    public override char Id => _id;

    public Box ConjugateBox { get; }

    public Box(Vector2 position, IWarehouseObject[,] warehouse): base(position, warehouse) {
        _id = '[';
        ConjugateBox = new Box(new Vector2(position.Y, position.X + 1), warehouse, this);
    }

    private Box(Vector2 position, IWarehouseObject[,] warehouse, Box conjugateBox): base(position, warehouse) {
        _id = ']';
        ConjugateBox = conjugateBox;
    }

    protected override bool CanMove(Direction direction)
    {
        if (direction is Direction.Left or Direction.Right) {
            return base.CanMove(direction);
        }

        return CanMoveConjugate(direction) && ConjugateBox.CanMoveConjugate(direction);
    }

    protected override void Move(Direction direction)
    {
        if (direction is Direction.Left or Direction.Right) {
            base.Move(direction);
        } else {
            MoveCojugate(direction);
            ConjugateBox.MoveCojugate(direction);
        }   
    }

    protected bool CanMoveConjugate(Direction direction) {
        var position = GetNextPosition(Position, direction);
        var nextObj = Warehouse.Get(position);

        return nextObj switch {
            Wall => false,
            Box box => box.CanMove(direction),
            null => true
        };
    } 

    protected void MoveCojugate(Direction direction) {
        var position = GetNextPosition(Position, direction);
        var nextObj = Warehouse.Get(position);

        var movableObject = nextObj as Box;
        movableObject?.Move(direction);

        Warehouse.PutEmpty(Position);
        Position = position;
        Warehouse.Put(this);
    }
}

class Robot: MovableObject
{
    public static char Name = '@';

    public override char Id => Name;

    public Robot(Vector2 position, IWarehouseObject[,] warehouse): base(position, warehouse) {
    }
}

static class WarehouseHelpers {
    public static void Put(this IWarehouseObject[,] warehouse, IWarehouseObject obj) {
        if (!warehouse.IsPointWithin(obj.Position)) {
            throw new Exception($"Point {obj.Position} of {obj.Id} is outside of warehouse!");
        }

        var position = obj.Position;
        warehouse[position.Y, position.X] = obj;
    }

    public static void PutEmpty(this IWarehouseObject[,] warehouse, Vector2 position) {
        if (!warehouse.IsPointWithin(position)) {
            throw new Exception($"Point {position} is outside of warehouse!");
        }

        warehouse[position.Y, position.X] = null;
    }

    public static IWarehouseObject Get(this IWarehouseObject[,] warehouse, Vector2 position) {
        if (!warehouse.IsPointWithin(position)) {
            throw new Exception($"Point {position} is outside of warehouse!");
        }

        return warehouse[position.Y, position.X];
    }

    public static bool IsPointWithin(this IWarehouseObject[,] warehouse, Vector2 position) {
        var height = warehouse.GetLength(0);
        var width = warehouse.GetLength(1);

        return position.Y >= 0 && position.Y < height && position.X >= 0 && position.X < width;
    }

    public static void Print(this IWarehouseObject[,] warehouse) {
        var height = warehouse.GetLength(0);
        var width = warehouse.GetLength(1);

        for (int i = 0; i < height; i++) {
            for (int j = 0; j < width; j++) {
                var item = warehouse[i,j];
                if (item != null) {
                    Write(item.Id);
                } 
                else {
                    Write('.');
                }
            }
            WriteLine();
        }
    }

    public static long CalculateCoords(this IWarehouseObject[,] warehouse) {
        var height = warehouse.GetLength(0);
        var width = warehouse.GetLength(1);

        var total = 0;
        for (int i = 0; i < height; i++) {
            for (int j = 0; j < width; j++) {
                var item = warehouse[i,j];
                if (item is Box box && box.Id == '[') {
                    total += box.Position.Y * 100 + box.Position.X;
                } 
            }
        }

        return total;
    }
}