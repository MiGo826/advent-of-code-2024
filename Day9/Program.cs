using static System.Console;

//using var file = File.OpenText("input_sample.txt");
using var file = File.OpenText("input.txt");

var diskMap = file.ReadToEnd();

var disk = new LinkedList<Block>();  
var currentSize = 0;
for (int i = 0; i < diskMap.Length; i++) {
    var blockSize = (int)char.GetNumericValue(diskMap[i]);
    
    var isFile = i % 2 == 0;
    Block block;
    if (isFile) {
        var fileId = i / 2;
        block = new Block(fileId, currentSize, blockSize);
    } else {
        block = Block.CreateEmpty(currentSize, blockSize);
    }
    disk.AddLast(block);
    currentSize += blockSize;
}

PrintDisk();

var currentFileNode = disk.Last;
while (currentFileNode != disk.First) {
    if (currentFileNode!.Value.IsEmpty()) {
        currentFileNode = currentFileNode!.Previous;
        continue;
    }

    var fileBlock = currentFileNode.Value;
    
    var emptyBlockNode = disk.First;
    while (emptyBlockNode != currentFileNode)
    {
        var block = emptyBlockNode.Value;
        if (block.IsEmpty() && block.Size >= fileBlock.Size) {
            break;
        }
        emptyBlockNode = emptyBlockNode!.Next;
    }

    if (emptyBlockNode == currentFileNode) {
        currentFileNode = currentFileNode!.Previous;
        continue;
    }

    var emptyBlock = emptyBlockNode.Value;

    var fileBlockCopy = new Block(fileBlock.Id, emptyBlock.StartsFrom, fileBlock.Size);

    if (emptyBlock.Size > fileBlock.Size) {
        var start = emptyBlock.StartsFrom + fileBlock.Size;
        var size = emptyBlock.Size - fileBlock.Size;
        var leftSpace = Block.CreateEmpty(start, size);
        disk.AddAfter(emptyBlockNode, leftSpace);
    }

    disk.AddAfter(emptyBlockNode, fileBlockCopy);
    disk.Remove(emptyBlockNode);
    
    currentFileNode.Value = Block.CreateEmpty(fileBlock.StartsFrom, fileBlock.Size);
    currentFileNode = currentFileNode!.Previous;
}

PrintDisk();
var checksum = CalculateChecksum();
WriteLine($"Checksum: {checksum}");

void PrintDisk() {
    foreach (var value in disk) {
        for (int i = 0; i < value.Size; i++)
        {
            var str = value.IsEmpty() ? "." : value.Id.ToString();
            Write(str);
        }
    }
    WriteLine();
}

long CalculateChecksum() {
    long checksum = 0;
    foreach (var block in disk) {
        if (block.IsEmpty()) continue;

        for (int i = block.StartsFrom; i < block.StartsFrom + block.Size; i++) {
            checksum += i * block.Id;
        }
    }

    return checksum;
}

class Block {
    public int Id { get; }

    public int StartsFrom { get; }

    public int Size { get; }

    public Block(int id, int startsFrom, int size) {
        Id = id;
        StartsFrom = startsFrom;
        Size = size;
    }

    public static Block CreateEmpty(int startsFrom, int size) {
        return new Block(-1, startsFrom, size);
    }

    public static bool operator==(Block first, Block second) {
        return first.Id == second.Id;
    }

    public static bool operator!=(Block first, Block second) {
        return first.Id != second.Id;
    }

    public bool IsEmpty() {
        return Id == -1;
    }

    public override string ToString()
    {
        return Id >= 0 ?  Id.ToString() : ".";
    }

    public override bool Equals(object? obj)
    {
        if (obj is Block block) {
            return Id == block?.Id;
        }
        return base.Equals(obj);
    }
}
