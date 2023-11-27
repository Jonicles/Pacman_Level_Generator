using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpriteHolder : MonoBehaviour
{
    public static TileSpriteHolder Instance;
    [SerializeField] Sprite emptySprite;
    [SerializeField] Sprite pelletSprite;
    [SerializeField] Sprite occupiedSprite;
    [SerializeField] Sprite edgeUpSprite;
    [SerializeField] Sprite edgeDownSprite;
    [SerializeField] Sprite edgeLeftSprite;
    [SerializeField] Sprite edgeRightSprite;
    [SerializeField] Sprite cornerUpLeftSprite;
    [SerializeField] Sprite cornerUpRightSprite;
    [SerializeField] Sprite cornerDownLeftSprite;
    [SerializeField] Sprite cornerDownRightSprite;
    [SerializeField] Sprite inverserCornerUpLeftSprite;
    [SerializeField] Sprite inverseCornerUpRightSprite;
    [SerializeField] Sprite inverseCornerDownLeftSprite;
    [SerializeField] Sprite inverseCornerDownRightSprite;
    
    [SerializeField] Sprite doubleInverseForwardSprite;
    [SerializeField] Sprite doubleInverseBackwardSprite;
    [SerializeField] Sprite SoftCornerDownLeftSprite;
    [SerializeField] Sprite SoftCornerDownRightSprite;
    [SerializeField] Sprite FunnelDownLeftSprite;
    [SerializeField] Sprite FunnerlDownRightSprite;


    [SerializeField] Sprite ghostDoorSprite;
    [SerializeField] Sprite ghostDoorConnectorRightSprite;
    [SerializeField] Sprite ghostDoorConnectorLeftSprite;
    [SerializeField] Sprite ghostBoxEdgeUpSprite;
    [SerializeField] Sprite ghostBoxEdgeDown;
    [SerializeField] Sprite ghostBoxEdgeLeftSprite;
    [SerializeField] Sprite ghostBoxEdgeRightSprite;
    [SerializeField] Sprite ghostBoxCornerUpRightSprite;
    [SerializeField] Sprite ghostBoxCornerUpLeftSprite;
    [SerializeField] Sprite ghostBoxCornerDownRightSprite;
    [SerializeField] Sprite ghostBoxCornerDownLeftSprite;

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
            TileSprite.EdgeUp => edgeUpSprite,
            TileSprite.EdgeDown => edgeDownSprite,
            TileSprite.EdgeRight => edgeRightSprite,
            TileSprite.EdgeLeft => edgeLeftSprite,
            TileSprite.CornerDownLeft => cornerDownLeftSprite,
            TileSprite.CornerDownRight => cornerDownRightSprite,
            TileSprite.CornerUpLeft => cornerUpLeftSprite,
            TileSprite.CornerUpRight => cornerUpRightSprite,
            TileSprite.InverseCornerDownLeft => inverseCornerDownLeftSprite,
            TileSprite.InverseCornerDownRight => inverseCornerDownRightSprite,
            TileSprite.InverseCornerUpLeft => inverserCornerUpLeftSprite,
            TileSprite.InverseCornerUpRight => inverseCornerUpRightSprite,
            //Unorthodox
            TileSprite.DoubleInverseForward => doubleInverseForwardSprite,
            TileSprite.DoubleInverseBackward => doubleInverseBackwardSprite,
            TileSprite.SoftCornerDownLeft => SoftCornerDownLeftSprite,
            TileSprite.SoftCornerDownRight => SoftCornerDownRightSprite,
            TileSprite.FunnelDownLeft => FunnelDownLeftSprite,
            TileSprite.FunnelDownRight => FunnerlDownRightSprite,
            //Ghost
            TileSprite.GhostDoor => ghostDoorSprite,
            TileSprite.GhostDoorConnectorRight => ghostDoorConnectorRightSprite,
            TileSprite.GhostDoorConnectorLeft => ghostDoorConnectorLeftSprite,
            TileSprite.GhostBoxEdgeUp => ghostBoxEdgeUpSprite,
            TileSprite.GhostBoxEdgeDown => ghostBoxEdgeDown,
            TileSprite.GhostBoxEdgeLeft => ghostBoxEdgeLeftSprite,
            TileSprite.GhostBoxEdgeRight => ghostBoxEdgeRightSprite,
            TileSprite.GhostBoxCornerUpRight => ghostBoxCornerUpRightSprite,
            TileSprite.GhostBoxCornerUpLeft => ghostBoxCornerUpLeftSprite,
            TileSprite.GhostBoxCornerDownRight => ghostBoxCornerDownRightSprite,
            TileSprite.GhostBoxCornerDownLeft => ghostBoxCornerDownLeftSprite,
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
    EdgeUp,
    EdgeDown,
    EdgeRight,
    EdgeLeft,
    CornerDownLeft,
    CornerDownRight,
    CornerUpLeft,
    CornerUpRight,
    InverseCornerDownLeft,
    InverseCornerDownRight,
    InverseCornerUpLeft,
    InverseCornerUpRight,
    
    DoubleInverseForward,
    DoubleInverseBackward,
    SoftCornerDownLeft,
    SoftCornerDownRight,
    FunnelDownLeft,
    FunnelDownRight,
    
    GhostDoor,
    GhostDoorConnectorRight,
    GhostDoorConnectorLeft,
    GhostBoxEdgeUp,
    GhostBoxEdgeDown,
    GhostBoxEdgeLeft,
    GhostBoxEdgeRight,
    GhostBoxCornerUpLeft,
    GhostBoxCornerUpRight,
    GhostBoxCornerDownLeft,
    GhostBoxCornerDownRight

}
