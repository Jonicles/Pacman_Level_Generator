using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject occupiedTilePrefab;
    Tile[,] grid;
    int horizontalSize = 10;
    int verticalSize = 10;

    private void Awake()
    {
        GenerateGrid(horizontalSize, verticalSize);
    }
    void GenerateGrid(int dimensionX, int dimensionY)
    {
        grid = new Tile[dimensionX, dimensionY];
        GameObject tileParent = new GameObject("Tile Parent");

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                //int num = Random.Range(0, 2);
                GameObject newTile;
                //if (num == 0)
                    newTile = Instantiate(tilePrefab, new Vector3(i, j), Quaternion.identity, tileParent.transform);
                //else
                //{
                //    newTile = Instantiate(occupiedTilePrefab, new Vector3(i, j), Quaternion.identity, tileParent.transform);
                //    newTile.GetComponent<Tile>().OccupyTile();
                //}

                SetTile(new Coordinate(i, j), newTile);
            }
        }
    }

    void SetTile(Coordinate coordinate, GameObject tileGO)
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
