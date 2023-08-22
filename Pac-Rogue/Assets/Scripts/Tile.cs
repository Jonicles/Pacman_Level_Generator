using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileState State { get; private set; }

    private void Awake()
    {
        State = TileState.Empty;
    }

    public void OccupyTile()
    {
        State = TileState.Occupied;
    }
}
