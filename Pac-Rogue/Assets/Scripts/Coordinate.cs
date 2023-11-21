public struct Coordinate
{
    public int X { get; }
    public int Y { get; }

    /// <summary>
    /// Shorthand for (0, 1)
    /// </summary>
    public static Coordinate north { get; } = new Coordinate(0, 1);
    /// <summary>
    /// Shorthand for (1, 1)
    /// </summary>
    public static Coordinate northEast { get; } = new Coordinate(1, 1);
    /// <summary>
    /// Shorthand for (1, 0)
    /// </summary>
    public static Coordinate east { get; } = new Coordinate(1, 0);
    /// <summary>
    /// Shorthand for (1, -1)
    /// </summary>
    public static Coordinate southEast { get; } = new Coordinate(1, -1);
    /// <summary>
    /// Shorthand for (0, -1)
    /// </summary>
    public static Coordinate south { get; } = new Coordinate(0, -1);
    /// <summary>
    /// Shorthand for (-1, -1)
    /// </summary>
    public static Coordinate southWest { get; } = new Coordinate(-1, -1);

    /// <summary>
    /// Shorthand for (-1, 0)
    /// </summary>
    public static Coordinate west { get; } = new Coordinate(-1, 0);

    /// <summary>
    /// Shorthand for (-1, 1)
    /// </summary>
    public static Coordinate northWest { get; } = new Coordinate(-1, 1);

    public Coordinate(int x, int y)
    {
        X = x; Y = y;
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }

    public static Coordinate operator +(Coordinate a, Coordinate b) => new Coordinate(a.X + b.X, a.Y + b.Y);

    public static Coordinate operator -(Coordinate a, Coordinate b) => new Coordinate(a.X - b.X, a.Y - b.Y);


    public static bool operator ==(Coordinate a, Coordinate b)
    {
        return (a.X == b.X && a.Y == b.Y);
    }

    public static bool operator !=(Coordinate a, Coordinate b)
    {
        return (a.X != b.X || a.Y != b.Y);
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Coordinate coordinate))
            return false;

        return (coordinate.X == this.X && coordinate.Y == this.Y);

    }
}
