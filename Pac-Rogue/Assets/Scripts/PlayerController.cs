using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [SerializeField] TileGrid currentGrid;

    [SerializeField] float speedBetweenTiles = 0.1f;
    [SerializeField][Range(0.1f, 1)] float forgivenessDistance = 1;

    Coroutine moveRoutine;
    PlayerSpriteHandler spriteHandler;

    Coordinate currentCoordinate = new Coordinate(0, 0);
    
    Direction desiredDirection = Direction.West;
    Direction currentDirection = Direction.West;

    bool active = false;
    const float mouthOpenWideDistance = 0.7f;
    const float mouthCloseDistance = 0.3f;

    private void Awake()
    {
        ActionMapManager.playerInput.InGame.MoveUp.performed += UpPressed;
        ActionMapManager.playerInput.InGame.MoveDown.performed += DownPressed;
        ActionMapManager.playerInput.InGame.MoveLeft.performed += LeftPressed;
        ActionMapManager.playerInput.InGame.MoveRight.performed += RightPressed;

        if (!TryGetComponent(out spriteHandler))
            Debug.LogError("Sprite handler component is missing from player prefab");
    }


    private void UpPressed(InputAction.CallbackContext context)
    {
        desiredDirection = Direction.North;
    }
    private void DownPressed(InputAction.CallbackContext context)
    {
        desiredDirection = Direction.South;
    }
    private void LeftPressed(InputAction.CallbackContext context)
    {
        desiredDirection = Direction.West;
    }
    private void RightPressed(InputAction.CallbackContext context)
    {
        desiredDirection = Direction.East;
    }

    void Update()
    {
        if (!active)
            return;

        if (desiredDirection != currentDirection)
        {
            if (CheckDesiredCoordinate(desiredDirection, out Coordinate desiredCoordinate))
            {
                Vector2 currentPosition = transform.position;
                float distanceToPreviousTile = Vector2.Distance(currentPosition, new Vector2(currentCoordinate.X, currentCoordinate.Y));

                if (distanceToPreviousTile <= forgivenessDistance)
                {
                    //If all these condtitions are met we will begin moving the player to the desired tile

                    if (moveRoutine != null)
                    {
                        StopCoroutine(moveRoutine);
                        moveRoutine = null;
                    }

                    currentDirection = desiredDirection;
                    spriteHandler.ChangeDirection(currentDirection);
                    StartMove(desiredCoordinate);
                    return;
                }
            }
        }


        if (CheckDesiredCoordinate(currentDirection, out Coordinate nextCoordinate))
        {
            StartMove(nextCoordinate);
        }
    }

    bool CheckDesiredCoordinate(Direction direction, out Coordinate desiredCoordinate)
    {
        desiredCoordinate = GetNextCoordinate(direction);

        if (!currentGrid.TryGetTile(desiredCoordinate, out Tile desiredTile))
            return false;

        if (desiredTile.State == TileState.Occupied || desiredTile.State == TileState.GhostSpace)
            return false;

        return true;
    }

    Coordinate GetNextCoordinate(Direction direction)
    {
        //We will round up y direction just so that it will take priority over the x direction.
        //So if the player is pushing several direction at one time it will prioritize the vertical directions
        return currentCoordinate + Coordinate.GetCoordinateFromDirection(direction);
    }
    void StartMove(Coordinate nextCoordinate)
    {
        if (moveRoutine == null)
        {
            spriteHandler.OpenMouth();
            moveRoutine = StartCoroutine(Move(nextCoordinate));
        }
    }
    IEnumerator Move(Coordinate nextCoordinate)
    {
        float distanceMultiplier = Vector2.Distance(transform.position, new Vector2(nextCoordinate.X, nextCoordinate.Y));
        float speed = speedBetweenTiles * distanceMultiplier;
        float elapsedTime = 0;

        Vector2 startPos = transform.position;
        Vector2 endPos = new Vector2(nextCoordinate.X, nextCoordinate.Y);

        while (speed > elapsedTime)
        {
            transform.position = Vector2.Lerp(startPos, endPos, elapsedTime / speed);

            float currentDistance = Vector2.Distance(transform.position, endPos);

            if (currentDistance < mouthOpenWideDistance && currentDistance > mouthCloseDistance)
                spriteHandler.OpenMouthWide();
            else if (currentDistance < mouthCloseDistance)
                spriteHandler.CloseMouth();

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        currentCoordinate = nextCoordinate;
        spriteHandler.OpenMouth();

        if (currentGrid.TryGetTile(currentCoordinate, out Tile currentTile))
            currentTile.Collect();

        moveRoutine = null;
    }

    public void SetPosition(Coordinate coordinate)
    {
        currentCoordinate = coordinate;
        transform.position = new Vector2(coordinate.X, coordinate.Y);
    }

    public void SetGrid(TileGrid grid)
    {
        currentGrid = grid;
    }

    public void Activate()
    {
        active = true;
    }

    public void Deactivate()
    {
        active = false;

        desiredDirection = Direction.West;
        currentDirection = Direction.East;

        if (moveRoutine != null)
            StopCoroutine(moveRoutine);
    }

    private void OnDisable()
    {
        ActionMapManager.playerInput.InGame.MoveUp.performed -= UpPressed;
        ActionMapManager.playerInput.InGame.MoveDown.performed -= DownPressed;
        ActionMapManager.playerInput.InGame.MoveLeft.performed -= LeftPressed;
        ActionMapManager.playerInput.InGame.MoveRight.performed -= RightPressed;

    }
}
