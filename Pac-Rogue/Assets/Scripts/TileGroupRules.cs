using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum Direction
{
    Up,
    Down,
    Right,
    Left
}

public static class TileGroupRules
{
    static TileGroupShape[] ConnectionUp = { TileGroupShape.Plus, TileGroupShape.TriangleDown, TileGroupShape.TriangleRight, TileGroupShape.TriangleLeft, TileGroupShape.CornerDownRight, TileGroupShape.CornerDownLeft, TileGroupShape.VerticalLine };
    static TileGroupShape[] NoConnectionUp = { TileGroupShape.TriangleUp, TileGroupShape.CornerUpRight, TileGroupShape.CornerUpLeft, TileGroupShape.HorizontalLine, TileGroupShape.Empty };

    static TileGroupShape[] ConnectionDown = { TileGroupShape.Plus, TileGroupShape.TriangleUp, TileGroupShape.TriangleRight, TileGroupShape.TriangleLeft, TileGroupShape.CornerUpRight, TileGroupShape.CornerUpLeft, TileGroupShape.VerticalLine };
    static TileGroupShape[] NoConnectionDown = { TileGroupShape.TriangleDown, TileGroupShape.CornerDownRight, TileGroupShape.CornerDownLeft, TileGroupShape.HorizontalLine, TileGroupShape.Empty };

    static TileGroupShape[] ConnectionRight = { TileGroupShape.Plus, TileGroupShape.TriangleUp, TileGroupShape.TriangleDown, TileGroupShape.TriangleLeft, TileGroupShape.CornerUpLeft, TileGroupShape.CornerDownLeft, TileGroupShape.HorizontalLine };
    static TileGroupShape[] NoConnectionRight = { TileGroupShape.TriangleRight, TileGroupShape.CornerUpRight, TileGroupShape.CornerDownRight, TileGroupShape.VerticalLine, TileGroupShape.Empty };

    static TileGroupShape[] ConnectionLeft = { TileGroupShape.Plus, TileGroupShape.TriangleUp, TileGroupShape.TriangleDown, TileGroupShape.TriangleRight, TileGroupShape.CornerUpRight, TileGroupShape.CornerDownRight, TileGroupShape.HorizontalLine };
    static TileGroupShape[] NoConnectionLeft = { TileGroupShape.TriangleLeft, TileGroupShape.CornerUpLeft, TileGroupShape.CornerDownLeft, TileGroupShape.VerticalLine, TileGroupShape.Empty };

    public static Dictionary<Tuple<TileGroupShape, Direction>, TileGroupShape[]> TileMatches = new Dictionary<Tuple<TileGroupShape, Direction>, TileGroupShape[]>
    {
        //Plus
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.Plus, Direction.Up), ConnectionUp },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.Plus, Direction.Down), ConnectionDown },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.Plus, Direction.Right), ConnectionRight },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.Plus, Direction.Left), ConnectionLeft },

        //TriangleUp
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleUp, Direction.Up), ConnectionUp },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleUp, Direction.Down), NoConnectionDown },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleUp, Direction.Right), ConnectionRight },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleUp, Direction.Left), ConnectionLeft },

        //TriangleDown
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleDown, Direction.Up), NoConnectionUp },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleDown, Direction.Down), ConnectionDown },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleDown, Direction.Right), ConnectionRight },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleDown, Direction.Left), ConnectionLeft },

        //TriangleRight
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleRight, Direction.Up), ConnectionUp },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleRight, Direction.Down), ConnectionDown },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleRight, Direction.Right), ConnectionRight },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleRight, Direction.Left), NoConnectionLeft },

        //TriangleLeft
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleLeft, Direction.Up), ConnectionUp },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleLeft, Direction.Down), ConnectionDown },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleLeft, Direction.Right), NoConnectionRight },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.TriangleLeft, Direction.Left), ConnectionLeft },

        //CornerUpRight
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerUpRight, Direction.Up), ConnectionUp },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerUpRight, Direction.Down), NoConnectionDown },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerUpRight, Direction.Right), ConnectionRight },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerUpRight, Direction.Left), NoConnectionLeft },

        //CornerDownRight
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerDownRight, Direction.Up), NoConnectionUp },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerDownRight, Direction.Down), ConnectionDown },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerDownRight, Direction.Right), ConnectionRight },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerDownRight, Direction.Left), NoConnectionLeft },

        //CornerUpLeft
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerUpLeft, Direction.Up), ConnectionUp },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerUpLeft, Direction.Down), NoConnectionDown },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerUpLeft, Direction.Right), ConnectionRight },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerUpLeft, Direction.Left), ConnectionLeft },

        //CornerDownLeft
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerDownLeft, Direction.Up), NoConnectionUp },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerDownLeft, Direction.Down), ConnectionDown },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerDownLeft, Direction.Right), NoConnectionLeft },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.CornerDownLeft, Direction.Left), ConnectionLeft },

        //VerticalLine
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.VerticalLine, Direction.Up), ConnectionUp },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.VerticalLine, Direction.Down), ConnectionDown},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.VerticalLine, Direction.Right), NoConnectionRight },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.VerticalLine, Direction.Left), NoConnectionLeft },

        //HorizontalLine
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.HorizontalLine, Direction.Up), NoConnectionUp},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.HorizontalLine, Direction.Down), NoConnectionDown },
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.HorizontalLine, Direction.Right), ConnectionRight},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.HorizontalLine, Direction.Left), ConnectionLeft},

        //Empty
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.Empty, Direction.Up), NoConnectionUp},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.Empty, Direction.Down), NoConnectionDown},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.Empty, Direction.Right), NoConnectionRight},
        { new Tuple<TileGroupShape, Direction>(TileGroupShape.Empty, Direction.Left), NoConnectionLeft},
    };
}