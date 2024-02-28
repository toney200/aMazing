using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoiningBehaviour : MonoBehaviour
{
    private string[] controllers;
    public GameObject[] playerPrefabs;
    private int playerCount = 0;
    private PlayerInputManager pim;

    [SerializeField]
    private MazeGeneratorInstant mgi;
    // Start is called before the first frame update
    void Start()
    {
        pim = gameObject.GetComponent<PlayerInputManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void JoinNewPlayer()
    {
        playerCount++;
        controllers = Input.GetJoystickNames();
        Debug.Log(controllers[playerCount - 1]);
        if(controllers[playerCount - 1] == "Pro Controller")
        {
            pim.playerPrefab = playerPrefabs[0];
        }
        else if (controllers[playerCount - 1] == "Wireless Controller")
        {
            pim.playerPrefab = playerPrefabs[1];
        }
        else if(controllers[playerCount - 1] == "Controller (Xbox One For Windows)")
        {
            pim.playerPrefab = playerPrefabs[2];
        }

        mgi.SpawnPlayers(pim.playerPrefab, playerCount - 1);
    }
}