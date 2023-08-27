using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    [SerializeField] GameObject tilePrefab;
    Tile[,] grid;
    Vector2[] directions = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

    int horizontalSize = 20;
    int verticalSize = 20;
    int pelletLineMaxLength;
    int pelletLineMinLenght = 4;

    const int maxGridWidth = 28;
    const int maxGridHeight = 29;
    const int minGridWidth = 10;
    const int minGridHeight = 11;
    const float pelletPercentage = 0.3f;

    private void Start()
    {
        GenerateGrid(horizontalSize, verticalSize);
    }
    void GenerateGrid(int dimensionX, int dimensionY)
    {
        //The width has to be an even number
        if (dimensionX % 2 != 0)
            dimensionX--;

        grid = new Tile[dimensionX, dimensionY];

        //Instantiate Tiles
        GameObject tileParent = new GameObject("Tile Parent");
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                GameObject newTile;
                newTile = Instantiate(tilePrefab, new Vector3(i, j), Quaternion.identity, tileParent.transform);
                SetTile(new Coordinate(i, j), newTile);
            }
        }

        //We multiply with 0.5f because we are going to generate half of the grid and then duplicate it on the other half.
        int halfWidth = (int)(dimensionX * 0.5f);
        int tilesToGenerate = halfWidth * dimensionY;
        int pelletTiles = (int)(tilesToGenerate * pelletPercentage);
        pelletLineMaxLength = dimensionY - 5;

        Coordinate currentCoordinate = new Coordinate(Random.Range(1, halfWidth), Random.Range(0, dimensionY));
        Vector2 currentDirection = directions[Random.Range(0, directions.Length)];

        while (pelletTiles > 0)
        {
            //Generate a new lineLength
            int lineLength = Random.Range(pelletLineMinLenght, pelletLineMaxLength);

            //While we have not reached the length
            while (lineLength > 0)
            {
                //Generate the next coordinate in the currentDirection
                currentCoordinate = new Coordinate(currentCoordinate.X + (int)currentDirection.x, currentCoordinate.Y + (int)currentDirection.y);

                bool isWithinBorder = currentCoordinate.X <= 0 || currentCoordinate.X >= grid.GetLength(0) * 0.5f ||
                    currentCoordinate.Y <= 0 || currentCoordinate.Y >= grid.GetLength(1) - 1;

                //Place pellet if it is within border
                if (isWithinBorder)
                {
                    if (TryGetTile(currentCoordinate, out Tile currentTile))
                    {
                        currentTile.PlacePellet();
                        pelletTiles--;
                    }
                }
                else
                    lineLength = 0;
            }

            //Make sure we get a new direction that is different than the last
            Vector2 newDirection = directions[Random.Range(0, directions.Length)];
            while (newDirection == currentDirection)
                newDirection = directions[Random.Range(0, directions.Length)];
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
