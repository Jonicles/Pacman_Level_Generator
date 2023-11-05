using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGridGenerator : MonoBehaviour
{
    [SerializeField] GameObject tilePrefab;
    [SerializeField] float pelletPercentage = 0.3f;
    [SerializeField] int horizontalSize = 29;
    [SerializeField] int verticalSize = 29;
    Vector2[] directions = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

    int pelletLineMaxLength;
    int pelletLineMinLength = 4;
    int halfWidth;

    const int maxGridWidth = 28;
    const int maxGridHeight = 29;
    const int minGridWidth = 10;
    const int minGridHeight = 11;

    private void Start()
    {        
        //GenerateGrid(horizontalSize, verticalSize);
    }
    public TileGrid GenerateGrid(int dimensionX, int dimensionY)
    {
        FuctionTest();
        //The width has to be an even number
        if (dimensionX % 2 != 0)
        {
            dimensionX--;
        }

        TileGrid grid = InstantiateTiles(dimensionX, dimensionY);

        //We multiply with 0.5f because we are going to generate half of the grid and then duplicate it on the other half.
        //We will also subtract one, because arrays start at 0 and not 1.
        halfWidth = (int)(dimensionX * 0.5f) - 1;
        int tilesToGenerate = halfWidth * dimensionY;
        int pelletTiles = (int)(tilesToGenerate * pelletPercentage);
        pelletLineMaxLength = dimensionY - 2;

        //Coordinate currentCoordinate = new Coordinate(Random.Range(0, halfWidth), Random.Range(0, dimensionY));
        //Vector2 currentDirection = directions[Random.Range(0, directions.Length)];

        //while (pelletTiles > 0)
        //{
        //    //Generate a new lineLength
        //    int lineLength = Random.Range(pelletLineMinLength, pelletLineMaxLength);

        //    //While we have not reached the length
        //    while (lineLength > 0)
        //    {
        //        //Generate the next coordinate in the currentDirection
        //        currentCoordinate = new Coordinate(currentCoordinate.X + (int)currentDirection.x, currentCoordinate.Y + (int)currentDirection.y);

        //        bool isWithinBorder = currentCoordinate.X > 0 && currentCoordinate.X <= halfWidth &&
        //            currentCoordinate.Y > 0 && currentCoordinate.Y < grid.GetLength(1) - 1;

        //        //Place pellet if it is within border
        //        if (isWithinBorder)
        //        {
        //            if (TryGetTile(currentCoordinate, out Tile currentTile))
        //            {
        //                currentTile.PlacePellet();
        //                pelletTiles--;
        //                lineLength--;
        //            }
        //        }
        //        else
        //            lineLength = 0;
        //    }

        //    //Make sure we get a new direction that is different than the last
        //    Vector2 newDirection = directions[Random.Range(0, directions.Length)];
        //    while (newDirection == currentDirection)
        //        newDirection = directions[Random.Range(0, directions.Length)];

        //    currentDirection = newDirection;
        //}

        //CopyTilesToRightSide(halfWidth, dimensionX, dimensionY);

        return grid;
    }

    private void FuctionTest()
    {
        int dimensionLength = 5;
        TileGroup[,] tileGroupMatrix = new TileGroup[dimensionLength, dimensionLength];

        for (int i = 0; i < dimensionLength; i++)
        {
            for (int j = 0; j < dimensionLength; j++)
            {
                tileGroupMatrix[i, j] = new TileGroup();
            }
        }


    }



    void UpdateTileGroup(TileGroup previousGroup, TileGroup currentGroup)
    {

    }

    private TileGrid InstantiateTiles(int dimensionX, int dimensionY)
    {
        TileGrid grid = new TileGrid(dimensionX, dimensionY);

        for (int i = 0; i < dimensionX; i++)
        {
            for (int j = 0; j < dimensionY; j++)
            {
                GameObject newTile;
                newTile = Instantiate(tilePrefab, new Vector3(i, j), Quaternion.identity);
                grid.SetTile(new Coordinate(i, j), newTile);
            }
        }

        return grid;
    }

    //private void CopyTilesToRightSide(int halfWidth, int dimensionX, int dimensionY)
    //{
    //    for (int i = 0; i <= halfWidth; i++)
    //    {
    //        for (int j = 0; j < grid.GetLength(1); j++)
    //        {
    //            if (TryGetTile(new Coordinate(i, j), out Tile tileToCopy))
    //            {
    //                if (TryGetTile(new Coordinate((dimensionX - 1) - i, j), out Tile newTile))
    //                {
    //                    switch (tileToCopy.State)
    //                    {
    //                        case TileState.Empty:
    //                            break;
    //                        case TileState.Pellet:
    //                            newTile.PlacePellet();
    //                            break;
    //                        case TileState.Occupied:
    //                            newTile.OccupyTile();
    //                            break;
    //                        default:
    //                            break;
    //                    }
    //                }
    //                else
    //                {
    //                    print("Trying to access non existent tile 2");
    //                }
    //            }
    //            else
    //            {
    //                print("Trying to acces non existent tile");
    //            }
    //        }
    //    }
    //}
}
