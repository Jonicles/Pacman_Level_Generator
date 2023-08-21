using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMapManager : MonoBehaviour
{
    public static PlayerInput playerInput;

    private void Awake()
    {
        if (playerInput == null)
        {
            playerInput = new PlayerInput();
            playerInput.InGame.Enable();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
