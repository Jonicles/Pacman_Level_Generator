using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    [SerializeField] GameObject tilePrefab;
    GameObject[,] grid;
    int horizontalSize = 10;
    int verticalSize = 10;

    private void Awake()
    {
        GenerateGrid(horizontalSize, verticalSize);
    }
    void GenerateGrid(int dimensionX, int dimensionY)
    {
        grid = new GameObject[dimensionX, dimensionY];
        GameObject tileParent = new GameObject("Tile Parent");

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                GameObject newTile = Instantiate(tilePrefab, new Vector3(i, j), Quaternion.identity, tileParent.transform);
                SetTile(new Coordinate(i,j), newTile);
            }
        }
    }

    void SetTile(Coordinate coordinate, GameObject tile)
    {
        if (grid == null)
            Debug.LogWarning("Trying to set tile on an empty grid");

        grid[coordinate.X, coordinate.Y] = tile;
    }

    public bool TryGetTile(Coordinate coordinate, out GameObject tile)
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
