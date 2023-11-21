using System;
using System.Collections.Generic;

public enum Direction
{
    Up,
    Down,
    Right,
    Left
}

public static class TileShapeRules
{
    public static TileGroupShape[] ConnectionUp = { TileGroupShape.Plus, TileGroupShape.TriangleUp, TileGroupShape.TriangleRight, TileGroupShape.TriangleLeft, TileGroupShape.CornerUpRight, TileGroupShape.CornerUpLeft, TileGroupShape.VerticalLine};
    public static TileGroupShape[] NoConnectionUp = { TileGroupShape.TriangleDown, TileGroupShape.CornerDownRight, TileGroupShape.CornerDownLeft, TileGroupShape.HorizontalLine, TileGroupShape.Empty };

    public static TileGroupShape[] ConnectionDown = { TileGroupShape.Plus, TileGroupShape.TriangleDown, TileGroupShape.TriangleRight, TileGroupShape.TriangleLeft, TileGroupShape.CornerDownRight, TileGroupShape.CornerDownLeft, TileGroupShape.VerticalLine};
    public static TileGroupShape[] NoConnectionDown = { TileGroupShape.TriangleUp, TileGroupShape.CornerUpRight, TileGroupShape.CornerUpLeft, TileGroupShape.HorizontalLine, TileGroupShape.Empty };

    public static TileGroupShape[] ConnectionRight = { TileGroupShape.Plus, TileGroupShape.TriangleUp, TileGroupShape.TriangleDown, TileGroupShape.TriangleRight, TileGroupShape.CornerUpRight, TileGroupShape.CornerDownRight, TileGroupShape.HorizontalLine};
    public static TileGroupShape[] NoConnectionRight = { TileGroupShape.TriangleLeft, TileGroupShape.CornerUpLeft, TileGroupShape.CornerDownLeft, TileGroupShape.VerticalLine, TileGroupShape.Empty };

    public static TileGroupShape[] ConnectionLeft = { TileGroupShape.Plus, TileGroupShape.TriangleUp, TileGroupShape.TriangleDown, TileGroupShape.TriangleLeft, TileGroupShape.CornerUpLeft, TileGroupShape.CornerDownLeft, TileGroupShape.HorizontalLine};
    public static TileGroupShape[] NoConnectionLeft = { TileGroupShape.TriangleRight, TileGroupShape.CornerUpRight, TileGroupShape.CornerDownRight, TileGroupShape.VerticalLine, TileGroupShape.Empty };

