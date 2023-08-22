public struct Coordinate
{
    public int X { get; }
    public int Y { get; }

    public Coordinate(int x, int y)
    {
        X = x; Y = y;
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}

public enum TileState { Empty, Occupied }
