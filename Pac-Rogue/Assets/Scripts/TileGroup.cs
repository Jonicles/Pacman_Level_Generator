using System;
using System.Collections.Generic;

public enum TileGroupShape
{
    Plus,
    TriangleUp,
    TriangleDown,
    TriangleRight,
    TriangleLeft,
    CornerUpRight,
    CornerDownRight,
    CornerUpLeft,
    CornerDownLeft,
    VerticalLine,
    HorizontalLine,
    Empty,
}

public class TileGroup
{
    public List<TileGroupShape> AvailableShapes { get; private set; }

    public TileGroup()
    {
        AvailableShapes = new List<TileGroupShape>((TileGroupShape[])Enum.GetValues(typeof(TileGroupShape)));
    }

    public void RemoveShape(TileGroupShape shape)
    {
        AvailableShapes.Remove(shape);
    }
}
