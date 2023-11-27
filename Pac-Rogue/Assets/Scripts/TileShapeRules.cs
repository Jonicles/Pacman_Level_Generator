using System;
using System.Collections.Generic;

public enum Direction
{
    North,
    South,
    East,
    West
}

public static class TileShapeRules
{
    public static TileGroupShape[] ConnectionNorth = { TileGroupShape.Plus, TileGroupShape.TriangleNorth, TileGroupShape.TriangleEast, TileGroupShape.TriangleWest, TileGroupShape.CornerNorthEast, TileGroupShape.CornerNorthWest, TileGroupShape.VerticalLine};
    public static TileGroupShape[] NoConnectionNorth = { TileGroupShape.TriangleSouth, TileGroupShape.CornerSouthEast, TileGroupShape.CornerSouthWest, TileGroupShape.HorizontalLine, TileGroupShape.Empty };

    public static TileGroupShape[] ConnectionSouth = { TileGroupShape.Plus, TileGroupShape.TriangleSouth, TileGroupShape.TriangleEast, TileGroupShape.TriangleWest, TileGroupShape.CornerSouthEast, TileGroupShape.CornerSouthWest, TileGroupShape.VerticalLine};
    public static TileGroupShape[] NoConnectionSouth = { TileGroupShape.TriangleNorth, TileGroupShape.CornerNorthEast, TileGroupShape.CornerNorthWest, TileGroupShape.HorizontalLine, TileGroupShape.Empty };

    public static TileGroupShape[] ConnectionEast = { TileGroupShape.Plus, TileGroupShape.TriangleNorth, TileGroupShape.TriangleSouth, TileGroupShape.TriangleEast, TileGroupShape.CornerNorthEast, TileGroupShape.CornerSouthEast, TileGroupShape.HorizontalLine};
    public static TileGroupShape[] NoConnectionEast = { TileGroupShape.TriangleWest, TileGroupShape.CornerNorthWest, TileGroupShape.CornerSouthWest, TileGroupShape.VerticalLine, TileGroupShape.Empty };

    public static TileGroupShape[] ConnectionWest = { TileGroupShape.Plus, TileGroupShape.TriangleNorth, TileGroupShape.TriangleSouth, TileGroupShape.TriangleWest, TileGroupShape.CornerNorthWest, TileGroupShape.CornerSouthWest, TileGroupShape.HorizontalLine};
    public static TileGroupShape[] NoConnectionWest = { TileGroupShape.TriangleEast, TileGroupShape.CornerNorthEast, TileGroupShape.CornerSouthEast, TileGroupShape.VerticalLine, TileGroupShape.Empty };

