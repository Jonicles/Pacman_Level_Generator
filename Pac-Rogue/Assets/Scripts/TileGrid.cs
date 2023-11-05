using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TileGrid
{
    Tile[,] grid;

    public TileGrid(int dimensionX, int dimensionY)
    {
        grid = new Tile[dimensionX, dimensionY];
    }

    public void SetTile(Coordinate coordinate, GameObject tileGO)
    {
        if (grid == null)
            Debug.LogWarning("Trying to set tile on an empty grid");

        if (tileGO.TryGetComponent(out Tile tileComponent))
            grid[coordinate.X, coordinate.Y] = tileComponent;
        else
            Debug.LogWarning("Tile prefab does not have Tile script attached");
    }

    public bool TryGetTile(Coordinate coordinate, out Tile tile)
    {
        bool outOfBounds = coordinate.X < 0 || coordinate.X > grid.GetLength(0) - 1 || coordinate.Y < 0 || coordinate.Y > grid.GetLength(1) - 1;
        if (outOfBounds)
        {
            tile = null;
            return false;
        }

        tile = grid[coordinate.X, coordinate.Y];
        return true;
    }
}