    public static Dictionary<Tuple<TileGroupShape, Direction>, TileGroupShape[]> ShapesToRemove = new Dictionary<Tuple<TileGroupShape, Direction>, TileGroupShape[]>
    {
        //Plus
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.Plus, Direction.Up), NoConnectionDown },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.Plus, Direction.Down), NoConnectionUp },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.Plus, Direction.Right), NoConnectionLeft},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.Plus, Direction.Left), NoConnectionRight},

        //TriangleUp
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleUp, Direction.Up), NoConnectionDown},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleUp, Direction.Down), ConnectionUp},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleUp, Direction.Right), NoConnectionLeft},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleUp, Direction.Left), NoConnectionRight},

        //TriangleDown
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleDown, Direction.Up), ConnectionDown},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleDown, Direction.Down), NoConnectionUp},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleDown, Direction.Right), NoConnectionLeft},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleDown, Direction.Left), NoConnectionRight},

        //TriangleRight
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleRight, Direction.Up), NoConnectionDown },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleRight, Direction.Down), NoConnectionUp},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleRight, Direction.Right), NoConnectionLeft},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleRight, Direction.Left), ConnectionRight},

        //TriangleLeft
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleLeft, Direction.Up), NoConnectionDown},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleLeft, Direction.Down), NoConnectionUp},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleLeft, Direction.Right), ConnectionLeft},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleLeft, Direction.Left), NoConnectionRight},

        //CornerUpRight
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerUpRight, Direction.Up), NoConnectionDown},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerUpRight, Direction.Down), ConnectionUp},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerUpRight, Direction.Right), NoConnectionLeft},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerUpRight, Direction.Left), ConnectionRight},

        //CornerDownRight
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerDownRight, Direction.Up), ConnectionDown},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerDownRight, Direction.Down), NoConnectionUp},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerDownRight, Direction.Right), NoConnectionLeft},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerDownRight, Direction.Left), ConnectionRight},

        //CornerUpLeft
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerUpLeft, Direction.Up), NoConnectionDown },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerUpLeft, Direction.Down), ConnectionUp},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerUpLeft, Direction.Right), ConnectionLeft},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerUpLeft, Direction.Left), NoConnectionRight},

        //CornerDownLeft
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerDownLeft, Direction.Up), ConnectionDown},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerDownLeft, Direction.Down), NoConnectionUp},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerDownLeft, Direction.Right), ConnectionLeft},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerDownLeft, Direction.Left), NoConnectionRight},

        //VerticalLine
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.VerticalLine, Direction.Up), NoConnectionDown},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.VerticalLine, Direction.Down), NoConnectionUp},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.VerticalLine, Direction.Right), ConnectionLeft},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.VerticalLine, Direction.Left), ConnectionRight},

        //HorizontalLine
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.HorizontalLine, Direction.Up), ConnectionDown},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.HorizontalLine, Direction.Down), ConnectionUp},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.HorizontalLine, Direction.Right), NoConnectionLeft},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.HorizontalLine, Direction.Left), NoConnectionRight},

        //Empty
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.Empty, Direction.Up), ConnectionDown},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.Empty, Direction.Down), ConnectionUp},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.Empty, Direction.Right), ConnectionLeft},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.Empty, Direction.Left), ConnectionRight},
    };

    public static bool[,] GetShapeDefinition(TileGroupShape shape)
    {
        bool[,] definitionArray;

        switch (shape)
        {
            case TileGroupShape.Plus:
                definitionArray = new bool[3, 3] {
                    { false, true, false },
                    { true, true, true },
                    { false, true, false } };
                break;
            case TileGroupShape.TriangleUp:
                definitionArray = new bool[3, 3] {
                    { false, true, false },
                    { false, true, true },
                    { false, true, false } };
                break;
            case TileGroupShape.TriangleDown:
                definitionArray = new bool[3, 3] {
                    { false, true, false },
                    { true, true, false },
                    { false, true, false } };
                break;
            case TileGroupShape.TriangleRight:
                definitionArray = new bool[3, 3] {
                    { false, false, false },
                    { true, true, true },
                    { false, true, false } };
                break;
            case TileGroupShape.TriangleLeft:
                definitionArray = new bool[3, 3] {
                    { false, true, false },
                    { true, true, true },
                    { false, false, false } };
                break;
            case TileGroupShape.CornerUpRight:
                definitionArray = new bool[3, 3] {
                    { false, false, false },
                    { false, true, true },
                    { false, true, false } };
                break;
            case TileGroupShape.CornerDownRight:
                definitionArray = new bool[3, 3] {
                    { false, false, false },
                    { true, true, false },
                    { false, true, false } };
                break;
            case TileGroupShape.CornerUpLeft:
                definitionArray = new bool[3, 3] {
                    { false, true, false },
                    { false, true, true},
                    { false, false, false } };
                break;
            case TileGroupShape.CornerDownLeft:
                definitionArray = new bool[3, 3] {
                    { false, true, false },
                    { true, true, false},
                    { false, false, false } };
                break;
            case TileGroupShape.VerticalLine:
                definitionArray = new bool[3, 3] {
                    { false, false, false },
                    { true, true, true},
                    { false, false, false } };
                break;
            case TileGroupShape.HorizontalLine:
                definitionArray = new bool[3, 3] {
                    { false, true, false },
                    { false, true, false},
                    { false, true, false } };
                break;
            case TileGroupShape.Empty:
                definitionArray = new bool[3, 3] {
                    { false, false, false },
                    { false, false, false},
                    { false, false, false } };
                break;
            default:
                definitionArray = new bool[3, 3] {
                    { false, false, false },
                    { false, false, false},
                    { false, false, false } };
                break;
        }

        return definitionArray;
    }
}

