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
    InverseCornerUpRight
}
