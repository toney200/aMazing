using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiplayerController : MonoBehaviour
{
    PlayerInputManager inputManager;
    PlayerInput playerInput;
    private int playerIndex;

    // Start is called before the first frame update
    void Start()
    {
       inputManager = GetComponent<PlayerInputManager>();
        playerInput = GetComponent<PlayerInput>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayerJoined(PlayerInput input)
    {
      
    }

    
}
