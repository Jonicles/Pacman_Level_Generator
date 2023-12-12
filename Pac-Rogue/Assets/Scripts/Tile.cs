using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public TileState State { get { return state; } private set { state = value; } }
    [SerializeField] TileState state;

    private void Awake()
    {
        if (!TryGetComponent(out spriteRenderer))
            Debug.LogWarning("There is no Sprite Renderer attached to Tile prefab");
        
        State = TileState.Empty;
    }

    public void PlacePellet()
    {
        State = TileState.Pellet;
        UpdateDisplay(TileSprite.Pellet);
    }

    public void PlacePowerPellet()
    {
        State = TileState.PowerPellet;
        UpdateDisplay(TileSprite.PowerPellet);
    }

    public void Collect()
    {
        EmptyTile();
    }

    public void OccupyTile()
    {
        State = TileState.Occupied;
        UpdateDisplay(TileSprite.Occupied);
    }

    public void EmptyTile()
    {
        State = TileState.Empty;
        UpdateDisplay(TileSprite.Empty);
    }

    public void GhostTile()
    {
        State = TileState.GhostSpace;
        UpdateDisplay(TileSprite.Empty);
    }

    public void UpdateDisplay(TileSprite tileSprite)
    {
        spriteRenderer.sprite = TileSpriteHolder.Instance.GetSprite(tileSprite);
    }

    public void SetColor(Color newColor)
    {
        spriteRenderer.color = newColor;
    }
}

public enum TileState 
{ 
    Empty, 
    Pellet, 
    PowerPellet,
    Occupied,
    GhostSpace
}
