using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GhostBox
{
    public readonly int width = 14;
    public readonly int height = 11;
    public readonly Dictionary<Coordinate, Tuple<TileState, TileSprite>> dict = new()
    {   //Column 1
        { new Coordinate(0,0), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(0,1), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(0,2), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(0,3), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(0,4), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(0,5), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(0,6), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(0,7), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(0,8), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(0,9), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(0,10), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        //Column 2
        { new Coordinate(1,0), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(1,1), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(1,2), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(1,3), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(1,4), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(1,5), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(1,6), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(1,7), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(1,8), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(1,9), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(1,10), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        //Column 3
        { new Coordinate(2,0), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Empty) },
        { new Coordinate(2,1), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Empty) },
        { new Coordinate(2,2), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(2,3), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(2,4), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(2,5), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(2,6), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(2,7), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(2,8), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(2,9), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(2,10), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        //Column 4
        { new Coordinate(3,0), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(3,1), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(3,2), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(3,3), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.GhostCornerSouthWest) },
        { new Coordinate(3,4), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.GhostEdgeWest) },
        { new Coordinate(3,5), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.GhostEdgeWest) },
        { new Coordinate(3,6), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.GhostEdgeWest) },
        { new Coordinate(3,7), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.GhostCornerNorthWest) },
        { new Coordinate(3,8), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(3,9), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(3,10), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        //Column 5
        { new Coordinate(4,0), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(4,1), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(4,2), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(4,3), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.GhostEdgeSouth) },
        { new Coordinate(4,4), new Tuple<TileState, TileSprite>(TileState.GhostSpace, TileSprite.Empty) },
        { new Coordinate(4,5), new Tuple<TileState, TileSprite>(TileState.GhostSpace, TileSprite.Empty) },
        { new Coordinate(4,6), new Tuple<TileState, TileSprite>(TileState.GhostSpace, TileSprite.Empty) },
        { new Coordinate(4,7), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.GhostEdgeNorth) },
        { new Coordinate(4,8), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(4,9), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(4,10), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        //Column 6
        { new Coordinate(5,0), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(5,1), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(5,2), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(5,3), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.GhostEdgeSouth) },
        { new Coordinate(5,4), new Tuple<TileState, TileSprite>(TileState.GhostSpace, TileSprite.Empty) },
        { new Coordinate(5,5), new Tuple<TileState, TileSprite>(TileState.GhostSpace, TileSprite.Empty) },
        { new Coordinate(5,6), new Tuple<TileState, TileSprite>(TileState.GhostSpace, TileSprite.Empty) },
        { new Coordinate(5,7), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.GhostDoorConnectorEast) },
        { new Coordinate(5,8), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(5,9), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(5,10), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        //Column 7
        { new Coordinate(6,0), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(6,1), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(6,2), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(6,3), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.GhostEdgeSouth) },
        { new Coordinate(6,4), new Tuple<TileState, TileSprite>(TileState.GhostSpace, TileSprite.Empty) },
        { new Coordinate(6,5), new Tuple<TileState, TileSprite>(TileState.GhostSpace, TileSprite.Empty) },
        { new Coordinate(6,6), new Tuple<TileState, TileSprite>(TileState.GhostSpace, TileSprite.Empty) },
        { new Coordinate(6,7), new Tuple<TileState, TileSprite>(TileState.GhostSpace, TileSprite.GhostDoor) },
        { new Coordinate(6,8), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(6,9), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(6,10), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        //Column 8
        { new Coordinate(7,0), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(7,1), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(7,2), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(7,3), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.GhostEdgeSouth) },
        { new Coordinate(7,4), new Tuple<TileState, TileSprite>(TileState.GhostSpace, TileSprite.Empty) },
        { new Coordinate(7,5), new Tuple<TileState, TileSprite>(TileState.GhostSpace, TileSprite.Empty) },
        { new Coordinate(7,6), new Tuple<TileState, TileSprite>(TileState.GhostSpace, TileSprite.Empty) },
        { new Coordinate(7,7), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.GhostDoor) },
        { new Coordinate(7,8), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(7,9), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(7,10), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        //Column 9 
        { new Coordinate(8,0), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(8,1), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(8,2), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(8,3), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.GhostEdgeSouth) },
        { new Coordinate(8,4), new Tuple<TileState, TileSprite>(TileState.GhostSpace, TileSprite.Empty) },
        { new Coordinate(8,5), new Tuple<TileState, TileSprite>(TileState.GhostSpace, TileSprite.Empty) },
        { new Coordinate(8,6), new Tuple<TileState, TileSprite>(TileState.GhostSpace, TileSprite.Empty) },
        { new Coordinate(8,7), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.GhostDoorConnectorWest) },
        { new Coordinate(8,8), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(8,9), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(8,10), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        //Column 10
        { new Coordinate(9,0), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(9,1), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(9,2), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(9,3), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.GhostEdgeSouth) },
        { new Coordinate(9,4), new Tuple<TileState, TileSprite>(TileState.GhostSpace, TileSprite.Empty) },
        { new Coordinate(9,5), new Tuple<TileState, TileSprite>(TileState.GhostSpace, TileSprite.Empty) },
        { new Coordinate(9,6), new Tuple<TileState, TileSprite>(TileState.GhostSpace, TileSprite.Empty) },
        { new Coordinate(9,7), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.GhostEdgeNorth) },
        { new Coordinate(9,8), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(9,9), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(9,10), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        //Column 11
        { new Coordinate(10,0), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(10,1), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(10,2), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(10,3), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.GhostCornerSouthEast) },
        { new Coordinate(10,4), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.GhostEdgeEast) },
        { new Coordinate(10,5), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.GhostEdgeEast) },
        { new Coordinate(10,6), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.GhostEdgeEast) },
        { new Coordinate(10,7), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.GhostCornerNorthEast) },
        { new Coordinate(10,8), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(10,9), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(10,10), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        //Column 12
        { new Coordinate(11,0), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(11,1), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(11,2), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(11,3), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(11,4), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(11,5), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(11,6), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(11,7), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(11,8), new Tuple<TileState, TileSprite>(TileState.Empty, TileSprite.Empty) },
        { new Coordinate(11,9), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(11,10), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        //Column 13
        { new Coordinate(12,0), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(12,1), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(12,2), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(12,3), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(12,4), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(12,5), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(12,6), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(12,7), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(12,8), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(12,9), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(12,10), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        //Column 14
        { new Coordinate(13,0), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(13,1), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(13,2), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(13,3), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(13,4), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(13,5), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(13,6), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(13,7), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(13,8), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(13,9), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
        { new Coordinate(13,10), new Tuple<TileState, TileSprite>(TileState.Occupied, TileSprite.Occupied) },
    };
}
