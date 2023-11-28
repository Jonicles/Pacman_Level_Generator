using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpriteHolder : MonoBehaviour
{
    public static TileSpriteHolder Instance;
    [SerializeField] Sprite emptySprite;
    [SerializeField] Sprite pelletSprite;
    [SerializeField] Sprite occupiedSprite;
    [SerializeField] Sprite edgeNorthSprite;
    [SerializeField] Sprite edgeSouthSprite;
    [SerializeField] Sprite edgeWestSprite;
    [SerializeField] Sprite edgeEastSprite;
    [SerializeField] Sprite cornerNorthWestSprite;
    [SerializeField] Sprite cornerNorthEastSprite;
    [SerializeField] Sprite cornerSouthWestSprite;
    [SerializeField] Sprite cornerSouthEastSprite;
    [SerializeField] Sprite inverseCornerNorthWestSprite;
    [SerializeField] Sprite inverseCornerNorthEastSprite;
    [SerializeField] Sprite inverseCornerSouthWestSprite;
    [SerializeField] Sprite inverseCornerSouthEastSprite;

    [SerializeField] Sprite borderEdgeNorthSprite;
    [SerializeField] Sprite borderEdgeSouthSprite;
    [SerializeField] Sprite borderEdgeWestSprite;
    [SerializeField] Sprite borderEdgeEastSprite;
    [SerializeField] Sprite borderCornerNorthWestSprite;
    [SerializeField] Sprite borderCornerNorthEastSprite;
    [SerializeField] Sprite borderCornerSouthWestSprite;
    [SerializeField] Sprite borderCornerSouthEastSprite;
    [SerializeField] Sprite borderNorthFunnelWestSprite;
    [SerializeField] Sprite borderNorthFunnelEastSprite;
    [SerializeField] Sprite borderSouthFunnelEastSprite;
    [SerializeField] Sprite borderSouthFunnelWestSprite;
    [SerializeField] Sprite borderWestFunnelNorthSprite;
    [SerializeField] Sprite borderWestFunnelSouthSprite;
    [SerializeField] Sprite borderEastFunnelNorthSprite;
    [SerializeField] Sprite borderEastFunnelSouthSprite;

    [SerializeField] Sprite ghostDoorSprite;
    [SerializeField] Sprite ghostDoorConnectorEastSprite;
    [SerializeField] Sprite ghostDoorConnectorWestSprite;
    [SerializeField] Sprite ghostEdgeNorthSprite;
    [SerializeField] Sprite ghostEdgeSouth;
    [SerializeField] Sprite ghostEdgeWestSprite;
    [SerializeField] Sprite ghostEdgeEastSprite;
    [SerializeField] Sprite ghostCornerNorthEastSprite;
    [SerializeField] Sprite ghostCornerNorthWestSprite;
    [SerializeField] Sprite ghostCornerSouthEastSprite;
    [SerializeField] Sprite ghostCornerSouthWestSprite;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this);
    }

    public Sprite GetSprite(TileSprite tileSprite)
    {
        Sprite sprite = tileSprite switch
        {
            //Normal tiles
            TileSprite.Empty => emptySprite,
            TileSprite.Pellet => pelletSprite,
            TileSprite.Occupied => occupiedSprite,
            TileSprite.EdgeNorth => edgeNorthSprite,
            TileSprite.EdgeSouth => edgeSouthSprite,
            TileSprite.EdgeEast => edgeEastSprite,
            TileSprite.EdgeWest => edgeWestSprite,
            TileSprite.CornerSouthWest => cornerSouthWestSprite,
            TileSprite.CornerSouthEast => cornerSouthEastSprite,
            TileSprite.CornerNorthWest => cornerNorthWestSprite,
            TileSprite.CornerNorthEast => cornerNorthEastSprite,
            TileSprite.InverseCornerSouthWest => inverseCornerSouthWestSprite,
            TileSprite.InverseCornerSouthEast => inverseCornerSouthEastSprite,
            TileSprite.InverseCornerNorthWest => inverseCornerNorthWestSprite,
            TileSprite.InverseCornerNorthEast => inverseCornerNorthEastSprite,

            //Border
            TileSprite.BorderEdgeNorth => borderEdgeNorthSprite,
            TileSprite.BorderEdgeSouth => borderEdgeSouthSprite,
            TileSprite.BorderEdgeWest => borderEdgeWestSprite,
            TileSprite.BorderEdgeEast => borderEdgeEastSprite,
            TileSprite.BorderCornerNorthWest => borderCornerNorthWestSprite,
            TileSprite.BorderCornerNorthEast => borderCornerNorthEastSprite,
            TileSprite.BorderCornerSouthWest => borderCornerSouthWestSprite,
            TileSprite.BorderCornerSouthEast => borderCornerSouthEastSprite,
            TileSprite.BorderNorthFunnelWest => borderNorthFunnelWestSprite,
            TileSprite.BorderNorthFunnelEast => borderNorthFunnelEastSprite,
            TileSprite.BorderSouthFunnelEast => borderSouthFunnelEastSprite,
            TileSprite.BorderSouthFunnelWest => borderSouthFunnelWestSprite,
            TileSprite.BorderWestFunnelNorth => borderWestFunnelNorthSprite,
            TileSprite.BorderWestFunnelSouth => borderWestFunnelSouthSprite,
            TileSprite.BorderEastFunnelNorth => borderEastFunnelNorthSprite,
            TileSprite.BorderEastFunnelSouth => borderEastFunnelSouthSprite,

            //Ghost
            TileSprite.GhostDoor => ghostDoorSprite,
            TileSprite.GhostDoorConnectorEast => ghostDoorConnectorEastSprite,
            TileSprite.GhostDoorConnectorWest => ghostDoorConnectorWestSprite,
            TileSprite.GhostEdgeNorth => ghostEdgeNorthSprite,
            TileSprite.GhostEdgeSouth => ghostEdgeSouth,
            TileSprite.GhostEdgeWest => ghostEdgeWestSprite,
            TileSprite.GhostEdgeEast => ghostEdgeEastSprite,
            TileSprite.GhostCornerNorthEast => ghostCornerNorthEastSprite,
            TileSprite.GhostCornerNorthWest => ghostCornerNorthWestSprite,
            TileSprite.GhostCornerSouthEast => ghostCornerSouthEastSprite,
            TileSprite.GhostCornerSouthWest => ghostCornerSouthWestSprite,
            _ => occupiedSprite,
        };
        return sprite;
    }
}

