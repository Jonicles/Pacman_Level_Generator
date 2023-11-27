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
        //Height always has to be odd for ghost box
        //IMPLEMENTATION

        //When we divide with two integers it is automatically "floored", fractions are thrown away.
        int groupWidth = (int)MathF.Floor(desiredWidth / tileGroupDimension * 0.5f);
        int groupHeight = (int)MathF.Floor((float)desiredHeight / (float)tileGroupDimension);

        TileGroup[,] tileGroupMatrix = GenerateTileGroups(groupWidth, groupHeight);

        TileGrid grid = InstantiateTiles(desiredWidth, desiredHeight);

        TransformTileStates(grid, tileGroupMatrix);

        //AddPelletsToWidth(grid, groupWidth);

        CopyTilesToRightSide(grid);

        CreateGhostBox(grid);

        AddMissingPellets(grid);

        RemoveIncorrectTiles(grid);

        ConnectPaths(grid);

        FindDisconnectedPath(grid);

        ChangeOccupiedTileSprites(grid);

        return grid;
    }

    private TileGroup[,] GenerateTileGroups(int width, int height)
    {
        TileGroup[,] tileGroupMatrix = CreateTileGroupMatrix(width, height);
        Dictionary<Coordinate, TileGroup> dict = new();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
                dict.Add(new Coordinate(i, j), tileGroupMatrix[i, j]);
        }

        RemoveShapesFromBorderGroups(tileGroupMatrix);

        System.Random rand = new();
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
                    currentTileGroup.RemoveAvailableShapes(TileShapeRules.ConnectionWest);
                    UpdateNeighboringTileGroups(currentTileGroup, new Coordinate(i, j), tileGroupMatrix);
                }

                if (j == 0)
                {
                    currentTileGroup.RemoveAvailableShapes(TileShapeRules.ConnectionSouth);
                    UpdateNeighboringTileGroups(currentTileGroup, new Coordinate(i, j), tileGroupMatrix);

                }

                if (j == tileGroupMatrix.GetLength(1) - 1)
                {
                    currentTileGroup.RemoveAvailableShapes(TileShapeRules.ConnectionNorth);
                    UpdateNeighboringTileGroups(currentTileGroup, new Coordinate(i, j), tileGroupMatrix);
                }
            }
        }
    }
    private void UpdateNeighboringTileGroups(TileGroup updatedGroup, Coordinate currentCoordinate, TileGroup[,] tileGroupMatrix)
    {
        Coordinate northCoordinate = currentCoordinate + Coordinate.North;
        Coordinate southCoordinate = currentCoordinate + Coordinate.South;
        Coordinate eastCoordinate = currentCoordinate + Coordinate.East;
        Coordinate westCoordinate = currentCoordinate + Coordinate.West;


        if (northCoordinate.Y < tileGroupMatrix.GetLength(1))
            CompareShapes(updatedGroup, northCoordinate, Direction.North, tileGroupMatrix);

        if (southCoordinate.Y >= 0)
            CompareShapes(updatedGroup, southCoordinate, Direction.South, tileGroupMatrix);

        if (eastCoordinate.X < tileGroupMatrix.GetLength(0))
            CompareShapes(updatedGroup, eastCoordinate, Direction.East, tileGroupMatrix);

        if (westCoordinate.X >= 0)
            CompareShapes(updatedGroup, westCoordinate, Direction.West, tileGroupMatrix);
    }
    private void CompareShapes(TileGroup updatedGroup, Coordinate nextCoordinate, Direction directionToNextGroup, TileGroup[,] tileGroupMatrix)
    {
        TileGroup nextGroup = tileGroupMatrix[nextCoordinate.X, nextCoordinate.Y];

        if (nextGroup.AvailableShapes.Count == 1)
            return;

        int previousShapeAmount = nextGroup.AvailableShapes.Count();

        Dictionary<TileGroupShape, int> shapeCounter = new();

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

        List<TileGroupShape> shapesToRemove = new();

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

    private TileGrid InstantiateTiles(int width, int height)
    {
        //GameObject parent = new GameObject();
        //parent.name = "Grid";

        TileGrid grid = new(width, height);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Coordinate coordinate = new(i, j);

                //GameObject newTile = Instantiate(tilePrefab, new Vector3(coordinate.X, coordinate.Y), Quaternion.identity, parent.transform);
                GameObject newTile = Instantiate(tilePrefab, new Vector3(coordinate.X, coordinate.Y), Quaternion.identity);
                newTile.name = coordinate.ToString();
                grid.SetTile(coordinate, newTile);
            }
        }

        return grid;
    }
    private void TransformTileStates(TileGrid grid, TileGroup[,] tileGroupMatrix)
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
                        Coordinate tileCoordinate = new(k + (i * tileGroupDimension), l + (j * tileGroupDimension));

                        if (grid.TryGetTile(tileCoordinate, out Tile tileComponent))
                        {
                            if (boolArray[k, l])
                            {
                                tileComponent.PlacePellet();
                                grid.PelletAmount++;
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

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid.TryGetTile(new Coordinate(i, j), out Tile tile);

                if (tile.State == TileState.Empty)
                    tile.OccupyTile();
            }
        }
    }
    private void CopyTilesToRightSide(TileGrid grid)
    {
        for (int i = 0; i < grid.GetLength(0) * 0.5f; i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid.TryGetTile(new Coordinate(i, j), out Tile tileToCopy))
                {
                    if (grid.TryGetTile(GetMirroredCoordinate(grid, i, j), out Tile newTile))
                    {
                        switch (tileToCopy.State)
                        {
                            case TileState.Empty:
                                newTile.EmptyTile();
                                break;
                            case TileState.Pellet:
                                newTile.PlacePellet();
                                grid.PelletAmount++;
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

    private Coordinate GetMirroredCoordinate(TileGrid grid, int x, int y)
    {
        return new Coordinate((grid.GetLength(0) - 1) - x, y);
    }

    private void CreateGhostBox(TileGrid grid)
    {
        GhostBox box = new();

        int halfHeight = /*(int)(0.5f * (grid.GetLength(1) - 1))*/15;
        int halfWidth = /*(int)(0.5f * grid.GetLength(0))*/14;
        int ghostBoxHeight = 11;
        int ghostBoxWidth = 14;
        int ghostBoxHalfHeight = 5;
        int ghostBoxHalfWidth = 7;

        Coordinate startCoordinate = new(halfWidth - ghostBoxHalfWidth, halfHeight - ghostBoxHalfHeight + 1);

        for (int i = 0; i < ghostBoxWidth; i++)
        {
            for (int j = 0; j < ghostBoxHeight; j++)
            {
                Coordinate currentCoordinate = startCoordinate + new Coordinate(i, j);

                if (grid.TryGetTile(currentCoordinate, out Tile currentTile))
                {
                    Tuple<TileState, TileSprite> currentValue = box.dict[new Coordinate(i, j)];

                    switch (currentValue.Item1)
                    {
                        case TileState.Empty:
                            currentTile.EmptyTile();
                            break;
                        case TileState.Pellet:
                            currentTile.PlacePellet();
                            break;
                        case TileState.Occupied:
                            currentTile.OccupyTile();
                            break;
                        default:
                            break;
                    }

                    currentTile.UpdateDisplay(currentValue.Item2);
                }
            }
        }

        for (int i = 0; i < ghostBoxWidth; i++)
        {
            for (int j = 0; j < ghostBoxHeight; j++)
            {
                Coordinate currentCoordinate = startCoordinate + new Coordinate(i, j);

                if (grid.TryGetTile(currentCoordinate, out Tile currentTile))
                {
                    if (i == 2 && j > 1 && j < ghostBoxHeight - 2)
                    {
                        Coordinate oneLeftCoordinate = currentCoordinate + new Coordinate(-3, 0);
                        Coordinate twoLeftCoordinate = currentCoordinate + new Coordinate(-4, 0);

                        if (grid.TryGetTile(oneLeftCoordinate, out Tile oneLeftTile) &&
                            grid.TryGetTile(twoLeftCoordinate, out Tile twoLeftTile))
                        {
                            if (oneLeftTile.State == TileState.Pellet && twoLeftTile.State == TileState.Pellet)
                            {
                                Coordinate C1 = currentCoordinate + new Coordinate(-1, 0);
                                Coordinate C2 = currentCoordinate + new Coordinate(-2, 0);

                                grid.TryGetTile(C1, out Tile T1);
                                grid.TryGetTile(C2, out Tile T2);

                                T1.EmptyTile();
                                T2.PlacePellet();
                            }
                        }
                    }

                    if (i == ghostBoxWidth - 3 && j > 1 && j < ghostBoxHeight - 2)
                    {
                        Coordinate oneRightCoordinate = currentCoordinate + new Coordinate(3, 0);
                        Coordinate twoRightCoordinate = currentCoordinate + new Coordinate(4, 0);

                        if (grid.TryGetTile(oneRightCoordinate, out Tile oneRightTile) &&
                            grid.TryGetTile(twoRightCoordinate, out Tile twoRightTile))
                        {
                            if (oneRightTile.State == TileState.Pellet && twoRightTile.State == TileState.Pellet)
                            {
                                Coordinate C1 = currentCoordinate + new Coordinate(1, 0);
                                Coordinate C2 = currentCoordinate + new Coordinate(2, 0);

                                grid.TryGetTile(C1, out Tile T1);
                                grid.TryGetTile(C2, out Tile T2);

                                T1.EmptyTile();
                                T2.PlacePellet();
                            }
                        }
                    }

                    if (j == 2 && i > 1 && i < ghostBoxWidth - 2)
                    {
                        Coordinate oneDownCoordinate = currentCoordinate + new Coordinate(0, -3);
                        Coordinate twoDownCoordinate = currentCoordinate + new Coordinate(0, -4);

                        if (grid.TryGetTile(oneDownCoordinate, out Tile oneDownTile) &&
                            grid.TryGetTile(twoDownCoordinate, out Tile twoDownTile))
                        {
                            if (oneDownTile.State == TileState.Pellet && twoDownTile.State == TileState.Pellet)
                            {
                                Coordinate C1 = currentCoordinate + new Coordinate(0, -1);
                                Coordinate C2 = currentCoordinate + new Coordinate(-0, -2);

                                grid.TryGetTile(C1, out Tile T1);
                                grid.TryGetTile(C2, out Tile T2);

                                T1.EmptyTile();
                                T2.EmptyTile();
                            }
                        }
                    }

                    if (j == ghostBoxHeight - 3 && i > 1 && i < ghostBoxWidth - 2)
                    {
                        Coordinate oneUpCoordinate = currentCoordinate + new Coordinate(0, 3);
                        Coordinate twoUpCoordinate = currentCoordinate + new Coordinate(0, 4);

                        if (grid.TryGetTile(oneUpCoordinate, out Tile oneUpTile) &&
                            grid.TryGetTile(twoUpCoordinate, out Tile twoUpTile))
                        {
                            if (oneUpTile.State == TileState.Pellet && twoUpTile.State == TileState.Pellet)
                            {
                                Coordinate C1 = currentCoordinate + new Coordinate(0, 1);
                                Coordinate C2 = currentCoordinate + new Coordinate(0, 2);

                                grid.TryGetTile(C1, out Tile T1);
                                grid.TryGetTile(C2, out Tile T2);

                                T1.EmptyTile();
                                T2.EmptyTile();
                            }
                        }
                    }
                }
            }
        }
    }

    private void AddMissingPellets(TileGrid grid)
    {
        AddPelletsToWidth(grid);
        AddPelletsToHeight(grid);
    }
    private void AddPelletsToWidth(TileGrid grid)
    {
        bool reUpdate = false;

        do
        {
            reUpdate = false;
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    Coordinate currentCoordinate = new(i, j);
                    if (grid.TryGetTile(currentCoordinate, out Tile currentTile))
                    {
                        if (currentTile.State == TileState.Occupied)
                        {
                            string code = DetermineTileCode(grid, currentCoordinate);

                            switch (code)
                            {
                                //Left
                                case "111110111":
                                    currentTile.PlacePellet();
                                    if (grid.TryGetTile(currentCoordinate + Coordinate.West, out Tile westTile))
                                        westTile.PlacePellet();
                                    reUpdate = true;
                                    break;
                                case "101100011":
                                    currentTile.PlacePellet();
                                    break;
                                case "111100011":
                                    currentTile.PlacePellet();
                                    break;
                                case "101100111":
                                    currentTile.PlacePellet();
                                    break;
                                //Right
                                case "111101111":
                                    currentTile.PlacePellet();
                                    if (grid.TryGetTile(currentCoordinate + Coordinate.East, out Tile eastTile))
                                        eastTile.PlacePellet();
                                    break;
                                case "111000110":
                                    currentTile.PlacePellet();
                                    break;
                                case "111100110":
                                    currentTile.PlacePellet();
                                    break;
                                case "111000111":
                                    currentTile.PlacePellet();
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        } while (reUpdate);
    }
    private void AddPelletsToHeight(TileGrid grid)
    {
        bool reUpdate;

        do
        {
            reUpdate = false;
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    Coordinate currentCoordinate = new(i, j);
                    if (grid.TryGetTile(currentCoordinate, out Tile currentTile))
                    {
                        if (currentTile.State == TileState.Occupied)
                        {
                            string code = DetermineTileCode(grid, currentCoordinate);

                            switch (code)
                            {
                                case "110111111":
                                    currentTile.PlacePellet();

                                    bool shouldPlacePelletsDown = true;
                                    bool insideGridDown = true;


                                    while (insideGridDown && shouldPlacePelletsDown)
                                    {
                                        bool northExists = grid.TryGetTile(currentCoordinate + Coordinate.South, out Tile southTile);
                                        bool eastExists = grid.TryGetTile(currentCoordinate + Coordinate.East, out Tile eastWhenSouthTile);
                                        bool westExists = grid.TryGetTile(currentCoordinate + Coordinate.West, out Tile westWhenSouthTile);

                                        insideGridDown = northExists && eastExists && westExists;

                                        if (insideGridDown)
                                        {
                                            if ((eastWhenSouthTile.State == TileState.Occupied && westWhenSouthTile.State == TileState.Occupied && southTile.State == TileState.Occupied))
                                            {
                                                southTile.PlacePellet();
                                                currentCoordinate += Coordinate.South;
                                            }
                                            else
                                            {
                                                southTile.PlacePellet();
                                                shouldPlacePelletsDown = false;
                                                break;
                                            }
                                        }
                                    }
                                    reUpdate = true;
                                    break;
                                case "111111101":
                                    currentTile.PlacePellet();

                                    bool shouldPlacePelletsUp = true;
                                    bool insideGridUp = true;


                                    while (insideGridUp && shouldPlacePelletsUp)
                                    {
                                        bool northExists = grid.TryGetTile(currentCoordinate + Coordinate.North, out Tile northTile);
                                        bool eastExists = grid.TryGetTile(currentCoordinate + Coordinate.East, out Tile eastWhenSouthTile);
                                        bool westExists = grid.TryGetTile(currentCoordinate + Coordinate.West, out Tile westWhenSouthTile);

                                        insideGridUp = northExists && eastExists && westExists;

                                        if (insideGridUp)
                                        {
                                            if ((eastWhenSouthTile.State == TileState.Occupied && westWhenSouthTile.State == TileState.Occupied && northTile.State == TileState.Occupied))
                                            {
                                                northTile.PlacePellet();
                                                currentCoordinate += Coordinate.North;
                                            }
                                            else
                                            {
                                                northTile.PlacePellet();
                                                shouldPlacePelletsUp = false;
                                                break;
                                            }
                                        }
                                    }
                                    reUpdate = true;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        } while (reUpdate);
    }

    private void RemoveIncorrectTiles(TileGrid grid)
    {
        Dictionary<Coordinate, Tile> tilesToEmpty = new();

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                Coordinate currentCoordinate = new(i, j);
                if (grid.TryGetTile(currentCoordinate, out Tile currentTile))
                {
                    if (currentTile.State == TileState.Occupied)
                    {
                        string code = DetermineTileCode(grid, currentCoordinate);

                        switch (code)
                        {
                            case "101000010":
                                Coordinate northCoordinate = currentCoordinate + Coordinate.North;
                                Coordinate southCoordinate = currentCoordinate + Coordinate.South;

                                if (!tilesToEmpty.ContainsKey(currentCoordinate))
                                    tilesToEmpty.Add(currentCoordinate, currentTile);

                                if (!tilesToEmpty.ContainsKey(northCoordinate))
                                {
                                    grid.TryGetTile(northCoordinate, out Tile northTile);
                                    tilesToEmpty.Add(northCoordinate, northTile);
                                }

                                if (!tilesToEmpty.ContainsKey(southCoordinate))
                                {
                                    grid.TryGetTile(southCoordinate, out Tile southTile);
                                    tilesToEmpty.Add(southCoordinate, southTile);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        foreach (KeyValuePair<Coordinate, Tile> tileToEmpty in tilesToEmpty)
        {
            if (tileToEmpty.Key == new Coordinate(10, 15) ||
                tileToEmpty.Key == new Coordinate(10, 16) ||
                tileToEmpty.Key == new Coordinate(10, 17) ||
                tileToEmpty.Key == new Coordinate(17, 15) ||
                tileToEmpty.Key == new Coordinate(17, 16) ||
                tileToEmpty.Key == new Coordinate(17, 17))
                continue;

            tileToEmpty.Value.EmptyTile();
        }
    }


    private void FindDisconnectedPath(TileGrid grid)
    {
        HashSet<Coordinate> connectedTiles = new HashSet<Coordinate>();
        bool disconnected = false;

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                Coordinate currentCoordinate = new Coordinate(i, j);

                if (connectedTiles.Contains(currentCoordinate))
                    continue;

                if (!grid.TryGetTile(currentCoordinate, out Tile currentTile))
                    continue;

                if (currentTile.State == TileState.Occupied)
                    continue;

                if (!disconnected)
                {
                    connectedTiles = FindConnectedTiles(connectedTiles, grid, currentCoordinate);
                }
                else
                {
                    connectedTiles = FindConnectedTiles(connectedTiles, grid, currentCoordinate);
                    TestMethod(connectedTiles, grid);
                }
            }
        }
    }

    private void TestMethod(HashSet<Coordinate> connectedTiles, TileGrid grid)
    {
        List<Coordinate> connectableCoordinates = new();

        foreach (var coordinate in connectedTiles)
        {
            if (coordinate.X < grid.GetLength(0) * 0.5f && coordinate.X % tileGroupDimension == 1 && coordinate.Y % tileGroupDimension == 3)
                connectableCoordinates.Add(coordinate);
        }

        foreach (var coordinate in connectableCoordinates)
        {
            FindClosestPossibleConnection(grid, coordinate);
        }
    }

    private void FindClosestPossibleConnection(TileGrid grid, Coordinate coordinate)
    {
        bool northConnection = FindValidConnectionAndLength(grid, coordinate, Direction.North, out int length);
    }

    private bool FindValidConnectionAndLength(TileGrid grid, Coordinate coordinate, Direction currentDirection, out int length)
    {
        Coordinate coordinateDirection = new();

        switch (currentDirection)
        {
            case Direction.North:
                coordinateDirection = Coordinate.North;
                break;
            case Direction.South:
                coordinateDirection = Coordinate.South;
                break;
            case Direction.East:
                coordinateDirection = Coordinate.East;
                break;
            case Direction.West:
                coordinateDirection = Coordinate.West;
                break;
            default:
                break;
        }

        length = 0;
        Coordinate nextCoordinate = coordinate + coordinateDirection;

        while (grid.TryGetTile(nextCoordinate, out Tile nextTile))
        {
            if (nextTile.State == TileState.Occupied)
                length++;
            else
                return true;

            nextCoordinate += coordinateDirection;
        }

        return false;
    }

    private HashSet<Coordinate> FindConnectedTiles(HashSet<Coordinate> connectedCoordinates, TileGrid grid, Coordinate startCoordinate)
    {
        connectedCoordinates.Add(startCoordinate);

        Coordinate northCoordinate = startCoordinate + Coordinate.North;
        Coordinate southCoordinate = startCoordinate + Coordinate.South;
        Coordinate westCoordinate = startCoordinate + Coordinate.West;
        Coordinate eastCoordinate = startCoordinate + Coordinate.East;


        if (grid.TryGetTile(northCoordinate, out Tile northTile))
        {
            if (northTile.State != TileState.Occupied && !connectedCoordinates.Contains(northCoordinate))
                FindConnectedTiles(connectedCoordinates, grid, northCoordinate);
        }
        if (grid.TryGetTile(southCoordinate, out Tile southTile))
        {
            if (southTile.State != TileState.Occupied && !connectedCoordinates.Contains(southCoordinate))
                FindConnectedTiles(connectedCoordinates, grid, southCoordinate);
        }
        if (grid.TryGetTile(westCoordinate, out Tile westTile))
        {
            if (westTile.State != TileState.Occupied && !connectedCoordinates.Contains(westCoordinate))
                FindConnectedTiles(connectedCoordinates, grid, westCoordinate);
        }
        if (grid.TryGetTile(eastCoordinate, out Tile eastTile))
        {
            if (eastTile.State != TileState.Occupied && !connectedCoordinates.Contains(eastCoordinate))
                FindConnectedTiles(connectedCoordinates, grid, eastCoordinate);
        }

        return connectedCoordinates;
    }

    private void ConnectPaths(TileGrid grid)
    {
        // .......
        // ->.....
        bool disconnected;

        for (int i = 0; i < grid.GetLength(1); i++)
        {
            disconnected = true;

            for (int j = 0; j < grid.GetLength(0); j++)
            {
                Coordinate currentCoordinate = new(j, i);

                if (grid.TryGetTile(currentCoordinate + Coordinate.North, out Tile northTile))
                {
                    if (northTile.State != TileState.Occupied)
                    {
                        disconnected = false;
                    }
                }
            }

            if (disconnected)
            {
                ConnectVertically(grid, i);
            }
        }
    }
    private void ConnectVertically(TileGrid grid, int row)
    {
        print($"Want to connect row: {row}");
        List<Coordinate> allowedCoordinateConnection = new();

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            Coordinate currentCoordinate = new(i, row);

            if (!grid.TryGetTile(currentCoordinate, out Tile currentTile))
                continue;

            if (currentTile.State != TileState.Occupied)
            {
                bool eligibleForConnection = i % tileGroupDimension == 1;

                if (eligibleForConnection)
                    allowedCoordinateConnection.Add(currentCoordinate);
            }
        }

        float shortestPathLength = Mathf.Infinity;
        Coordinate coordToUse = new();

        foreach (var coordinate in allowedCoordinateConnection)
        {
            float tempLenght = CheckLengthToNearestPellet(grid, coordinate);

            if (tempLenght < shortestPathLength)
            {
                coordToUse = coordinate;
                shortestPathLength = tempLenght;
            }
        }

        if (shortestPathLength == Mathf.Infinity)
            return;

        print($"Placing pellets for coordinate: {coordToUse}");
        for (int i = 1; i <= shortestPathLength; i++)
        {
            Coordinate coordToPlacePelletOn = new Coordinate(coordToUse.X, coordToUse.Y + i);
            if (grid.TryGetTile(coordToPlacePelletOn, out Tile tile))
            {
                print($"placing on coordinate: ({coordToPlacePelletOn.X}, {coordToPlacePelletOn.Y})");
                tile.PlacePellet();

                if (grid.TryGetTile(GetMirroredCoordinate(grid, coordToPlacePelletOn.X, coordToPlacePelletOn.Y), out Tile mirroredTile))
                    mirroredTile.PlacePellet();

            }
        }
    }
    private float CheckLengthToNearestPellet(TileGrid grid, Coordinate coordinate)
    {
        int length = 0;
        Coordinate nextCoordinate = coordinate + Coordinate.North;

        while (grid.TryGetTile(nextCoordinate, out Tile nextTile))
        {
            if (nextTile.State == TileState.Occupied)
                length++;
            else
                return length;

            nextCoordinate += Coordinate.North;
        }

        return Mathf.Infinity;
    }

    private void ChangeOccupiedTileSprites(TileGrid grid)
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                Coordinate currentCoordinate = new(i, j);
                if (grid.TryGetTile(currentCoordinate, out Tile currentTile))
                {
                    if (currentTile.State == TileState.Occupied)
                    {
                        string code = DetermineTileCode(grid, currentCoordinate);

                        if (TryGetTileSprite(code, out TileSprite sprite))
                        {
                            currentTile.UpdateDisplay(sprite);
                        }
                    }
                }
            }
        }
    }
    private string DetermineTileCode(TileGrid grid, Coordinate currentCoordinate)
    {
        Coordinate northWestCoordinate = currentCoordinate + Coordinate.NorthWest;
        Coordinate northCoordinate = currentCoordinate + Coordinate.North;
        Coordinate northEastCoordinate = currentCoordinate + Coordinate.NorthEast;
        Coordinate eastCoordinate = currentCoordinate + Coordinate.East;
        Coordinate southEastCoordinate = currentCoordinate + Coordinate.SouthEast;
        Coordinate southCoordinate = currentCoordinate + Coordinate.South;
        Coordinate southWestCoordinate = currentCoordinate + Coordinate.SouthWest;
        Coordinate westCoordinate = currentCoordinate + Coordinate.West;

        List<Coordinate> coordinates = new()
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
                //If tile is outside grid
                code += 1;
        }

        return code;
    }
    private bool TryGetTileSprite(string code, out TileSprite tileSprite)
    {
        tileSprite = TileSprite.Occupied;

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
            case "111101000":
                tileSprite = TileSprite.CornerDownLeft;
                break;
            case "101101001":
                tileSprite = TileSprite.CornerDownLeft;
                break;
            case "111010000":
                tileSprite = TileSprite.CornerDownRight;
                break;
            case "111110000":
                tileSprite = TileSprite.CornerDownRight;
                break;
            case "111010100":
                tileSprite = TileSprite.CornerDownRight;
                break;
            case "100001011":
                tileSprite = TileSprite.CornerUpLeft;
                break;
            case "100001111":
                tileSprite = TileSprite.CornerUpLeft;
                break;
            case "100101011":
                tileSprite = TileSprite.CornerUpLeft;
                break;
            case "100010110":
                tileSprite = TileSprite.CornerUpRight;
                break;
            case "100010111":
                tileSprite = TileSprite.CornerUpRight;
                break;
            case "110010110":
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

            ////Unorthodox cases
            //case "111101101":
            //    reUpdateRequired = true;
            //    currentTile.PlacePellet();
            //    break;
            //case "111110101":
            //    reUpdateRequired = true;
            //    currentTile.PlacePellet();
            //    break;
            //case "100011101":
            //    reUpdateRequired = true;
            //    currentTile.PlacePellet();
            //    print("New case!");
            //    break;
            //case "111110111":
            //    reUpdateRequired = true;
            //    currentTile.PlacePellet();
            //    grid.TryGetTile(currentCoordinate + Coordinate.north, out Tile upperTileRightSide);
            //    upperTileRightSide.PlacePellet();
            //    print("Placing pellets left side");
            //    break;
            //case "111101111":
            //    reUpdateRequired = true;
            //    currentTile.PlacePellet();
            //    grid.TryGetTile(currentCoordinate + Coordinate.north, out Tile upperTileLeftSide);
            //    upperTileLeftSide.PlacePellet();
            //    print("Placing pellets right side");
            //    break;
            //case "101101110":
            //    tileSprite = TileSprite.FunnelDownLeft;
            //    grid.TryGetTile(currentCoordinate + Coordinate.southWest, out Tile soutWestTile);
            //    grid.TryGetTile(currentCoordinate + Coordinate.south, out Tile southTile);
            //    southTile.UpdateDisplay(TileSprite.EdgeRight);
            //    soutWestTile.UpdateDisplay(TileSprite.SoftCornerDownRight);
            //    print("YUP");
            //    break;
            //case "111011011":
            //    tileSprite = TileSprite.DoubleInverseForward;
            //    break;
            //case "101111110":
            //    tileSprite = TileSprite.DoubleInverseBackward;
            //    break;
            default:
                return false;
        }

        return true;
    }
}
