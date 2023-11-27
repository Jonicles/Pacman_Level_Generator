public struct Coordinate
{
    public int X { get; }
    public int Y { get; }

    /// <summary>
    /// Shorthand for (0, 1)
    /// </summary>
    public static Coordinate North { get; } = new Coordinate(0, 1);
    /// <summary>
    /// Shorthand for (1, 1)
    /// </summary>
    public static Coordinate NorthEast { get; } = new Coordinate(1, 1);
    /// <summary>
    /// Shorthand for (1, 0)
    /// </summary>
    public static Coordinate East { get; } = new Coordinate(1, 0);
    /// <summary>
    /// Shorthand for (1, -1)
    /// </summary>
    public static Coordinate SouthEast { get; } = new Coordinate(1, -1);
    /// <summary>
    /// Shorthand for (0, -1)
    /// </summary>
    public static Coordinate South { get; } = new Coordinate(0, -1);
    /// <summary>
    /// Shorthand for (-1, -1)
    /// </summary>
    public static Coordinate SouthWest { get; } = new Coordinate(-1, -1);

    /// <summary>
    /// Shorthand for (-1, 0)
    /// </summary>
    public static Coordinate West { get; } = new Coordinate(-1, 0);

    /// <summary>
    /// Shorthand for (-1, 1)
    /// </summary>
    public static Coordinate NorthWest { get; } = new Coordinate(-1, 1);

    public Coordinate(int x, int y)
    {
        X = x; Y = y;
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }

    public static Coordinate operator +(Coordinate a, Coordinate b) => new(a.X + b.X, a.Y + b.Y);

    public static Coordinate operator -(Coordinate a, Coordinate b) => new(a.X - b.X, a.Y - b.Y);


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
        if (obj is not Coordinate coordinate)
            return false;

        return (coordinate.X == this.X && coordinate.Y == this.Y);

    }
}
