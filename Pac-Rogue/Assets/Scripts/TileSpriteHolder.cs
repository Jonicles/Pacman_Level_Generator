using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpriteHolder : MonoBehaviour
{
    public static TileSpriteHolder Instance;
    [SerializeField] Sprite emptyTile;
    [SerializeField] Sprite pelletTile;
    [SerializeField] Sprite occupiedTile;

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

    public Sprite GetSprite(TileState tileState)
    {
        Sprite sprite;

        switch (tileState)
        {
            case TileState.Empty:
                sprite = emptyTile;
                break;
            case TileState.Pellet:
                sprite = pelletTile;
                break;
            case TileState.Occupied:
                sprite = occupiedTile;
                break;
            default:
                sprite = emptyTile;
                break;
        }

        return sprite;
    }
}
