using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
    [SerializeField] TileGrid currentGrid;
    [SerializeField] float desiredSpeed = 0.1f;
    Coroutine moveRoutine;

    Vector2 currentDirection = Vector2.right;

    Coordinate currentCoordinate = new Coordinate(0, 0);

    private void Start()
    {
        ActionMapManager.playerInput.InGame.Move.performed += ChangeDirection;
    }
    private void Update()
    {
        Coordinate nextCoordinate = GetNextCoordinate();
        print(nextCoordinate);

        if (currentGrid.TryGetTile(nextCoordinate, out GameObject tile))
        {
            StartMove(nextCoordinate);
        }
    }

    private void ChangeDirection(InputAction.CallbackContext context)
    {
        currentDirection = context.ReadValue<Vector2>();
        currentDirection.Normalize();

        if (moveRoutine != null)
        {
            Vector2 currentPos = transform.position;

            Coordinate nextCoordinate = GetNextCoordinate();
            float distanceToNextTile = Vector2.Distance(currentPos, new Vector2(nextCoordinate.Y, nextCoordinate.Y));
            float distanceToPreviousTile = Vector2.Distance(currentPos, new Vector2(currentCoordinate.X, currentCoordinate.Y));

            if (distanceToPreviousTile > distanceToNextTile) 
            {
                StopCoroutine(moveRoutine);
                moveRoutine = null;
            }
        }

        
    }
    Coordinate GetNextCoordinate()
    {
        return new Coordinate(currentCoordinate.X + (int)currentDirection.x, currentCoordinate.Y + (int)Math.Round(currentDirection.y));
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
