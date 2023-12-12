using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteHandler : MonoBehaviour
{
    [SerializeField] Sprite mouthOpenWideUp;
    [SerializeField] Sprite mouthOpenUp;

    [SerializeField] Sprite mouthOpenWideDown;
    [SerializeField] Sprite mouthOpenDown;

    [SerializeField] Sprite mouthOpenWideRight;
    [SerializeField] Sprite mouthOpenRight;

    [SerializeField] Sprite mouthOpenWideLeft;
    [SerializeField] Sprite mouthOpenLeft;
    
    [SerializeField] Sprite mouthClosed;

    SpriteRenderer spriteRenderer;
    Sprite currentMouthOpen;
    Sprite currentMouthOpenWide;

    public void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        currentMouthOpen = mouthOpenLeft;
        currentMouthOpenWide = mouthOpenWideLeft;
    }

    public void OpenMouth()
    {
        spriteRenderer.sprite = currentMouthOpen;
    }
    public void OpenMouthWide()
    {
        spriteRenderer.sprite = currentMouthOpenWide;
    }

    public void CloseMouth()
    {
        spriteRenderer.sprite = mouthClosed;
    }

    public void ChangeDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                currentMouthOpen = mouthOpenUp;
                currentMouthOpenWide = mouthOpenWideUp;
                break;
            case Direction.South:
                currentMouthOpen = mouthOpenDown;
                currentMouthOpenWide = mouthOpenWideDown;
                break;
            case Direction.East:
                currentMouthOpen = mouthOpenRight;
                currentMouthOpenWide = mouthOpenWideRight;
                break;
            case Direction.West:
                currentMouthOpen = mouthOpenLeft;
                currentMouthOpenWide = mouthOpenWideLeft;
                break;
            default:
                break;
        }
    }

}
