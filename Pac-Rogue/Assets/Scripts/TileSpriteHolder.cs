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
        Sprite sprite;

        switch (tileSprite)
        {
            //Normal tiles
            case TileSprite.Empty:
                sprite = emptySprite;
                break;
            case TileSprite.Pellet:
                sprite = pelletSprite;
                break;
            case TileSprite.Occupied:
                sprite = occupiedSprite;
                break;
            case TileSprite.EdgeUp:
                sprite = edgeUpSprite;
                break;
            case TileSprite.EdgeDown:
                sprite = edgeDownSprite;
                break;
            case TileSprite.EdgeRight:
                sprite = edgeRightSprite;
                break;
            case TileSprite.EdgeLeft:
                sprite = edgeLeftSprite;
                break;
            case TileSprite.CornerDownLeft:
                sprite = cornerDownLeftSprite;
                break;
            case TileSprite.CornerDownRight:
                sprite = cornerDownRightSprite;
                break;
            case TileSprite.CornerUpLeft:
                sprite = cornerUpLeftSprite;
                break;
            case TileSprite.CornerUpRight:
                sprite = cornerUpRightSprite;
                break;
            case TileSprite.InverseCornerDownLeft:
                sprite = inverseCornerDownLeftSprite;
                break;
            case TileSprite.InverseCornerDownRight:
                sprite = inverseCornerDownRightSprite;
                break;
            case TileSprite.InverseCornerUpLeft:
                sprite = inverserCornerUpLeftSprite;
                break;
            case TileSprite.InverseCornerUpRight:
                sprite = inverseCornerUpRightSprite;
                break;

                //Unorthodox
            case TileSprite.DoubleInverseForward:
                sprite = doubleInverseForwardSprite;
                break;
            case TileSprite.DoubleInverseBackward:
                sprite = doubleInverseBackwardSprite;
                break;
            case TileSprite.SoftCornerDownLeft:
                sprite = SoftCornerDownLeftSprite;
                break;
            case TileSprite.SoftCornerDownRight:
                sprite = SoftCornerDownRightSprite;
                break;
            case TileSprite.FunnelDownLeft:
                sprite = FunnelDownLeftSprite;
                break;
            case TileSprite.FunnelDownRight:
                sprite = FunnerlDownRightSprite;
                break;

                //Ghost
            case TileSprite.GhostDoor:
                sprite = ghostDoorSprite;
                break;
            case TileSprite.GhostDoorConnectorRight:
                sprite = ghostDoorConnectorRightSprite;
                break;
            case TileSprite.GhostDoorConnectorLeft:
                sprite = ghostDoorConnectorLeftSprite;
                break;
            case TileSprite.GhostBoxEdgeUp:
                sprite = ghostBoxEdgeUpSprite;
                break;
            case TileSprite.GhostBoxEdgeDown:
                sprite = ghostBoxEdgeDown;
                break;
            case TileSprite.GhostBoxEdgeLeft:
                sprite = ghostBoxEdgeLeftSprite;
                break;
            case TileSprite.GhostBoxEdgeRight:
                sprite = ghostBoxEdgeRightSprite;
                break;
            case TileSprite.GhostBoxCornerUpRight:
                sprite = ghostBoxCornerUpRightSprite;
                break;
            case TileSprite.GhostBoxCornerUpLeft:
                sprite = ghostBoxCornerUpLeftSprite;
                break;
            case TileSprite.GhostBoxCornerDownRight:
                sprite = ghostBoxCornerDownRightSprite;
                break;
            case TileSprite.GhostBoxCornerDownLeft:
                sprite = ghostBoxCornerDownLeftSprite;
                break;
            default:
                sprite = occupiedSprite;
                break;
        }

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
