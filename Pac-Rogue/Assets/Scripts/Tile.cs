using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public TileState State { get; private set; }

    private void Awake()
    {
        if (!TryGetComponent(out spriteRenderer))
            Debug.LogWarning("There is no Sprite Renderer attached to Tile prefab");
        State = TileState.Empty;
    }

    public void PlacePellet()
    {
        State = TileState.Pellet;
        UpdateDisplay();
    }

    public void OccupyTile()
    {
        State = TileState.Occupied;
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        spriteRenderer.sprite = TileSpriteHolder.Instance.GetSprite(State);
    }
}
