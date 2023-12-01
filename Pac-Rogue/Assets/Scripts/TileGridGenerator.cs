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

    bool StartInfinity = false;
    int iteration = 0;
    const string OCCUPIED = "";
    const string EDGE_NORTH = "";
    const string EDGE_SOUTH = "";
    const string EDGE_WEST = "";
    const string EDGE_EAST = "";
    const string CORNER_NORTHEAST = "";
    const string CORNER_NORTHWEST = "";
    const string CORNER_SOUTHEAST = "";
    const string CORNER_SOUTHWEST = "";
    const string INVERSE_CORNER_NORTHEAST = "";
    const string INVERSE_CORNER_NORTHWEST = "";
    const string INVERSE_CORNER_SOUTHEAST = "";
    const string INVERSE_CORNER_SOUTHWEST = "";


    const string BORDER_EDGE_NORTH = "";
    const string BORDER_EDGE_SOUTH = "";
    const string BORDER_EDGE_WEST = "";
    const string BORDER_EDGE_EAST = "";
    const string BORDER_CORNER_NORTHEAST = "";
    const string BORDER_CORNER_NORTHWEST = "";
    const string BORDER_CORNER_SOUTHEAST = "";
    const string BORDER_CORNER_SOUTHWEST = "";




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

        if (Input.GetKeyDown(KeyCode.LeftAlt))
            StartInfinity = true;


        if (StartInfinity)
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

        CopyTilesToRightSide(grid);

        CreateGhostBox(grid);

        AddMissingPellets(grid);

        RemoveIncorrectTiles(grid);

        bool fullyConnected = CheckForConnectedPath(grid);

        while (!fullyConnected)
        {
            fullyConnected = CheckForConnectedPath(grid);
            RemoveIncorrectTiles(grid);
        }

        RemoveIncorrectTiles(grid);

        ChangeOccupiedTileSprites(grid);

        ChangeBorderSprites(grid);

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
                        case TileState.GhostSpace:
                            currentTile.GhostTile();
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

    private bool CheckForConnectedPath(TileGrid grid)
    {
        HashSet<Coordinate> connectedCoordinates = new HashSet<Coordinate>();
        bool connected = false;

        for (int i = 1; i < grid.GetLength(1); i++)
        {
            for (int j = 1; j < grid.GetLength(0); j++)
            {
                Coordinate currentCoordinate = new Coordinate(j, i);

                if (connectedCoordinates.Contains(currentCoordinate))
                    continue;

                if (!grid.TryGetTile(currentCoordinate, out Tile currentTile))
                    continue;

                if (currentTile.State == TileState.Occupied)
                    continue;

                if (!connected)
                {
                    connectedCoordinates = FindConnectedCoordinates(connectedCoordinates, grid, currentCoordinate);
                    connected = true;
                }
                else
                {
                    HashSet<Coordinate> disconnectedCoordinates = new HashSet<Coordinate>();
                    disconnectedCoordinates = FindConnectedCoordinates(disconnectedCoordinates, grid, currentCoordinate);
                    PlacePelletsForDisconnectedPath(disconnectedCoordinates, connectedCoordinates, grid);
                    return false;
                }
            }
        }
        return true;
    }
    private HashSet<Coordinate> FindConnectedCoordinates(HashSet<Coordinate> connectedCoordinates, TileGrid grid, Coordinate startCoordinate)
    {
        connectedCoordinates.Add(startCoordinate);

        Coordinate northCoordinate = startCoordinate + Coordinate.North;
        Coordinate southCoordinate = startCoordinate + Coordinate.South;
        Coordinate westCoordinate = startCoordinate + Coordinate.West;
        Coordinate eastCoordinate = startCoordinate + Coordinate.East;


        if (grid.TryGetTile(northCoordinate, out Tile northTile))
        {
            if (northTile.State != TileState.Occupied && !connectedCoordinates.Contains(northCoordinate))
                FindConnectedCoordinates(connectedCoordinates, grid, northCoordinate);
        }
        if (grid.TryGetTile(southCoordinate, out Tile southTile))
        {
            if (southTile.State != TileState.Occupied && !connectedCoordinates.Contains(southCoordinate))
                FindConnectedCoordinates(connectedCoordinates, grid, southCoordinate);
        }
        if (grid.TryGetTile(westCoordinate, out Tile westTile))
        {
            if (westTile.State != TileState.Occupied && !connectedCoordinates.Contains(westCoordinate))
                FindConnectedCoordinates(connectedCoordinates, grid, westCoordinate);
        }
        if (grid.TryGetTile(eastCoordinate, out Tile eastTile))
        {
            if (eastTile.State != TileState.Occupied && !connectedCoordinates.Contains(eastCoordinate))
                FindConnectedCoordinates(connectedCoordinates, grid, eastCoordinate);
        }

        return connectedCoordinates;
    }
    private void PlacePelletsForDisconnectedPath(HashSet<Coordinate> disconnectedCoordinates, HashSet<Coordinate> connectedCoordinates, TileGrid grid)
    {
        List<Coordinate> elegibleCoordinates = new();

        foreach (var coordinate in connectedCoordinates)
        {
            bool halfGrid = coordinate.X < grid.GetLength(0) * 0.5f - 1;
            bool centerPellet = coordinate.X % tileGroupDimension == 1 && coordinate.Y % tileGroupDimension == 1;
            bool westEdge = grid.TryGetTile(coordinate + Coordinate.West, out Tile westTile) && westTile.State == TileState.Occupied;
            bool southEdge = grid.TryGetTile(coordinate + Coordinate.South, out Tile southTile) && southTile.State == TileState.Occupied;
            bool eastEdge = grid.TryGetTile(coordinate + Coordinate.East, out Tile eastTile) && eastTile.State == TileState.Occupied;
            bool northEdge = grid.TryGetTile(coordinate + Coordinate.North, out Tile northTile) && northTile.State == TileState.Occupied;


            if (halfGrid && centerPellet && (westEdge || southEdge || eastEdge || northEdge))
            {
                elegibleCoordinates.Add(coordinate);
            }
        }

        foreach (var currentCoordinate in elegibleCoordinates)
        {
            if (FindClosestPossibleConnection(grid, currentCoordinate, disconnectedCoordinates, out Direction direction, out int length))
            {
                for (int i = 1; i <= length; i++)
                {
                    Coordinate coordinateDirection = Coordinate.GetCoordinateFromDirection(direction);
                    Coordinate nextCoordinate = currentCoordinate + new Coordinate(coordinateDirection.X * i, coordinateDirection.Y * i);

                    if (grid.TryGetTile(nextCoordinate, out Tile tile))
                    {
                        tile.PlacePellet();
                        Coordinate mirroredCoordinate = GetMirroredCoordinate(grid, nextCoordinate.X, nextCoordinate.Y);

                        if (grid.TryGetTile(mirroredCoordinate, out Tile mirroredTile))
                            mirroredTile.PlacePellet();

                    }
                }
            }
        }
    }
    private bool FindClosestPossibleConnection(TileGrid grid, Coordinate coordinate, HashSet<Coordinate> disconnectedCoordinates, out Direction direction, out int length)
    {
        bool northConnection = ValidateDirection(grid, coordinate, Direction.North, out int northLength, out Coordinate northConnectionCoordinate);
        bool southConnection = ValidateDirection(grid, coordinate, Direction.South, out int southLength, out Coordinate southConnectionCoordinate);
        bool eastConnection = ValidateDirection(grid, coordinate, Direction.East, out int eastLength, out Coordinate eastConnectionCoordinate);
        bool westConnection = ValidateDirection(grid, coordinate, Direction.West, out int westLength, out Coordinate westConnectionCoordinate);

        Dictionary<Direction, int> directionLengths = new Dictionary<Direction, int>();

        if (northConnection && northLength > 0 && disconnectedCoordinates.Contains(northConnectionCoordinate))
            directionLengths.Add(Direction.North, northLength);
        if (southConnection && southLength > 0 && disconnectedCoordinates.Contains(southConnectionCoordinate))
            directionLengths.Add(Direction.South, southLength);
        if (eastConnection && eastLength > 0 && disconnectedCoordinates.Contains(eastConnectionCoordinate))
            directionLengths.Add(Direction.East, eastLength);
        if (westConnection && westLength > 0 && disconnectedCoordinates.Contains(westConnectionCoordinate))
            directionLengths.Add(Direction.West, westLength);

        direction = Direction.North;
        length = Int32.MaxValue;

        foreach (KeyValuePair<Direction, int> directionLength in directionLengths)
        {
            if (directionLength.Value < length)
            {
                direction = directionLength.Key;
                length = directionLength.Value;
            }
        }

        if (length == Int32.MaxValue)
            return false;
        else
            return true;
    }
    private bool ValidateDirection(TileGrid grid, Coordinate startCoordinate, Direction currentDirection, out int length, out Coordinate connectionCoordinate)
    {
        length = 0;
        connectionCoordinate = new();
        Coordinate nextCoordinate = startCoordinate + Coordinate.GetCoordinateFromDirection(currentDirection);

        while (grid.TryGetTile(nextCoordinate, out Tile nextTile))
        {
            if (nextTile.State == TileState.Occupied)
                length++;
            else
            {
                connectionCoordinate = nextCoordinate;
                return true;
            }

            nextCoordinate += Coordinate.GetCoordinateFromDirection(currentDirection);
        }
        return false;
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

    private void ChangeBorderSprites(TileGrid grid)
    {
        HashSet<Coordinate> borderCoordinates = new HashSet<Coordinate>();
        Coordinate firstBorderCoordinate = new();
        bool borderFound = false;
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                Coordinate startCoordinate = new Coordinate(x, y);

                if (x == 0)
                {
                    if (FindBorder(grid, startCoordinate, Direction.North, out Coordinate borderCoordinate))
                    {
                        borderFound = true;
                        firstBorderCoordinate = borderCoordinate;
                        break;
                    }
                }

                if (x == grid.GetLength(1) - 1)
                {
                    if (FindBorder(grid, startCoordinate, Direction.South, out Coordinate borderCoordinate))
                    {
                        borderFound = true;
                        firstBorderCoordinate = borderCoordinate;
                        break;
                    }

                }

                if (y == 0)
                {
                    if (FindBorder(grid, startCoordinate, Direction.East, out Coordinate borderCoordinate))
                    {
                        borderFound = true;
                        firstBorderCoordinate = borderCoordinate;
                        break;
                    }
                }

                if (y == grid.GetLength(0) - 1)
                {
                    if (FindBorder(grid, startCoordinate, Direction.West, out Coordinate borderCoordinate))
                    {
                        borderFound = true;
                        firstBorderCoordinate = borderCoordinate;
                        break;
                    }
                }
            }

            if (borderFound)
                break;
        }

        if (!borderFound)
            return;
        deter
        borderCoordinates = FindBorderCoordinates(grid, firstBorderCoordinate, borderCoordinates);

        //for (int i = 0; i < grid.GetLength(0); i++)
        //{
        //    for (int j = 0; j < grid.GetLength(1); j++)
        //    {
        //        Coordinate startCoordinate = new Coordinate(i, j);

        //        if (j == 0)
        //        {
        //            if (FindBorder(grid, startCoordinate, Direction.North, out Coordinate borderCoordinate))
        //            {
        //                if (!borderCoordinates.Contains(borderCoordinate))
        //                    borderCoordinates.Add(borderCoordinate);
        //            }
        //        }

        //        if (j == grid.GetLength(1) - 1)
        //        {
        //            if (FindBorder(grid, startCoordinate, Direction.South, out Coordinate borderCoordinate))
        //            {
        //                if (!borderCoordinates.Contains(borderCoordinate))
        //                    borderCoordinates.Add(borderCoordinate);
        //            }

        //        }

        //        if (i == 0)
        //        {
        //            if (FindBorder(grid, startCoordinate, Direction.East, out Coordinate borderCoordinate))
        //            {
        //                if (!borderCoordinates.Contains(borderCoordinate))
        //                    borderCoordinates.Add(borderCoordinate);
        //            }
        //        }

        //        if (i == grid.GetLength(0) - 1)
        //        {
        //            if (FindBorder(grid, startCoordinate, Direction.West, out Coordinate borderCoordinate))
        //            {
        //                if (!borderCoordinates.Contains(borderCoordinate))
        //                    borderCoordinates.Add(borderCoordinate);
        //            }
        //        }
        //    }
        //}

        //string code = "";
        //HashSet<Coordinate> newBorderCoordinates = new HashSet<Coordinate>();

        //foreach (var coordinate in borderCoordinates)
        //{
        //    code = DetermineTileCode(grid, coordinate);

        //    FindAdditionalTiles(grid, code, coordinate, newBorderCoordinates, borderCoordinates);
        //}

        //foreach (var coordinate in newBorderCoordinates)
        //{
        //    borderCoordinates.Add(coordinate);
        //}

        //List<Coordinate> incorrectCoordinates = new List<Coordinate>();

        //foreach (var coordinate in borderCoordinates)
        //{
        //    code = DetermineTileCode(grid, coordinate);
        //    if (!TryGetBorderSprite(grid, coordinate, code, out TileSprite sprite, out Coordinate incorrectCoordinate))
        //        continue;

        //    incorrectCoordinates.Add(incorrectCoordinate);

        //    if (!grid.TryGetTile(coordinate, out Tile tile))
        //        continue;

        //    tile.UpdateDisplay(sprite);
        //}
    }

    private HashSet<Coordinate> FindBorderCoordinates(TileGrid grid, Coordinate startCoordinate, HashSet<Coordinate> borderCoordinates)
    {
        borderCoordinates.Add(startCoordinate);
        string code = DetermineTileCode(grid, startCoordinate);

        HashSet<Direction> availableDirection = new HashSet<Direction>();
        availableDirection.Add(Direction.North);
        availableDirection.Add(Direction.South);
        availableDirection.Add(Direction.East);
        availableDirection.Add(Direction.West);


        CheckForDivot(grid, startCoordinate, code,)

        Coordinate northCoordinate = startCoordinate + Coordinate.North;
        Coordinate southCoordinate = startCoordinate + Coordinate.South;
        Coordinate westCoordinate = startCoordinate + Coordinate.West;
        Coordinate eastCoordinate = startCoordinate + Coordinate.East;


        if (grid.TryGetTile(northCoordinate, out Tile northTile))
        {
            string code = DetermineTileCode(grid, northCoordinate);

            if (northTile.State == TileState.Occupied && code != "111111111")
                FindConnectedCoordinates(borderCoordinates, grid, northCoordinate);
        }
        if (grid.TryGetTile(southCoordinate, out Tile southTile))
        {
            if (southTile.State != TileState.Occupied && !connectedCoordinates.Contains(southCoordinate))
                FindConnectedCoordinates(connectedCoordinates, grid, southCoordinate);
        }
        if (grid.TryGetTile(westCoordinate, out Tile westTile))
        {
            if (westTile.State != TileState.Occupied && !connectedCoordinates.Contains(westCoordinate))
                FindConnectedCoordinates(connectedCoordinates, grid, westCoordinate);
        }
        if (grid.TryGetTile(eastCoordinate, out Tile eastTile))
        {
            if (eastTile.State != TileState.Occupied && !connectedCoordinates.Contains(eastCoordinate))
                FindConnectedCoordinates(connectedCoordinates, grid, eastCoordinate);
        }

        return borderCoordinates;
    }

    private bool FindBorder(TileGrid grid, Coordinate startCoordinate, Direction direction, out Coordinate borderCoordinate)
    {
        bool reachedOtherSide = false;
        Coordinate coordinateToCheck = startCoordinate;

        do
        {
            TileState tileState = TileState.Occupied;
            string code = DetermineTileCode(grid, coordinateToCheck);
            bool validTile = grid.TryGetTile(coordinateToCheck, out Tile currentTile);

            if (validTile)
                tileState = currentTile.State;

            if (tileState == TileState.Pellet)
                break;

            if (code != "111111111")
            {
                borderCoordinate = coordinateToCheck;
                return true;
            }

            coordinateToCheck += Coordinate.GetCoordinateFromDirection(direction);

            if (!grid.TryGetTile(coordinateToCheck))
                reachedOtherSide = true;

        } while (!reachedOtherSide);

        borderCoordinate = new();
        return false;
    }
    private void FindAdditionalTiles(TileGrid grid, string code, Coordinate currentCoordinate, HashSet<Coordinate> newBorderCoordinates, HashSet<Coordinate> foundBorderCoordinates)
    {
        Direction startDirection = Direction.North;
        Direction backUpDirection = Direction.North;

        if (CheckForDivot(grid, currentCoordinate, code, out bool edgeCase))
        {
            return;
        }

        switch (code)
        {
            //SouthEastCorner
            case "101111111":
                startDirection = Direction.North;
                backUpDirection = Direction.West;
                break;
            //NorthWestCorner
            case "111111110":
                startDirection = Direction.South;
                backUpDirection = Direction.East;
                break;
            //SouthWestCorner
            case "111011111":
                startDirection = Direction.North;
                backUpDirection = Direction.East;
                break;
            //NorthEastCorner
            case "111111011":
                startDirection = Direction.South;
                backUpDirection = Direction.West;
                break;
            //NorthEdge
            case "111111000":
                startDirection = Direction.East;
                backUpDirection = Direction.West;
                break;
            //NorthEdge
            case "111111001":
                startDirection = Direction.East;
                backUpDirection = Direction.West;
                break;
            //NorthEdge
            case "111111100":
                startDirection = Direction.East;
                backUpDirection = Direction.West;
                break;
            //SouthEdge
            case "100011111":
                startDirection = Direction.East;
                backUpDirection = Direction.West;
                break;
            //SouthEdge
            case "100111111":
                startDirection = Direction.East;
                backUpDirection = Direction.West;
                break;
            //SouthEdge
            case "110011111":
                startDirection = Direction.East;
                backUpDirection = Direction.West;
                break;
            //WestEdge
            case "111010110":
                startDirection = Direction.North;
                backUpDirection = Direction.South;
                break;
            case "111110110":
                startDirection = Direction.North;
                backUpDirection = Direction.South;
                break;
            case "111010111":
                startDirection = Direction.North;
                backUpDirection = Direction.South;
                break;
            //EastEdge
            case "101101011":
                startDirection = Direction.North;
                backUpDirection = Direction.South;
                break;
            case "111101011":
                startDirection = Direction.North;
                backUpDirection = Direction.South;
                break;
            case "101101111":
                startDirection = Direction.North;
                backUpDirection = Direction.South;
                break;
            default:
                return;
        }

        bool foundBorder = false;
        Direction currentDirection;
        Coordinate previousCoordinate;

        if (!DetermineStartDirection(grid, currentCoordinate, startDirection, backUpDirection, newBorderCoordinates, foundBorderCoordinates, out currentDirection))
            return;

        while (!foundBorder)
        {
            previousCoordinate = currentCoordinate;
            currentCoordinate += Coordinate.GetCoordinateFromDirection(currentDirection);

            if (!grid.TryGetTile(currentCoordinate, out Tile currentTile))
                break;

            string newCode = DetermineTileCode(grid, currentCoordinate);


            if (newCode != "111111111" && currentTile.State == TileState.Occupied)
            {
                if (foundBorderCoordinates.Contains(currentCoordinate))
                {
                    foundBorder = true;
                }
                else
                {
                    if (CheckForDivot(grid, currentCoordinate, newCode, out edgeCase))
                    {
                        foundBorder = true;
                    }

                    if (!edgeCase)
                        newBorderCoordinates.Add(currentCoordinate);
                }
            }
            else
            {
                if (!FindNextDirection(grid, previousCoordinate, newBorderCoordinates, foundBorderCoordinates, out Direction nextDirection))
                {
                    break;
                }

                currentCoordinate = previousCoordinate;
                currentDirection = nextDirection;
            }
        }
    }
    private bool CheckForDivot(TileGrid grid, Coordinate currentCoordinate, string code, out bool edgeCase)
    {
        edgeCase = false;
        Direction horizontalDivotDirection = Direction.North;
        Direction verticalDivotDirection = Direction.North;

        bool checkForEdgeDivot = false;

        switch (code)
        {
            //SouthEast
            case "101111111":
                verticalDivotDirection = Direction.South;
                horizontalDivotDirection = Direction.East;
                break;
            //NorthWest
            case "111111110":
                verticalDivotDirection = Direction.North;
                horizontalDivotDirection = Direction.West;
                break;
            //SouthWest
            case "111011111":
                verticalDivotDirection = Direction.South;
                horizontalDivotDirection = Direction.West;
                break;
            //NorthEast
            case "111111011":
                verticalDivotDirection = Direction.North;
                horizontalDivotDirection = Direction.East;
                break;
            default:
                checkForEdgeDivot = true;
                break;
        }

        if (checkForEdgeDivot)
        {
            string edgeCode = "";
            switch (code)
            {
                //North Edge cases
                case "111111000":
                    Coordinate northCoordinate = currentCoordinate + Coordinate.GetCoordinateFromDirection(Direction.North);
                    if (!grid.TryGetTile(northCoordinate))
                        break;

                    edgeCode = DetermineTileCode(grid, northCoordinate);
                    if (edgeCode == "101111111" || edgeCode == "111111110" || edgeCode == "111011111" || edgeCode == "111111011")
                    {
                        edgeCase = true;
                        return true;
                    }
                    else
                        return false;
                case "111111001":
                    northCoordinate = currentCoordinate + Coordinate.GetCoordinateFromDirection(Direction.North);
                    if (!grid.TryGetTile(northCoordinate))
                        break;

                    edgeCode = DetermineTileCode(grid, northCoordinate);
                    if (edgeCode == "101111111" || edgeCode == "111111110" || edgeCode == "111011111" || edgeCode == "111111011")
                    {
                        edgeCase = true;
                        return true;
                    }
                    else
                        return false;
                case "111111100":
                    northCoordinate = currentCoordinate + Coordinate.GetCoordinateFromDirection(Direction.North);
                    if (!grid.TryGetTile(northCoordinate))
                        break;

                    edgeCode = DetermineTileCode(grid, northCoordinate);
                    if (edgeCode == "101111111" || edgeCode == "111111110" || edgeCode == "111011111" || edgeCode == "111111011")
                    {
                        edgeCase = true;
                        return true;
                    }
                    else
                        return false;

                //South Edge cases
                case "100011111":
                    Coordinate southCoordinate = currentCoordinate + Coordinate.GetCoordinateFromDirection(Direction.South);
                    if (!grid.TryGetTile(southCoordinate))
                        break;

                    edgeCode = DetermineTileCode(grid, southCoordinate);
                    if (edgeCode == "101111111" || edgeCode == "111111110" || edgeCode == "111011111" || edgeCode == "111111011")
                    {
                        edgeCase = true;
                        return true;
                    }
                    else
                        return false;
                case "110011111":
                    southCoordinate = currentCoordinate + Coordinate.GetCoordinateFromDirection(Direction.South);
                    if (!grid.TryGetTile(southCoordinate))
                        break;

                    edgeCode = DetermineTileCode(grid, southCoordinate);
                    if (edgeCode == "101111111" || edgeCode == "111111110" || edgeCode == "111011111" || edgeCode == "111111011")
                    {
                        edgeCase = true;
                        return true;
                    }
                    else
                        return false;
                case "100111111":
                    southCoordinate = currentCoordinate + Coordinate.GetCoordinateFromDirection(Direction.South);
                    if (!grid.TryGetTile(southCoordinate))
                        break;

                    edgeCode = DetermineTileCode(grid, southCoordinate);
                    if (edgeCode == "101111111" || edgeCode == "111111110" || edgeCode == "111011111" || edgeCode == "111111011")
                    {
                        edgeCase = true;
                        return true;
                    }
                    else
                        return false;

                //West Edge cases
                case "111010110":
                    Coordinate westCoordinate = currentCoordinate + Coordinate.GetCoordinateFromDirection(Direction.West);
                    if (!grid.TryGetTile(westCoordinate))
                        break;

                    edgeCode = DetermineTileCode(grid, westCoordinate);
                    if (edgeCode == "101111111" || edgeCode == "111111110" || edgeCode == "111011111" || edgeCode == "111111011")
                    {
                        edgeCase = true;
                        return true;
                    }
                    else
                        return false;
                case "111110110":
                    westCoordinate = currentCoordinate + Coordinate.GetCoordinateFromDirection(Direction.West);
                    if (!grid.TryGetTile(westCoordinate))
                        break;

                    edgeCode = DetermineTileCode(grid, westCoordinate);
                    if (edgeCode == "101111111" || edgeCode == "111111110" || edgeCode == "111011111" || edgeCode == "111111011")
                    {
                        edgeCase = true;
                        return true;
                    }
                    else
                        return false;
                case "111010111":
                    westCoordinate = currentCoordinate + Coordinate.GetCoordinateFromDirection(Direction.West);
                    if (!grid.TryGetTile(westCoordinate))
                        break;

                    edgeCode = DetermineTileCode(grid, westCoordinate);
                    if (edgeCode == "101111111" || edgeCode == "111111110" || edgeCode == "111011111" || edgeCode == "111111011")
                    {
                        edgeCase = true;
                        return true;
                    }
                    else
                        return false;

                //East Edge cases
                case "101101011":
                    Coordinate eastCoordinate = currentCoordinate + Coordinate.GetCoordinateFromDirection(Direction.East);
                    if (!grid.TryGetTile(eastCoordinate))
                        break;

                    edgeCode = DetermineTileCode(grid, eastCoordinate);
                    if (edgeCode == "101111111" || edgeCode == "111111110" || edgeCode == "111011111" || edgeCode == "111111011")
                    {
                        edgeCase = true;
                        return true;
                    }
                    else
                        return false;
                case "111101011":
                    eastCoordinate = currentCoordinate + Coordinate.GetCoordinateFromDirection(Direction.East);
                    if (!grid.TryGetTile(eastCoordinate))
                        break;

                    edgeCode = DetermineTileCode(grid, eastCoordinate);
                    if (edgeCode == "101111111" || edgeCode == "111111110" || edgeCode == "111011111" || edgeCode == "111111011")
                    {
                        edgeCase = true;
                        return true;
                    }
                    else
                        return false;
                case "101101111":
                    eastCoordinate = currentCoordinate + Coordinate.GetCoordinateFromDirection(Direction.East);

                    if (!grid.TryGetTile(eastCoordinate))
                        break;

                    edgeCode = DetermineTileCode(grid, eastCoordinate);
                    if (edgeCode == "101111111" || edgeCode == "111111110" || edgeCode == "111011111" || edgeCode == "111111011")
                    {
                        edgeCase = true;
                        return true;
                    }
                    else
                        return false;
                default:
                    return false;
            }
        }

        Coordinate horizontalCoordinate = currentCoordinate + Coordinate.GetCoordinateFromDirection(horizontalDivotDirection);
        Coordinate verticalCoordinate = currentCoordinate + Coordinate.GetCoordinateFromDirection(verticalDivotDirection);

        bool divot = false;

        if (grid.TryGetTile(horizontalCoordinate, out Tile horizontalTile))
        {
            string horizontalCode = DetermineTileCode(grid, horizontalCoordinate);
            bool corner = horizontalCode == "101111111" || horizontalCode == "111111110" || horizontalCode == "111011111" || horizontalCode == "111111011";
            bool westEdge = horizontalCode == "111010110" || horizontalCode == "111110110" || horizontalCode == "111010111";
            bool eastEdge = horizontalCode == "101101011" || horizontalCode == "111101011" || horizontalCode == "100111111";

            if (corner || westEdge || eastEdge)
                divot = true;

        }

        if (grid.TryGetTile(verticalCoordinate, out Tile verticalTile))
        {
            string verticalCode = DetermineTileCode(grid, verticalCoordinate);

            bool corner = verticalCode == "101111111" || verticalCode == "111111110" || verticalCode == "111011111" || verticalCode == "111111011";
            bool northEdge = verticalCode == "111111000" || verticalCode == "111111001" || verticalCode == "111111100";
            bool southEdge = verticalCode == "100011111" || verticalCode == "110011111" || verticalCode == "100111111";

            if (corner || northEdge || southEdge)
                divot = true;
        }

        return divot;
    }
    private bool CheckForDivot(TileGrid grid, Coordinate currentCoordinate, string code, out Direction divotDirection)
    {
        Direction horizontalDivotDirection = Direction.North;
        Direction verticalDivotDirection = Direction.North;

        divotDirection = Direction.North;

        switch (code)
        {
            //SouthEast
            case "101111111":
                verticalDivotDirection = Direction.South;
                horizontalDivotDirection = Direction.East;
                break;
            //NorthWest
            case "111111110":
                verticalDivotDirection = Direction.North;
                horizontalDivotDirection = Direction.West;
                break;
            //SouthWest
            case "111011111":
                verticalDivotDirection = Direction.South;
                horizontalDivotDirection = Direction.West;
                break;
            //NorthEast
            case "111111011":
                verticalDivotDirection = Direction.North;
                horizontalDivotDirection = Direction.East;
                break;
            default:
                break;
        }

        Coordinate horizontalCoordinate = currentCoordinate + Coordinate.GetCoordinateFromDirection(horizontalDivotDirection);
        Coordinate verticalCoordinate = currentCoordinate + Coordinate.GetCoordinateFromDirection(verticalDivotDirection);

        bool divot = false;

        if (grid.TryGetTile(horizontalCoordinate))
        {
            string horizontalCode = DetermineTileCode(grid, horizontalCoordinate);
            bool corner = horizontalCode == "101111111" || horizontalCode == "111111110" || horizontalCode == "111011111" || horizontalCode == "111111011";
            bool westEdge = horizontalCode == "111010110" || horizontalCode == "111110110" || horizontalCode == "111010111";
            bool eastEdge = horizontalCode == "101101011" || horizontalCode == "111101011" || horizontalCode == "100111111";

            if (corner || westEdge || eastEdge)
            {
                divotDirection = horizontalDivotDirection;
                divot = true;
            }

        }

        if (grid.TryGetTile(verticalCoordinate))
        {
            string verticalCode = DetermineTileCode(grid, verticalCoordinate);

            bool corner = verticalCode == "101111111" || verticalCode == "111111110" || verticalCode == "111011111" || verticalCode == "111111011";
            bool northEdge = verticalCode == "111111000" || verticalCode == "111111001" || verticalCode == "111111100";
            bool southEdge = verticalCode == "100011111" || verticalCode == "110011111" || verticalCode == "100111111";

            if (corner || northEdge || southEdge)
            {
                divotDirection = verticalDivotDirection;
                divot = true;
            }
        }

        return divot;
    }
    private bool DetermineStartDirection(TileGrid grid, Coordinate currentCoordinate, Direction horizontalDirection, Direction verticalDirection, HashSet<Coordinate> newBorderCoordinates, HashSet<Coordinate> foundBorderCoordinates, out Direction startDirection)
    {
        startDirection = Direction.North;
        Coordinate horizontalCoordinate = currentCoordinate + Coordinate.GetCoordinateFromDirection(horizontalDirection);
        Coordinate verticalCoordinate = currentCoordinate + Coordinate.GetCoordinateFromDirection(verticalDirection);

        if (grid.TryGetTile(horizontalCoordinate, out Tile horizontalTile) && !newBorderCoordinates.Contains(horizontalCoordinate) && !foundBorderCoordinates.Contains(horizontalCoordinate))
        {
            startDirection = horizontalDirection;
            return true;
        }

        else if (grid.TryGetTile(verticalCoordinate, out Tile verticalTile) && !newBorderCoordinates.Contains(verticalCoordinate) && !foundBorderCoordinates.Contains(verticalCoordinate))
        {
            startDirection = verticalDirection;
            return true;
        }

        return false;
    }
    private bool FindNextDirection(TileGrid grid, Coordinate startCoordinate, HashSet<Coordinate> newBorderCoordinates, HashSet<Coordinate> foundBorderCoordinates, out Direction direction)
    {
        direction = Direction.North;
        Coordinate northCoordinate = startCoordinate + Coordinate.North;
        Coordinate southCoordinate = startCoordinate + Coordinate.South;
        Coordinate westCoordinate = startCoordinate + Coordinate.West;
        Coordinate eastCoordinate = startCoordinate + Coordinate.East;


        if (grid.TryGetTile(northCoordinate, out Tile northTile))
        {
            if (northTile.State == TileState.Occupied && !newBorderCoordinates.Contains(northCoordinate) && !foundBorderCoordinates.Contains(northCoordinate) && DetermineTileCode(grid, northCoordinate) != "111111111")
            {
                direction = Direction.North;
                return true;
            }
        }
        if (grid.TryGetTile(southCoordinate, out Tile southTile))
        {
            if (southTile.State == TileState.Occupied && !newBorderCoordinates.Contains(southCoordinate) && !foundBorderCoordinates.Contains(southCoordinate) && DetermineTileCode(grid, southCoordinate) != "111111111")
            {
                direction = Direction.South;
                return true;
            }
        }
        if (grid.TryGetTile(westCoordinate, out Tile westTile))
        {
            if (westTile.State == TileState.Occupied && !newBorderCoordinates.Contains(westCoordinate) && !foundBorderCoordinates.Contains(westCoordinate) && DetermineTileCode(grid, westCoordinate) != "111111111")
            {
                direction = Direction.West;
                return true;
            }
        }
        if (grid.TryGetTile(eastCoordinate, out Tile eastTile))
        {
            if (eastTile.State == TileState.Occupied && !newBorderCoordinates.Contains(eastCoordinate) && !foundBorderCoordinates.Contains(eastCoordinate) && DetermineTileCode(grid, eastCoordinate) != "111111111")
            {
                direction = Direction.East;
                return true;
            }
        }

        return false;
    }
    private bool TryGetBorderSprite(TileGrid grid, Coordinate coordinate, string code, out TileSprite sprite, out Coordinate incorrectCoordinate)
    {
        sprite = TileSprite.Occupied;
        incorrectCoordinate = new(-1, -1);

        switch (code)
        {
            case "111111000":
                if (CheckForDivot(grid, coordinate, "111111000", out bool edgeCase))
                    sprite = TileSprite.EdgeSouth;
                else
                    sprite = TileSprite.BorderEdgeNorth;
                break;
            case "111111001":
                if (CheckForDivot(grid, coordinate, "111111001", out edgeCase))
                    sprite = TileSprite.EdgeSouth;
                else
                    sprite = TileSprite.BorderEdgeNorth;
                break;
            case "111111100":
                if (CheckForDivot(grid, coordinate, "111111100", out edgeCase))
                    sprite = TileSprite.EdgeSouth;
                else
                    sprite = TileSprite.BorderEdgeNorth;
                break;
            case "100011111":
                if (CheckForDivot(grid, coordinate, "100011111", out edgeCase))
                    sprite = TileSprite.EdgeNorth;
                else
                    sprite = TileSprite.BorderEdgeSouth;
                break;
            case "110011111":
                if (CheckForDivot(grid, coordinate, "110011111", out edgeCase))
                    sprite = TileSprite.EdgeNorth;
                else
                    sprite = TileSprite.BorderEdgeSouth;
                break;
            case "100111111":
                if (CheckForDivot(grid, coordinate, "100111111", out edgeCase))
                    sprite = TileSprite.EdgeNorth;
                else
                    sprite = TileSprite.BorderEdgeSouth;
                break;
            case "111010110":
                sprite = TileSprite.BorderEdgeWest;
                break;
            case "111110110":
                sprite = TileSprite.BorderEdgeWest;
                break;
            case "111010111":
                sprite = TileSprite.BorderEdgeWest;
                break;
            case "101101011":
                sprite = TileSprite.BorderEdgeEast;
                break;
            case "111101011":
                sprite = TileSprite.BorderEdgeEast;
                break;
            case "101101111":
                sprite = TileSprite.BorderEdgeEast;
                break;
            case "101111111":
                if (CheckForDivot(grid, coordinate, "101111111", out Direction divotDirection))
                {
                    if (grid.TryGetTile(coordinate + Coordinate.South))
                    {
                        if (DetermineTileCode(grid, coordinate + Coordinate.South) == "111111001")
                        {
                            sprite = TileSprite.InverseCornerNorthWest;
                            break;
                        }
                    }
                    switch (divotDirection)
                    {
                        case Direction.East:
                            sprite = TileSprite.BorderSouthFunnelWest;
                            break;
                        case Direction.South:
                            sprite = TileSprite.BorderEastFunnelNorth;
                            break;
                        default:
                            sprite = TileSprite.BorderCornerSouthEast;
                            break;
                    }
                }
                else
                    sprite = TileSprite.BorderCornerSouthEast;
                break;
            case "111111110":
                if (CheckForDivot(grid, coordinate, "111111110", out divotDirection))
                {
                    if (grid.TryGetTile(coordinate + Coordinate.North))
                    {
                        if (DetermineTileCode(grid, coordinate + Coordinate.North) == "110011111")
                        {
                            sprite = TileSprite.InverseCornerSouthEast;
                            break;
                        }
                    }
                    switch (divotDirection)
                    {
                        case Direction.West:
                            sprite = TileSprite.BorderNorthFunnelEast;
                            break;
                        case Direction.North:
                            sprite = TileSprite.BorderWestFunnelSouth;
                            incorrectCoordinate = coordinate + Coordinate.North;
                            break;
                        default:
                            sprite = TileSprite.BorderCornerNorthWest;
                            break;
                    }
                }
                else
                    sprite = TileSprite.BorderCornerNorthWest;
                break;
            case "111011111":
                if (CheckForDivot(grid, coordinate, "111011111", out divotDirection))
                {
                    if (grid.TryGetTile(coordinate + Coordinate.South))
                    {
                        if (DetermineTileCode(grid, coordinate + Coordinate.South) == "111111100")
                        {
                            sprite = TileSprite.InverseCornerNorthEast;
                            break;
                        }
                    }

                    switch (divotDirection)
                    {
                        case Direction.West:
                            sprite = TileSprite.BorderSouthFunnelEast;
                            break;
                        case Direction.South:
                            sprite = TileSprite.BorderWestFunnelNorth;
                            break;
                        default:
                            sprite = TileSprite.BorderCornerSouthWest;
                            break;
                    }
                }
                else
                    sprite = TileSprite.BorderCornerSouthWest;
                break;
            case "111111011":
                if (CheckForDivot(grid, coordinate, "111111011", out divotDirection))
                {
                    if (grid.TryGetTile(coordinate + Coordinate.North))
                    {
                        if (DetermineTileCode(grid, coordinate + Coordinate.North) == "100111111")
                        {
                            sprite = TileSprite.InverseCornerSouthWest;
                            break;
                        }
                    }
                    switch (divotDirection)
                    {
                        case Direction.East:
                            sprite = TileSprite.BorderNorthFunnelWest;
                            break;
                        case Direction.North:
                            sprite = TileSprite.BorderEastFunnelSouth;
                            break;
                        default:
                            sprite = TileSprite.BorderCornerNorthEast;
                            break;
                    }
                }
                else
                    sprite = TileSprite.BorderCornerNorthEast;
                break;
            default:
                return false;
        }

        return true;
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
                tileSprite = TileSprite.EdgeEast;
                break;
            case "111010111":
                tileSprite = TileSprite.EdgeEast;
                break;
            case "111110110":
                tileSprite = TileSprite.EdgeEast;
                break;
            case "101101011":
                tileSprite = TileSprite.EdgeWest;
                break;
            case "101101111":
                tileSprite = TileSprite.EdgeWest;
                break;
            case "111101011":
                tileSprite = TileSprite.EdgeWest;
                break;
            case "100011111":
                tileSprite = TileSprite.EdgeNorth;
                break;
            case "100111111":
                tileSprite = TileSprite.EdgeNorth;
                break;
            case "110011111":
                tileSprite = TileSprite.EdgeNorth;
                break;
            case "111111000":
                tileSprite = TileSprite.EdgeSouth;
                break;
            case "111111001":
                tileSprite = TileSprite.EdgeSouth;
                break;
            case "111111100":
                tileSprite = TileSprite.EdgeSouth;
                break;
            case "101101000":
                tileSprite = TileSprite.CornerSouthWest;
                break;
            case "111101000":
                tileSprite = TileSprite.CornerSouthWest;
                break;
            case "101101001":
                tileSprite = TileSprite.CornerSouthWest;
                break;
            case "111010000":
                tileSprite = TileSprite.CornerSouthEast;
                break;
            case "111110000":
                tileSprite = TileSprite.CornerSouthEast;
                break;
            case "111010100":
                tileSprite = TileSprite.CornerSouthEast;
                break;
            case "100001011":
                tileSprite = TileSprite.CornerNorthWest;
                break;
            case "100001111":
                tileSprite = TileSprite.CornerNorthWest;
                break;
            case "100101011":
                tileSprite = TileSprite.CornerNorthWest;
                break;
            case "100010110":
                tileSprite = TileSprite.CornerNorthEast;
                break;
            case "100010111":
                tileSprite = TileSprite.CornerNorthEast;
                break;
            case "110010110":
                tileSprite = TileSprite.CornerNorthEast;
                break;
            case "101111111":
                tileSprite = TileSprite.InverseCornerNorthWest;
                break;
            case "111011111":
                tileSprite = TileSprite.InverseCornerNorthEast;
                break;
            case "111111011":
                tileSprite = TileSprite.InverseCornerSouthWest;
                break;
            case "111111110":
                tileSprite = TileSprite.InverseCornerSouthEast;
                break;
            default:
                return false;
        }

        return true;
    }
}
