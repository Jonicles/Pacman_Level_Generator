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

    //Indexes for tile code, a 1 represent an occupied tile and a 0 an empty or pellet tile

    //[1] [2] [3]
    //[4] [0] [5]
    //[6] [7] [8]
    const string OCCUPIED_SURROUND = "111111111";

    const string EDGE_NORTH_1 = "100011111";
    const string EDGE_NORTH_2 = "100111111";
    const string EDGE_NORTH_3 = "110011111";

    const string EDGE_SOUTH_1 = "111111000";
    const string EDGE_SOUTH_2 = "111111001";
    const string EDGE_SOUTH_3 = "111111100";

    const string EDGE_WEST_1 = "101101011";
    const string EDGE_WEST_2 = "101101111";
    const string EDGE_WEST_3 = "111101011";

    const string EDGE_EAST_1 = "111010110";
    const string EDGE_EAST_2 = "111010111";
    const string EDGE_EAST_3 = "111110110";

    const string CORNER_NORTHEAST_1 = "100010110";
    const string CORNER_NORTHEAST_2 = "100010111";
    const string CORNER_NORTHEAST_3 = "110010110";

    const string CORNER_NORTHWEST_1 = "100001011";
    const string CORNER_NORTHWEST_2 = "100001111";
    const string CORNER_NORTHWEST_3 = "100101011";

    const string CORNER_SOUTHEAST_1 = "111010000";
    const string CORNER_SOUTHEAST_2 = "111110000";
    const string CORNER_SOUTHEAST_3 = "111010100";

    const string CORNER_SOUTHWEST_1 = "101101000";
    const string CORNER_SOUTHWEST_2 = "111101000";
    const string CORNER_SOUTHWEST_3 = "101101001";

    const string INVERSE_CORNER_NORTHEAST = "111011111";
    const string INVERSE_CORNER_NORTHWEST = "101111111";
    const string INVERSE_CORNER_SOUTHEAST = "111111110";
    const string INVERSE_CORNER_SOUTHWEST = "111111011";

    const string INCORRECT_BIG_C = "111110111";
    const string INCORRECT_SMALL_C = "101100011";
    const string INCORRECT_TL = "111100011";
    const string INCORRECT_CRANE = "101100111";

    const string INCORRECT_BIG_C_REVERSE = "111101111";
    const string INCORRECT_SMALL_C_REVERSE = "111000110";
    const string INCORRECT_TL_REVERSE = "111100110";
    const string INCORRECT_CRANE_REVERSE = "111000111";

    const string INCORRECT_U = "110111111";
    const string INCORRECT_U_UPSIDEDOWN = "111111101";

    const string INCORRECT_OCCUPIED = "101000010";

    const string BORDER_EDGE_NORTH_1 = "111111000";
    const string BORDER_EDGE_NORTH_2 = "111111100";
    const string BORDER_EDGE_NORTH_3 = "111111001";

    const string BORDER_EDGE_SOUTH_1 = "100011111";
    const string BORDER_EDGE_SOUTH_2 = "110011111";
    const string BORDER_EDGE_SOUTH_3 = "100111111";

    const string BORDER_EDGE_WEST_1 = "111010110";
    const string BORDER_EDGE_WEST_2 = "111110110";
    const string BORDER_EDGE_WEST_3 = "111010111";

    const string BORDER_EDGE_EAST_1 = "101101011";
    const string BORDER_EDGE_EAST_2 = "111101011";
    const string BORDER_EDGE_EAST_3 = "101101111";

    const string BORDER_CORNER_NORTHEAST = "111111011";
    const string BORDER_CORNER_NORTHWEST = "111111110";
    const string BORDER_CORNER_SOUTHEAST = "101111111";
    const string BORDER_CORNER_SOUTHWEST = "111011111";


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
        if (desiredHeight % 2 == 0)
            desiredHeight--;

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

        int halfHeight = (grid.GetLength(1) - 1) / 2;
        int halfWidth = grid.GetLength(0) / 2;

        int ghostBoxHeight = box.height;
        int ghostBoxWidth = box.width;
        int ghostBoxHalfHeight = (ghostBoxHeight - 1) / 2 ;
        int ghostBoxHalfWidth = ghostBoxWidth / 2;

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
                                case INCORRECT_BIG_C:
                                    currentTile.PlacePellet();
                                    if (grid.TryGetTile(currentCoordinate + Coordinate.West, out Tile westTile))
                                        westTile.PlacePellet();
                                    reUpdate = true;
                                    break;
                                case INCORRECT_SMALL_C:
                                    currentTile.PlacePellet();
                                    break;
                                case INCORRECT_TL:
                                    currentTile.PlacePellet();
                                    break;
                                case INCORRECT_CRANE:
                                    currentTile.PlacePellet();
                                    break;
                                //Right
                                case INCORRECT_BIG_C_REVERSE:
                                    currentTile.PlacePellet();
                                    if (grid.TryGetTile(currentCoordinate + Coordinate.East, out Tile eastTile))
                                        eastTile.PlacePellet();
                                    break;
                                case INCORRECT_SMALL_C_REVERSE:
                                    currentTile.PlacePellet();
                                    break;
                                case INCORRECT_TL_REVERSE:
                                    currentTile.PlacePellet();
                                    break;
                                case INCORRECT_CRANE_REVERSE:
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
                                case INCORRECT_U:
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
                                case INCORRECT_U_UPSIDEDOWN:
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

                        if (code != INCORRECT_OCCUPIED)
                            continue;

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
                            currentTile.UpdateDisplay(sprite);
                    }
                }
            }
        }
    }
    private bool TryGetTileSprite(string code, out TileSprite tileSprite)
    {
        tileSprite = TileSprite.Occupied;

        switch (code)
        {
            case OCCUPIED_SURROUND:
                tileSprite = TileSprite.Occupied;
                break;
            case EDGE_EAST_1:
                tileSprite = TileSprite.EdgeEast;
                break;
            case EDGE_EAST_2:
                tileSprite = TileSprite.EdgeEast;
                break;
            case EDGE_EAST_3:
                tileSprite = TileSprite.EdgeEast;
                break;
            case EDGE_WEST_1:
                tileSprite = TileSprite.EdgeWest;
                break;
            case EDGE_WEST_2:
                tileSprite = TileSprite.EdgeWest;
                break;
            case EDGE_WEST_3:
                tileSprite = TileSprite.EdgeWest;
                break;
            case EDGE_NORTH_1:
                tileSprite = TileSprite.EdgeNorth;
                break;
            case EDGE_NORTH_2:
                tileSprite = TileSprite.EdgeNorth;
                break;
            case EDGE_NORTH_3:
                tileSprite = TileSprite.EdgeNorth;
                break;
            case EDGE_SOUTH_1:
                tileSprite = TileSprite.EdgeSouth;
                break;
            case EDGE_SOUTH_2:
                tileSprite = TileSprite.EdgeSouth;
                break;
            case EDGE_SOUTH_3:
                tileSprite = TileSprite.EdgeSouth;
                break;
            case CORNER_SOUTHWEST_1:
                tileSprite = TileSprite.CornerSouthWest;
                break;
            case CORNER_SOUTHWEST_2:
                tileSprite = TileSprite.CornerSouthWest;
                break;
            case CORNER_SOUTHWEST_3:
                tileSprite = TileSprite.CornerSouthWest;
                break;
            case CORNER_SOUTHEAST_1:
                tileSprite = TileSprite.CornerSouthEast;
                break;
            case CORNER_SOUTHEAST_2:
                tileSprite = TileSprite.CornerSouthEast;
                break;
            case CORNER_SOUTHEAST_3:
                tileSprite = TileSprite.CornerSouthEast;
                break;
            case CORNER_NORTHWEST_1:
                tileSprite = TileSprite.CornerNorthWest;
                break;
            case CORNER_NORTHWEST_2:
                tileSprite = TileSprite.CornerNorthWest;
                break;
            case CORNER_NORTHWEST_3:
                tileSprite = TileSprite.CornerNorthWest;
                break;
            case CORNER_NORTHEAST_1:
                tileSprite = TileSprite.CornerNorthEast;
                break;
            case CORNER_NORTHEAST_2:
                tileSprite = TileSprite.CornerNorthEast;
                break;
            case CORNER_NORTHEAST_3:
                tileSprite = TileSprite.CornerNorthEast;
                break;
            case INVERSE_CORNER_NORTHWEST:
                tileSprite = TileSprite.InverseCornerNorthWest;
                break;
            case INVERSE_CORNER_NORTHEAST:
                tileSprite = TileSprite.InverseCornerNorthEast;
                break;
            case INVERSE_CORNER_SOUTHWEST:
                tileSprite = TileSprite.InverseCornerSouthWest;
                break;
            case INVERSE_CORNER_SOUTHEAST:
                tileSprite = TileSprite.InverseCornerSouthEast;
                break;
            default:
                return false;
        }

        return true;
    }

    private void ChangeBorderSprites(TileGrid grid)
    {
        HashSet<Coordinate> borderCoordinates = new HashSet<Coordinate>();

        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                Coordinate startCoordinate = new Coordinate(x, y);

                if (x == 0)
                {
                    if (FindBorder(grid, startCoordinate, Direction.East, out Coordinate borderCoordinate))
                        borderCoordinates.Add(borderCoordinate);
                }

                if (x == grid.GetLength(0) - 1)
                {
                    if (FindBorder(grid, startCoordinate, Direction.West, out Coordinate borderCoordinate))
                        borderCoordinates.Add(borderCoordinate);
                }

                if (y == 0)
                {
                    if (FindBorder(grid, startCoordinate, Direction.North, out Coordinate borderCoordinate))
                        borderCoordinates.Add(borderCoordinate);
                }

                if (y == grid.GetLength(1) - 1)
                {
                    if (FindBorder(grid, startCoordinate, Direction.South, out Coordinate borderCoordinate))
                        borderCoordinates.Add(borderCoordinate);
                }
            }
        }


        HashSet<Coordinate> additionalBorderCoordinates = new HashSet<Coordinate>();

        foreach (var coordinate in borderCoordinates)
        {
            additionalBorderCoordinates.Add(coordinate);
        }

        foreach (var coordinate in borderCoordinates)
        {
            additionalBorderCoordinates = FindBorderCoordinates(grid, coordinate, additionalBorderCoordinates);
        }

        string code = "";
        foreach (var coordinate in additionalBorderCoordinates)
        {
            code = DetermineTileCode(grid, coordinate);

            if (!TryGetBorderSprite(grid, coordinate, code, out TileSprite sprite))
                continue;

            if (!grid.TryGetTile(coordinate, out Tile tile))
                continue;

            tile.UpdateDisplay(sprite);
        }
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

            if (code != OCCUPIED_SURROUND)
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
    private HashSet<Coordinate> FindBorderCoordinates(TileGrid grid, Coordinate startCoordinate, HashSet<Coordinate> borderCoordinates)
    {
        borderCoordinates.Add(startCoordinate);
        string code = DetermineTileCode(grid, startCoordinate);

        HashSet<Direction> availableDirections = new HashSet<Direction>();
        availableDirections.Add(Direction.North);
        availableDirections.Add(Direction.South);
        availableDirections.Add(Direction.East);
        availableDirections.Add(Direction.West);


        if (CheckForDivot(grid, startCoordinate, code, out Direction divotDirection))
            availableDirections.Remove(divotDirection);

        List<Coordinate> coordinatesToCheck = new List<Coordinate>();

        foreach (var direction in availableDirections)
            coordinatesToCheck.Add(startCoordinate + Coordinate.GetCoordinateFromDirection(direction));

        foreach (var coordinate in coordinatesToCheck)
        {
            if (!grid.TryGetTile(coordinate, out Tile tile))
                continue;

            if (borderCoordinates.Contains(coordinate))
                continue;

            if (tile.State != TileState.Occupied)
                continue;

            string newCoordinateCode = DetermineTileCode(grid, coordinate);
            if (newCoordinateCode == OCCUPIED_SURROUND)
                continue;

            FindBorderCoordinates(grid, coordinate, borderCoordinates);
        }

        return borderCoordinates;
    }
    private bool CheckForDivot(TileGrid grid, Coordinate currentCoordinate, string code, out Direction divotDirection)
    {
        Direction horizontalDirection = Direction.North;
        Direction verticalDirection = Direction.North;
        divotDirection = Direction.North;

        bool corner = false;

        switch (code)
        {
            //SouthEast
            case BORDER_CORNER_SOUTHEAST:
                verticalDirection = Direction.South;
                horizontalDirection = Direction.East;
                corner = true;
                break;
            //NorthWest
            case BORDER_CORNER_NORTHWEST:
                verticalDirection = Direction.North;
                horizontalDirection = Direction.West;
                corner = true;
                break;
            //SouthWest
            case BORDER_CORNER_SOUTHWEST:
                verticalDirection = Direction.South;
                horizontalDirection = Direction.West;
                corner = true;
                break;
            //NorthEast
            case BORDER_CORNER_NORTHEAST:
                verticalDirection = Direction.North;
                horizontalDirection = Direction.East;
                corner = true;
                break;
            //SouthEdge
            case BORDER_EDGE_SOUTH_1:
                verticalDirection = Direction.South;
                break;
            case BORDER_EDGE_SOUTH_2:
                verticalDirection = Direction.South;
                break;
            case BORDER_EDGE_SOUTH_3:
                verticalDirection = Direction.South;
                break;
            //NorthEdge
            case BORDER_EDGE_NORTH_1:
                verticalDirection = Direction.North;
                break;
            case BORDER_EDGE_NORTH_2:
                verticalDirection = Direction.North;
                break;
            case BORDER_EDGE_NORTH_3:
                verticalDirection = Direction.North;
                break;
            //EastEdge
            case BORDER_EDGE_EAST_1:
                verticalDirection = Direction.East;
                break;
            case BORDER_EDGE_EAST_2:
                verticalDirection = Direction.East;
                break;
            case BORDER_EDGE_EAST_3:
                verticalDirection = Direction.East;
                break;
            //WestEdge
            case BORDER_EDGE_WEST_1:
                verticalDirection = Direction.West;
                break;
            case BORDER_EDGE_WEST_2:
                verticalDirection = Direction.West;
                break;
            case BORDER_EDGE_WEST_3:
                verticalDirection = Direction.West;
                break;
            default:
                return false;
        }

        Coordinate horizontalCoordinate = currentCoordinate + Coordinate.GetCoordinateFromDirection(horizontalDirection);
        Coordinate verticalCoordinate = currentCoordinate + Coordinate.GetCoordinateFromDirection(verticalDirection);

        bool divot = false;

        if (corner)
            divot = CheckCornerDivot(grid, horizontalDirection, verticalDirection, horizontalCoordinate, verticalCoordinate, out divotDirection);
        else
            divot = CheckEdgeDivot(grid, code, verticalCoordinate, out divotDirection);

        return divot;
    }
    private bool CheckEdgeDivot(TileGrid grid, string code, Coordinate coordinate, out Direction divotDirection)
    {
        divotDirection = Direction.North;
        bool divot = false;

        if (grid.TryGetTile(coordinate))
        {
            string newCode = DetermineTileCode(grid, coordinate);

            if (code == BORDER_EDGE_WEST_1 || code == BORDER_EDGE_WEST_2 || code == BORDER_EDGE_WEST_3)
            {
                if (newCode == BORDER_CORNER_NORTHEAST)
                {
                    divotDirection = Direction.South;
                    divot = true;
                }

                if (newCode == BORDER_CORNER_SOUTHEAST)
                {
                    divotDirection = Direction.North;
                    divot = true;
                }

            }

            if (code == BORDER_EDGE_EAST_1 || code == BORDER_EDGE_EAST_2 || code == BORDER_EDGE_EAST_3)
            {
                if (newCode == BORDER_CORNER_NORTHWEST)
                {
                    divotDirection = Direction.South;
                    divot = true;
                }

                if (newCode == BORDER_CORNER_SOUTHWEST)
                {
                    divotDirection = Direction.North;
                    divot = true;
                }
            }

            if (code == BORDER_EDGE_NORTH_1 || code == BORDER_EDGE_NORTH_2 || code == BORDER_EDGE_NORTH_3)
            {
                if (newCode == BORDER_CORNER_SOUTHWEST)
                {
                    divotDirection = Direction.East;
                    divot = true;
                }

                if (newCode == BORDER_CORNER_SOUTHEAST)
                {
                    divotDirection = Direction.West;
                    divot = true;
                }

            }

            if (code == BORDER_EDGE_SOUTH_1 || code == BORDER_EDGE_SOUTH_2 || code == BORDER_EDGE_SOUTH_3)
            {
                if (newCode == BORDER_CORNER_NORTHWEST)
                {
                    divotDirection = Direction.East;
                    divot = true;
                }

                if (newCode == BORDER_CORNER_NORTHEAST)
                {
                    divotDirection = Direction.West;
                    divot = true;
                }
            }
        }

        return divot;
    }
    private bool CheckCornerDivot(TileGrid grid, Direction horizontalDirection, Direction verticalDirection, Coordinate horizontalCoordinate, Coordinate verticalCoordinate, out Direction divotDirection)
    {
        bool divot = false;
        divotDirection = Direction.North;

        if (grid.TryGetTile(horizontalCoordinate))
        {
            string horizontalCode = DetermineTileCode(grid, horizontalCoordinate);

            bool adjacentCorner = horizontalCode == BORDER_CORNER_NORTHWEST || horizontalCode == BORDER_CORNER_NORTHEAST || horizontalCode == BORDER_CORNER_SOUTHWEST || horizontalCode == BORDER_CORNER_SOUTHEAST;
            bool adjacentEdgeWest = horizontalCode == BORDER_EDGE_WEST_1 || horizontalCode == BORDER_EDGE_WEST_2 || horizontalCode == BORDER_EDGE_WEST_3;
            bool adjacentEdgeEast = horizontalCode == BORDER_EDGE_EAST_1 || horizontalCode == BORDER_EDGE_EAST_2 || horizontalCode == BORDER_EDGE_EAST_3;

            if (adjacentCorner || adjacentEdgeWest || adjacentEdgeEast)
            {
                divotDirection = TranslateDirection(verticalDirection);
                divot = true;
                return divot;
            }

        }

        if (grid.TryGetTile(verticalCoordinate))
        {
            string verticalCode = DetermineTileCode(grid, verticalCoordinate);

            bool adjacentCorner = verticalCode == BORDER_CORNER_NORTHWEST || verticalCode == BORDER_CORNER_NORTHEAST || verticalCode == BORDER_CORNER_SOUTHWEST || verticalCode == BORDER_CORNER_SOUTHEAST;
            bool adjacentEdgeNorth = verticalCode == "100011111" || verticalCode == "110011111" || verticalCode == "100111111";
            bool adjacentEdgeSouth = verticalCode == "111111000" || verticalCode == "111111100" || verticalCode == "111111001";


            if (adjacentCorner || adjacentEdgeNorth || adjacentEdgeSouth)
            {
                divotDirection = TranslateDirection(horizontalDirection);
                divot = true;
                return divot;
            }
        }
        return false;
    }
    private Direction TranslateDirection(Direction verticalDirection)
    {
        switch (verticalDirection)
        {
            case Direction.North:
                return Direction.South;
            case Direction.South:
                return Direction.North;
            case Direction.East:
                return Direction.West;
            case Direction.West:
                return Direction.East;
            default:
                return Direction.North;
        }
    }
    private bool TryGetBorderSprite(TileGrid grid, Coordinate coordinate, string code, out TileSprite sprite)
    {
        sprite = TileSprite.Occupied;

        switch (code)
        {
            case BORDER_EDGE_NORTH_1:
                if (CheckForDivot(grid, coordinate, BORDER_EDGE_NORTH_1, out Direction direction))
                    sprite = TileSprite.EdgeSouth;
                else
                    sprite = TileSprite.BorderEdgeNorth;
                break;
            case BORDER_EDGE_NORTH_2:
                if (CheckForDivot(grid, coordinate, BORDER_EDGE_NORTH_2, out direction))
                    sprite = TileSprite.EdgeSouth;
                else
                    sprite = TileSprite.BorderEdgeNorth;
                break;
            case BORDER_EDGE_NORTH_3:
                if (CheckForDivot(grid, coordinate, BORDER_EDGE_NORTH_3, out direction))
                    sprite = TileSprite.EdgeSouth;
                else
                    sprite = TileSprite.BorderEdgeNorth;
                break;
            case BORDER_EDGE_SOUTH_1:
                if (CheckForDivot(grid, coordinate, BORDER_EDGE_SOUTH_1, out direction))
                    sprite = TileSprite.EdgeNorth;
                else
                    sprite = TileSprite.BorderEdgeSouth;
                break;
            case BORDER_EDGE_SOUTH_2:
                if (CheckForDivot(grid, coordinate, BORDER_EDGE_SOUTH_2, out direction))
                    sprite = TileSprite.EdgeNorth;
                else
                    sprite = TileSprite.BorderEdgeSouth;
                break;
            case BORDER_EDGE_SOUTH_3:
                if (CheckForDivot(grid, coordinate, BORDER_EDGE_SOUTH_3, out direction))
                    sprite = TileSprite.EdgeNorth;
                else
                    sprite = TileSprite.BorderEdgeSouth;
                break;
            case BORDER_EDGE_WEST_1:
                if (CheckForDivot(grid, coordinate, BORDER_EDGE_WEST_1, out direction))
                    sprite = TileSprite.EdgeEast;
                else
                    sprite = TileSprite.BorderEdgeWest;
                break;
            case BORDER_EDGE_WEST_2:
                if (CheckForDivot(grid, coordinate, BORDER_EDGE_WEST_2, out direction))
                    sprite = TileSprite.EdgeEast;
                else
                    sprite = TileSprite.BorderEdgeWest;
                break;
            case BORDER_EDGE_WEST_3:
                if (CheckForDivot(grid, coordinate, BORDER_EDGE_WEST_3, out direction))
                    sprite = TileSprite.EdgeEast;
                else
                    sprite = TileSprite.BorderEdgeWest;
                break;
            case BORDER_EDGE_EAST_1:
                if (CheckForDivot(grid, coordinate, BORDER_EDGE_EAST_1, out direction))
                    sprite = TileSprite.EdgeWest;
                else
                    sprite = TileSprite.BorderEdgeEast;
                break;
            case BORDER_EDGE_EAST_2:
                if (CheckForDivot(grid, coordinate, BORDER_EDGE_EAST_2, out direction))
                    sprite = TileSprite.EdgeWest;
                else
                    sprite = TileSprite.BorderEdgeEast;
                break;
            case BORDER_EDGE_EAST_3:
                if (CheckForDivot(grid, coordinate, BORDER_EDGE_EAST_3, out direction))
                    sprite = TileSprite.EdgeWest;
                else
                    sprite = TileSprite.BorderEdgeEast;
                break;
            case BORDER_CORNER_SOUTHEAST:
                if (CheckForDivot(grid, coordinate, BORDER_CORNER_SOUTHEAST, out Direction divotDirection))
                {
                    //if (grid.TryGetTile(coordinate + Coordinate.South))
                    //{
                    //    if (DetermineTileCode(grid, coordinate + Coordinate.South) == "111111001")
                    //    {
                    //        sprite = TileSprite.InverseCornerNorthWest;
                    //        break;
                    //    }
                    //}
                    switch (divotDirection)
                    {
                        case Direction.West:
                            sprite = TileSprite.BorderEastFunnelNorth;
                            break;
                        case Direction.North:
                            sprite = TileSprite.BorderSouthFunnelWest;
                            break;
                        default:
                            sprite = TileSprite.BorderCornerSouthEast;
                            break;
                    }
                }
                else
                    sprite = TileSprite.BorderCornerSouthEast;
                break;
            case BORDER_CORNER_SOUTHWEST:
                if (CheckForDivot(grid, coordinate, BORDER_CORNER_SOUTHWEST, out divotDirection))
                {
                    //if (grid.TryGetTile(coordinate + Coordinate.South))
                    //{
                    //    if (DetermineTileCode(grid, coordinate + Coordinate.South) == "111111100")
                    //    {
                    //        sprite = TileSprite.InverseCornerNorthEast;
                    //        break;
                    //    }
                    //}

                    switch (divotDirection)
                    {
                        case Direction.East:
                            sprite = TileSprite.BorderWestFunnelNorth;
                            break;
                        case Direction.North:
                            sprite = TileSprite.BorderSouthFunnelEast;
                            break;
                        default:
                            sprite = TileSprite.BorderCornerSouthWest;
                            break;
                    }
                }
                else
                    sprite = TileSprite.BorderCornerSouthWest;
                break;
            case BORDER_CORNER_NORTHWEST:
                if (CheckForDivot(grid, coordinate, BORDER_CORNER_NORTHWEST, out divotDirection))
                {
                    //if (grid.TryGetTile(coordinate + Coordinate.North))
                    //{
                    //    if (DetermineTileCode(grid, coordinate + Coordinate.North) == "110011111")
                    //    {
                    //        sprite = TileSprite.InverseCornerSouthEast;
                    //        break;
                    //    }
                    //}
                    switch (divotDirection)
                    {
                        case Direction.East:
                            sprite = TileSprite.BorderWestFunnelSouth;
                            break;
                        case Direction.South:
                            sprite = TileSprite.BorderNorthFunnelEast;
                            break;
                        default:
                            sprite = TileSprite.BorderCornerNorthWest;
                            break;
                    }
                }
                else
                    sprite = TileSprite.BorderCornerNorthWest;
                break;
            case BORDER_CORNER_NORTHEAST:
                if (CheckForDivot(grid, coordinate, BORDER_CORNER_NORTHEAST, out divotDirection))
                {
                    //if (grid.TryGetTile(coordinate + Coordinate.North))
                    //{
                    //    if (DetermineTileCode(grid, coordinate + Coordinate.North) == "100111111")
                    //    {
                    //        sprite = TileSprite.InverseCornerSouthWest;
                    //        break;
                    //    }
                    //}
                    switch (divotDirection)
                    {
                        case Direction.West:
                            sprite = TileSprite.BorderEastFunnelSouth;
                            break;
                        case Direction.South:
                            sprite = TileSprite.BorderNorthFunnelWest;
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
}
