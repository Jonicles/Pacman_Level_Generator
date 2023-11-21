using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
public class TileGridGenerator : MonoBehaviour
{
    [SerializeField] GameObject tilePrefab;
    [SerializeField] int desiredWidth = 15;
    [SerializeField] int desiredHeight = 15;

    const int tileGroupDimension = 3;
    TileGrid currentGrid;

    private void Start()
    {
        //GenerateGrid(5, 8);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentGrid != null)
            {
                currentGrid.DestroyGrid();
            }

            currentGrid = GenerateGrid(desiredWidth, desiredHeight);
        }
    }
    public TileGrid GenerateGrid(int desiredWidth, int desiredHeight)
    {
        //Width always has to be even 
        if (desiredWidth % 2 != 0)
            desiredWidth--;

        //When we divide with two integers it is automatically "floored", fractions are thrown away.
        int groupWidth = (int)MathF.Floor(desiredWidth / tileGroupDimension * 0.5f);
        int groupHeight = (int)MathF.Floor((float)desiredHeight / (float)tileGroupDimension);

        print($"Width: {groupWidth}, Height: {groupHeight}");
        TileGroup[,] tileGroupMatrix = GenerateTileGroups(groupWidth, groupHeight);

        TileGrid grid = InstantiateTiles(desiredWidth, desiredHeight);

        TransformTileStates(grid, tileGroupMatrix, desiredWidth, desiredHeight);

        AddMissingPellets(grid, groupWidth);

        CopyTilesToRightSide(grid);

        ChangeOccupiedTileSprites(grid);

        return grid;
    }

    private void AddMissingPellets(TileGrid grid, int groupWidth)
    {
        int missingTiles = grid.GetLength(0) % tileGroupDimension;
        int xCoord = (int)(grid.GetLength(0) * 0.5f) - 1;

        if (missingTiles == 0)
            return;

        for (int i = 0; i < grid.GetLength(1); i++)
        {
            Coordinate coordinate;
            if (missingTiles == 1)
                coordinate = new Coordinate(xCoord - 2, i);
            else
                coordinate = new Coordinate(xCoord - 1, i);

            if (grid.TryGetTile(coordinate, out Tile pelletTile))
            {
                int num;

                if (missingTiles == 1)
                    num = 1;
                else
                    num = 0;

                for (int j = 0; j <= num; j++)
                {
                    if (grid.TryGetTile(new Coordinate(xCoord - j, i), out Tile emptyTile))
                    {
                        if (pelletTile.State == TileState.Pellet)
                        {
                            //print($"Pellet found at ({xCoord - 1}, {i}). Placing at ({xCoord + j}, {i})");
                            emptyTile.PlacePellet();
                        }
                    }

                }
            }
        }
    }

    private void ChangeOccupiedTileSprites(TileGrid grid)
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                Coordinate currentCoordinate = new Coordinate(i, j);
                if (grid.TryGetTile(currentCoordinate, out Tile currentTile))
                {
                    if (currentTile.State == TileState.Occupied)
                    {
                        string code = DetermineTileCode(grid, currentCoordinate, currentTile);
                        TileSprite tileSprite = GetTileSprite(code);
                        currentTile.UpdateDisplay(tileSprite);
                    }
                }
            }
        }
    }

    private TileSprite GetTileSprite(string code)
    {
        TileSprite tileSprite = TileSprite.Occupied;

        switch (code)
        {
            case "11111111":
                tileSprite = TileSprite.Occupied;
                break;
            case "111010110":
                tileSprite = TileSprite.EdgeRight;
                break;
            case "111010111":
                tileSprite = TileSprite.EdgeRight;
                break;
            case "111110110":
                tileSprite = TileSprite.EdgeRight;
                break;
            case "101101011":
                tileSprite = TileSprite.EdgeLeft;
                break;
            case "101101111":
                tileSprite = TileSprite.EdgeLeft;
                break;
            case "111101011":
                tileSprite = TileSprite.EdgeLeft;
                break;
            case "100011111":
                tileSprite = TileSprite.EdgeUp;
                break;
            case "100111111":
                tileSprite = TileSprite.EdgeUp;
                break;
            case "110011111":
                tileSprite = TileSprite.EdgeUp;
                break;
            case "111111000":
                tileSprite = TileSprite.EdgeDown;
                break;
            case "111111001":
                tileSprite = TileSprite.EdgeDown;
                break;
            case "111111100":
                tileSprite = TileSprite.EdgeDown;
                break;
            case "101101000":
                tileSprite = TileSprite.CornerDownLeft;
                break;
            case "111010000":
                tileSprite = TileSprite.CornerDownRight;
                break;
            case "100001011":
                tileSprite = TileSprite.CornerUpLeft;
                break;
            case "100010110":
                tileSprite = TileSprite.CornerUpRight;
                break;
            case "101111111":
                tileSprite = TileSprite.InverseCornerUpLeft;
                break;
            case "111011111":
                tileSprite = TileSprite.InverseCornerUpRight;
                break;
            case "111111011":
                tileSprite = TileSprite.InverseCornerDownLeft;
                break;
            case "111111110":
                tileSprite = TileSprite.InverseCornerDownRight;
                break;
            default:
                break;
        }

        return tileSprite;
    }

    private string DetermineTileCode(TileGrid grid, Coordinate currentCoordinate, Tile currentTile)
    {
        Coordinate northWestCoordinate = currentCoordinate + Coordinate.northWest;
        Coordinate northCoordinate = currentCoordinate + Coordinate.north;
        Coordinate northEastCoordinate = currentCoordinate + Coordinate.northEast;
        Coordinate eastCoordinate = currentCoordinate + Coordinate.east;
        Coordinate southEastCoordinate = currentCoordinate + Coordinate.southEast;
        Coordinate southCoordinate = currentCoordinate + Coordinate.south;
        Coordinate southWestCoordinate = currentCoordinate + Coordinate.southWest;
        Coordinate westCoordinate = currentCoordinate + Coordinate.west;

        List<Coordinate> coordinates = new List<Coordinate>()
        {
            currentCoordinate,
            northWestCoordinate,
            northCoordinate,
            northEastCoordinate,
            westCoordinate,
            eastCoordinate,
            southWestCoordinate,
            southCoordinate,
            southEastCoordinate,
        };

        //1 will equal occupied tile, 0 will equal unocccupied
        string code = "";

        for (int i = 0; i < coordinates.Count; i++)
        {
            if (grid.TryGetTile(coordinates[i], out Tile tile))
            {
                if (tile.State == TileState.Occupied)
                    code += 1;
                else
                    code += 0;
            }
            else
                code += 1;
        }

        return code;
    }

    private void TransformTileStates(TileGrid grid, TileGroup[,] tileGroupMatrix, int desiredDimensionX, int desiredDimensionY)
    {

        for (int i = 0; i < tileGroupMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < tileGroupMatrix.GetLength(1); j++)
            {

                bool[,] boolArray = TileShapeRules.GetShapeDefinition(tileGroupMatrix[i, j].DefiniteShape);

                for (int k = 0; k < 3; k++)
                {
                    for (int l = 0; l < 3; l++)
                    {
                        Coordinate tileCoordinate = new Coordinate(k + (i * tileGroupDimension), l + (j * tileGroupDimension));

                        if (grid.TryGetTile(tileCoordinate, out Tile tileComponent))
                        {
                            if (boolArray[k, l])
                            {
                                tileComponent.PlacePellet();
                            }
                            else
                            {
                                tileComponent.OccupyTile();
                            }
                        }
                    }
                }
            }
        }

        //Very ineffective, needs to optimize
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                Coordinate coordinate = new Coordinate(i, j);

                if (grid.TryGetTile(coordinate, out Tile tile))
                {
                    if (tile.State == TileState.Empty)
                    {
                        tile.OccupyTile();
                    }
                }
            }
        }
    }

    private TileGrid InstantiateTiles(int width, int height)
    {
        //We multiply with two so we later can copy over the tiles to the right side.
        //int tileGridWidth = desiredDimensionX * tileGroupDimension * 2;
        //int tileGridHeight = desiredDimensionY * tileGroupDimension;

        TileGrid grid = new TileGrid(width, height);
        GameObject parent = new GameObject();
        parent.name = "Grid";

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Coordinate coordinate = new Coordinate(i, j);

                GameObject newTile = Instantiate(tilePrefab, new Vector3(coordinate.X, coordinate.Y), Quaternion.identity, parent.transform);
                newTile.name = coordinate.ToString();
                grid.SetTile(coordinate, newTile);
            }
        }

        return grid;
    }

    private TileGroup[,] GenerateTileGroups(int width, int height)
    {
        TileGroup[,] tileGroupMatrix = CreateTileGroupMatrix(width, height);
        Dictionary<Coordinate, TileGroup> dict = new Dictionary<Coordinate, TileGroup>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
                dict.Add(new Coordinate(i, j), tileGroupMatrix[i, j]);
        }

        RemoveShapesFromBorderGroups(tileGroupMatrix);

        System.Random rand = new System.Random();
        while (dict.Count > 0)
        {
            Coordinate startCoordinate = dict.ElementAt(rand.Next(0, dict.Count)).Key;
            TileGroup startingTileGroup = tileGroupMatrix[startCoordinate.X, startCoordinate.Y];

            if (!startingTileGroup.DefiniteShapeSet)
            {
                startingTileGroup.SetRandomDefiniteShape();
                UpdateNeighboringTileGroups(startingTileGroup, startCoordinate, tileGroupMatrix);
                dict.Remove(startCoordinate);
            }
        }

        return tileGroupMatrix;
    }

    private void RemoveShapesFromBorderGroups(TileGroup[,] tileGroupMatrix)
    {
        for (int i = 0; i < tileGroupMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < tileGroupMatrix.GetLength(1); j++)
            {
                TileGroup currentTileGroup = tileGroupMatrix[i, j];

                //If any of these conditions are true it means it is a TileGroup on the border
                //We will not remove shapes from tilegroups on the right side since we will later copy tiles over to the right side. 

                if (i == 0)
                {
                    currentTileGroup.RemoveAvailableShapes(TileShapeRules.ConnectionLeft);
                    UpdateNeighboringTileGroups(currentTileGroup, new Coordinate(i, j), tileGroupMatrix);
                }

                if (j == 0)
                {
                    currentTileGroup.RemoveAvailableShapes(TileShapeRules.ConnectionDown);
                    UpdateNeighboringTileGroups(currentTileGroup, new Coordinate(i, j), tileGroupMatrix);

                }

                if (j == tileGroupMatrix.GetLength(1) - 1)
                {
                    currentTileGroup.RemoveAvailableShapes(TileShapeRules.ConnectionUp);
                    UpdateNeighboringTileGroups(currentTileGroup, new Coordinate(i, j), tileGroupMatrix);
                }
            }
        }
    }

    private void UpdateNeighboringTileGroups(TileGroup updatedGroup, Coordinate currentCoordinate, TileGroup[,] tileGroupMatrix)
    {
        Coordinate northCoordinate = currentCoordinate + Coordinate.north;
        Coordinate southCoordinate = currentCoordinate + Coordinate.south;
        Coordinate eastCoordinate = currentCoordinate + Coordinate.east;
        Coordinate westCoordinate = currentCoordinate + Coordinate.west;


        if (northCoordinate.Y < tileGroupMatrix.GetLength(1))
            CompareShapes(updatedGroup, northCoordinate, Direction.Up, tileGroupMatrix);

        if (southCoordinate.Y >= 0)
            CompareShapes(updatedGroup, southCoordinate, Direction.Down, tileGroupMatrix);

        if (eastCoordinate.X < tileGroupMatrix.GetLength(0))
            CompareShapes(updatedGroup, eastCoordinate, Direction.Right, tileGroupMatrix);

        if (westCoordinate.X >= 0)
            CompareShapes(updatedGroup, westCoordinate, Direction.Left, tileGroupMatrix);
    }

    private void CompareShapes(TileGroup updatedGroup, Coordinate nextCoordinate, Direction directionToNextGroup, TileGroup[,] tileGroupMatrix)
    {
        TileGroup nextGroup = tileGroupMatrix[nextCoordinate.X, nextCoordinate.Y];

        if (nextGroup.AvailableShapes.Count == 1)
            return;

        int previousShapeAmount = nextGroup.AvailableShapes.Count();

        Dictionary<TileGroupShape, int> shapeCounter = new Dictionary<TileGroupShape, int>();

        foreach (var shape in updatedGroup.AvailableShapes)
        {
            foreach (var connectionShape in TileShapeRules.ShapesToRemove[new Tuple<TileGroupShape, Direction>(shape, directionToNextGroup)])
            {
                if (!shapeCounter.ContainsKey(connectionShape))
                {
                    shapeCounter.Add(connectionShape, 1);
                }
                else
                {
                    shapeCounter[connectionShape]++;
                }
            }

        }

        List<TileGroupShape> shapesToRemove = new List<TileGroupShape>();

        foreach (KeyValuePair<TileGroupShape, int> shapeCount in shapeCounter)
        {
            if (shapeCount.Value == updatedGroup.AvailableShapes.Count)
            {
                shapesToRemove.Add(shapeCount.Key);
            }
        }

        nextGroup.RemoveAvailableShapes(shapesToRemove.ToArray());

        if (previousShapeAmount > nextGroup.AvailableShapes.Count())
            UpdateNeighboringTileGroups(nextGroup, nextCoordinate, tileGroupMatrix);
    }

    TileGroup[,] CreateTileGroupMatrix(int width, int height)
    {
        TileGroup[,] tileGroupMatrix = new TileGroup[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
                tileGroupMatrix[i, j] = new TileGroup();
        }

        return tileGroupMatrix;
    }

    private void CopyTilesToRightSide(TileGrid grid)
    {
        for (int i = 0; i < grid.GetLength(0) * 0.5f; i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid.TryGetTile(new Coordinate(i, j), out Tile tileToCopy))
                {
                    if (grid.TryGetTile(new Coordinate((grid.GetLength(0) - 1) - i, j), out Tile newTile))
                    {
                        switch (tileToCopy.State)
                        {
                            case TileState.Empty:
                                break;
                            case TileState.Pellet:
                                newTile.PlacePellet();
                                break;
                            case TileState.Occupied:
                                newTile.OccupyTile();
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        print("Trying to access non existent tile 2");
                    }
                }
                else
                {
                    print("Trying to acces non existent tile");
                }
            }
        }
    }
}
