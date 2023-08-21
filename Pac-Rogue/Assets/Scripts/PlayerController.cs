using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
    int xCoordinate = 0;
    int yCoordinate = 0;
    Vector2 currentDirection = Vector2.zero;
    float speed = 0.2f;
    float currentTime = 0;

    [SerializeField] TileGrid currentGrid;

    private void Start()
    {
        ActionMapManager.playerInput.InGame.Move.performed += ChangeDirection;
    }

    private void ChangeDirection(InputAction.CallbackContext context)
    {
        currentDirection = context.ReadValue<Vector2>();
        currentDirection.Normalize();

    }

    private void Update()
    {
        currentTime += Time.deltaTime;

        if(currentTime > speed)
        {
            Move();
            currentTime = 0;
        }
    }

    void Move()
    {
        print("movin");
        int x = (int)currentDirection.x;
        int y = (int)currentDirection.y;
        if(currentGrid.TryGetTile(xCoordinate + x, yCoordinate + y, out GameObject tile))
        {
            xCoordinate += x;
            yCoordinate += y;
            transform.position = new Vector2(xCoordinate, yCoordinate);
        }
    }
}
