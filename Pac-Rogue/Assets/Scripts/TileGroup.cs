using UnityEngine;
using System.Collections.Generic;
using System;
public enum TileGroupShape
{
    Plus,
    TriangleNorth,
    TriangleSouth,
    TriangleEast,
    TriangleWest,
    CornerNorthEast,
    CornerSouthEast,
    CornerNorthWest,
    CornerSouthWest,
    VerticalLine,
    HorizontalLine,
    Empty,
}

public class TileGroup
{
    public List<TileGroupShape> AvailableShapes { get; private set; }
    public TileGroupShape DefiniteShape { get; private set; }

    public bool DefiniteShapeSet { get; private set; }
    public TileGroup()
    {
        AvailableShapes = new List<TileGroupShape>((TileGroupShape[])Enum.GetValues(typeof(TileGroupShape)));
        //AvailableShapes = new List<TileGroupShape>();
        //AvailableShapes.Add(TileGroupShape.Plus);
        //AvailableShapes.Add(TileGroupShape.TriangleUp);
        //AvailableShapes.Add(TileGroupShape.TriangleDown);
        //AvailableShapes.Add(TileGroupShape.TriangleRight);
        //AvailableShapes.Add(TileGroupShape.TriangleLeft);
        //AvailableShapes.Add(TileGroupShape.CornerUpRight);
        //AvailableShapes.Add(TileGroupShape.CornerDownRight);
        //AvailableShapes.Add(TileGroupShape.CornerUpLeft);
        //AvailableShapes.Add(TileGroupShape.CornerDownLeft);
        //AvailableShapes.Add(TileGroupShape.VerticalLine);
        //AvailableShapes.Add(TileGroupShape.HorizontalLine);
        //AvailableShapes.Add(TileGroupShape.Empty);

    }

    public void RemoveAvailableShapes(TileGroupShape[] shapes)
    {
        if (AvailableShapes.Count == 1)
            return;

        foreach (TileGroupShape shape in shapes)
        {
            AvailableShapes.Remove(shape);
        }
    }

    public void SetDefiniteShape(TileGroupShape shape)
    {
        DefiniteShape = shape;

        AvailableShapes = new List<TileGroupShape>();
        AvailableShapes.Add(shape);
        DefiniteShapeSet = true;
    }

    public void SetRandomDefiniteShape()
    {
        SetDefiniteShape(AvailableShapes[UnityEngine.Random.Range(0, AvailableShapes.Count)]);
    }
}