    public static Dictionary<Tuple<TileGroupShape, Direction>, TileGroupShape[]> ShapesToRemove = new()
    {
        //Plus
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.Plus, Direction.North), NoConnectionSouth },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.Plus, Direction.South), NoConnectionNorth },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.Plus, Direction.East), NoConnectionWest},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.Plus, Direction.West), NoConnectionEast},

        //TriangleUp
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleNorth, Direction.North), NoConnectionSouth},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleNorth, Direction.South), ConnectionNorth},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleNorth, Direction.East), NoConnectionWest},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleNorth, Direction.West), NoConnectionEast},

        //TriangleDown
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleSouth, Direction.North), ConnectionSouth},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleSouth, Direction.South), NoConnectionNorth},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleSouth, Direction.East), NoConnectionWest},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleSouth, Direction.West), NoConnectionEast},

        //TriangleRight
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleEast, Direction.North), NoConnectionSouth },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleEast, Direction.South), NoConnectionNorth},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleEast, Direction.East), NoConnectionWest},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleEast, Direction.West), ConnectionEast},

        //TriangleLeft
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleWest, Direction.North), NoConnectionSouth},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleWest, Direction.South), NoConnectionNorth},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleWest, Direction.East), ConnectionWest},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleWest, Direction.West), NoConnectionEast},

        //CornerUpRight
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerNorthEast, Direction.North), NoConnectionSouth},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerNorthEast, Direction.South), ConnectionNorth},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerNorthEast, Direction.East), NoConnectionWest},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerNorthEast, Direction.West), ConnectionEast},

        //CornerDownRight
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerSouthEast, Direction.North), ConnectionSouth},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerSouthEast, Direction.South), NoConnectionNorth},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerSouthEast, Direction.East), NoConnectionWest},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerSouthEast, Direction.West), ConnectionEast},

        //CornerUpLeft
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerNorthWest, Direction.North), NoConnectionSouth },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerNorthWest, Direction.South), ConnectionNorth},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerNorthWest, Direction.East), ConnectionWest},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerNorthWest, Direction.West), NoConnectionEast},

        //CornerDownLeft
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerSouthWest, Direction.North), ConnectionSouth},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerSouthWest, Direction.South), NoConnectionNorth},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerSouthWest, Direction.East), ConnectionWest},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerSouthWest, Direction.West), NoConnectionEast},

        //VerticalLine
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.VerticalLine, Direction.North), NoConnectionSouth},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.VerticalLine, Direction.South), NoConnectionNorth},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.VerticalLine, Direction.East), ConnectionWest},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.VerticalLine, Direction.West), ConnectionEast},

        //HorizontalLine
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.HorizontalLine, Direction.North), ConnectionSouth},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.HorizontalLine, Direction.South), ConnectionNorth},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.HorizontalLine, Direction.East), NoConnectionWest},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.HorizontalLine, Direction.West), NoConnectionEast},

        //Empty
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.Empty, Direction.North), ConnectionSouth},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.Empty, Direction.South), ConnectionNorth},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.Empty, Direction.East), ConnectionWest},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.Empty, Direction.West), ConnectionEast},
    };

    public static bool[,] GetShapeDefinition(TileGroupShape shape)
    {
        bool[,] definitionArray = shape switch
        {
            TileGroupShape.Plus => new bool[3, 3] {
                    { false, true, false },
                    { true, true, true },
                    { false, true, false } },
            TileGroupShape.TriangleNorth => new bool[3, 3] {
                    { false, true, false },
                    { false, true, true },
                    { false, true, false } },
            TileGroupShape.TriangleSouth => new bool[3, 3] {
                    { false, true, false },
                    { true, true, false },
                    { false, true, false } },
            TileGroupShape.TriangleEast => new bool[3, 3] {
                    { false, false, false },
                    { true, true, true },
                    { false, true, false } },
            TileGroupShape.TriangleWest => new bool[3, 3] {
                    { false, true, false },
                    { true, true, true },
                    { false, false, false } },
            TileGroupShape.CornerNorthEast => new bool[3, 3] {
                    { false, false, false },
                    { false, true, true },
                    { false, true, false } },
            TileGroupShape.CornerSouthEast => new bool[3, 3] {
                    { false, false, false },
                    { true, true, false },
                    { false, true, false } },
            TileGroupShape.CornerNorthWest => new bool[3, 3] {
                    { false, true, false },
                    { false, true, true},
                    { false, false, false } },
            TileGroupShape.CornerSouthWest => new bool[3, 3] {
                    { false, true, false },
                    { true, true, false},
                    { false, false, false } },
            TileGroupShape.VerticalLine => new bool[3, 3] {
                    { false, false, false },
                    { true, true, true},
                    { false, false, false } },
            TileGroupShape.HorizontalLine => new bool[3, 3] {
                    { false, true, false },
                    { false, true, false},
                    { false, true, false } },
            TileGroupShape.Empty => new bool[3, 3] {
                    { false, false, false },
                    { false, false, false},
                    { false, false, false } },
            _ => new bool[3, 3] {
                    { false, false, false },
                    { false, false, false},
                    { false, false, false } },
        };
        return definitionArray;
    }
}

