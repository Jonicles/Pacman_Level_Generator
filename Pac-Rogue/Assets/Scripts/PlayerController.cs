using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    int xCoordinate = 0;
    int yCoordinate = 0;

    [SerializeField] TileGrid currentGrid;

    private void Start()
    {
        ActionMapManager.playerInput.InGame.Move.performed += Move;
    }

    private void Move(InputAction.CallbackContext context)
    {
        Vector2 direction = context.ReadValue<Vector2>();

        print(direction);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (currentGrid.TryGetTile(xCoordinate, yCoordinate + 1, out GameObject tile))
            {
                transform.position = new Vector2(xCoordinate, yCoordinate + 1);
                yCoordinate++;
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (currentGrid.TryGetTile(xCoordinate + 1, yCoordinate, out GameObject tile))
            {
                transform.position = new Vector2(xCoordinate + 1, yCoordinate);
                xCoordinate++;
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (currentGrid.TryGetTile(xCoordinate, yCoordinate - 1, out GameObject tile))
            {
                transform.position = new Vector2(xCoordinate, yCoordinate - 1);
                yCoordinate--;
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            if (currentGrid.TryGetTile(xCoordinate - 1, yCoordinate, out GameObject tile))
            {
                transform.position = new Vector2(xCoordinate - 1, yCoordinate);
                xCoordinate--;
            }
        }
    }
}