public enum TileSprite
{
    Empty,
    Pellet,
    Occupied,
    EdgeNorth,
    EdgeSouth,
    EdgeEast,
    EdgeWest,
    CornerSouthWest,
    CornerSouthEast,
    CornerNorthWest,
    CornerNorthEast,
    InverseCornerSouthWest,
    InverseCornerSouthEast,
    InverseCornerNorthWest,
    InverseCornerNorthEast,

    BorderEdgeNorth,
    BorderEdgeSouth,
    BorderEdgeWest,
    BorderEdgeEast,
    BorderCornerNorthWest,
    BorderCornerNorthEast,
    BorderCornerSouthWest,
    BorderCornerSouthEast,
    BorderNorthFunnelWest,
    BorderNorthFunnelEast,
    BorderSouthFunnelEast,
    BorderSouthFunnelWest,
    BorderWestFunnelNorth,
    BorderWestFunnelSouth,
    BorderEastFunnelNorth,
    BorderEastFunnelSouth,


    GhostDoor,
    GhostDoorConnectorEast,
    GhostDoorConnectorWest,
    GhostEdgeNorth,
    GhostEdgeSouth,
    GhostEdgeWest,
    GhostEdgeEast,
    GhostCornerNorthWest,
    GhostCornerNorthEast,
    GhostCornerSouthWest,
    GhostCornerSouthEast
}
