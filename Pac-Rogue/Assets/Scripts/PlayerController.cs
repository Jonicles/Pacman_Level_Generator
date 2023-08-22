using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [SerializeField] TileGrid currentGrid;
    [SerializeField] float desiredSpeed = 0.1f;
    [SerializeField][Range(0.1f, 1)] float maxForgiveDistance = 1;
    Coroutine moveRoutine;

    Vector2 desiredDirection;
    Vector2 currentDirection = Vector2.left;
    Coordinate currentCoordinate = new Coordinate(0, 0);



    private void Start()
    {
        ActionMapManager.playerInput.InGame.Move.performed += ChangeDirection;
    }
    private void Update()
    {
        Coordinate nextCoordinate = GetNextCoordinate(currentDirection);

        if (currentGrid.TryGetTile(nextCoordinate, out Tile nextTile))
        {
            if (nextTile.State == TileState.Empty)
                StartMove(nextCoordinate);
        }
    }

    private void ChangeDirection(InputAction.CallbackContext context)
    {
        currentDirection = context.ReadValue<Vector2>();
        currentDirection.Normalize();

        if (moveRoutine == null)
            return;

        //if(GetNextCoordinate())

        Vector2 currentPosition = transform.position;
        float distanceToPreviousTile = Vector2.Distance(currentPosition, new Vector2(currentCoordinate.X, currentCoordinate.Y));

        if (distanceToPreviousTile <= maxForgiveDistance)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }


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
