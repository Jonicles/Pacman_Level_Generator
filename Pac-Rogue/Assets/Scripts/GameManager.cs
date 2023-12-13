using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Sits on game manager game object
public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    PlayerController currentPlayer;
    TileGridGenerator generator;
    TileGrid currentGrid;

    Coordinate startCoordinate = new Coordinate(13, 13);
    private void Awake()
    {
        if (!TryGetComponent(out generator))
            Debug.LogError("No generator on game manager object");
    }

    private void Start()
    {
        GenerateNewLevel();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateNewLevel();
        }
    }

    public void GenerateNewLevel()
    {
        
        if (currentPlayer == null)
        {
            GameObject playerObject = Instantiate(playerPrefab, new Vector2(startCoordinate.X, startCoordinate.Y), Quaternion.identity);
            currentPlayer = playerObject.GetComponent<PlayerController>();
        }
        else
            currentPlayer.Deactivate();
        
        if(currentGrid != null)
            currentGrid.DestroyGrid();

        currentGrid = generator.GenerateGrid();

        currentPlayer.SetGrid(currentGrid);
        currentPlayer.SetPosition(startCoordinate);
        currentPlayer.Activate();

    }
}
