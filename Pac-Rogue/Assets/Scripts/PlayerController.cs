using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [SerializeField] TileGridGenerator currentGrid;
    [SerializeField] float desiredSpeed = 0.1f;
    [SerializeField][Range(0.1f, 1)] float maxForgivenessDistance = 1;
    Coroutine moveRoutine;

    Vector2 desiredDirection = Vector2.left;
    Vector2 currentDirection = Vector2.left;
    Coordinate currentCoordinate = new Coordinate(0, 0);

    void Start()
    {
        ActionMapManager.playerInput.InGame.Move.performed += ChangeDirection;
    }
    void Update()
    {
        if (desiredDirection != currentDirection)
        {
            if (CheckDesiredCoordinate(desiredDirection, out Coordinate desiredCoordinate))
            {
                Vector2 currentPosition = transform.position;
                float distanceToPreviousTile = Vector2.Distance(currentPosition, new Vector2(currentCoordinate.X, currentCoordinate.Y));

                if (distanceToPreviousTile <= maxForgivenessDistance)
                {
                    //If all these condtitions are met we will begin moving the player to the desired tile

                    if (moveRoutine != null)
                    {
                        StopCoroutine(moveRoutine);
                        moveRoutine = null;
                    }

                    currentDirection = desiredDirection;
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

    bool CheckDesiredCoordinate(Vector2 direction, out Coordinate desiredCoordinate)
    {
        desiredCoordinate = GetNextCoordinate(direction);

        //if (!currentGrid.TryGetTile(desiredCoordinate, out Tile desiredTile))
        //    return false;

        //if (desiredTile.State == TileState.Occupied)
        //    return false;

        return true;
    }

    void ChangeDirection(InputAction.CallbackContext context)
    {
        desiredDirection = context.ReadValue<Vector2>();
        desiredDirection.Normalize();
    }
    Coordinate GetNextCoordinate(Vector2 direction)
    {
        //We will round up y direction just so that it will take priority over the x direction.
        //So if the player is pushing several direction at one time it will prioritize the vertical directions
        return new Coordinate(currentCoordinate.X + (int)direction.x, currentCoordinate.Y + (int)Math.Round(direction.y));
    }
    void StartMove(Coordinate nextCoordinate)
    {
        if (moveRoutine == null)
        {
            moveRoutine = StartCoroutine(Move(nextCoordinate));
        }
    }
    IEnumerator Move(Coordinate nextCoordinate)
    {
        float distanceMultiplier = Vector2.Distance(transform.position, new Vector2(nextCoordinate.X, nextCoordinate.Y));
        float speed = desiredSpeed * distanceMultiplier;
        float elapsedTime = 0;

        Vector2 startPos = transform.position;
        Vector2 endPos = new Vector2(nextCoordinate.X, nextCoordinate.Y);

        while (speed > elapsedTime)
        {
            transform.position = Vector2.Lerp(startPos, endPos, elapsedTime / speed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        currentCoordinate = nextCoordinate;
        moveRoutine = null;
    }
}
